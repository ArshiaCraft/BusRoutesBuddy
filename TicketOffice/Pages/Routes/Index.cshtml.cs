using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TicketOffice.Data;
using TicketOffice.Models;
using Route = TicketOffice.Models.Route;

namespace TicketOffice.Pages.Routes;

public class IndexModel : PageModel
{
    // Error massage displaying when passenger's last name validation failed.
    public string? PassengerLastNameValidationError;

    // Error massage displaying when passenger's first name validation failed.
    public string? PassengerFirstNameValidationError;

    // Error massage displaying when passenger's place validation failed.
    public string? PassengerPlaceValidationError;

    private readonly TicketOfficeContext context;

    public IndexModel(TicketOfficeContext context)
    {
        this.context = context;
    }

    // Route list representing search results.
    [BindProperty]
    public List<Route>? Routes { get; set; }

    // Object representing ticket which user wants to buy.
    [BindProperty]
    public Ticket? Ticket { get; set; }

    // Search condition: departure city.
    [BindProperty(SupportsGet = true)]
    public string? From { get; set; }

    // Search condition: arrival city.
    [BindProperty(SupportsGet = true)]
    public string? To { get; set; }

    // Search condition: departure date.
    [BindProperty(SupportsGet = true)]
    public DateTime? Date { get; set; } = DateTime.Today;

    // Sort condition: determines in which order tickets will be displayed.
    // in the search results table
    [BindProperty(SupportsGet = true)]
    public string? SortString { get; set; }

    // Called when GET request is sent to the page. Retrieves routes based on
    // search conditions and sorts them.
    public ActionResult OnGet()
    {
        GetRoutes();
        return Page();
    }

    // Called when POST request is sent to the page (when user tries to buy a
    // ticket). Validates input, creates new ticket in the database and
    // redirects to "Account" page, where all bought tickets are shown.
    public ActionResult OnPost()
    {
        GetRoutes();

        if (!PassengerNameValidation(Ticket!.PassengerLastName,
                out PassengerLastNameValidationError) |
            !PassengerNameValidation(Ticket.PassengerFirstName,
                out PassengerFirstNameValidationError) |
            !PassengerPlaceValidation(Ticket.PassengerPlace,
                out PassengerPlaceValidationError))
        {
            return OnGet();
        }

        CopyDataToTicket();
        RevertChangesToRouteCities();

        Ticket.OderDate = DateTime.Now;
        context.Ticket.Add(Ticket);
        context.SaveChanges();

        return RedirectToPage("/Auth/Account");
    }

    // Sorts routes by routes' number.
    public void OnGetSortByNumber()
    {
        OnGet();

        if (SortString == "increasingNumber")
        {
            Routes!.Sort((x, y) =>
                Math.Clamp(x.Number - y.Number, -1, 1));
        }
        else
        {
            Routes!.Sort((x, y) =>
                Math.Clamp(y.Number - x.Number, -1, 1));
        }
    }

    // Sorts routes by routes' departure time .
    public void OnGetSortByDeparture()
    {
        OnGet();

        Routes!.Sort((x, y) =>
        {
            TimeSpan? totalDuration;

            if (SortString == "increasingDeparture")
            {
                totalDuration = x.Cities.First().DepartureTime -
                                y.Cities.First().DepartureTime;
            }
            else
            {
                totalDuration = y.Cities.First().DepartureTime -
                                x.Cities.First().DepartureTime;
            }

            return
                Math.Clamp((int)totalDuration!.Value.TotalMilliseconds, -1, 1);
        });
    }

    // Sorts routes by routes' arrival time.
    public void OnGetSortByArrival()
    {
        OnGet();

        Routes!.Sort((x, y) =>
        {
            TimeSpan? totalDuration;

            if (SortString == "increasingArrival")
            {
                totalDuration = x.Cities.Last().ArrivalTime -
                                y.Cities.Last().ArrivalTime;
            }
            else
            {
                totalDuration = y.Cities.Last().ArrivalTime -
                                x.Cities.Last().ArrivalTime;
            }

            return
                Math.Clamp((int)totalDuration!.Value.TotalMilliseconds, -1, 1);
        });
    }

    // Sorts routes by routes' duration.
    public void OnGetSortByDuration()
    {
        OnGet();

        Routes!.Sort((x, y) =>
        {
            TimeSpan? xDuration = x.Cities.Last().ArrivalTime -
                                  x.Cities.First().DepartureTime;
            TimeSpan? yDuration = y.Cities.Last().ArrivalTime -
                                  y.Cities.First().DepartureTime;

            TimeSpan? totalDuration;

            if (SortString == "increasingDuration")
            {
                totalDuration = xDuration - yDuration;
            }
            else
            {
                totalDuration = yDuration - xDuration;
            }

            return
                Math.Clamp((int)totalDuration!.Value.TotalMilliseconds, -1, 1);
        });
    }

    // Returns remaining route's capacity depending on
    // arrival and departure cities.
    public int GetRemainingCapacity(Route route)
    {
        int remainingCapacity = route.Capacity;

        foreach (var ticket in route.Tickets!)
        {
            List<DateTime?> intersection = GetCitiesDates(route.Cities.ToList())
                .Intersect(GetCitiesDates(ticket.Cities.ToList()))
                .ToList();

            if (intersection.Count > 1)
            {
                remainingCapacity--;
            }
        }

        return remainingCapacity;
    }

    // Returns true if place is available otherwise returns false.
    public bool IsTakenPlace(Route route, int place)
    {
        foreach (var ticket in route.Tickets!.Where(t => t.PassengerPlace == place))
        {
            List<DateTime?> intersection = GetCitiesDates(route.Cities.ToList())
                .Intersect(GetCitiesDates(ticket.Cities.ToList()))
                .ToList();

            if (intersection.Count > 1)
            {
                return true;
            }
        }

        return false;
    }

    // Returns list of route cities' departure dates.
    public List<DateTime?> GetCitiesDates(List<RouteCity> cities)
    {
        List<DateTime?> citiesDates = new List<DateTime?>();

        foreach (var city in cities)
        {
            citiesDates.Add(city.DepartureTime);
        }

        return citiesDates;
    }

    // Overload of the method above. Returns list of ticket cities' departure dates.
    public List<DateTime?> GetCitiesDates(List<TicketCity> cities)
    {
        List<DateTime?> citiesDates = new List<DateTime?>();

        foreach (var city in cities)
        {
            citiesDates.Add(city.DepartureTime);
        }

        return citiesDates;
    }

    private void RetrieveAllRoutes()
    {
        Routes = context.Route
            .Include(r => r.Cities)
            .Include(r => r.Tickets)
            .ToList();

        // Add cities to tickets.
        for (int i = 0; i < Routes.Count; i++)
        {
            for (int j = 0; j < Routes[i].Tickets!.Count; j++)
            {
                Routes[i].Tickets!.ToList()[j].Cities = context.TicketCity
                    .Where(tc => tc.Ticket == Routes[i].Tickets!.ToList()[j])
                    .ToList();
            }
        }
    }

    private void FilterRoutesByCities()
    {
        if (From == To)
        {
            Routes!.RemoveAll(_ => true);
            return;
        }

        Routes!.RemoveAll(r =>
            r.Cities.All(c => c.Name.ToLower() != From!.ToLower().Trim())
            || r.Cities.All(c => c.Name.ToLower() != To!.ToLower().Trim()));

        if (Routes.Count == 0)
        {
            return;
        }

        RouteCity? fromCityFirst;
        RouteCity? toCityFirst;

        RouteCity? fromCityLast;
        RouteCity? toCityLast;

        foreach (var route in Routes!)
        {
            fromCityLast = route.Cities.LastOrDefault(c =>
                c.Name.ToLower() == From!.ToLower().Trim());

            toCityLast = route.Cities.LastOrDefault(c =>
                c.Name.ToLower() == To!.ToLower().Trim());

            if (fromCityLast == null || toCityLast == null)
            {
                continue;
            }

            if (fromCityLast.Id > toCityLast.Id)
            {
                fromCityFirst = route.Cities.First(c =>
                    c.Name.ToLower() == From!.ToLower().Trim());

                toCityFirst = route.Cities.First(c =>
                    c.Name.ToLower() == To!.ToLower().Trim());

                route.Cities = route.Cities
                    .SkipWhile(c => c != fromCityFirst)
                    .TakeWhile(c =>
                        route.Cities.ToList().IndexOf(c) !=
                        route.Cities.ToList().IndexOf(toCityFirst) + 1)
                    .ToList();
            }
            else
            {
                route.Cities = route.Cities
                    .SkipWhile(c => c != fromCityLast)
                    .TakeWhile(c =>
                        route.Cities.ToList().IndexOf(c) !=
                        route.Cities.ToList().IndexOf(toCityLast) + 1)
                    .ToList();
            }
        }
    }

    private void FilterRoutesByDate()
    {
        if (Date < DateTime.Today)
        {
            Routes!.RemoveAll(_ => true);
            return;
        }

        Routes!.RemoveAll(r =>
            r.Cities.First().DepartureTime!.Value.Date != Date?.Date);
    }

    private void GetRoutes()
    {
        if (string.IsNullOrWhiteSpace(From) || string.IsNullOrWhiteSpace(To) ||
            Date == null)
        {
            return;
        }

        RetrieveAllRoutes();
        FilterRoutesByCities();
        FilterRoutesByDate();
    }

    private bool PassengerNameValidation(
        string? name,
        out string validationError)
    {
        if (String.IsNullOrEmpty(name))
        {
            validationError = "this field is required";
            return false;
        }

        validationError = String.Empty;
        return true;
    }

    private bool PassengerPlaceValidation(
        int place,
        out string validationError)
    {
        if (place == 0)
        {
            validationError = "this field is required";
            return false;
        }

        if (IsTakenPlace(
                Routes?.Where(r => r.Id == Ticket!.RouteId).ToList()[0]!,
                Ticket!.PassengerPlace))
        {
            validationError = "This seat is already taken.";
            return false;
        }

        validationError = String.Empty;
        return true;
    }

    private void CopyDataToTicket()
    {
        List<RouteCity> routeCities =
            Routes!.Find(r => r.Id == Ticket!.RouteId)!.Cities.ToList();

        Ticket!.Cities = new List<TicketCity>();

        foreach (var city in routeCities)
        {
            Ticket.Cities.Add(new TicketCity
            {
                Name = city.Name,
                DepartureTime = city.DepartureTime,
                ArrivalTime = city.ArrivalTime,
                CostFromPreviousCity = city.CostFromPreviousCity
            });
        }
    }

    private void RevertChangesToRouteCities()
    {
        context.ChangeTracker.Entries()
            .Where(e =>
                e.Metadata.Name == "TicketOffice.Models.RouteCity")
            .ToList().ForEach(e => e.State = EntityState.Unchanged);
    }
}
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TicketOffice.Data;
using TicketOffice.Models;
using TicketOffice.Services;
using Route = TicketOffice.Models.Route;

namespace TicketOffice.Pages.Management.Routes;

public class EditModel : PageModel
{
    // Error massage displaying when route number validation failed.
    public string NumberValidationError = null!;

    // Error massage displaying when route capacity validation failed.
    public string CapacityValidationError = null!;

    // Array of error massages displaying when route name validation failed.
    public string[] NameValidationError = null!;

    // Array of error massages displaying when cities
    // departure time validation failed.
    public string[] DepartureTimeValidationError = null!;

    // Array of error massages displaying when cities
    // arrival time validation failed.
    public string[] ArrivalTimeValidationError = null!;

    private readonly TicketOfficeContext context;
    private readonly UserValidationService validationService;

    public EditModel(TicketOfficeContext context,
        UserValidationService validationService)
    {
        this.context = context;
        this.validationService = validationService;
    }

    // Object representing that will be created.
    [BindProperty]
    public Route? Route { get; set; }

    // Object holding cities' arrival/departure dates.
    [BindProperty]
    public DateString[] TimeStrings { get; set; } = null!;

    // Holds cities' ids between loading and saving
    [BindProperty]
    public int[] CityIds { get; set; } = null!;

    // Called when GET request is sent to the page.
    // Retrieves route.
    public IActionResult OnGet(int? id)
    {
        if (!validationService.IsManager(HttpContext))
        {
            return RedirectToPage("/Index");
        }

        if (id == null)
        {
            return NotFound();
        }

        Route = context.Route.Where(m => m.Id == id)
            .Include(r => r.Cities)
            .First();

        InitializeArrays();

        TimeStrings = new DateString[Route.Cities.Count];
        for (int i = 0; i < TimeStrings.Length; i++)
        {
            TimeStrings[i] = new DateString();
        }

        InsertDatesIntoStrings();
        SaveCityIds();

        if (Route == null)
        {
            return NotFound();
        }
        return Page();
    }

    // Called when POST request is sent to the page.
    // Saves changes made to route.
    public IActionResult OnPost(int? id)
    {
        InitializeArrays();
        InsertDatesIntoCities();
        LoadCityIds();
        Route!.Id = (int)id;

        if (!ValidateInput())
        {
            return Page();
        }

        context.Attach(Route).State = EntityState.Modified;

        foreach (var city in Route.Cities)
        {
            context.Attach(city).State = EntityState.Modified;
        }

        try
        {
            context.SaveChanges();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!RouteExists(Route.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }

    private bool RouteExists(int id)
    {
        return context.Route.Any(e => e.Id == id);
    }

    private void InsertDatesIntoCities()
    {
        for (int i = 0; i < Route!.Cities.Count; i++)
        {

            try
            {
                Route.Cities[i].DepartureTime =
                    ConvertStringToDate(TimeStrings[i].DepartureDate!);
            }
            catch (Exception)
            {
                if (Route.Cities.Count > 2)
                {
                    DepartureTimeValidationError = InitializeArray<string>(
                        Route.Cities.Count, "");
                }

                DepartureTimeValidationError[i] = "Format: dd.MM.yyyy, hh:mm";
            }

            try
            {
                Route.Cities[i].ArrivalTime =
                    ConvertStringToDate(TimeStrings[i].ArrivalDate!);
            }
            catch (Exception)
            {
                if (Route.Cities.Count > 2)
                {
                    ArrivalTimeValidationError = InitializeArray<string>(
                        Route.Cities.Count, "");
                }

                ArrivalTimeValidationError[i] = "Format: dd.MM.yyyy, hh:mm";
            }
        }

        DateTime? ConvertStringToDate(string dateStr)
        {
            if (String.IsNullOrWhiteSpace(dateStr) ||
                String.IsNullOrEmpty(dateStr))
            {
                return null;
            }

            string[] date = dateStr.Split(",")[0].Split(".");
            string[] time = dateStr.Split(",")[1].Split(":");

            date.ToList().ForEach(s => s.Trim());
            time.ToList().ForEach(s => s.Trim());

            return new DateTime(
                Int32.Parse(date[2]),
                Int32.Parse(date[1]),
                Int32.Parse(date[0]),
                Int32.Parse(time[0]),
                Int32.Parse(time[1]),
                0);
        }
    }

    private void InsertDatesIntoStrings()
    {
        for (int i = 0; i < Route!.Cities.Count; i++)
        {
            if (Route.Cities[i].DepartureTime != null)
            {
                TimeStrings[i].DepartureDate = Route.Cities[i].DepartureTime!
                    .Value.ToString("dd.MM.yyyy, hh:mm");
            }

            if (Route.Cities[i].ArrivalTime != null)
            {
                TimeStrings[i].ArrivalDate = Route.Cities[i].ArrivalTime!
                    .Value.ToString("dd.MM.yyyy, hh:mm");
            }
        }
    }

    // This method doesn't actually work, but stays here for representing
    // an idea. It should be removed.
    private void SaveCityIds()
    {
        CityIds = new int[Route!.Cities.Count];

        for (int i = 0; i < Route.Cities.Count; i++)
        {
            CityIds[i] = Route.Cities[i].Id;
        }
    }

    private void LoadCityIds()
    {
        for (int i = 0; i < CityIds.Length; i++)
        {
            Route!.Cities[i].Id = CityIds[i];
        }
    }

    private bool ValidateInput()
    {
        bool isValidNumber = ValidateNumber(Route!.Number, out NumberValidationError);

        bool isValidCapacity = ValidateCapacity(Route.Capacity, out CapacityValidationError);

        for (int i = 0; i < Route.Cities.Count; i++)
        {
            if (Route.Cities.Count > 2)
            {
                NameValidationError = InitializeArray<string>(
                    Route.Cities.Count, "");
            }

            ValidateName(Route.Cities[i].Name, out NameValidationError[i]);
        }

        for (int i = 0; i < Route.Cities.Count; i++)
        {
            if (Route.Cities.Count > 2)
            {
                DepartureTimeValidationError = InitializeArray<string>(
                    Route.Cities.Count, "");
            }

            ValidateDate(Route.Cities[i].DepartureTime, out DepartureTimeValidationError[i]);
        }

        for (int i = 0; i < Route.Cities.Count; i++)
        {
            if (Route.Cities.Count > 2)
            {
                ArrivalTimeValidationError = InitializeArray<string>(
                    Route.Cities.Count, "");
            }

            ValidateDate(Route.Cities[i].ArrivalTime, out ArrivalTimeValidationError[i]);
        }

        if (!isValidNumber || !isValidCapacity ||
            NameValidationError.Any(e => e != "") ||
            DepartureTimeValidationError.Any(e => e != "") ||
            ArrivalTimeValidationError.Any(e => e != ""))
        {
            return false;
        }

        return true;

        bool ValidateNumber(int number, out string validationError)
        {
            validationError = "";

            if (number < 1 || number > 9999)
            {
                validationError = "Must be between 1 and 9999";
                return false;
            }

            return true;
        }

        bool ValidateCapacity(int capacity, out string validationError)
        {
            validationError = "";

            if (capacity < 1 || capacity > 40)
            {
                validationError = "Must be between 5 and 45";
                return false;
            }

            return true;
        }

        bool ValidateName(string name, out string validationError)
        {
            validationError = "";

            if (String.IsNullOrWhiteSpace(name) || String.IsNullOrEmpty(name))
            {
                validationError = "this field is required";
                return false;
            }

            return true;
        }

        bool ValidateDate(DateTime? date, out string validationError)
        {
            validationError = "";

            if (date == null)
            {
                return true;
            }

            if (date < DateTime.Today)
            {
                validationError = $"Must be later than {DateTime.Today.ToString(CultureInfo.GetCultureInfo("en"))}";
                return false;
            }

            return true;
        }
    }

    private T[] InitializeArray<T>(int length, T initVal)
    {
        T[] arr = new T[length];

        for (int i = 0; i < length; i++)
        {
            arr[i] = initVal;
        }

        return arr;
    }

    private void InitializeArrays()
    {
        NameValidationError = InitializeArray<string>(Route!.Cities.Count, "");
        DepartureTimeValidationError = InitializeArray<string>(Route.Cities.Count, "");
        ArrivalTimeValidationError = InitializeArray<string>(Route.Cities.Count, "");
    }
}


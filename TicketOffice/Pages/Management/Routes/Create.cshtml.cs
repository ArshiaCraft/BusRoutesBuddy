using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketOffice.Data;
using TicketOffice.Models;
using TicketOffice.Services;
using Route = TicketOffice.Models.Route;

namespace TicketOffice.Pages.Management.Routes;

public class CreateModel : PageModel
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

    public CreateModel(TicketOfficeContext context,
        UserValidationService validationService)
    {
        this.context = context;
        this.validationService = validationService;
    }

    // Object representing that will be created.
    [BindProperty]
    public Route Route { get; set; } = null!;

    // Object holding cities' arrival/departure dates.
    [BindProperty]
    public DateString[] TimeStrings { get; set; } = null!;

    // Amount of cities to be added to the route
    [BindProperty]
    public int? CitiesCount { get; set; }

    // Called when GET request is sent to the page.
    public IActionResult OnGet()
    {
        if (!validationService.IsManager(HttpContext))
        {
            return RedirectToPage("/Index");
        }

        return Page();
    }

    // Called when POST request is sent to the page (when user tries to add new
    // route). Validates input, creates new route in the database and
    // redirects to "Management/Routes" page.
    public ActionResult OnPost()
    {
        if (CitiesCount != null)
        {
            Route = new Route();
            Route.Cities = new List<RouteCity>();

            TimeStrings = new DateString[(int)CitiesCount];
            for (int i = 0; i < CitiesCount; i++)
            {
                TimeStrings[i] = new DateString();
                Route.Cities.Add(new RouteCity());
            }

            NameValidationError = new string[(int)CitiesCount];
            DepartureTimeValidationError = new string[(int)CitiesCount];
            ArrivalTimeValidationError = new string[(int)CitiesCount];

            return Page();
        }

        NameValidationError = new string[Route.Cities.Count];
        DepartureTimeValidationError = new string[Route.Cities.Count];
        ArrivalTimeValidationError = new string[Route.Cities.Count];

        InsertDatesIntoCities();

        if (!ValidateInput())
        {
            return Page();
        }

        context.Route.Add(Route);
        context.SaveChanges();

        return RedirectToPage("./Index");
    }

    private void InsertDatesIntoCities()
    {
        for (int i = 0; i < Route.Cities.Count; i++)
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
                    InitializeArray<string>(DepartureTimeValidationError,
                        Route.Cities.Count,
                        "");
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
                    InitializeArray<string>(ArrivalTimeValidationError,
                        Route.Cities.Count,
                        "");
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

            if (dateStr.Count(c => c == '.') != 2 &&
                dateStr.Count(c => c == ':') != 2 &&
                dateStr.Count(c => c == ',') != 1)
            {
                throw new ArgumentException("Invalid input format");
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

    private bool ValidateInput()
    {
        bool isValidNumber = ValidateNumber(Route.Number, out NumberValidationError);

        bool isValidCapacity = ValidateCapacity(Route.Capacity, out CapacityValidationError);

        for (int i = 0; i < Route.Cities.Count; i++)
        {
            if (Route.Cities.Count > 2)
            {
                InitializeArray<string>(NameValidationError,
                    Route.Cities.Count,
                    "");
            }

            ValidateName(Route.Cities[i].Name, out NameValidationError[i]);
        }

        for (int i = 0; i < Route.Cities.Count; i++)
        {
            if (Route.Cities.Count > 2)
            {
                InitializeArray<string>(DepartureTimeValidationError,
                    Route.Cities.Count,
                    "");
            }

            ValidateDate(Route.Cities[i].DepartureTime, out DepartureTimeValidationError[i]);
        }

        for (int i = 0; i < Route.Cities.Count; i++)
        {
            if (Route.Cities.Count > 2)
            {
                InitializeArray<string>(ArrivalTimeValidationError,
                    Route.Cities.Count,
                    "");
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
                validationError = $"Must not be earlier than {DateTime.Today.ToString(CultureInfo.GetCultureInfo("en")).Split(" ")[0]}";
                return false;
            }

            return true;
        }
    }

    private void InitializeArray<T>(T[] arr, int length, T initVal)
    {
        arr = new T[length];

        for (int i = 0; i < length; i++)
        {
            arr[i] = initVal;
        }
    }
}
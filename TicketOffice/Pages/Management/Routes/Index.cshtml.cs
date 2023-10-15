using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TicketOffice.Data;
using TicketOffice.Models;
using TicketOffice.Services;
using Route = TicketOffice.Models.Route;

namespace TicketOffice.Pages.Management.Routes
{
    public class IndexModel : PageModel
    {
        private readonly TicketOfficeContext context;
        private readonly UserValidationService validationService;

        public IndexModel(TicketOfficeContext context,
            UserValidationService validationService)
        {
            this.context = context;
            this.validationService = validationService;
        }

        public List<Route>? Routes { get; set; }

        // Search condition: route number.
        [BindProperty(SupportsGet = true)]
        public int? Number { get; set; }

        // Search condition: departure city.
        [BindProperty(SupportsGet = true)]
        public string? From { get; set; }

        // Search condition: arrival city.
        [BindProperty(SupportsGet = true)]
        public string? To { get; set; }

        // Search condition: departure date.
        [BindProperty(SupportsGet = true)]
        public DateTime? Date { get; set; }

        // Called when GET request is sent to the page.
        // Retrieves routes based on search conditions.
        public ActionResult OnGet()
        {
            if (!validationService.IsManager(HttpContext))
            {
                return RedirectToPage("/Index");
            }

            RetrieveAllRoutes();
            FilterRoutesByNumber();
            FilterRoutesByFrom();
            FilterRoutesByTo();
            FilterRoutesByDate();

            return Page();
        }

        // Called when user confirms route deletion.
        public ActionResult OnGetDeleteRoute(int deleteRouteId)
        {
            OnGet();

            Route? deleteRoute = context.Route.Find(deleteRouteId);

            if (deleteRoute != null)
            {
                context.Remove(deleteRoute);
                context.SaveChanges();
                return RedirectToPage("./Index");
            }

            return NotFound();
        }

        private void RetrieveAllRoutes()
        {
            Routes = context.Route
                .Include(r => r.Cities)
                .Include(r => r.Tickets)
                .ToList();
        }

        private void FilterRoutesByNumber()
        {
            if (Number == null || Number < 1)
            {
                return;
            }

            Routes!.RemoveAll(r => r.Number != Number);
        }

        private void FilterRoutesByFrom()
        {
            if (String.IsNullOrWhiteSpace(From) || String.IsNullOrEmpty(From))
            {
                return;
            }

            Routes!.RemoveAll(r => r.Cities.All(c => c.Name != From));
        }

        private void FilterRoutesByTo()
        {
            if (String.IsNullOrWhiteSpace(To) || String.IsNullOrEmpty(To))
            {
                return;
            }

            Routes!.RemoveAll(r => r.Cities.All(c => c.Name != To));
        }

        private void FilterRoutesByDate()
        {
            if (Date == null)
            {
                return;
            }

            Routes!.RemoveAll(r =>
                r.Cities.All(c =>
                    c.DepartureTime?.Date != Date?.Date &&
                    c.ArrivalTime?.Date != Date?.Date));
        }
    }
}

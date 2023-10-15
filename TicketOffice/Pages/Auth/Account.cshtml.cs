using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TicketOffice.Data;
using TicketOffice.Models;
using TicketOffice.Services;

namespace TicketOffice.Pages.Auth;

public class AccountModel : PageModel
{
    private readonly TicketOfficeContext context;
    private readonly UserValidationService validationService;
    private readonly PdfService pdfService;

    public AccountModel(TicketOfficeContext context,
        UserValidationService validationService,
        PdfService pdfService)
    {
        this.context = context;
        this.validationService = validationService;
        this.pdfService = pdfService;
    }

    // User's tickets.
    public List<Ticket> Tickets { get; set; } = null!;

    // Called when GET request is sent to the page. Checks if the session is
    // valid then retrieves all user's tickets. 
    public ActionResult OnGet()
    {
        if (!validationService.IsAuthorized(HttpContext))
        {
            return RedirectToPage("/Auth/Login");
        }

        Tickets = context.Ticket
                .Where(t =>
                    t.UserId == HttpContext.Session.GetInt32("UserId"))
                .Include(t => t.Route)
                .Include(t => t.Cities)
                .ToList();

        return Page();
    }

    // Called when user confirms ticket return.
    public ActionResult OnGetReturnTicket(int returnTicketId)
    {
        OnGet();

        Ticket? returnTicket = context.Ticket.Find(returnTicketId);

        if (returnTicket != null)
        {
            context.Remove(returnTicket);
            context.SaveChanges();
            return RedirectToPage("./Account");
        }

        return NotFound();
    }

    // Downloads ticket in PDF format
    public ActionResult OnGetTicketPdf(int pdfTicketId)
    {
        OnGet();

        Ticket? ticket = Tickets.Find(t => t.Id == pdfTicketId);

        if (ticket == null)
        {
            return NotFound();
        }

        return pdfService.GetTicketPdf(ticket);
    }
}
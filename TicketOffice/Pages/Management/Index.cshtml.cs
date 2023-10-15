using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketOffice.Services;

namespace TicketOffice.Pages.Management;

public class Index : PageModel
{
    private readonly UserValidationService validationService;

    public Index(UserValidationService validationService)
    {
        this.validationService = validationService;
    }

    public ActionResult OnGet()
    {
        if (!validationService.IsAuthorized(HttpContext))
        {
            return RedirectToPage("/Index");
        }
        else
        {
            return RedirectToPage("./Routes");
        }
    }
}
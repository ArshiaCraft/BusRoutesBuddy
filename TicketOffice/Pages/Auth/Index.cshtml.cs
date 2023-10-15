using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketOffice.Services;

namespace TicketOffice.Pages.Auth;

public class IndexModel : PageModel
{
    private readonly UserValidationService validationService;

    public IndexModel(UserValidationService validationService)
    {
        this.validationService = validationService;
    }

    // Called when GET request is sent to the page. Determines what page
    // user will be redirected to depending on his/her authorization status.
    public ActionResult OnGet()
    {
        if (validationService.IsAuthorized(HttpContext))
        {
            return RedirectToPage("/Auth/Account");
        }

        return RedirectToPage("/Auth/Login");
    }
}
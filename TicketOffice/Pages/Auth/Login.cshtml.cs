using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketOffice.Data;
using TicketOffice.Models;
using TicketOffice.Services;

namespace TicketOffice.Pages.Auth;

public class LoginModel : PageModel
{
    // Error massage displaying when email validation failed.
    public string EmailValidationError = null!;

    // Error massage displaying when password validation failed.
    public string PasswordValidationError = null!;

    private readonly TicketOfficeContext context;
    private readonly UserValidationService validationService;

    public LoginModel(TicketOfficeContext context,
        UserValidationService validationService)
    {
        this.context = context;
        this.validationService = validationService;
    }

    // Object representing a user who wants to login.
    [BindProperty]
    public new User? User { get; set; }

    // Called when GET request is sent to the page. Validates the session and
    // redirects to "Account" page if user already logged in.
    public ActionResult OnGet()
    {
        if (validationService.IsAuthorized(HttpContext))
        {
            return RedirectToPage("/Auth/Account");
        }

        return Page();
    }

    // Called when POST request is sent to the page. Validates login form and
    // redirects to "Account" page if the validation succeed.
    public ActionResult OnPost()
    {
        if (ValidateForm())
        {
            User? user = context.User
                .FirstOrDefault(u => u.Email == User!.Email);

            HttpContext.Session.SetInt32("UserId", user!.Id);
            HttpContext.Session.SetInt32("IsManager", user.IsManager ? 1 : 0);
            return RedirectToPage("/Auth/Account");
        }

        return Page();
    }

    private bool ValidateForm()
    {
        var user = context.User.FirstOrDefault(u => u.Email == User!.Email);

        return ValidateEmail(User!.Email, out EmailValidationError) &&
               ValidatePassword(User.Password, out PasswordValidationError);

        bool ValidateEmail(string email, out string validationError)
        {
            if (user != null)
            {
                validationError = String.Empty;
                return true;
            }

            if (String.IsNullOrWhiteSpace(email))
            {
                validationError = "this field is required";
                return false;
            }

            Regex emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            if (!emailRegex.IsMatch(email))
            {
                validationError = "incorrect E-mail";
                return false;
            }

            validationError = "this E-mail address is not registered";
            return false;
        }

        bool ValidatePassword(string password, out string validationError)
        {
            if (user!.Password == password)
            {
                validationError = String.Empty;
                return true;
            }

            if (String.IsNullOrWhiteSpace(password))
            {
                validationError = "this field is required";
                return false;
            }

            validationError = "Incorrect password!";
            return false;
        }
    }
}
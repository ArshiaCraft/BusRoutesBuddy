namespace TicketOffice.Services;

public class UserValidationService
{
    // Determines if user is authiruzed
    public bool IsAuthorized(HttpContext context)
    {
        return context.Session.GetInt32("UserId") != null;
    }

    // Determines if user has and administrative permissions
    public bool IsManager(HttpContext context)
    {
        return context.Session.GetInt32("IsManager") != null;
    }
}
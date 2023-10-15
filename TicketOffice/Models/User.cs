using System.ComponentModel.DataAnnotations;

namespace TicketOffice.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [MaxLength(48, ErrorMessage = "E-mail cannot be longer than 48 characters"),
     MinLength(6, ErrorMessage = "E-mail cannot be shorter than 6 characters")]
    [Required(ErrorMessage = "this field is required")]
    [Display(Name = "E-mail")]
    [DataType(DataType.EmailAddress)]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
     ErrorMessage = "Invalid E-mail!")]
    public string Email { get; set; } = null!;

    [MaxLength(32, ErrorMessage = "Password must be less than 32 characters"),
     MinLength(8, ErrorMessage = "The password must be longer than 8 characters")]
    [Required(ErrorMessage = "this field is required")]
    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$",
     ErrorMessage = "The password must contain capital and small Latin letters, numbers and special characters (@, $, %, etc.)")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "this field is required")]
    [Display(Name = "Administrator?")]
    public bool IsManager { get; set; }


    public List<Ticket>? Tickets { get; set; }
}
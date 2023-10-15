using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketOffice.Models;

public class Ticket
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "this field is required")]
    [Display(Name = "Passenger's first name")]
    public string PassengerFirstName { get; set; } = null!;

    [Required(ErrorMessage = "this field is required")]
    [Display(Name = "Passenger's last name")]
    public string PassengerLastName { get; set; } = null!;

    [Required(ErrorMessage = "this field is required")]
    [Display(Name = "Passenger seat number")]
    public int PassengerPlace { get; set; }

    [Required]
    [Display(Name = "Date of purchasing ticket")]
    public DateTime OderDate { get; set; }


    [Required]
    public List<TicketCity> Cities { get; set; } = null!;

    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [ForeignKey("Route")]
    public int RouteId { get; set; }
    public Route Route { get; set; } = null!;

    public double GetTotalCost()
    {
        double cost = 0;

        for (int i = 1; i < Cities.Count; i++)
        {
            cost += Cities.ToList()[i].CostFromPreviousCity ?? 0;
        }

        return cost;
    }
}
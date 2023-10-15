using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TicketOffice.Models;

public class Route
{
    [Key]
    [BindRequired]
    public int Id { get; set; }

    [Required(ErrorMessage = "this field is required")]
    [Display(Name = "Number")]
    [Range(1, 9999)]
    public int Number { get; set; }

    [Required(ErrorMessage = "this field is required")]
    [Display(Name = "Capacity")]
    [Range(5, 40)]
    public int Capacity { get; set; }

    [Required]
    public List<RouteCity> Cities { get; set; } = null!;

    public List<Ticket>? Tickets { get; set; }

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
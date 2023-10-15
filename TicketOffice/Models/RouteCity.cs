using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketOffice.Models;

public class RouteCity
{
    [Key]
    public int Id { get; set; }

    [MaxLength(24, ErrorMessage = "The name of the city cannot be longer than 24 characters."),
     MinLength(2, ErrorMessage = "The name of the city cannot be less than 2 characters")]
    [Display(Name = "Name of the city")]
    [Required(ErrorMessage = "this field is required")]
    public string Name { get; set; } = null!;

    [Display(Name = "Departure date")]
    [DataType(DataType.Date)]
    public DateTime? ArrivalTime { get; set; }

    [Display(Name = "Arrival date")]
    [DataType(DataType.Date)]
    public DateTime? DepartureTime { get; set; }

    [Display(Name = "Price of the trip from the previous city")]
    [DataType(DataType.Currency)]
    public double? CostFromPreviousCity { get; set; }

    [ForeignKey("Route")]
    public int RouteId { get; set; }
    public Route Route { get; set; } = null!;
}
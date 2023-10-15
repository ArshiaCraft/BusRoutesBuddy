using Microsoft.EntityFrameworkCore;
using TicketOffice.Data;

namespace TicketOffice.Models;

public class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context =
            new TicketOfficeContext(serviceProvider
                .GetRequiredService<DbContextOptions<TicketOfficeContext>>());

        if (context == null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        if (context.User.Any() | context.Route.Any() |
            context.RouteCity.Any() | context.Ticket.Any()) // Data has been seeded
        {
            return;
        }

        context.Database.EnsureCreated();

        context.User.AddRange(new User[]
        {
            new User
            {
                Email = "admin",
                Password = "admin",
                IsManager = true
            },
            new User
            {
                Email = "user",
                Password = "user",
                IsManager = false
            }
        });

        context.Route.AddRange(new Route[]
        {
            new Route()
            {
                Number = 027,
                Capacity = 30,
                Cities = new List<RouteCity>()
                {
                    new RouteCity
                    {
                        Name = "Tehran",
                        ArrivalTime = null,
                        DepartureTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            6,
                            30,
                            0),
                        CostFromPreviousCity = null
                    },
                    new RouteCity
                    {
                        Name = "Semnan",
                        ArrivalTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            7,
                            10,
                            0),
                        DepartureTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            7,
                            20,
                            0),
                        CostFromPreviousCity = 30
                    },
                    new RouteCity
                    {
                        Name = "Mashhad",
                        ArrivalTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            7,
                            50,
                            0),
                        DepartureTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            8,
                            0,
                            0),
                        CostFromPreviousCity = 30
                    },
                    new RouteCity
                    {
                        Name = "Birjand",
                        ArrivalTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            8,
                            30,
                            0),
                        DepartureTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            8,
                            40,
                            0),
                        CostFromPreviousCity = 15
                    },
                    new RouteCity
                    {
                        Name = "Yazd",
                        ArrivalTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            9,
                            10,
                            0),
                        DepartureTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            9,
                            20,
                            0),
                        CostFromPreviousCity = 15
                    },
                    new RouteCity
                    {
                        Name = "Esfahan",
                        ArrivalTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            9,
                            50,
                            0),
                        DepartureTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            12,
                            0,
                            0),
                        CostFromPreviousCity = 20
                    },
                    new RouteCity
                    {
                        Name = "Araak",
                        ArrivalTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            12,
                            30,
                            0),
                        DepartureTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            12,
                            40,
                            0),
                        CostFromPreviousCity = 20
                    },
                    new RouteCity
                    {
                        Name = "Hamedan",
                        ArrivalTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            13,
                            10,
                            0),
                        DepartureTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            13,
                            20,
                            0),
                        CostFromPreviousCity = 15
                    },
                    new RouteCity
                    {
                        Name = "Ghazvin",
                        ArrivalTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            13,
                            50,
                            0),
                        DepartureTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            14,
                            0,
                            0),
                        CostFromPreviousCity = 15
                    },
                    new RouteCity
                    {
                        Name = "Karaj",
                        ArrivalTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            14,
                            30,
                            0),
                        DepartureTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            14,
                            40,
                            0),
                        CostFromPreviousCity = 30
                    },
                    new RouteCity
                    {
                        Name = "Tehran",
                        ArrivalTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            15,
                            20,
                            0),
                        DepartureTime = null,
                        CostFromPreviousCity = 30
                    }
                }
            },
            new Route()
            {
                Number = 013,
                Capacity = 25,
                Cities = new List<RouteCity>()
                {
                    new RouteCity
                    {
                        Name = "Mashhad",
                        ArrivalTime = null,
                        DepartureTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            7,
                            0,
                            0),
                        CostFromPreviousCity = null
                    },
                    new RouteCity
                    {
                        Name = "Yazd",
                        ArrivalTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            7,
                            30,
                            0),
                        DepartureTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            7,
                            40,
                            0),
                        CostFromPreviousCity = 15
                    },
                    new RouteCity
                    {
                        Name = "Shiraz",
                        ArrivalTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            8,
                            10,
                            0),
                        DepartureTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            8,
                            20,
                            0),
                        CostFromPreviousCity = 15
                    },
                    new RouteCity
                    {
                        Name = "Esfahan",
                        ArrivalTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            9,
                            20,
                            0),
                        DepartureTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            11,
                            20,
                            0),
                        CostFromPreviousCity = 40
                    },
                    new RouteCity
                    {
                        Name = "Qom",
                        ArrivalTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            12,
                            20,
                            0),
                        DepartureTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            12,
                            30,
                            0),
                        CostFromPreviousCity = 40
                    },
                    new RouteCity
                    {
                        Name = "Tehran",
                        ArrivalTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            13,
                            0,
                            0),
                        DepartureTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            13,
                            10,
                            0),
                        CostFromPreviousCity = 15
                    },
                    new RouteCity
                    {
                        Name = "Saari",
                        ArrivalTime = new DateTime(
                            DateTime.Today.Year,
                            DateTime.Today.Month,
                            DateTime.Today.Day,
                            13,
                            40,
                            0),
                        DepartureTime = null,
                        CostFromPreviousCity = 15
                    }
                }
            }
        });

        context.SaveChanges();
    }
}
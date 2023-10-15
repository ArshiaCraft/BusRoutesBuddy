#nullable disable
using Microsoft.EntityFrameworkCore;

namespace TicketOffice.Data
{
    public class TicketOfficeContext : DbContext
    {
        public TicketOfficeContext(DbContextOptions<TicketOfficeContext> options)
            : base(options)
        {
        }

        public DbSet<Models.User> User { get; set; }

        public DbSet<Models.Route> Route { get; set; }

        public DbSet<Models.RouteCity> RouteCity { get; set; }
        public DbSet<Models.TicketCity> TicketCity { get; set; }

        public DbSet<Models.Ticket> Ticket { get; set; }
    }
}

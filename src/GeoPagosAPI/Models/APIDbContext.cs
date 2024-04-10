using Microsoft.EntityFrameworkCore;

namespace GeoPagosAPI.Models
{
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options)
        {
        }

        public DbSet<Autorizacion> Autorizaciones { get; set; } = null!;
        public DbSet<TablaAprobada> TablaAprobadas { get; set; } = null!;
        public DbSet<TablaReversa> TablaReversas { get; set; } = null!;

    }
}

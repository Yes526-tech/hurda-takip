using FireHurdaTakip.Models;
using Microsoft.EntityFrameworkCore;

namespace FireHurdaTakip.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<HurdaKayit> HurdaKayitlar { get; set; }
    }
}

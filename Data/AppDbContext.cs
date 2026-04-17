using Microsoft.EntityFrameworkCore;
using CareerCompass.API.Models;

namespace CareerCompass.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Career> Careers { get; set; }
        public DbSet<FavoriteCareer> FavoriteCareers { get; set; }
        public DbSet<SavedCareer> SavedCareers { get; set; }

    }
}

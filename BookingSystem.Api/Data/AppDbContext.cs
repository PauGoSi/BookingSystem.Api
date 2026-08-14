using Microsoft.EntityFrameworkCore;
using BookingSystem.Api.Models;

namespace BookingSystem.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Email)
                    .HasMaxLength(254)
                    .IsRequired();

                entity.Property(u => u.NormalizedEmail)
                    .HasMaxLength(254)
                    .IsRequired();

                entity.HasIndex(u => u.NormalizedEmail)
                    .IsUnique();
            });
        }
    }
}
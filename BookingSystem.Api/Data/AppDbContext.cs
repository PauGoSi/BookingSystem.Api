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
                entity.Property(u => u.FirstName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(u => u.LastName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(u => u.Email)
                    .HasMaxLength(254)
                    .IsRequired();

                entity.Property(u => u.NormalizedEmail)
                    .HasMaxLength(254)
                    .IsRequired();

                entity.HasIndex(u => u.NormalizedEmail)
                    .IsUnique();
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(r => r.Name)
                    .HasMaxLength(50)
                    .IsRequired();
            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.Property(r => r.Name)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(r => r.Description)
                    .HasMaxLength(1000);

                entity.Property(r => r.Location)
                    .HasMaxLength(200);

                entity.ToTable(t =>
                    t.HasCheckConstraint(
                        "CK_Resources_Capacity_Positive",
                        "[Capacity] > 0"));
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.Property(b => b.Notes)
                    .HasMaxLength(2000);

                entity.ToTable(t =>
                    t.HasCheckConstraint(
                        "CK_Bookings_EndTime_After_StartTime",
                        "[EndTime] > [StartTime]"));
            });

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Resource)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.ResourceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
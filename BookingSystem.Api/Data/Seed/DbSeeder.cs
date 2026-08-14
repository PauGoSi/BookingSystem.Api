using BookingSystem.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Data.Seed
{
    public static class DbSeeder
    {
        // Seeds default roles and development admin user
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var passwordHasher = new PasswordHasher<User>();

            await context.Database.MigrateAsync();

            var adminRole = await context.Roles
                .FirstOrDefaultAsync(r => r.Name == "Admin");

            if (adminRole == null)
            {
                adminRole = new Role
                {
                    Name = "Admin"
                };

                context.Roles.Add(adminRole);
                await context.SaveChangesAsync();
            }

            var userRole = await context.Roles
                .FirstOrDefaultAsync(r => r.Name == "User");

            if (userRole == null)
            {
                userRole = new Role
                {
                    Name = "User"
                };

                context.Roles.Add(userRole);
                await context.SaveChangesAsync();
            }

            var adminEmail = configuration["DevelopmentAdmin:Email"]
                ?? "admin@bookingsystem.local";

            var normalizedAdminEmail = adminEmail.Trim().ToUpperInvariant();

            var adminPassword = configuration["DevelopmentAdmin:Password"]
                ?? throw new InvalidOperationException(
                    "Development admin password is not configured.");

            var adminUser = await context.Users
                .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedAdminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    FirstName = "Development",
                    LastName = "Admin",
                    Email = adminEmail.Trim(),
                    NormalizedEmail = normalizedAdminEmail,
                    RoleId = adminRole.Id,
                    CreatedAt = DateTime.UtcNow,
                    PasswordHash = string.Empty
                };

                adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, adminPassword);

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
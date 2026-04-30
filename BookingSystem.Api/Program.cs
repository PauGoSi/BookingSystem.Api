using BookingSystem.Api.BackgroundServices;
using BookingSystem.Api.Data;
using BookingSystem.Api.Middleware;
using BookingSystem.Api.Services.Bookings;
using BookingSystem.Api.Services.Resources;
using BookingSystem.Api.Services.Roles;
using BookingSystem.Api.Services.Users;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace BookingSystem.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IResourceService, ResourceService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();

            builder.Services.AddHostedService<BookingStatusBackgroundService>();

            builder.Services.AddOpenApi();

            var app = builder.Build();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "BookingSystem.Api v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
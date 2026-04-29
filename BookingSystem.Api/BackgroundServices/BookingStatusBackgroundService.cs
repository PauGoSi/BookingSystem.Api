using BookingSystem.Api.Data;
using BookingSystem.Api.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.BackgroundServices
{
    public class BookingStatusBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BookingStatusBackgroundService> _logger;

        // Injects scope factory and logger for background processing
        public BookingStatusBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<BookingStatusBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        // Runs the booking status update loop in the background
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateCompletedBookingsAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        // Updates expired active bookings to completed
        private async Task UpdateCompletedBookingsAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var now = DateTime.UtcNow;

            var expiredBookings = await context.Bookings
                .Where(b =>
                    b.Status == BookingStatus.Active &&
                    b.EndTime <= now)
                .ToListAsync(stoppingToken);

            if (!expiredBookings.Any())
            {
                return;
            }

            foreach (var booking in expiredBookings)
            {
                booking.Status = BookingStatus.Completed;
            }

            await context.SaveChangesAsync(stoppingToken);

            _logger.LogInformation(
                "Updated {Count} bookings to Completed.",
                expiredBookings.Count);
        }
    }
}
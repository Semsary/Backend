using Microsoft.EntityFrameworkCore;
using semsary_backend.EntityConfigurations;
using semsary_backend.Enums;
using semsary_backend.Service;

namespace semsary_backend.BackgroundJobs
{
    public class LateArrivalDateTenant : IHostedService, IDisposable
    {

        private Timer? timer;
        private readonly IServiceScopeFactory _scopeFactory;

        public LateArrivalDateTenant(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(ReturnWarrantyMoneyToLandlord, null, TimeSpan.Zero, TimeSpan.FromMinutes(60));
            return Task.CompletedTask;
        }
        private void ReturnWarrantyMoneyToLandlord(object? state)
        {
            using var scope = _scopeFactory.CreateScope();
            var apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();

            Task.Run(async () =>
            {
                var lateArrivals = await apiContext.Rentals
                    .Where(r => r.status == RentalStatus.Accepted && r.EndArrivalDate < DateTime.Now)
                    .Include(r => r.Tenant)
                    .Include(r => r.House.owner)
                    .ToListAsync();

                foreach (var rental in lateArrivals)
                {
                    var landlord = rental.House.owner;
                    var tenant = rental.Tenant;
                    tenant.Balance -= rental.WarrantyMoney;
                    landlord.Balance += rental.WarrantyMoney;
                    rental.status = RentalStatus.ArrivalReject;

                    apiContext.RentalUnits.RemoveRange(rental.RentalUnit);
                    apiContext.Rentals.Remove(rental);
                    await apiContext.SaveChangesAsync();
                }
            });
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        public void Dispose()
        {
            timer?.Dispose();
        }

    }
}

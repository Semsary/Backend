
using Microsoft.EntityFrameworkCore;
using semsary_backend.EntityConfigurations;
using semsary_backend.Enums;
using semsary_backend.Service;

namespace semsary_backend.BackgroundJobs
{
    public class PendingRentalRequests : IHostedService , IDisposable
    {
        private Timer? timer;
        private readonly IServiceScopeFactory _scopeFactory;

        public PendingRentalRequests(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(RemoveRentals , null, TimeSpan.Zero, TimeSpan.FromMinutes(60));
            return Task.CompletedTask;
        }
        private void RemoveRentals(object? state)
        {
            using var scope = _scopeFactory.CreateScope();
            var apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();

            Task.Run(async () =>
            {
                var pendingRequests = await apiContext.Rentals
                    .Where(r => r.status == RentalStatus.Bending && r.ResponseDate.AddDays(7) < DateTime.Now)
                    .Include(r => r.TenantUsername) 
                    .ToListAsync();

                foreach (var request in pendingRequests)
                {
                    request.status = RentalStatus.Rejected;

                    var tenant = await apiContext.Tenant
                        .FirstOrDefaultAsync(u => u.Username == request.TenantUsername);

                    var title = "رفض طلب الإيجار";
                    var message = "نأسف, لقد تم رفض طلب الإيجار خاصتك أوتوماتيكيا لتخطيه المدة المحددة دون الحصول علي رد من المؤجر";

                    await notificationService.SendNotificationAsync(title, message, tenant);
                }
                await apiContext.SaveChangesAsync();
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

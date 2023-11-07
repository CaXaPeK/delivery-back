using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Delivery_Service.Context;

namespace Delivery_Service.Controllers
{
    public class AutoDeleter : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;

        public AutoDeleter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DeleteExpiredRecords, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        private void DeleteExpiredRecords(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DeliveryDbContext>();

                var currentTime = DateTime.Now;
                var recordsToDelete = dbContext.BadTokens
                    .Where(x => (currentTime - x.AddedAt).TotalMinutes >= 10)
                    .ToList();

                dbContext.BadTokens.RemoveRange(recordsToDelete);
                dbContext.SaveChanges();
            }
        }

        private Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace ChronoScheduler.Hosting
{
    /// <summary>
    /// Wraps <see cref="Scheduler"/> as an <see cref="IHostedService"/> so it
    /// starts and stops with the application host lifetime.
    /// </summary>
    public class ChronoSchedulerHostedService : IHostedService
    {
        private readonly Scheduler _scheduler;

        /// <summary>
        /// Creates a new hosted service wrapping the given scheduler.
        /// </summary>
        public ChronoSchedulerHostedService(Scheduler scheduler)
        {
            _scheduler = scheduler;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _scheduler.StartAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _scheduler.StopAsync();
        }
    }
}


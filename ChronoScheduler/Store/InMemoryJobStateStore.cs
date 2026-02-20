using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ChronoScheduler.Store
{
    /// <summary>
    /// In-memory job state store. State is lost when the process exits.
    /// This is the default store used by the scheduler.
    /// </summary>
    public class InMemoryJobStateStore : IJobStateStore
    {
        private readonly ConcurrentDictionary<string, DateTimeOffset> _state
            = new ConcurrentDictionary<string, DateTimeOffset>();

        /// <inheritdoc />
        public Task<DateTimeOffset?> GetLastRunAsync(string jobId, CancellationToken cancellationToken = default)
        {
            if (_state.TryGetValue(jobId, out var lastRun))
                return Task.FromResult<DateTimeOffset?>(lastRun);

            return Task.FromResult<DateTimeOffset?>(null);
        }

        /// <inheritdoc />
        public Task SaveLastRunAsync(string jobId, DateTimeOffset utcNow, CancellationToken cancellationToken = default)
        {
            _state[jobId] = utcNow;
            return Task.CompletedTask;
        }
    }
}


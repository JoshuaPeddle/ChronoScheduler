using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChronoScheduler.Store
{
    /// <summary>
    /// Persists job execution state so that the scheduler can survive restarts.
    /// </summary>
    public interface IJobStateStore
    {
        /// <summary>
        /// Gets the last run time for a job, or null if it has never run.
        /// </summary>
        Task<DateTimeOffset?> GetLastRunAsync(string jobId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves the last run time for a job.
        /// </summary>
        Task SaveLastRunAsync(string jobId, DateTimeOffset utcNow, CancellationToken cancellationToken = default);
    }
}


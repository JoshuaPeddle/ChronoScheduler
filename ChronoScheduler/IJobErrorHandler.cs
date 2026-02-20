using System;
using System.Threading.Tasks;

namespace ChronoScheduler
{
    /// <summary>
    /// Handles errors that occur during job execution.
    /// </summary>
    public interface IJobErrorHandler
    {
        /// <summary>
        /// Called when a job throws an exception.
        /// </summary>
        /// <param name="jobId">The ID of the job that failed.</param>
        /// <param name="exception">The exception that was thrown.</param>
        Task OnErrorAsync(string jobId, Exception exception);
    }
}


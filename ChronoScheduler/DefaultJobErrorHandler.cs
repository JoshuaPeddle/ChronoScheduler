using System;
using System.Threading.Tasks;

namespace ChronoScheduler
{
    /// <summary>
    /// Default error handler that silently swallows exceptions.
    /// Replace with your own implementation to log or act on errors.
    /// </summary>
    public class DefaultJobErrorHandler : IJobErrorHandler
    {
        /// <inheritdoc />
        public Task OnErrorAsync(string jobId, Exception exception)
        {
            // Default: swallow. Users should replace this with a logging handler.
            return Task.CompletedTask;
        }
    }
}


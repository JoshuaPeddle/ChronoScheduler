using System.Threading;
using System.Threading.Tasks;

namespace ChronoScheduler
{
    /// <summary>
    /// Represents a unit of work that can be scheduled and executed by the scheduler.
    /// Implement this interface to define your job logic.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Executes the job logic.
        /// </summary>
        /// <param name="cancellationToken">Token to signal cancellation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}


using System.Threading;
using System.Threading.Tasks;

namespace ChronoScheduler.UnitTests
{
    internal class CountingJob : IJob
    {
        public bool HasExecuted { get; private set; }
        public int ExecutionCount { get; private set; }

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            HasExecuted = true;
            ExecutionCount++;
            return Task.CompletedTask;
        }
    }
}


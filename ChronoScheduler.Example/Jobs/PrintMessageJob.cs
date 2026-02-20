using System;
using System.Threading;
using System.Threading.Tasks;
using ChronoScheduler;

namespace ChronoScheduler.Example.Jobs
{
    /// <summary>
    /// A simple example job that prints a message to the console.
    /// </summary>
    public class PrintMessageJob : IJob
    {
        private readonly string _message;

        public PrintMessageJob(string message)
        {
            _message = message;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"[{DateTimeOffset.UtcNow:HH:mm:ss}] {_message}");
            return Task.CompletedTask;
        }
    }
}


using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChronoScheduler.UnitTests
{
    internal class FailingJob : IJob
    {
        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("Boom!");
        }
    }

    internal class RecordingErrorHandler : IJobErrorHandler
    {
        public string? LastJobId { get; private set; }
        public Exception? LastException { get; private set; }
        public int ErrorCount { get; private set; }

        public Task OnErrorAsync(string jobId, Exception exception)
        {
            LastJobId = jobId;
            LastException = exception;
            ErrorCount++;
            return Task.CompletedTask;
        }
    }

    public class ErrorHandlingTests
    {
        [Test]
        public async Task ErrorHandlerIsCalledOnJobFailure()
        {
            var clock = new MockClock(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
            var errorHandler = new RecordingErrorHandler();
            var scheduler = new Scheduler(clock: clock, errorHandler: errorHandler);

            scheduler.Schedule("failing-job", new FailingJob())
                .Every(TimeSpan.FromMinutes(1))
                .Build();

            await scheduler.RunStepAsync();

            Assert.Multiple(() =>
            {
                Assert.That(errorHandler.ErrorCount, Is.EqualTo(1));
                Assert.That(errorHandler.LastJobId, Is.EqualTo("failing-job"));
                Assert.That(errorHandler.LastException, Is.InstanceOf<InvalidOperationException>());
                Assert.That(errorHandler.LastException!.Message, Is.EqualTo("Boom!"));
            });
        }

        [Test]
        public async Task FailingJobDoesNotPreventOtherJobs()
        {
            var clock = new MockClock(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
            var errorHandler = new RecordingErrorHandler();
            var scheduler = new Scheduler(clock: clock, errorHandler: errorHandler);

            var healthyJob = new CountingJob();

            scheduler.Schedule("failing-job", new FailingJob())
                .Every(TimeSpan.FromMinutes(1))
                .Build();

            scheduler.Schedule("healthy-job", healthyJob)
                .Every(TimeSpan.FromMinutes(1))
                .Build();

            await scheduler.RunStepAsync();

            Assert.Multiple(() =>
            {
                Assert.That(errorHandler.ErrorCount, Is.EqualTo(1));
                Assert.That(healthyJob.ExecutionCount, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task FailedJobRecordsLastRunToPreventSpamming()
        {
            var clock = new MockClock(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
            var errorHandler = new RecordingErrorHandler();
            var scheduler = new Scheduler(clock: clock, errorHandler: errorHandler);

            scheduler.Schedule("failing-job", new FailingJob())
                .Every(TimeSpan.FromMinutes(5))
                .Build();

            // First step — fires and fails
            await scheduler.RunStepAsync();
            Assert.That(errorHandler.ErrorCount, Is.EqualTo(1));

            // Immediately again — should NOT fire because last run was just recorded
            await scheduler.RunStepAsync();
            Assert.That(errorHandler.ErrorCount, Is.EqualTo(1));

            // Advance past interval — should fire again
            clock.Advance(TimeSpan.FromMinutes(5));
            await scheduler.RunStepAsync();
            Assert.That(errorHandler.ErrorCount, Is.EqualTo(2));
        }
    }
}


using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChronoScheduler.UnitTests
{
    public class SchedulerLifecycleTests
    {
        [Test]
        public async Task StartAndStopAsync()
        {
            var clock = new MockClock(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
            var scheduler = new Scheduler(clock: clock, tickInterval: TimeSpan.FromMilliseconds(50));

            var job = new CountingJob();

            scheduler.Schedule("lifecycle-job", job)
                .Every(TimeSpan.FromMilliseconds(10))
                .Build();

            await scheduler.StartAsync();

            // Let it run a few ticks
            await Task.Delay(200);

            await scheduler.StopAsync();

            Assert.That(job.ExecutionCount, Is.GreaterThan(0));
        }

        [Test]
        public void StartingTwiceThrows()
        {
            var scheduler = new Scheduler();

            scheduler.StartAsync();

            Assert.ThrowsAsync<InvalidOperationException>(async () => await scheduler.StartAsync());

            // Cleanup
            scheduler.StopAsync().GetAwaiter().GetResult();
        }

        [Test]
        public async Task StopWhenNotStartedIsNoOp()
        {
            var scheduler = new Scheduler();
            // Should not throw
            await scheduler.StopAsync();
        }

        [Test]
        public void BuildWithoutTriggerThrows()
        {
            var scheduler = new Scheduler();
            var job = new CountingJob();

            Assert.Throws<InvalidOperationException>(() =>
                scheduler.Schedule("no-trigger", job).Build());
        }

        [Test]
        public async Task ExternalCancellationStopsScheduler()
        {
            var scheduler = new Scheduler(tickInterval: TimeSpan.FromMilliseconds(50));
            var job = new CountingJob();

            scheduler.Schedule("cancel-job", job)
                .Every(TimeSpan.FromMilliseconds(10))
                .Build();

            using var cts = new CancellationTokenSource();
            await scheduler.StartAsync(cts.Token);

            await Task.Delay(150);
            cts.Cancel();

            // Give it a moment to wind down
            await Task.Delay(100);

            var countAfterCancel = job.ExecutionCount;
            await Task.Delay(200);

            // Should not have run any more after cancellation
            Assert.That(job.ExecutionCount, Is.EqualTo(countAfterCancel));
        }
    }
}


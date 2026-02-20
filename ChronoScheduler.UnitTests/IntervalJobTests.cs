using System;
using System.Threading.Tasks;

namespace ChronoScheduler.UnitTests
{
    public class IntervalJobTests
    {
        private Scheduler _scheduler;
        private MockClock _clock;

        [SetUp]
        public void Setup()
        {
            _clock = new MockClock(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
            _scheduler = new Scheduler(clock: _clock);
        }

        [Test]
        public async Task DoesNotExecuteBeforeIntervalElapsed()
        {
            var job = new CountingJob();

            _scheduler.Schedule("job-1", job)
                .Every(TimeSpan.FromMinutes(1))
                .Build();

            // First step — job has never run, so it should fire immediately
            await _scheduler.RunStepAsync();
            Assert.That(job.ExecutionCount, Is.EqualTo(1));

            // Advance less than the interval — should not fire again
            _clock.Advance(TimeSpan.FromSeconds(30));
            await _scheduler.RunStepAsync();
            Assert.That(job.ExecutionCount, Is.EqualTo(1));
        }

        [Test]
        public async Task ExecutesAfterIntervalElapsed()
        {
            var job = new CountingJob();

            _scheduler.Schedule("job-1", job)
                .Every(TimeSpan.FromMinutes(1))
                .Build();

            // First run (immediate since never run before)
            await _scheduler.RunStepAsync();
            Assert.That(job.ExecutionCount, Is.EqualTo(1));

            // Advance past the interval
            _clock.Advance(TimeSpan.FromMinutes(1));
            await _scheduler.RunStepAsync();
            Assert.Multiple(() =>
            {
                Assert.That(job.HasExecuted, Is.True);
                Assert.That(job.ExecutionCount, Is.EqualTo(2));
            });
        }

        [Test]
        public async Task ContinuesExecutingOnSubsequentIntervals()
        {
            var job = new CountingJob();

            _scheduler.Schedule("job-1", job)
                .Every(TimeSpan.FromMinutes(1))
                .Build();

            // First run
            await _scheduler.RunStepAsync();
            Assert.That(job.ExecutionCount, Is.EqualTo(1));

            // Second run
            _clock.Advance(TimeSpan.FromMinutes(1));
            await _scheduler.RunStepAsync();
            Assert.That(job.ExecutionCount, Is.EqualTo(2));

            // Third run
            _clock.Advance(TimeSpan.FromMinutes(1));
            await _scheduler.RunStepAsync();
            Assert.That(job.ExecutionCount, Is.EqualTo(3));
        }
    }
}


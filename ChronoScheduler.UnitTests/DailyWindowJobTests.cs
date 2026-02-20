using System;
using System.Threading.Tasks;

namespace ChronoScheduler.UnitTests
{
    public class DailyWindowJobTests
    {
        private Scheduler _scheduler;
        private MockClock _clock;

        [SetUp]
        public void Setup()
        {
            // Start at midnight UTC
            _clock = new MockClock(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
            _scheduler = new Scheduler(clock: _clock);
        }

        [Test]
        public async Task ExecutesWithinWindow()
        {
            var job = new CountingJob();

            _scheduler.Schedule("daily-1", job)
                .DailyBetween(new TimeSpan(2, 0, 0), new TimeSpan(4, 0, 0))
                .Build();

            // At midnight — outside window
            await _scheduler.RunStepAsync();
            Assert.That(job.HasExecuted, Is.False);

            // At 1 AM — still outside
            _clock.Advance(TimeSpan.FromHours(1));
            await _scheduler.RunStepAsync();
            Assert.That(job.HasExecuted, Is.False);

            // At 2 AM — inside window, should fire
            _clock.Advance(TimeSpan.FromHours(1));
            await _scheduler.RunStepAsync();
            Assert.Multiple(() =>
            {
                Assert.That(job.HasExecuted, Is.True);
                Assert.That(job.ExecutionCount, Is.EqualTo(1));
            });

            // At 3 AM same day — inside window but already ran today
            _clock.Advance(TimeSpan.FromHours(1));
            await _scheduler.RunStepAsync();
            Assert.That(job.ExecutionCount, Is.EqualTo(1));

            // Next day at 2 AM — should fire again
            _clock.Advance(TimeSpan.FromHours(23));
            await _scheduler.RunStepAsync();
            Assert.That(job.ExecutionCount, Is.EqualTo(2));
        }

        [Test]
        public async Task DoesNotExecuteAfterWindow()
        {
            var job = new CountingJob();

            _scheduler.Schedule("daily-2", job)
                .DailyBetween(new TimeSpan(2, 0, 0), new TimeSpan(4, 0, 0))
                .Build();

            // Jump straight to 5 AM — past the window
            _clock.Advance(TimeSpan.FromHours(5));
            await _scheduler.RunStepAsync();
            Assert.That(job.HasExecuted, Is.False);
        }

        [Test]
        public async Task ContinuesAcrossMultipleDays()
        {
            var job = new CountingJob();

            _scheduler.Schedule("daily-3", job)
                .DailyBetween(new TimeSpan(2, 0, 0), new TimeSpan(4, 0, 0))
                .Build();

            // Day 1 at 2 AM
            _clock.Advance(TimeSpan.FromHours(2));
            await _scheduler.RunStepAsync();
            Assert.That(job.ExecutionCount, Is.EqualTo(1));

            // Day 2 at 2 AM
            _clock.Advance(TimeSpan.FromHours(24));
            await _scheduler.RunStepAsync();
            Assert.That(job.ExecutionCount, Is.EqualTo(2));
        }
    }
}


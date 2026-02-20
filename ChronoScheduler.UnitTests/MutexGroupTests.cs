using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ChronoScheduler.UnitTests
{
    /// <summary>
    /// A job that records timing information to verify mutex serialization.
    /// </summary>
    internal class TimingJob : IJob
    {
        private readonly ConcurrentBag<(DateTimeOffset Start, DateTimeOffset End)> _runs;
        private readonly TimeSpan _duration;

        public TimingJob(ConcurrentBag<(DateTimeOffset Start, DateTimeOffset End)> runs, TimeSpan duration)
        {
            _runs = runs;
            _duration = duration;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var start = DateTimeOffset.UtcNow;
            await Task.Delay(_duration, cancellationToken);
            _runs.Add((start, DateTimeOffset.UtcNow));
        }
    }

    public class MutexGroupTests
    {
        [Test]
        public async Task JobsInSameMutexGroupRunSerially()
        {
            var clock = new MockClock(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
            var scheduler = new Scheduler(clock: clock);

            var runs = new ConcurrentBag<(DateTimeOffset Start, DateTimeOffset End)>();
            var jobA = new TimingJob(runs, TimeSpan.FromMilliseconds(100));
            var jobB = new TimingJob(runs, TimeSpan.FromMilliseconds(100));

            scheduler.Schedule("a", jobA)
                .Every(TimeSpan.FromSeconds(1))
                .InMutexGroup("db-ops")
                .Build();

            scheduler.Schedule("b", jobB)
                .Every(TimeSpan.FromSeconds(1))
                .InMutexGroup("db-ops")
                .Build();

            await scheduler.RunStepAsync();

            var runList = runs.ToArray();
            Assert.That(runList, Has.Length.EqualTo(2));

            // With mutex, the runs should NOT overlap
            var first = runList[0].Start < runList[1].Start ? runList[0] : runList[1];
            var second = runList[0].Start < runList[1].Start ? runList[1] : runList[0];
            Assert.That(second.Start, Is.GreaterThanOrEqualTo(first.End));
        }

        [Test]
        public async Task JobsInDifferentGroupsRunInParallel()
        {
            var clock = new MockClock(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
            var scheduler = new Scheduler(clock: clock);

            var runs = new ConcurrentBag<(DateTimeOffset Start, DateTimeOffset End)>();
            var jobA = new TimingJob(runs, TimeSpan.FromMilliseconds(100));
            var jobB = new TimingJob(runs, TimeSpan.FromMilliseconds(100));

            scheduler.Schedule("a", jobA)
                .Every(TimeSpan.FromSeconds(1))
                .InMutexGroup("group-1")
                .Build();

            scheduler.Schedule("b", jobB)
                .Every(TimeSpan.FromSeconds(1))
                .InMutexGroup("group-2")
                .Build();

            await scheduler.RunStepAsync();

            var runList = runs.ToArray();
            Assert.That(runList, Has.Length.EqualTo(2));

            // With different groups, they should overlap (both start before either ends)
            var first = runList[0].Start < runList[1].Start ? runList[0] : runList[1];
            var second = runList[0].Start < runList[1].Start ? runList[1] : runList[0];
            Assert.That(second.Start, Is.LessThan(first.End));
        }

        [Test]
        public async Task JobsWithNoMutexGroupRunInParallel()
        {
            var clock = new MockClock(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
            var scheduler = new Scheduler(clock: clock);

            var runs = new ConcurrentBag<(DateTimeOffset Start, DateTimeOffset End)>();
            var jobA = new TimingJob(runs, TimeSpan.FromMilliseconds(100));
            var jobB = new TimingJob(runs, TimeSpan.FromMilliseconds(100));

            scheduler.Schedule("a", jobA)
                .Every(TimeSpan.FromSeconds(1))
                .Build();

            scheduler.Schedule("b", jobB)
                .Every(TimeSpan.FromSeconds(1))
                .Build();

            await scheduler.RunStepAsync();

            var runList = runs.ToArray();
            Assert.That(runList, Has.Length.EqualTo(2));

            // No mutex — they should overlap
            var first = runList[0].Start < runList[1].Start ? runList[0] : runList[1];
            var second = runList[0].Start < runList[1].Start ? runList[1] : runList[0];
            Assert.That(second.Start, Is.LessThan(first.End));
        }
    }
}


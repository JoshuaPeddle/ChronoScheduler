
namespace ChronoScheduler.UnitTests
{
    public class RecurringIntervalTests
    {
        private ChronoScheduler _chronoScheduler;
        private MockTimeService _mockTimeService;

        [SetUp]
        public void Setup()
        {
            _mockTimeService = new MockTimeService(new DateTime(2024, 1, 1, 0, 0, 0));
            _chronoScheduler = new ChronoScheduler(_mockTimeService);
        }

        [Test]
        public void DoesExecuteTaskAtScheduledTime()
        {
            var interval = new TimeInterval(0, 1);
            var taskA = new TestTask();
            var taskAArguments = new TestTaskArguments();

            _chronoScheduler.AddRecurringIntervalTask(taskA, taskAArguments, interval);

            _chronoScheduler.RunSchedulerStep();

            Assert.That(taskA.HasExecuted, Is.False);

            _mockTimeService.AdvanceTime(TimeSpan.FromMinutes(1));

            _chronoScheduler.RunSchedulerStep();
            Assert.Multiple(() =>
            {
                Assert.That(taskA.HasExecuted, Is.True);
                Assert.That(taskA.ExecutionCount, Is.EqualTo(1));
            });
        }

        [Test]
        public void DoesContinueExecutingTask()
        {
            var interval = new TimeInterval(0, 1);
            var taskA = new TestTask();
            var taskAArguments = new TestTaskArguments();

            _chronoScheduler.AddRecurringIntervalTask(taskA, taskAArguments, interval);

            _chronoScheduler.RunSchedulerStep();

            Assert.That(taskA.HasExecuted, Is.False);

            _mockTimeService.AdvanceTime(TimeSpan.FromMinutes(1));

            _chronoScheduler.RunSchedulerStep();
            Assert.Multiple(() =>
            {
                Assert.That(taskA.HasExecuted, Is.True);
                Assert.That(taskA.ExecutionCount, Is.EqualTo(1));
            });

            _mockTimeService.AdvanceTime(TimeSpan.FromMinutes(1));
            _chronoScheduler.RunSchedulerStep();
            Assert.Multiple(() =>
            {
                Assert.That(taskA.HasExecuted, Is.True);
                Assert.That(taskA.ExecutionCount, Is.EqualTo(2));
            });
        }
    }
}
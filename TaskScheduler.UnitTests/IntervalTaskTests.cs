
namespace TaskScheduler.UnitTests
{
    public class RecurringIntervalTests
    {
        private TaskScheduler _taskScheduler;
        private MockTimeService _mockTimeService;

        [SetUp]
        public void Setup()
        {
            _mockTimeService = new MockTimeService(new DateTime(2024, 1, 1, 0, 0, 0));
            _taskScheduler = new TaskScheduler(_mockTimeService);
        }

        [Test]
        public void DoesExecuteTaskAtScheduledTime()
        {
            var interval = new TimeInterval(0, 1);
            var taskA = new TestTask();
            var taskAArguments = new TestTaskArguments();

            _taskScheduler.AddRecurringIntervalTask(taskA, taskAArguments, interval);

            _taskScheduler.RunSchedulerStep();

            Assert.That(taskA.HasExecuted, Is.False);

            _mockTimeService.AdvanceTime(TimeSpan.FromMinutes(1));

            _taskScheduler.RunSchedulerStep();
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

            _taskScheduler.AddRecurringIntervalTask(taskA, taskAArguments, interval);

            _taskScheduler.RunSchedulerStep();

            Assert.That(taskA.HasExecuted, Is.False);

            _mockTimeService.AdvanceTime(TimeSpan.FromMinutes(1));

            _taskScheduler.RunSchedulerStep();
            Assert.Multiple(() =>
            {
                Assert.That(taskA.HasExecuted, Is.True);
                Assert.That(taskA.ExecutionCount, Is.EqualTo(1));
            });

            _mockTimeService.AdvanceTime(TimeSpan.FromMinutes(1));
            _taskScheduler.RunSchedulerStep();
            Assert.Multiple(() =>
            {
                Assert.That(taskA.HasExecuted, Is.True);
                Assert.That(taskA.ExecutionCount, Is.EqualTo(2));
            });
        }
    }
}
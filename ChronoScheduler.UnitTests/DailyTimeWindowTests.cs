namespace ChronoScheduler.UnitTests
{
    public class DailyTimeWindowTests
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
        public void DoesExecuteTaskWithinTimeframe()
        {
            var taskA = new TestTask();
            var taskAArguments = new TestTaskArguments();

            // Current time is Midnight
            _chronoScheduler.AddDailyTimeWindowTask(taskA, taskAArguments, new TimeSpan(2, 0, 0), new TimeSpan(4, 0, 0)); // Schedule for 2 AM

            // Set time to 1 AM, task should not execute
            _mockTimeService.AdvanceTime(TimeSpan.FromHours(1));
            _chronoScheduler.RunSchedulerStep();
            Assert.That(taskA.HasExecuted, Is.False);

            // At 2 AM, task should execute
            _mockTimeService.AdvanceTime(TimeSpan.FromHours(1));
            _chronoScheduler.RunSchedulerStep();
            Assert.Multiple(() =>
            {
                Assert.That(taskA.HasExecuted, Is.True);
                Assert.That(taskA.ExecutionCount, Is.EqualTo(1));
            });

            // At 3 AM on the same day, task should not execute again
            _mockTimeService.AdvanceTime(TimeSpan.FromHours(1));
            _chronoScheduler.RunSchedulerStep();
            Assert.That(taskA.ExecutionCount, Is.EqualTo(1));

            // On the next day at 2 AM, the task should execute again
            _mockTimeService.AdvanceTime(TimeSpan.FromHours(23));
            _chronoScheduler.RunSchedulerStep();
            Assert.That(taskA.ExecutionCount, Is.EqualTo(2));
        }

        [Test]
        public void DoesNotExecuteAfterTimeframe()
        {
            var taskA = new TestTask();
            var taskAArguments = new TestTaskArguments();

            // Current time is Midnight
            _chronoScheduler.AddDailyTimeWindowTask(taskA, taskAArguments, new TimeSpan(2, 0, 0), new TimeSpan(4, 0, 0)); // Schedule for 2 AM

            // Set time to 1 AM, task should not execute
            _mockTimeService.AdvanceTime(TimeSpan.FromHours(1));
            _chronoScheduler.RunSchedulerStep();
            Assert.That(taskA.HasExecuted, Is.False);

            // At 5 AM on the same day, task should not execute 
            _mockTimeService.AdvanceTime(TimeSpan.FromHours(4));
            _chronoScheduler.RunSchedulerStep();
            Assert.That(taskA.HasExecuted, Is.False);
        }

        [Test]
        public void DoesContinueExecutingTask()
        {
            var taskA = new TestTask();
            var taskAArguments = new TestTaskArguments();

            // Current time is Midnight
            _chronoScheduler.AddDailyTimeWindowTask(taskA, taskAArguments, new TimeSpan(2, 0, 0), new TimeSpan(4, 0, 0)); // Schedule for 2 AM

            // Set time to 1 AM, task should not execute
            _mockTimeService.AdvanceTime(TimeSpan.FromHours(1));
            _chronoScheduler.RunSchedulerStep();
            Assert.That(taskA.HasExecuted, Is.False);

            // At 2 AM, task should execute
            _mockTimeService.AdvanceTime(TimeSpan.FromHours(1));
            _chronoScheduler.RunSchedulerStep();
            Assert.Multiple(() =>
            {
                Assert.That(taskA.HasExecuted, Is.True);
                Assert.That(taskA.ExecutionCount, Is.EqualTo(1));
            });

            // On the next day at 2 AM, the task should execute again
            _mockTimeService.AdvanceTime(TimeSpan.FromHours(24));
            _chronoScheduler.RunSchedulerStep();
            Assert.That(taskA.ExecutionCount, Is.EqualTo(2));
        }
    }
}

namespace TaskScheduler.UnitTests
{
    public class TimeOfDayTests
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
        public void DoesExecuteTimeOfDayTask()
        {
            var taskA = new TestTask();
            var taskAArguments = new TestTaskArguments();

            _taskScheduler.ScheduleTimeOfDayTask(taskA, taskAArguments, new TimeSpan(2, 0, 0)); // Schedule for 2 AM

            // Before 2 AM, task should not execute
            _mockTimeService.AdvanceTime(TimeSpan.FromHours(1));
            _taskScheduler.RunSchedulerStep();
            Assert.That(taskA.HasExecuted, Is.False);

            // At 2 AM, task should execute
            _mockTimeService.AdvanceTime(TimeSpan.FromHours(1));
            _taskScheduler.RunSchedulerStep();
            Assert.Multiple(() =>
            {
                Assert.That(taskA.HasExecuted, Is.True);
                Assert.That(taskA.ExecutionCount, Is.EqualTo(1));
            });

            // After 2 AM, on the same day, task should not execute again
            _mockTimeService.AdvanceTime(TimeSpan.FromHours(1));
            _taskScheduler.RunSchedulerStep();
            Assert.That(taskA.ExecutionCount, Is.EqualTo(1));

            // On the next day at 2 AM, the task should execute again
            _mockTimeService.AdvanceTime(TimeSpan.FromHours(24));
            _taskScheduler.RunSchedulerStep();
            Assert.That(taskA.ExecutionCount, Is.EqualTo(2));
        }
    }
}

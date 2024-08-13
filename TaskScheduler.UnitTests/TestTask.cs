
namespace TaskScheduler.UnitTests
{
    internal class TestTaskArguments { }

    internal class TestTask : ITask<TestTaskArguments>
    {
        public bool HasExecuted { get; set; }
        public int ExecutionCount { get; set; }
        public void Execute(TestTaskArguments args)
        {
            HasExecuted = true;
            ExecutionCount++;
        }
    }
}
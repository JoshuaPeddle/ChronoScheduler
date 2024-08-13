namespace TaskScheduler.Example.Tasks
{
    internal class PrintArgsTaskArguments(string message)
    {
        public string Message { get; set; } = message;
    }
}
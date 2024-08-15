namespace ChronoScheduler.Example.Tasks
{
    internal class PrintArgsTask : ITask<PrintArgsTaskArguments>
    {
        public void Execute(PrintArgsTaskArguments args)
        {
            Console.WriteLine(args.Message + " " + DateTime.Now);
        }
    }
}

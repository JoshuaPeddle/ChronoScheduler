using ChronoScheduler;
using ChronoScheduler.Example.Tasks;

var _chronoScheduler = new ChronoScheduler.ChronoScheduler(new RealTimeService());

// Schedule a task to run once every minute
_chronoScheduler.AddRecurringIntervalTask(
    new PrintArgsTask(),
    new PrintArgsTaskArguments(message: "Task 1"),
    new TimeInterval(hours: 0, minutes: 1)
    );

// Schedule a task to run between 2 AM and 4 AM
_chronoScheduler.AddDailyTimeWindowTask(
    new PrintArgsTask(),
    new PrintArgsTaskArguments(message: "Task 2"),
    new TimeSpan(hours: 2, minutes: 0, seconds: 0),
    new TimeSpan(hours: 4, minutes: 0, seconds: 0));

Console.WriteLine($"Starting task scheduler at {DateTime.Now}");

_chronoScheduler.Start();


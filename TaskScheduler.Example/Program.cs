using TaskScheduler;
using TaskScheduler.Example.Tasks;

var _taskScheduler = new TaskScheduler.TaskScheduler(new RealTimeService());

// Schedule a task to run once every minute
_taskScheduler.AddRecurringIntervalTask(
    new PrintArgsTask(),
    new PrintArgsTaskArguments(message: "Task 1"),
    new TimeInterval(hours: 0, minutes: 1)
    );

// Schedule a task to run between 2 AM and 4 AM
_taskScheduler.AddDailyTimeWindowTask(
    new PrintArgsTask(),
    new PrintArgsTaskArguments(message: "Task 2"),
    new TimeSpan(hours: 2, minutes: 0, seconds: 0),
    new TimeSpan(hours: 4, minutes: 0, seconds: 0));

Console.WriteLine($"Starting task scheduler at {DateTime.Now}");

_taskScheduler.Start();


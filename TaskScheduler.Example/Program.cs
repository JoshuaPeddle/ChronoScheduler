using TaskScheduler;
using TaskScheduler.Example.Tasks;

var _taskScheduler = new TaskScheduler.TaskScheduler(new RealTimeService());

_taskScheduler.ScheduleIntervalTask(new PrintArgsTask(), new PrintArgsTaskArguments(message: "Task 1"), new TimeInterval(0, 1));

_taskScheduler.ScheduleIntervalTask(new PrintArgsTask(), new PrintArgsTaskArguments(message: "Task 2"), new TimeInterval(0, 2));
 
_taskScheduler.Start();

Console.WriteLine($"Started task scheduler at {DateTime.Now}");
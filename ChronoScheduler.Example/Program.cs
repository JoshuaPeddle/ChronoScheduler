using ChronoScheduler;
using ChronoScheduler.Example.Jobs;

var scheduler = new Scheduler();

// Schedule a job to run once every 5 seconds
scheduler.Schedule("print-fast", new PrintMessageJob("Fast job (every 5s)"))
    .Every(TimeSpan.FromSeconds(5))
    .Build();

// Schedule a job to run once every 15 seconds, in a mutex group
scheduler.Schedule("print-slow", new PrintMessageJob("Slow job (every 15s)"))
    .Every(TimeSpan.FromSeconds(15))
    .InMutexGroup("printing")
    .Build();

// Schedule a job to run between 2 AM and 4 AM UTC
scheduler.Schedule("nightly", new PrintMessageJob("Nightly maintenance"))
    .DailyBetween(new TimeSpan(2, 0, 0), new TimeSpan(4, 0, 0))
    .Build();

Console.WriteLine($"ChronoScheduler started at {DateTimeOffset.UtcNow:u}");
Console.WriteLine("Press Ctrl+C to stop.");

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

await scheduler.StartAsync(cts.Token);

try
{
    await Task.Delay(Timeout.Infinite, cts.Token);
}
catch (OperationCanceledException)
{
    // expected
}

await scheduler.StopAsync();
Console.WriteLine("Scheduler stopped.");

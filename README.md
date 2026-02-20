# ChronoScheduler

A lightweight, zero-dependency task scheduler for .NET with a fluent API, async execution, mutex groups, and pluggable persistence.

[![NuGet](https://img.shields.io/nuget/v/ChronoScheduler.svg)](https://www.nuget.org/packages/ChronoScheduler)

## Features

- **Fluent scheduling API** — readable, chainable job configuration
- **Async-first** — all jobs are `async Task`, with full `CancellationToken` support
- **Parallel execution** — jobs run concurrently by default
- **Mutex groups** — jobs in the same group are serialised, preventing conflicts
- **Pluggable persistence** — swap in Redis or EF Core to survive restarts
- **Custom triggers** — implement `ITrigger` for any scheduling logic
- **Error handling** — plug in your own `IJobErrorHandler` for logging, retries, or alerts
- **Zero dependencies** — the core package has no third-party dependencies
- **Companion packages** — Cron expressions, Redis, EF Core, and ASP.NET Core hosting

## Packages

| Package | Description | Dependencies |
|---------|-------------|-------------|
| **ChronoScheduler** | Core library | None |
| **ChronoScheduler.Cron** | Cron expression triggers | [Cronos](https://github.com/HangfireIO/Cronos) |
| **ChronoScheduler.Store.Redis** | Redis state persistence | [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) |
| **ChronoScheduler.Store.EfCore** | Database state persistence | [EF Core](https://github.com/dotnet/efcore) |
| **ChronoScheduler.Hosting** | ASP.NET Core / Generic Host integration | Microsoft.Extensions.Hosting |

## Quick Start

### 1. Define a job

```csharp
using ChronoScheduler;

public class CleanupJob : IJob
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Your job logic here
        await Database.CleanupOldRecordsAsync(cancellationToken);
    }
}
```

### 2. Schedule it

```csharp
var scheduler = new Scheduler();

scheduler.Schedule("cleanup", new CleanupJob())
    .Every(TimeSpan.FromHours(1))
    .Build();

await scheduler.StartAsync();
```

### 3. Stop it

```csharp
await scheduler.StopAsync();
```

## Scheduling Options

### Recurring interval

```csharp
scheduler.Schedule("heartbeat", new HeartbeatJob())
    .Every(TimeSpan.FromSeconds(30))
    .Build();
```

### Daily time window

Run once per day within a UTC time window:

```csharp
scheduler.Schedule("nightly-backup", new BackupJob())
    .DailyBetween(new TimeSpan(2, 0, 0), new TimeSpan(4, 0, 0))
    .Build();

// Or with TimeOnly (net6.0+):
scheduler.Schedule("nightly-backup", new BackupJob())
    .DailyBetween(new TimeOnly(2, 0), new TimeOnly(4, 0))
    .Build();
```

### Cron expressions (companion package)

```csharp
using ChronoScheduler.Cron;

scheduler.Schedule("every-five-min", new PollJob())
    .WithCron("*/5 * * * *")
    .Build();
```

### Custom triggers

```csharp
public class WeekdaysOnlyTrigger : ITrigger
{
    private readonly TimeSpan _interval;

    public WeekdaysOnlyTrigger(TimeSpan interval) => _interval = interval;

    public bool IsDue(DateTimeOffset utcNow, DateTimeOffset? lastRunUtc)
    {
        if (utcNow.DayOfWeek == DayOfWeek.Saturday || utcNow.DayOfWeek == DayOfWeek.Sunday)
            return false;

        if (lastRunUtc == null) return true;
        return utcNow - lastRunUtc.Value >= _interval;
    }
}

scheduler.Schedule("weekday-report", new ReportJob())
    .WithTrigger(new WeekdaysOnlyTrigger(TimeSpan.FromHours(1)))
    .Build();
```

## Mutex Groups

By default, all jobs run in parallel. Use mutex groups to prevent specific jobs from overlapping:

```csharp
// These two will never run at the same time:
scheduler.Schedule("db-update", new UpdateJob())
    .Every(TimeSpan.FromMinutes(10))
    .InMutexGroup("database")
    .Build();

scheduler.Schedule("db-backup", new BackupJob())
    .DailyBetween(new TimeSpan(3, 0, 0), new TimeSpan(5, 0, 0))
    .InMutexGroup("database")
    .Build();

// This job runs independently, even if the above are busy:
scheduler.Schedule("send-emails", new EmailJob())
    .Every(TimeSpan.FromMinutes(5))
    .Build();
```

## Error Handling

By default, exceptions are swallowed and the job's last-run time is still recorded (to prevent a broken job from firing every tick). Plug in your own handler:

```csharp
public class LoggingErrorHandler : IJobErrorHandler
{
    public Task OnErrorAsync(string jobId, Exception exception)
    {
        Console.Error.WriteLine($"[{jobId}] Failed: {exception.Message}");
        return Task.CompletedTask;
    }
}

var scheduler = new Scheduler(errorHandler: new LoggingErrorHandler());
```

## Persistence

By default, job state lives in memory. To survive process restarts, use a persistent store:

### Redis

```csharp
using ChronoScheduler.Store.Redis;
using StackExchange.Redis;

var redis = ConnectionMultiplexer.Connect("localhost");
var store = new RedisJobStateStore(redis);

var scheduler = new Scheduler(store: store);
```

### Entity Framework Core

```csharp
using ChronoScheduler.Store.EfCore;

var options = new DbContextOptionsBuilder<ChronoSchedulerDbContext>()
    .UseSqlServer(connectionString)
    .Options;

var db = new ChronoSchedulerDbContext(options);
await db.Database.EnsureCreatedAsync();

var store = new EfCoreJobStateStore(db);
var scheduler = new Scheduler(store: store);
```

## ASP.NET Core / Generic Host Integration

```csharp
using ChronoScheduler.Hosting;

builder.Services.AddChronoScheduler(scheduler =>
{
    scheduler.Schedule("cleanup", new CleanupJob())
        .Every(TimeSpan.FromHours(1))
        .Build();

    scheduler.Schedule("report", new ReportJob())
        .DailyBetween(new TimeSpan(8, 0, 0), new TimeSpan(9, 0, 0))
        .Build();
});
```

Or with full DI control:

```csharp
builder.Services.AddChronoScheduler(sp =>
{
    var store = sp.GetRequiredService<IJobStateStore>();
    var errorHandler = sp.GetRequiredService<IJobErrorHandler>();
    var scheduler = new Scheduler(store: store, errorHandler: errorHandler);

    scheduler.Schedule("my-job", sp.GetRequiredService<MyJob>())
        .Every(TimeSpan.FromMinutes(5))
        .Build();

    return scheduler;
});
```

## Testing

The scheduler is fully testable. Use `ISystemClock` to control time and `RunStepAsync()` to manually tick:

```csharp
public class MockClock : ISystemClock
{
    private DateTimeOffset _now;
    public MockClock(DateTimeOffset start) => _now = start;
    public DateTimeOffset UtcNow => _now;
    public void Advance(TimeSpan duration) => _now = _now.Add(duration);
}

var clock = new MockClock(DateTimeOffset.UtcNow);
var scheduler = new Scheduler(clock: clock);

scheduler.Schedule("test-job", myJob)
    .Every(TimeSpan.FromMinutes(5))
    .Build();

await scheduler.RunStepAsync(); // First run (immediate)

clock.Advance(TimeSpan.FromMinutes(5));
await scheduler.RunStepAsync(); // Second run
```

## API Reference

### Core Types

| Type | Description |
|------|-------------|
| `IJob` | Implement to define job logic (`ExecuteAsync`) |
| `Scheduler` | The main scheduler — schedules, starts, stops |
| `JobBuilder` | Fluent builder returned by `Scheduler.Schedule()` |
| `JobDefinition` | Describes a registered job (id, trigger, mutex group) |
| `ISystemClock` | Abstraction over system time for testability |
| `SystemClock` | Default real-time clock |

### Triggers

| Type | Description |
|------|-------------|
| `ITrigger` | Implement to define custom scheduling logic |
| `IntervalTrigger` | Fires at a fixed recurring interval |
| `DailyWindowTrigger` | Fires once per day within a time window |
| `CronTrigger` | Fires on a cron schedule (companion package) |

### Persistence

| Type | Description |
|------|-------------|
| `IJobStateStore` | Implement to persist job state |
| `InMemoryJobStateStore` | Default in-memory store (no persistence) |
| `RedisJobStateStore` | Redis-backed store (companion package) |
| `EfCoreJobStateStore` | EF Core-backed store (companion package) |

### Error Handling

| Type | Description |
|------|-------------|
| `IJobErrorHandler` | Implement to handle job failures |
| `DefaultJobErrorHandler` | Swallows exceptions silently |

## License

[MIT](LICENSE)

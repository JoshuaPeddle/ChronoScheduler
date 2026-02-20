# ChronoScheduler.Example

A simple console application demonstrating ChronoScheduler's fluent API.

## Running

```sh
dotnet run --project ChronoScheduler.Example
```

Press `Ctrl+C` to stop the scheduler gracefully.

## What it does

- Schedules a fast job that runs every 5 seconds
- Schedules a slow job that runs every 15 seconds in a mutex group
- Schedules a nightly job that runs between 2–4 AM UTC


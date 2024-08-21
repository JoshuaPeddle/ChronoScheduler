

# ChronoScheduler



This repository contains a simple yet powerful task-scheduling system.

## Features

- **Interval Task Scheduling**: Schedule tasks to run at specified intervals (e.g., every minute, every two hours).
- **Time Window Scheduling**: Schedule tasks to run in a time window once a day.
- **Continuous Execution**: The scheduler runs continuously in a separate thread, checking the scheduled tasks and executing them when appropriate.
- **Customizable Time Service**: Allows the use of a custom time service, enabling easier testing and flexibility.

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
      For Example project

### Installation

1. Clone the repository:

    ```sh
    git clone https://github.com/your-username/ChronoScheduler.git
    cd ChronoScheduler
    ```

2. Build the project:

    ```sh
    dotnet build
    ```

3. Run the example:

    ```sh
    dotnet run --project ChronoScheduler.Example
    ```

### Usage

The `ChronoScheduler` class is the core of this scheduling system. Below is a brief explanation of how to use it.

1. **Create the Scheduler**: Instantiate the `ChronoScheduler` with a time service.

    ```csharp
    var chronoScheduler = new ChronoScheduler(new RealTimeService());
    ```

2. **Schedule Tasks**:

    - **Recurring Interval Task**: Schedule a task to run at a regular interval.

      ```csharp
      // Schedule a task to run once every minute
      _chronoScheduler.AddRecurringIntervalTask(
          new PrintArgsTask(),
          new PrintArgsTaskArguments(message: "Task 1"),
          new TimeInterval(hours: 0, minutes: 1)
          );
      ```

    - **Time Window Task**: Schedule a task to run between specific times of the day.

      ```csharp
      // Schedule a task to run between 2 AM and 4 AM
      _chronoScheduler.AddDailyTimeWindowTask(
          new PrintArgsTask(),
          new PrintArgsTaskArguments(message: "Task 2"),
          new TimeSpan(hours: 2, minutes: 0, seconds: 0),
          new TimeSpan(hours: 4, minutes: 0, seconds: 0));
      ```

3. **Start the Scheduler**: Start the continuous execution of scheduled tasks.

    ```csharp
    chronoScheduler.Start();
    ```

### Example Project

An example project is provided in the `ChronoScheduler.Example` namespace, demonstrating how to schedule and execute tasks using the `ChronoScheduler` class.

### Designing jobs for the scheduler

Implement the `ITask<TArgs>` interface for your task logic, and create a corresponding `TArgs` class to define the arguments needed by your task.

```csharp
public class MyCustomTask : ITask<MyCustomTaskArguments>
{
    public void Execute(MyCustomTaskArguments args)
    {
        // Custom task logic here
    }
}

public class MyCustomTaskArguments
{
    public string MyArgument { get; set; }
}
```

### Contributing

Contributions are welcome! Please submit a pull request or open an issue for any suggestions or improvements.

### License

This project is licensed under the MIT License. Please take a look at the [LICENSE](LICENSE) file for details.

---

This `README.md` provides an overview of the project, explains how to set it up and use it, and includes instructions for extending the functionality and contributing to the project.

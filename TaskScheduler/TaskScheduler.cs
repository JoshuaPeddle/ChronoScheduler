using System;
using System.Collections.Generic;
using System.Threading;
using TaskScheduler.Tasks;

namespace TaskScheduler
{
    public class TaskScheduler
    {
        private readonly List<TimedTask> _tasks = new List<TimedTask>();
        private readonly ITimeService _timeService;
        private readonly int _tickInterval;

        public TaskScheduler(ITimeService timeService, int tickInterval = 1000)
        {
            _timeService = timeService;
            _tickInterval = tickInterval;
        }

        public void AddRecurringIntervalTask<TArgs>(ITask<TArgs> task, TArgs taskArguments, TimeInterval interval) where TArgs : class
        {
            _tasks.Add(new RecurringIntervalTask<TArgs>(task, taskArguments, interval, _timeService));
        }

        public void AddDailyFixedTimeTask<TArgs>(ITask<TArgs> task, TArgs taskArguments, TimeSpan timeOfDay) where TArgs : class
        {
            _tasks.Add(new DailyFixedTimeTask<TArgs>(task, taskArguments, timeOfDay));
        }

        public void AddDailyTimeWindowTask<TArgs>(ITask<TArgs> task, TArgs taskArguments, TimeSpan startTime, TimeSpan endTime) where TArgs : class
        {
            _tasks.Add(new DailyTimeWindowTask<TArgs>(task, taskArguments, startTime, endTime));
        }

        public void RunSchedulerStep()
        {
            var currentTime = _timeService.Now;

            foreach (var task in _tasks)
            {
                if (task.ShouldExecute(currentTime))
                {
                    task.Execute(currentTime);
                }
            }
        }

        public void StartContinuousScheduler()
        {
            while (true)
            {
                RunSchedulerStep();
                Thread.Sleep(_tickInterval);
            }
        }

        public void Start()
        {
            new Thread(StartContinuousScheduler).Start();
        }
    }
}
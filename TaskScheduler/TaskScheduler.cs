using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace TaskScheduler
{
    public class TaskScheduler
    {
        private readonly List<TimedTask> _timedTasks = new List<TimedTask>();
        private readonly ITimeService _timeService;
        private readonly int _tickInterval;

        public TaskScheduler(ITimeService timeService, int tickInterval = 1000)
        {
            _timeService = timeService;
            _tickInterval = tickInterval;
        }

        public void ScheduleIntervalTask<TArgs>(ITask<TArgs> task, TArgs taskArguments, TimeInterval interval) where TArgs : class
        {
            var timeIntervalTask = new TimeIntervalTask<TArgs>(task, taskArguments, interval, _timeService);
            _timedTasks.Add(timeIntervalTask);
        }

        public void ScheduleTimeOfDayTask<TArgs>(ITask<TArgs> task, TArgs taskArguments, TimeSpan timeOfDay) where TArgs : class
        {
            var timeOfDayTask = new TimeOfDayTask<TArgs>(task, taskArguments, timeOfDay);
            _timedTasks.Add(timeOfDayTask);
        }

        public void RunSchedulerStep()
        {
            var currentTime = _timeService.Now;

            foreach (var timeTask in _timedTasks)
            {
                if (timeTask.ShouldExecute(currentTime))
                {
                    timeTask.Execute(currentTime);
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
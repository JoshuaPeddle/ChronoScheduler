using System;

namespace TaskScheduler.Tasks
{
    internal class RecurringIntervalTask<TArgs> : TimedTask where TArgs : class
    {
        private readonly ITask<TArgs> _task;
        private readonly TArgs _taskArguments;
        private readonly TimeInterval _interval;
        private DateTime _lastExecutionTime;

        public RecurringIntervalTask(ITask<TArgs> task, TArgs taskArguments, TimeInterval interval, ITimeService timeService)
        {
            _task = task;
            _taskArguments = taskArguments;
            _interval = interval;
            _lastExecutionTime = timeService.Now;
        }

        public override bool ShouldExecute(DateTime currentTime)
        {
            return currentTime - _lastExecutionTime >= _interval.ToTimeSpan();
        }

        public override void Execute(DateTime currentTime)
        {
            _task.Execute(_taskArguments);
            _lastExecutionTime = currentTime;
        }
    }
}
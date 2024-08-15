using System;

namespace ChronoScheduler.Tasks
{
    internal class DailyTimeWindowTask<TArgs> : TimedTask where TArgs : class
    {
        private readonly ITask<TArgs> _task;
        private readonly TArgs _taskArguments;
        private readonly TimeSpan _startTime;
        private readonly TimeSpan _endTime;
        private DateTime _lastExecutionDate;

        public DailyTimeWindowTask(ITask<TArgs> task, TArgs taskArguments, TimeSpan startTime, TimeSpan endTime)
        {
            _task = task;
            _taskArguments = taskArguments;
            _startTime = startTime;
            _endTime = endTime;
            _lastExecutionDate = DateTime.MinValue;
        }

        public override bool ShouldExecute(DateTime currentTime)
        {
            bool isWithinWindow = currentTime.TimeOfDay >= _startTime && currentTime.TimeOfDay <= _endTime;
            bool isNewDay = _lastExecutionDate.Date < currentTime.Date;

            return isWithinWindow && isNewDay;
        }

        public override void Execute(DateTime currentTime)
        {
            _task.Execute(_taskArguments);
            _lastExecutionDate = currentTime.Date;
        }
    }
}

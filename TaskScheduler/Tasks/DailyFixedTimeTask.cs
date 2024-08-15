using System;

namespace TaskScheduler.Tasks
{
    internal class DailyFixedTimeTask<TArgs> : TimedTask where TArgs : class
    {
        private readonly ITask<TArgs> _task;
        private readonly TArgs _taskArguments;
        private readonly TimeSpan _timeOfDay;
        private bool _isFirstRun = true;
        private DateTime _lastExecutionDate;

        public DailyFixedTimeTask(ITask<TArgs> task, TArgs taskArguments, TimeSpan timeOfDay)
        {
            _task = task;
            _taskArguments = taskArguments;
            _timeOfDay = timeOfDay;
            _lastExecutionDate = DateTime.MinValue;
        }

        public override bool ShouldExecute(DateTime currentTime)
        {
            if (_isFirstRun)
            {
                _isFirstRun = false;
                return currentTime.TimeOfDay >= _timeOfDay;
            }

            _isFirstRun = false;
            bool isAfterTimeOfDay = currentTime.TimeOfDay >= _timeOfDay;
            bool isNewDay = _lastExecutionDate.Date < currentTime.Date;

            return isAfterTimeOfDay && isNewDay;
        }

        public override void Execute(DateTime currentTime)
        {
            _task.Execute(_taskArguments);
            _lastExecutionDate = currentTime.Date;
        }
    }
}

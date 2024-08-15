using System;
using System.Collections.Generic;
using System.Text;

namespace TaskScheduler.Tasks
{
    internal class DailyTimeWindowTask<TArgs> : TimedTask where TArgs : class
    {
        private readonly ITask<TArgs> _task;
        private readonly TArgs _taskArguments;
        private readonly TimeSpan _startTime;
        private readonly TimeSpan _endTime;
        private bool _isFirstRun = true;
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
            if (_isFirstRun)
            {
                _isFirstRun = false;
                return currentTime.TimeOfDay >= _startTime && currentTime.TimeOfDay <= _endTime;
            }
            _isFirstRun = false;

            bool isAfterTimeOfDay = currentTime.TimeOfDay >= _startTime && currentTime.TimeOfDay <= _endTime;
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

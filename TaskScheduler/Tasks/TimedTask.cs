using System;

namespace TaskScheduler.Tasks
{
    internal class TimedTask
    {
        public virtual bool ShouldExecute(DateTime currentTime)
        {
            return false;
        }

        public virtual void Execute(DateTime currentTime)
        {
        }
    }
}
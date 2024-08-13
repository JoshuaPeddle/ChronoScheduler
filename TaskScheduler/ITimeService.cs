using System;

namespace TaskScheduler
{
    public interface ITimeService
    {
        DateTime Now { get; }
    }
}

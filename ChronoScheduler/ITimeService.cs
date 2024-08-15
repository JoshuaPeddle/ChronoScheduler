using System;

namespace ChronoScheduler
{
    public interface ITimeService
    {
        DateTime Now { get; }
    }
}

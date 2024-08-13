using System;

namespace TaskScheduler
{
    public class RealTimeService : ITimeService
    {
        public DateTime Now => DateTime.Now;

        // No-op in real-time mode, but needed for interface compatibility
        public void AdvanceTime(TimeSpan timeSpan)
        {
            throw new NotSupportedException("Cannot advance time in real-time mode.");
        }
    }
}
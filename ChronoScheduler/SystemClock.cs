using System;

namespace ChronoScheduler
{
    /// <summary>
    /// Default clock implementation that returns the real system time.
    /// </summary>
    public class SystemClock : ISystemClock
    {
        /// <inheritdoc />
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}


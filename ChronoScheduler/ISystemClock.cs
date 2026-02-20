using System;

namespace ChronoScheduler
{
    /// <summary>
    /// Provides the current time. Abstracted to allow testing with mock clocks.
    /// </summary>
    public interface ISystemClock
    {
        /// <summary>
        /// Gets the current UTC time.
        /// </summary>
        DateTimeOffset UtcNow { get; }
    }
}


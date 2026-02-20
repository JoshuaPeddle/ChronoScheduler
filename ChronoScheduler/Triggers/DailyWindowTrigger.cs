using System;

namespace ChronoScheduler.Triggers
{
    /// <summary>
    /// Triggers a job once per day within a specified time window.
    /// </summary>
    public class DailyWindowTrigger : ITrigger
    {
        private readonly TimeSpan _windowStart;
        private readonly TimeSpan _windowEnd;

        /// <summary>
        /// Creates a new daily window trigger.
        /// </summary>
        /// <param name="windowStart">Start of the daily window (time of day, UTC).</param>
        /// <param name="windowEnd">End of the daily window (time of day, UTC). Must be after start.</param>
        /// <exception cref="ArgumentException">Thrown when the window end is not after the window start.</exception>
        public DailyWindowTrigger(TimeSpan windowStart, TimeSpan windowEnd)
        {
            if (windowEnd <= windowStart)
                throw new ArgumentException("Window end must be after window start.");

            _windowStart = windowStart;
            _windowEnd = windowEnd;
        }

#if NET6_0_OR_GREATER
        /// <summary>
        /// Creates a new daily window trigger using TimeOnly values.
        /// </summary>
        /// <param name="windowStart">Start of the daily window (UTC).</param>
        /// <param name="windowEnd">End of the daily window (UTC). Must be after start.</param>
        public DailyWindowTrigger(TimeOnly windowStart, TimeOnly windowEnd)
            : this(windowStart.ToTimeSpan(), windowEnd.ToTimeSpan())
        {
        }
#endif

        /// <inheritdoc />
        public bool IsDue(DateTimeOffset utcNow, DateTimeOffset? lastRunUtc)
        {
            var timeOfDay = utcNow.TimeOfDay;
            bool inWindow = timeOfDay >= _windowStart && timeOfDay <= _windowEnd;

            if (!inWindow)
                return false;

            // Only run once per day
            if (lastRunUtc != null && lastRunUtc.Value.Date >= utcNow.Date)
                return false;

            return true;
        }
    }
}


using System;

namespace ChronoScheduler.Triggers
{
    /// <summary>
    /// Triggers a job at a fixed recurring interval.
    /// </summary>
    public class IntervalTrigger : ITrigger
    {
        private readonly TimeSpan _interval;

        /// <summary>
        /// Creates a new interval trigger.
        /// </summary>
        /// <param name="interval">The interval between executions. Must be positive.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the interval is zero or negative.</exception>
        public IntervalTrigger(TimeSpan interval)
        {
            if (interval <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(interval), "Interval must be positive.");

            _interval = interval;
        }

        /// <inheritdoc />
        public bool IsDue(DateTimeOffset utcNow, DateTimeOffset? lastRunUtc)
        {
            if (lastRunUtc == null)
                return true;

            return utcNow - lastRunUtc.Value >= _interval;
        }
    }
}


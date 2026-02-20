using System;
using Cronos;
using ChronoScheduler.Triggers;

namespace ChronoScheduler.Cron
{
    /// <summary>
    /// A trigger that uses a cron expression to determine when a job is due.
    /// Uses the Cronos library for parsing.
    /// </summary>
    /// <remarks>
    /// Supports standard 5-field cron expressions (minute, hour, day-of-month, month, day-of-week).
    /// Examples: "*/5 * * * *" (every 5 min), "0 2 * * *" (daily at 2 AM).
    /// </remarks>
    public class CronTrigger : ITrigger
    {
        private readonly CronExpression _expression;

        /// <summary>
        /// Creates a new cron trigger from a cron expression string.
        /// </summary>
        /// <param name="cronExpression">A standard 5-field cron expression.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="cronExpression"/> is null.</exception>
        /// <exception cref="CronFormatException">Thrown when the expression is not valid.</exception>
        public CronTrigger(string cronExpression)
        {
            if (cronExpression == null)
                throw new ArgumentNullException(nameof(cronExpression));

            _expression = CronExpression.Parse(cronExpression);
        }

        /// <inheritdoc />
        public bool IsDue(DateTimeOffset utcNow, DateTimeOffset? lastRunUtc)
        {
            // Find the next occurrence after the last run (or epoch if never run).
            var after = lastRunUtc ?? utcNow.AddMinutes(-1);
            var nextOccurrence = _expression.GetNextOccurrence(after.UtcDateTime, inclusive: false);

            if (nextOccurrence == null)
                return false;

            return utcNow.UtcDateTime >= nextOccurrence.Value;
        }
    }
}


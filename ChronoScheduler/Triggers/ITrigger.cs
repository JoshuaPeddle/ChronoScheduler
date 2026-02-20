using System;

namespace ChronoScheduler.Triggers
{
    /// <summary>
    /// Determines when a job should execute based on timing rules.
    /// </summary>
    public interface ITrigger
    {
        /// <summary>
        /// Returns true if the job is due to execute at the given time.
        /// </summary>
        /// <param name="utcNow">The current UTC time.</param>
        /// <param name="lastRunUtc">The last time this job ran (null if never).</param>
        bool IsDue(DateTimeOffset utcNow, DateTimeOffset? lastRunUtc);
    }
}


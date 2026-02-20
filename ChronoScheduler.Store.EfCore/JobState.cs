using System;
using System.ComponentModel.DataAnnotations;

namespace ChronoScheduler.Store.EfCore
{
    /// <summary>
    /// Entity representing persisted job state.
    /// </summary>
    public class JobState
    {
        /// <summary>
        /// The unique job identifier (primary key).
        /// </summary>
        [Key]
        [MaxLength(256)]
        public string JobId { get; set; } = string.Empty;

        /// <summary>
        /// The last time this job was executed (UTC ticks).
        /// </summary>
        public long LastRunUtcTicks { get; set; }
    }
}


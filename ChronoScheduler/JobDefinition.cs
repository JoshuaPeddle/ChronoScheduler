using System;
using ChronoScheduler.Triggers;

namespace ChronoScheduler
{
    /// <summary>
    /// Describes a scheduled job: its identity, trigger, job instance, and concurrency group.
    /// </summary>
    public class JobDefinition
    {
        /// <summary>
        /// Unique identifier for this job.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The trigger that determines when this job should run.
        /// </summary>
        public ITrigger Trigger { get; internal set; } = null!;

        /// <summary>
        /// The job instance to execute.
        /// </summary>
        public IJob Job { get; }

        /// <summary>
        /// Optional mutex group name. Jobs in the same mutex group will not run concurrently.
        /// Null means the job has no mutex constraints and can run in parallel with anything.
        /// </summary>
        public string? MutexGroup { get; internal set; }

        internal JobDefinition(string id, IJob job)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Job = job ?? throw new ArgumentNullException(nameof(job));
        }
    }
}



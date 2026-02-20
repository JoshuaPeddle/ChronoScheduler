using System;
using ChronoScheduler.Triggers;

namespace ChronoScheduler
{
    /// <summary>
    /// Fluent builder for configuring a scheduled job.
    /// </summary>
    public class JobBuilder
    {
        private readonly JobDefinition _definition;
        private readonly Action<JobDefinition> _onBuild;

        internal JobBuilder(string id, IJob job, Action<JobDefinition> onBuild)
        {
            _definition = new JobDefinition(id, job);
            _onBuild = onBuild;
        }

        /// <summary>
        /// Schedules the job to run at a fixed interval.
        /// </summary>
        /// <param name="interval">The interval between executions.</param>
        public JobBuilder Every(TimeSpan interval)
        {
            _definition.Trigger = new IntervalTrigger(interval);
            return this;
        }

        /// <summary>
        /// Schedules the job to run once per day within a time window (UTC).
        /// </summary>
        /// <param name="start">Start of the daily window (time of day).</param>
        /// <param name="end">End of the daily window (time of day).</param>
        public JobBuilder DailyBetween(TimeSpan start, TimeSpan end)
        {
            _definition.Trigger = new DailyWindowTrigger(start, end);
            return this;
        }

#if NET6_0_OR_GREATER
        /// <summary>
        /// Schedules the job to run once per day within a time window (UTC).
        /// </summary>
        /// <param name="start">Start of the daily window.</param>
        /// <param name="end">End of the daily window.</param>
        public JobBuilder DailyBetween(TimeOnly start, TimeOnly end)
        {
            _definition.Trigger = new DailyWindowTrigger(start, end);
            return this;
        }
#endif

        /// <summary>
        /// Assigns the job to a trigger. Use this to plug in custom or third-party triggers
        /// (e.g. CronTrigger from ChronoScheduler.Cron).
        /// </summary>
        /// <param name="trigger">The trigger to use.</param>
        public JobBuilder WithTrigger(ITrigger trigger)
        {
            _definition.Trigger = trigger ?? throw new ArgumentNullException(nameof(trigger));
            return this;
        }

        /// <summary>
        /// Places this job in a mutex group. Jobs in the same group will never run concurrently.
        /// </summary>
        /// <param name="groupName">The mutex group name.</param>
        public JobBuilder InMutexGroup(string groupName)
        {
            _definition.MutexGroup = groupName ?? throw new ArgumentNullException(nameof(groupName));
            return this;
        }

        /// <summary>
        /// Finalises the job definition and registers it with the scheduler.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when no trigger has been configured.</exception>
        public JobDefinition Build()
        {
            if (_definition.Trigger == null)
                throw new InvalidOperationException(
                    $"Job '{_definition.Id}' has no trigger. Call .Every(), .DailyBetween(), or .WithTrigger() before .Build().");

            _onBuild(_definition);
            return _definition;
        }
    }
}


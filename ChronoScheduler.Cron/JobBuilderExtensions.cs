namespace ChronoScheduler.Cron
{
    /// <summary>
    /// Extension methods for adding cron triggers to the fluent builder.
    /// </summary>
    public static class JobBuilderExtensions
    {
        /// <summary>
        /// Schedules the job using a cron expression.
        /// </summary>
        /// <param name="builder">The job builder.</param>
        /// <param name="cronExpression">A standard 5-field cron expression (e.g. "0 */2 * * *").</param>
        /// <returns>The builder for further chaining.</returns>
        public static JobBuilder WithCron(this JobBuilder builder, string cronExpression)
        {
            return builder.WithTrigger(new CronTrigger(cronExpression));
        }
    }
}


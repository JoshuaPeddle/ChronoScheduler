using System;
using Microsoft.Extensions.DependencyInjection;

namespace ChronoScheduler.Hosting
{
    /// <summary>
    /// Extension methods for registering ChronoScheduler with the .NET Generic Host.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds ChronoScheduler as a hosted service. Use the <paramref name="configure"/>
        /// callback to schedule jobs via the fluent API.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">A callback to configure the scheduler (schedule jobs, set options).</param>
        /// <returns>The service collection for chaining.</returns>
        /// <example>
        /// <code>
        /// services.AddChronoScheduler(scheduler =>
        /// {
        ///     scheduler.Schedule("cleanup", new CleanupJob())
        ///         .Every(TimeSpan.FromHours(1))
        ///         .Build();
        /// });
        /// </code>
        /// </example>
        public static IServiceCollection AddChronoScheduler(
            this IServiceCollection services,
            Action<Scheduler> configure)
        {
            var scheduler = new Scheduler();
            configure(scheduler);

            services.AddSingleton(scheduler);
            services.AddHostedService<ChronoSchedulerHostedService>();

            return services;
        }

        /// <summary>
        /// Adds ChronoScheduler as a hosted service with full control over construction.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="schedulerFactory">A factory that builds and configures the Scheduler.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddChronoScheduler(
            this IServiceCollection services,
            Func<IServiceProvider, Scheduler> schedulerFactory)
        {
            services.AddSingleton(schedulerFactory);
            services.AddHostedService<ChronoSchedulerHostedService>();

            return services;
        }
    }
}


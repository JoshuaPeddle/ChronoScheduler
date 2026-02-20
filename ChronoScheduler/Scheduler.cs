using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChronoScheduler.Store;

namespace ChronoScheduler
{
    /// <summary>
    /// The core scheduler. Manages job definitions, evaluates triggers, and
    /// executes due jobs with support for parallel execution and mutex groups.
    /// </summary>
    public class Scheduler
    {
        private readonly List<JobDefinition> _jobs = new List<JobDefinition>();
        private readonly ISystemClock _clock;
        private readonly IJobStateStore _store;
        private readonly IJobErrorHandler _errorHandler;
        private readonly TimeSpan _tickInterval;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _mutexGroups
            = new ConcurrentDictionary<string, SemaphoreSlim>();

        private CancellationTokenSource? _cts;
        private Task? _runningTask;

        /// <summary>
        /// Creates a new scheduler with the specified dependencies.
        /// </summary>
        /// <param name="clock">The clock to use for time. Defaults to <see cref="SystemClock"/>.</param>
        /// <param name="store">The state store for persistence. Defaults to <see cref="InMemoryJobStateStore"/>.</param>
        /// <param name="errorHandler">The error handler. Defaults to <see cref="DefaultJobErrorHandler"/>.</param>
        /// <param name="tickInterval">How often the scheduler checks for due jobs. Defaults to 1 second.</param>
        public Scheduler(
            ISystemClock? clock = null,
            IJobStateStore? store = null,
            IJobErrorHandler? errorHandler = null,
            TimeSpan? tickInterval = null)
        {
            _clock = clock ?? new SystemClock();
            _store = store ?? new InMemoryJobStateStore();
            _errorHandler = errorHandler ?? new DefaultJobErrorHandler();
            _tickInterval = tickInterval ?? TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Gets a read-only view of all registered job definitions.
        /// </summary>
        public IReadOnlyList<JobDefinition> Jobs => _jobs.AsReadOnly();

        /// <summary>
        /// Begins building a scheduled job with the fluent API.
        /// </summary>
        /// <param name="id">A unique identifier for this job.</param>
        /// <param name="job">The job instance to execute.</param>
        /// <returns>A <see cref="JobBuilder"/> to configure the trigger and options.</returns>
        public JobBuilder Schedule(string id, IJob job)
        {
            return new JobBuilder(id, job, definition => _jobs.Add(definition));
        }

        /// <summary>
        /// Starts the scheduler loop in the background.
        /// </summary>
        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_cts != null)
                throw new InvalidOperationException("Scheduler is already running.");

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _runningTask = RunLoopAsync(_cts.Token);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Signals the scheduler to stop and waits for the current tick to complete.
        /// </summary>
        public async Task StopAsync()
        {
            if (_cts == null)
                return;

            _cts.Cancel();

            if (_runningTask != null)
            {
                try
                {
                    await _runningTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // Expected on cancellation.
                }
            }

            _cts.Dispose();
            _cts = null;
            _runningTask = null;
        }

        /// <summary>
        /// Executes a single scheduler tick — evaluates all triggers and runs due jobs.
        /// Useful for testing without starting the background loop.
        /// </summary>
        public async Task RunStepAsync(CancellationToken cancellationToken = default)
        {
            var utcNow = _clock.UtcNow;
            var tasks = new List<Task>();

            foreach (var job in _jobs)
            {
                var lastRun = await _store.GetLastRunAsync(job.Id, cancellationToken).ConfigureAwait(false);

                if (!job.Trigger.IsDue(utcNow, lastRun))
                    continue;

                tasks.Add(ExecuteJobAsync(job, utcNow, cancellationToken));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private async Task ExecuteJobAsync(JobDefinition job, DateTimeOffset utcNow, CancellationToken cancellationToken)
        {
            SemaphoreSlim? mutex = null;

            if (job.MutexGroup != null)
            {
                mutex = _mutexGroups.GetOrAdd(job.MutexGroup, _ => new SemaphoreSlim(1, 1));
                await mutex.WaitAsync(cancellationToken).ConfigureAwait(false);
            }

            try
            {
                await job.Job.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                await _store.SaveLastRunAsync(job.Id, utcNow, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                // Still record the run so we don't spam a broken job every tick.
                await _store.SaveLastRunAsync(job.Id, utcNow, cancellationToken).ConfigureAwait(false);
                await _errorHandler.OnErrorAsync(job.Id, ex).ConfigureAwait(false);
            }
            finally
            {
                mutex?.Release();
            }
        }

        private async Task RunLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await RunStepAsync(cancellationToken).ConfigureAwait(false);
                await Task.Delay(_tickInterval, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}




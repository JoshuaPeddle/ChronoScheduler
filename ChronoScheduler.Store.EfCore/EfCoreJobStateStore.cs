using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ChronoScheduler.Store.EfCore
{
    /// <summary>
    /// EF Core-backed implementation of <see cref="IJobStateStore"/>.
    /// Stores last-run times in a database table via Entity Framework Core.
    /// </summary>
    public class EfCoreJobStateStore : IJobStateStore
    {
        private readonly ChronoSchedulerDbContext _db;

        /// <summary>
        /// Creates a new EF Core job state store.
        /// </summary>
        /// <param name="db">The ChronoScheduler DbContext.</param>
        public EfCoreJobStateStore(ChronoSchedulerDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <inheritdoc />
        public async Task<DateTimeOffset?> GetLastRunAsync(string jobId, CancellationToken cancellationToken = default)
        {
            var state = await _db.JobStates
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.JobId == jobId, cancellationToken)
                .ConfigureAwait(false);

            if (state == null)
                return null;

            return new DateTimeOffset(state.LastRunUtcTicks, TimeSpan.Zero);
        }

        /// <inheritdoc />
        public async Task SaveLastRunAsync(string jobId, DateTimeOffset utcNow, CancellationToken cancellationToken = default)
        {
            var state = await _db.JobStates
                .FirstOrDefaultAsync(s => s.JobId == jobId, cancellationToken)
                .ConfigureAwait(false);

            if (state == null)
            {
                state = new JobState { JobId = jobId, LastRunUtcTicks = utcNow.UtcTicks };
                _db.JobStates.Add(state);
            }
            else
            {
                state.LastRunUtcTicks = utcNow.UtcTicks;
            }

            await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}


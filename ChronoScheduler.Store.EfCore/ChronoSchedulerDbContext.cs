using Microsoft.EntityFrameworkCore;

namespace ChronoScheduler.Store.EfCore
{
    /// <summary>
    /// EF Core DbContext for ChronoScheduler job state.
    /// Add this to your DI container or inherit from it in your own DbContext.
    /// </summary>
    public class ChronoSchedulerDbContext : DbContext
    {
        /// <summary>
        /// The job state table.
        /// </summary>
        public DbSet<JobState> JobStates { get; set; } = null!;

        /// <summary>
        /// Creates a new context with the specified options.
        /// </summary>
        public ChronoSchedulerDbContext(DbContextOptions<ChronoSchedulerDbContext> options)
            : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobState>(entity =>
            {
                entity.ToTable("ChronoSchedulerJobStates");
                entity.HasKey(e => e.JobId);
            });
        }
    }
}


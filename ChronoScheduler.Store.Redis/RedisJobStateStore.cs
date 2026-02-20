using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ChronoScheduler.Store;
using StackExchange.Redis;

namespace ChronoScheduler.Store.Redis
{
    /// <summary>
    /// Redis-backed implementation of <see cref="IJobStateStore"/>.
    /// Stores last-run times as UTC ticks in Redis hash fields.
    /// </summary>
    public class RedisJobStateStore : IJobStateStore
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly string _keyPrefix;

        /// <summary>
        /// Creates a new Redis job state store.
        /// </summary>
        /// <param name="redis">An existing Redis connection multiplexer.</param>
        /// <param name="keyPrefix">Optional prefix for Redis keys. Defaults to "chrono:".</param>
        public RedisJobStateStore(IConnectionMultiplexer redis, string keyPrefix = "chrono:")
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
            _keyPrefix = keyPrefix;
        }

        /// <inheritdoc />
        public async Task<DateTimeOffset?> GetLastRunAsync(string jobId, CancellationToken cancellationToken = default)
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(GetKey(jobId)).ConfigureAwait(false);

            if (value.IsNullOrEmpty)
                return null;

            if (long.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var ticks))
                return new DateTimeOffset(ticks, TimeSpan.Zero);

            return null;
        }

        /// <inheritdoc />
        public async Task SaveLastRunAsync(string jobId, DateTimeOffset utcNow, CancellationToken cancellationToken = default)
        {
            var db = _redis.GetDatabase();
            await db.StringSetAsync(
                GetKey(jobId),
                utcNow.UtcTicks.ToString(CultureInfo.InvariantCulture))
                .ConfigureAwait(false);
        }

        private string GetKey(string jobId) => $"{_keyPrefix}lastrun:{jobId}";
    }
}


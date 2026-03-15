using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using teamseven.EzExam.Services.Interfaces;

namespace teamseven.EzExam.Services.Services
{
    /// <summary>
    /// Redis-backed cache service with JSON serialization.
    /// All operations are wrapped in try/catch so the app continues
    /// working even if Redis goes down (graceful degradation).
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;

        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
        {
            _cache  = cache;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var bytes = await _cache.GetAsync(key);
                if (bytes is null) return default;

                return JsonSerializer.Deserialize<T>(bytes, _jsonOpts);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[Cache] GET failed for key '{Key}' — falling back to DB", key);
                return default;
            }
        }

        /// <inheritdoc/>
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                var opts = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromHours(24)
                };

                var bytes = JsonSerializer.SerializeToUtf8Bytes(value, _jsonOpts);
                await _cache.SetAsync(key, bytes, opts);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[Cache] SET failed for key '{Key}' — skipping cache write", key);
            }
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[Cache] REMOVE failed for key '{Key}'", key);
            }
        }

        /// <inheritdoc/>
        public async Task RemoveByPrefixAsync(string prefix)
        {
            // IDistributedCache does not natively support pattern delete.
            // For Redis, we access IConnectionMultiplexer via StackExchange.Redis.
            // For other providers (e.g. InMemory in tests), we skip silently.
            try
            {
                if (_cache is StackExchange.Redis.IConnectionMultiplexer)
                    return; // handled below via multiplexer

                // Try to obtain the StackExchange.Redis multiplexer through reflection
                // on the internal Redis cache implementation — safe and avoids tight coupling.
                var field = _cache.GetType()
                    .GetField("_connection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?? _cache.GetType()
                    .GetField("_cache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (field?.GetValue(_cache) is StackExchange.Redis.IConnectionMultiplexer mux)
                {
                    var db = mux.GetDatabase();
                    var server = mux.GetServers().FirstOrDefault();
                    if (server is null) return;

                    // SCAN-based pattern delete (safe for production)
                    var keys = server.Keys(pattern: $"{prefix}*").ToArray();
                    if (keys.Length > 0)
                        await db.KeyDeleteAsync(keys);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[Cache] REMOVE_BY_PREFIX failed for prefix '{Prefix}'", prefix);
            }
        }
    }
}

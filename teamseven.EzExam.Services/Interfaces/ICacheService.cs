namespace teamseven.EzExam.Services.Interfaces
{
    /// <summary>
    /// Abstraction over distributed cache (Redis) with JSON serialization.
    /// Falls back gracefully if Redis is unavailable.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>Returns cached item or default if not found / cache unavailable.</summary>
        Task<T?> GetAsync<T>(string key);

        /// <summary>Stores an item with optional TTL (defaults to 24 hours).</summary>
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);

        /// <summary>Removes a single key.</summary>
        Task RemoveAsync(string key);

        /// <summary>
        /// Removes all keys matching a prefix pattern (e.g. "textbooks:*").
        /// Uses SCAN under the hood, so safe for production.
        /// </summary>
        Task RemoveByPrefixAsync(string prefix);
    }
}

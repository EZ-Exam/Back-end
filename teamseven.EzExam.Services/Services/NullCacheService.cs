using teamseven.EzExam.Services.Interfaces;

namespace teamseven.EzExam.Services.Services
{
    /// <summary>
    /// No-op implementation of ICacheService used when the real cache
    /// is not available (unit tests, local dev without Redis, etc.).
    /// </summary>
    public sealed class NullCacheService : ICacheService
    {
        public static readonly NullCacheService Instance = new();
        private NullCacheService() { }

        public Task<T?> GetAsync<T>(string key)                          => Task.FromResult<T?>(default);
        public Task SetAsync<T>(string key, T value, TimeSpan? expiry)   => Task.CompletedTask;
        public Task RemoveAsync(string key)                               => Task.CompletedTask;
        public Task RemoveByPrefixAsync(string prefix)                    => Task.CompletedTask;
    }
}

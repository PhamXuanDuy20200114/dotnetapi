using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Api.Services
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task RemoveAsync(string key);
        Task RemoveByPrefixAsync(string prefix);
    }

    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(5);

        public CacheService(IDistributedCache cache) => _cache = cache;

        public async Task<T?> GetAsync<T>(string key)
        {
            var data = await _cache.GetStringAsync(key);
            return data == null ? default : JsonSerializer.Deserialize<T>(data);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? DefaultExpiry
            };
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(value), options);
        }

        public async Task RemoveAsync(string key) => await _cache.RemoveAsync(key);

        public async Task RemoveByPrefixAsync(string prefix)
        {
            // Xóa các key liên quan khi data thay đổi
            await _cache.RemoveAsync($"{prefix}:all");
        }
    }
}

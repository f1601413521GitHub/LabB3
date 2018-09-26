using System;
using System.Runtime.Caching;

namespace ThinkPower.LabB3.Domain.Resources
{
    /// <summary>
    /// 資料暫存類別
    /// </summary>
    public class CacheProvider
    {
        private static ObjectCache _cache = MemoryCache.Default;

        public static object GetCache(string cacheKey)
        {
            return _cache[cacheKey];
        }

        public static void SetCache(string cacheKey, object contents)
        {
            _cache.Set(cacheKey, contents, new CacheItemPolicy()
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(20),
            });
        }
    }
}
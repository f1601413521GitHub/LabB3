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

        public static string state { get; private set; }

        public static object GetCache(string key, object data, bool overwrite = false, CacheItemPolicy policy = null)
        {
            object result = _cache[key];
            state = "get";

            if (result == null || overwrite)
            {
                _cache.Set(key, data, policy ?? new CacheItemPolicy()
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(20),
                });

                state = "set";
                result = data;
            }

            return result;
        }
    }
}
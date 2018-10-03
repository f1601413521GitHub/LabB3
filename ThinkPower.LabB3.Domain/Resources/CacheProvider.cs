using System;
using System.Runtime.Caching;

namespace ThinkPower.LabB3.Domain.Resources
{
    /// <summary>
    /// 資料暫存服務供應類別
    /// </summary>
    public class CacheProvider
    {
        private static ObjectCache _cache = MemoryCache.Default;

        public static string state { get; private set; }

        /// <summary>
        /// 取得暫存資料
        /// </summary>
        /// <param name="cacheKey">暫存資料提取鍵值</param>
        /// <returns>暫存資料</returns>
        public static object GetCache(string cacheKey)
        {
            state = "get";
            return _cache[cacheKey];
        }


        /// <summary>
        /// 設定暫存資料
        /// </summary>
        /// <param name="cacheKey">暫存資料提取鍵值</param>
        /// <param name="data">暫存資料</param>
        /// <param name="overwrite">是否覆蓋</param>
        /// <param name="policy">暫存資料回收設定</param>
        /// <returns>暫存資料</returns>
        public static object SetCache(string cacheKey, object data, bool overwrite = false, CacheItemPolicy policy = null)
        {
            object result = null;

            if (data != null)
            {
                _cache.Set(cacheKey, data, policy ?? new CacheItemPolicy()
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
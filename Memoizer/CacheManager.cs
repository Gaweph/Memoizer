using System;
using System.Collections.Generic;
//using System.Runtime.Serialization.Json;
//using System.Text;
//using Newtonsoft.Json;
//using Json.Net

namespace Memoizer
{
    internal class CacheManager
    {
        private readonly Dictionary<string, (DateTimeOffset? Expiry, object CachedValue)> cachedResults = new Dictionary<string, (DateTimeOffset? Expiry, object cachedValue)>();

        public bool TryGetCachedValue(string key, out object cachedValue)
        {
            var hasValue = cachedResults.TryGetValue(key, out var value);
            if (hasValue)
            {
                if (value.Expiry == null || value.Expiry > DateTimeOffset.UtcNow)
                {
                    cachedValue = value.CachedValue;
                    return true;
                }
            }
            cachedValue = null;
            return false;
        }

        public void AddCachedValue(string key, object cachedValue, DateTimeOffset? expiry = null)
        {
            cachedResults[key] = (Expiry: expiry, CachedValue: cachedValue);
        }
    }

}

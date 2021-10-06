using System;
using System.Collections.Generic;
using System.Linq;
using AspectInjector.Broker;
using Newtonsoft.Json;

namespace Memoizer
{
    [Aspect(Scope.Global)]
    public class MemoizeAspect
    {
        private JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        private Dictionary<string, (DateTimeOffset? Expiry, object CachedValue)> cachedResults = new Dictionary<string, (DateTimeOffset? Expiry, object cachedValue)>();

        private bool TryGetCachedValue(string key, out object cachedValue)
        {
            var hasValue = cachedResults.TryGetValue(key, out var value);
            if(hasValue)
            {
                if(value.Expiry == null || value.Expiry > DateTimeOffset.UtcNow)
                {
                    cachedValue = value.CachedValue;
                    return true;
                }
            }
            cachedValue = null;
            return false;
        }

        private void AddCachedValue(string key, CacheAttribute cacheAttribute, object cachedValue)
        {
            var expiry =
                cacheAttribute.MilliSeconds == null ?
                    null :
                    (DateTimeOffset?)DateTimeOffset.UtcNow.AddMilliseconds((int)cacheAttribute.MilliSeconds);
            cachedResults[key] = (Expiry: expiry, CachedValue: cachedValue);
        }

        [Advice(Kind.Around, Targets = Target.Method)]
        public object HandleMethod(
               [Argument(Source.Name)] string name,
               [Argument(Source.Arguments)] object[] arguments,
               [Argument(Source.Target)] Func<object[], object> method,
               [Argument(Source.Triggers)] Attribute[] triggers)
        {
            var type = method.Target.GetType().FullName;
            var key = $"{type}-{name}-{JsonConvert.SerializeObject(arguments, _settings)}";

            lock (cachedResults)
            {
                if (TryGetCachedValue(key, out var cachedValue))
                {
                    return cachedValue;
                }

                var cachedAttribute = triggers.OfType<CacheAttribute>().First();
                var result = method(arguments);
                AddCachedValue(key, cachedAttribute, result);
                return result;
            }
        }
    }
}

using System;
using System.IO;
using System.Linq;
//using System.Runtime.Serialization.Json;
//using System.Text;
using AspectInjector.Broker;
//using Newtonsoft.Json;
//using Json.Net

namespace Memoizer
{

    [Aspect(Scope.Global)]
    public class MemoizeAspect
    {
        private readonly CacheManager cache = new CacheManager();

        private string GetKey(object target, Type type, string methodName, object[] arguments)
        {
            return $"{target.GetHashCode()}-{type.FullName}-{methodName}-{SerializeHelper.Serialize(arguments)}";
        }
        
        [Advice(Kind.Around, Targets = Target.Method)]
        public object HandleMethod(
               [Argument(Source.Name)] string name,
               [Argument(Source.Arguments)] object[] arguments,
               [Argument(Source.Target)] Func<object[], object> method,
               [Argument(Source.Triggers)] Attribute[] triggers)
        {
            var target = method.Target;
            var key = GetKey(target, target.GetType(), name, arguments);
            lock (cache)
            {
                if (cache.TryGetCachedValue(key, out var cachedValue))
                {
                    return cachedValue;
                }

                var result = method(arguments);
                var expiry = triggers.OfType<CacheAttribute>().First().ExpiryFromNow;
                cache.AddCachedValue(key, result, expiry);
                return result;
            }
        }
    }

}

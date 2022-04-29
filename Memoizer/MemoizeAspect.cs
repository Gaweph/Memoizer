using System;
using System.IO;
using System.Linq;
//using System.Runtime.Serialization.Json;
//using System.Text;
using AspectInjector.Broker;
//using Newtonsoft.Json;
//using Json.Net
using System.Runtime.Serialization;

namespace Memoizer
{

    [Aspect(Scope.Global)]
    public class MemoizeAspect
    {
        private readonly CacheManager cache = new CacheManager();
        private readonly ObjectIDGenerator idGenerator = new ObjectIDGenerator();
        private string GetKey(object target, Type type, string methodName, object[] arguments)
        {
            long id = 0;
            if (target != null)
            {
                id = idGenerator.GetId(target, out var firstTime);
            }
            return $"{id}-{type.FullName}-{methodName}-{SerializeHelper.Serialize(arguments)}";
        }

        [Advice(Kind.Around, Targets = Target.Method)]
        public object HandleMethod(
               [Argument(Source.Name)] string name,
               [Argument(Source.Arguments)] object[] arguments,
               [Argument(Source.Target)] Func<object[], object> method,
               [Argument(Source.Triggers)] Attribute[] triggers)
        {
            var type = method.Target?.GetType() ?? method.Method.DeclaringType;
            var key = GetKey(method.Target, type, name, arguments);
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

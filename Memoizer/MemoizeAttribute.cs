using System;
using System.Collections.Generic;
using System.Linq;
using AspectInjector.Broker;
using Newtonsoft.Json;

namespace Memoizer
{
    /// <summary>
    /// Result of the method call is cached for future calls to method with equivalent input arguments
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [Injection(typeof(MemoizeAspect))]
    public class MemoizeAttribute : Attribute
    {
    }

    [Aspect(Scope.Global)]
    public class MemoizeAspect
    {
        private JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        private Dictionary<string, object> cachedResults = new Dictionary<string, object>();
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
                if (cachedResults.ContainsKey(key))
                {
                    return cachedResults[key];
                }
                var result = method(arguments);
                cachedResults[key] = result;
                return result;
            }
        }
    }
}

using System;
using AspectInjector.Broker;

namespace Memoizer
{
    /// <summary>
    /// Result of the method call is cached for future calls to method with equivalent input arguments
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [Injection(typeof(MemoizeAspect))]
    public class CacheAttribute : Attribute
    {
        public readonly int? MilliSeconds;
        public CacheAttribute()
        {
            MilliSeconds = null;
        }
        public CacheAttribute(int milliSeconds, Time unit = Time.Millisecond)
        {
            if (unit == Time.Second)
            {
                MilliSeconds = milliSeconds * 1000;
            }
            else
            {
                MilliSeconds = milliSeconds;
            }
        }
    }
}

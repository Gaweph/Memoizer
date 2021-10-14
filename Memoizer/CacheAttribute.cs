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
        private readonly int? MilliSeconds;
        public CacheAttribute()
        {
            MilliSeconds = null;
        }
        public CacheAttribute(int expiry, Time unit = Time.Millisecond)
        {
            if (unit == Time.Second)
            {
                MilliSeconds = expiry * 1000;
            }
            else
            {
                MilliSeconds = expiry;
            }
        }

        public DateTimeOffset? ExpiryFromNow => MilliSeconds == null ? null : (DateTimeOffset?)DateTimeOffset.UtcNow.AddMilliseconds((int)MilliSeconds);
    }
}

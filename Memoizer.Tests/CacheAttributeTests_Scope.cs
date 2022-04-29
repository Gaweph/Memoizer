using FluentAssertions;
using System;
using Xunit;

namespace Memoizer.Tests
{
    public class CacheAttributeTests_Scope
    {
        private class DemoScopeClass
        {
            private readonly string _payload;
            public int Count = 0;
            public DemoScopeClass(string payload)
            {
                _payload = payload;
            }

            [Cache]
            public (string Payload, Guid Guid) Get()
            {
                Count++;
                return (_payload, Guid.NewGuid());
            }

            [Cache]
            public static Guid GetStatic()
            {
                return Guid.NewGuid();
            }
        }

        [Fact]
        public void Get__Should_ReturnCorrectInstanceOfCache()
        {
            // ARRANGE
            var demo = new DemoScopeClass("abc");
            var demo2 = new DemoScopeClass("xyz");

            // ACT
            var res = demo.Get();
            var res2 = demo2.Get();

            // ASSERT
            demo.Count.Should().Be(1);
            demo2.Count.Should().Be(1);
            res.Payload.Should().Be("abc");
            res2.Payload.Should().Be("xyz");
            res.Guid.Should().NotBe(res2.Guid);
        }
        [Fact]
        public void GetStatic__Should_ReturnCachedVersion_When_CalledOnDIfferentInstances()
        {
            // ARRANGE & ACT
            var res = DemoScopeClass.GetStatic();
            var res2 = DemoScopeClass.GetStatic();

            // ASSERT
            res.Should().Be(res2);
        }
        
    }
}

﻿using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace Memoizer.Tests
{
    public static class DemoExtensionClass
    {
        [Cache]
        public static (string Payload, Guid Guid) ExtensionMethod(this CacheAttributeTests_Scope.DemoScopeClass obj)
        {
            obj.Count++;
            return (obj.Payload, Guid.NewGuid());
        }
    }

    public class CacheAttributeTests_Scope
    {
        public class DemoScopeClass
        {
            public readonly string Payload;
            public int Count = 0;
            public DemoScopeClass(string payload)
            {
                Payload = payload;
            }

            [Cache]
            public (string Payload, Guid Guid) Get()
            {
                Count++;
                return (Payload, Guid.NewGuid());
            }

            [Cache]
            public static Guid GetStatic()
            {
                return Guid.NewGuid();
            }

        }


        [Fact]
        public void Get__Should_ReturnCacchedValue_When_SameInstance()
        {
            // ARRANGE
            var payload = "abc";
            var demo = new DemoScopeClass(payload);

            // ACT
            var res1 = demo.Get();
            var res2 = demo.Get();
            var res3 = demo.Get();
            var res4 = demo.Get();

            // ASSERT
            res1.Payload.Should().Be(payload);
            res2.Payload.Should().Be(payload);
            res3.Payload.Should().Be(payload);
            res4.Payload.Should().Be(payload);

            var guids = new[] { res1.Guid, res2.Guid, res3.Guid, res4.Guid };
            guids.Distinct().Should().HaveCount(1);
            demo.Count.Should().Be(1);
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
            res.Payload.Should().Be("abc");
            res2.Payload.Should().Be("xyz");
            res.Guid.Should().NotBe(res2.Guid);
            demo.Count.Should().Be(1);
            demo2.Count.Should().Be(1);
        }
        [Fact]
        public void GetStatic__Should_ShareCacheAcrossDifferentInstances()
        {
            // ARRANGE & ACT
            var res = DemoScopeClass.GetStatic();
            var res2 = DemoScopeClass.GetStatic();

            // ASSERT
            res.Should().Be(res2);
        }

        [Fact]
        public void ExtensionMethod__Should_ShareCacheAcrossDifferentInstances()
        {
            // ARRANGE
            var demo = new DemoScopeClass("abc");
            var demo2 = new DemoScopeClass("xyz");

            // ACT
            var res = demo.ExtensionMethod();
            var res2 = demo2.ExtensionMethod();

            // ASSERT
            res.Should().Be(res2);
            demo.Count.Should().Be(1);
            demo2.Count.Should().Be(0); // Shouldn't have called anything on Demo2
        }
        
    }
}

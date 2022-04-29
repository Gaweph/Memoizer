using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Memoizer.Tests
{
    public class CacheAttributeExpiryTests
    {
        private class TestExpiryClass
        {
            public int Count = 0;
            [Cache(500)]
            public Guid HalfSecondCache(string str)
            {
                Count++;
                return Guid.NewGuid();
            }

            [Cache(1, Time.Second)]
            public Guid OneSecondCache(string str)
            {
                Count++;
                return Guid.NewGuid();
            }
        }

        [Fact]
        public async Task HalfSecondCache__Should_ReturnCachedResult_When_CacheNotExpired()
        {
            // ARRANGE
            var demo = new TestExpiryClass();
            var str = "HalfSecondCache_200";

            // ACT 
            var res = demo.HalfSecondCache(str);
            await Task.Delay(200);
            var res2 = demo.HalfSecondCache(str);

            // ASSERT
            demo.Count.Should().Be(1);
            res.Should().Be(res2);
        }

        [Fact]
        public async Task HalfSecondCache__Should_ReturnNonCachedResult_When_CacheExpired()
        {
            // ARRANGE
            var demo = new TestExpiryClass();
            var str = "HalfSecondCache_550";

            // ACT 
            var res = demo.HalfSecondCache(str);
            await Task.Delay(550);
            var res2 = demo.HalfSecondCache(str);

            // ASSERT
            demo.Count.Should().Be(2);
            res.Should().NotBe(res2);
        }

        [Fact]
        public async Task OneSecondCache__Should_ReturnCachedResult_When_CacheNotExpired()
        {
            // ARRANGE
            var demo = new TestExpiryClass();
            var str = "OneSecondCache_500";

            // ACT 
            var res = demo.OneSecondCache(str);
            await Task.Delay(500);
            var res2 = demo.OneSecondCache(str);

            // ASSERT
            demo.Count.Should().Be(1);
            res.Should().Be(res2);
        }

        [Fact]
        public async Task OneSecondCache__Should_ReturnNonCachedResult_When_CacheExpired()
        {
            // ARRANGE
            var demo = new TestExpiryClass();
            var str = "OneSecondCache_1200";

            // ACT 
            var res = demo.OneSecondCache(str);
            await Task.Delay(1200);
            var res2 = demo.OneSecondCache(str);

            // ASSERT
            demo.Count.Should().Be(2);
            res.Should().NotBe(res2);
        }
    }
}

using FluentAssertions;
using Xunit;
using System.Numerics;

namespace Memoizer.Tests
{
    public class FibonacciTest
    {
        [Memoize]
        public BigInteger Fibonacci(int n)
        {
            if (n < 2) return n;
            return Fibonacci(n - 1) + Fibonacci(n - 2);
        }

        [Fact]
        public void Fibonacci__Should_Return_Expected()
        {
            // ARRANGE
            var n = 7;
            var expected = 13;

            // ACT 
            var res = Fibonacci(n);

            // ASSERT
            res.Should().Be(expected);
        }

        [Theory]
        [InlineData(100, "354224848179261915075")]
        [InlineData(200, "280571172992510140037611932413038677189525")]
        [InlineData(300, "222232244629420445529739893461909967206666939096499764990979600")]
        public void Fibonacci__Should_Return_Expected_When_N_Is_Large(int n, string expected)
        {
            // ACT 
            var res = Fibonacci(n);

            // ASSERT
            res.Should().Be(BigInteger.Parse(expected));
        }
    }
}

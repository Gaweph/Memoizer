using FluentAssertions;
using Xunit;
using System.Numerics;

namespace Memoizer.Tests
{
    public class FibonacciTest
    {
        [Cache]
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
            var actual = Fibonacci(n);

            // ASSERT
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(100, "354224848179261915075")]
        [InlineData(200, "280571172992510140037611932413038677189525")]
        [InlineData(300, "222232244629420445529739893461909967206666939096499764990979600")]
        public void Fibonacci__Should_Return_Expected_When_N_Is_Large(int n, string shouldBe)
        {
            // ACT 
            var actual = Fibonacci(n);
            var expected = BigInteger.Parse(shouldBe);

            // ASSERT
            actual.Should().Be(expected);
        }
    }
}

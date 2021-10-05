using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Memoizator.Tests
{

    public class MemoizeAttributeTests
    {
        public class DemoClass
        {
            private int count = 0;

            [Memoize]
            public int TestMethod(string str)
            {
                count++;
                return count;
            }

            [Memoize]
            public int ComplexArgs
                (
                object obj,
                (int Foo, string Bar) tuple,
                IEnumerable<object> list,
                int[] arr,
                Guid guid,
                string str
                )
            {
                count++;
                return count;
            }
        }

        [Fact]
        public void TestMethod__Should_ReturnCachedResult_When_SameInput()
        {
            // ARRANGE
            var demo = new DemoClass();

            // ACT 
            var res = demo.TestMethod("A");
            var res2 = demo.TestMethod("A");

            // ASSERT
            res.Should().Be(1);
            res2.Should().Be(1);
        }

        [Fact]
        public void TestMethod__Should_ReturnNonCachedResult_When_DiffInput()
        {
            // ARRANGE
            var demo = new DemoClass();

            // ACT 
            var res = demo.TestMethod("A");
            var res2 = demo.TestMethod("B");

            // ASSERT
            res.Should().Be(1);
            res2.Should().Be(2);
        }

        [Fact]
        public void TestMethod__Should_ReturnCachedResult_When_ComplexParameters_With_SameInput()
        {
            // ARRANGE
            var demo = new DemoClass();
            var obj = DateTime.Now;
            var tuple = (Foo: 35, Bar: "Ace 'What a Guy' Rimmer");
            var list = new object[] { DateTime.Now.AddYears(-36), 123, "A_B_C", true };
            var arr = new[] { 1, 2, 3 };
            var guid = Guid.NewGuid();
            var str = "(╯°□°)╯︵ ┻━┻";

            // ACT 
            var res = demo.ComplexArgs(obj, tuple, list, arr, guid, str);
            var res2 = demo.ComplexArgs(obj, tuple, list, arr, guid, str);

            // ASSERT
            res.Should().Be(1);
            res2.Should().Be(1);
        }
    }
}

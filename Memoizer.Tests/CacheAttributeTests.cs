using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Memoizer.Tests
{
    public class CacheAttributeTests
    {
        private class DemoClass
        {
            public int Count = 0;

            [Cache]
            public Guid SimpleMethod()
            {
                Count++;
                return Guid.NewGuid();
            }

            [Cache]
            public Guid TestMethod(string str)
            {
                Count++;
                return Guid.NewGuid();
            }

            [Cache]
            public Guid ComplexArgs
                (
                object obj,
                (int Foo, string Bar) tuple,
                IEnumerable<object> list,
                int[] arr,
                Guid guid,
                string str
                )
            {
                Count++;
                return Guid.NewGuid();
            }

            [Cache]
            public Guid ObjectArgs(object obj)
            {
                Count++;
                return Guid.NewGuid();
            }

            [Cache]
            public Guid GenericArg<T>(T obj)
            {
                Count++;
                return Guid.NewGuid();
            }
        }


        [Fact]
        public void SimpleMethod__Should_ReturnCachedResult_When_Second_Call()
        {
            // ARRANGE
            var demo = new DemoClass();

            // ACT 
            var res = demo.SimpleMethod();
            var res2 = demo.SimpleMethod();

            // ASSERT
            demo.Count.Should().Be(1);
            res.Should().Be(res2);
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
            demo.Count.Should().Be(1);
            res.Should().Be(res2);
        }

        [Fact]
        public void TestMethod__Should_ReturnNonCachedResult_When_DiffInput()
        {
            // ARRANGE
            var demo = new DemoClass();

            // ACT 
            var res = demo.TestMethod("X");
            var res2 = demo.TestMethod("Y");

            // ASSERT
            demo.Count.Should().Be(2);
            res.Should().NotBe(res2);
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
            demo.Count.Should().Be(1);
            res.Should().Be(res2);
        }
        
        private class ObjectWithRecurringReference
        {
            public string Name { get; set; }
            public ObjectWithRecurringReference Object { get; set; }
        }
        [Fact]
        public async Task ObjectArgs__Should_ReturnCachedResult_When_SelfReferencingObject()
        {
            // ARRANGE
            var demo = new DemoClass();

            var objA = new ObjectWithRecurringReference
            {
                Name = "Object a"
            };

            var objB = new ObjectWithRecurringReference
            {
                Name = "Object B"
            };

            // self referencing 
            objA.Object = objB;
            objB.Object = objA;

            // ACT 
            var res = demo.ObjectArgs(objA);
            var res2 = demo.ObjectArgs(objA);

            // ASSERT
            demo.Count.Should().Be(1);
            res.Should().Be(res2);
        }

        [Fact]
        public void GenericArg__Should_ReturnCachedResult_When_SameInput()
        {
            // ARRANGE
            var demo = new DemoClass();

            // ACT 
            var res = demo.GenericArg(new { Name = "Dave" });
            var res2 = demo.GenericArg(new { Name = "Dave" });

            // ASSERT
            demo.Count.Should().Be(1);
            res.Should().Be(res2);
        }

        [Fact]
        public void GenericArg__Should_ReturnNonCachedResult_When_DiffInput()
        {
            // ARRANGE
            var demo = new DemoClass();

            // ACT 
            var res = demo.GenericArg(new { Name = "Alice" });
            var res2 = demo.GenericArg(new { Name = "Bob" });

            // ASSERT
            demo.Count.Should().Be(2);
            res.Should().NotBe(res2);
        }
        
    }
}

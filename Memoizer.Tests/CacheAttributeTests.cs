using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Memoizer.Tests
{

    public class CacheAttributeTests
    {
        public class DemoClass
        {
            private int count = 0;

            [Cache]
            public int SimpleMethod() => ++count;

            [Cache]
            public int TestMethod(string str)
            {
                count++;
                return count;
            }

            [Cache]
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

            [Cache(500)]
            public int HalfSecondCache(string str)
            {
                count++;
                return count;
            }

            [Cache(1, Time.Second)]
            public int OneSecondCache(string str)
            {
                count++;
                return count;
            }

            [Cache]
            public int ObjectArgs(object obj)
            {
                count++;
                return count;
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
            res.Should().Be(1);
            res2.Should().Be(1);
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
            var res = demo.TestMethod("X");
            var res2 = demo.TestMethod("Y");

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

        [Fact]
        public async Task HalfSecondCache__Should_ReturnCachedResult_When_CacheNotExpired()
        {
            // ARRANGE
            var demo = new DemoClass();
            var str = "HalfSecondCache_200";

            // ACT 
            var res = demo.HalfSecondCache(str);
            await Task.Delay(200);
            var res2 = demo.HalfSecondCache(str);

            // ASSERT
            res.Should().Be(1);
            res2.Should().Be(1);
        }

        [Fact]
        public async Task HalfSecondCache__Should_ReturnNonCachedResult_When_CacheExpired()
        {
            // ARRANGE
            var demo = new DemoClass();
            var str = "HalfSecondCache_550";

            // ACT 
            var res = demo.HalfSecondCache(str);
            await Task.Delay(550);
            var res2 = demo.HalfSecondCache(str);

            // ASSERT
            res.Should().Be(1);
            res2.Should().Be(2);
        }

        [Fact]
        public async Task OneSecondCache__Should_ReturnCachedResult_When_CacheNotExpired()
        {
            // ARRANGE
            var demo = new DemoClass();
            var str = "OneSecondCache_500";

            // ACT 
            var res = demo.OneSecondCache(str);
            await Task.Delay(500);
            var res2 = demo.OneSecondCache(str);

            // ASSERT
            res.Should().Be(1);
            res2.Should().Be(1);
        }

        [Fact]
        public async Task OneSecondCache__Should_ReturnNonCachedResult_When_CacheExpired()
        {
            // ARRANGE
            var demo = new DemoClass();
            var str = "OneSecondCache_1200";

            // ACT 
            var res = demo.OneSecondCache(str);
            await Task.Delay(1200);
            var res2 = demo.OneSecondCache(str);

            // ASSERT
            res.Should().Be(1);
            res2.Should().Be(2);
        }

        private class ObjectWithReference
        {
            public string Name { get; set; }
            public ObjectWithReference Object { get; set; }
        }
        [Fact]
        public async Task ObjectArgs__Should_ReturnCachedResult_When_SelfReferencingObject()
        {
            // ARRANGE
            var demo = new DemoClass();

            var objA = new ObjectWithReference
            {
                Name = "Object a"
            };

            var objB = new ObjectWithReference
            {
                Name = "Object B",
                Object = objA
            };

            objA.Object = objB; // self referencing - referencing ObjA with reference ObjA again

            // ACT 
            var res = demo.ObjectArgs(objA);
            var res2 = demo.ObjectArgs(objA);

            // ASSERT
            res.Should().Be(1);
            res2.Should().Be(1);
        }
     
        
        [Fact]
        public void Cache_is_not_used_for_different_instances_of_same_type()
        {
            var one = new MyClass("hello");
            var another = new MyClass("how are you?");

            one.Get().Should().NotBeEquivalentTo(another.Get());
        }

        private class MyClass
        {
            private readonly string payload;

            public MyClass(string payload)
            {
                this.payload = payload;
            }

            [Cache]
            public string Get() => payload;
        }
    }
}

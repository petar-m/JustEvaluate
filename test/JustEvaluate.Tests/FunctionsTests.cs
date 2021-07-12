using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentAssertions;
using Xunit;

namespace JustEvaluate.Tests
{

    public class FunctionsTests
    {
        [Fact]
        public void Function_With_0_Parameters_IsStored()
        {
            Expression<Func<decimal>> func = () => 1m;
            var functions = new Functions(allowOverride: false);

            functions.Add("func 0", func);
            var storedFunc = functions.Get("func 0", 0);

            storedFunc.Should().Be(func);
        }

        [Fact]
        public void Function_With_1_Parameters_IsStored()
        {
            Expression<Func<decimal, decimal>> func = (p1) => 1m;
            var functions = new Functions(allowOverride: false);

            functions.Add("func 1", func);
            var storedFunc = functions.Get("func 1", 1);

            storedFunc.Should().Be(func);
        }

        [Fact]
        public void Function_With_2_Parameters_IsStored()
        {
            Expression<Func<decimal, decimal, decimal>> func = (p1, p2) => 1m;
            var functions = new Functions(allowOverride: false);

            functions.Add("func 2", func);
            var storedFunc = functions.Get("func 2", 2);

            storedFunc.Should().Be(func);
        }

        [Fact]
        public void Function_With_3_Parameters_IsStored()
        {
            Expression<Func<decimal, decimal, decimal, decimal>> func = (p1, p2, p3) => 1m;
            var functions = new Functions(allowOverride: false);

            functions.Add("func 3", func);
            var storedFunc = functions.Get("func 3", 3);

            storedFunc.Should().Be(func);
        }

        [Fact]
        public void Function_With_4_Parameters_IsStored()
        {
            Expression<Func<decimal, decimal, decimal, decimal, decimal>> func = (p1, p2, p3, p4) => 1m;
            var functions = new Functions(allowOverride: false);

            functions.Add("func 4", func);
            var storedFunc = functions.Get("func 4", 4);

            storedFunc.Should().Be(func);
        }

        [Fact]
        public void Function_With_5_Parameters_IsStored()
        {
            Expression<Func<decimal, decimal, decimal, decimal, decimal, decimal>> func = (p1, p2, p3, p4, p5) => 1m;
            var functions = new Functions(allowOverride: false);

            functions.Add("func 5", func);
            var storedFunc = functions.Get("func 5", 5);

            storedFunc.Should().Be(func);
        }

        [Fact]
        public void Function_With_6_Parameters_IsStored()
        {
            Expression<Func<decimal, decimal, decimal, decimal, decimal, decimal, decimal>> func = (p1, p2, p3, p4, p5, p6) => 1m;
            var functions = new Functions(allowOverride: false);

            functions.Add("func 6", func);
            var storedFunc = functions.Get("func 6", 6);

            storedFunc.Should().Be(func);
        }

        [Fact]
        public void Function_With_7_Parameters_IsStored()
        {
            Expression<Func<decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal>> func = (p1, p2, p3, p4, p5, p6, p7) => 1m;
            var functions = new Functions(allowOverride: false);

            functions.Add("func 7", func);
            var storedFunc = functions.Get("func 7", 7);

            storedFunc.Should().Be(func);
        }

        [Fact]
        public void Function_With_8_Parameters_IsStored()
        {
            Expression<Func<decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal>> func = (p1, p2, p3, p4, p5, p6, p7, p8) => 1m;
            var functions = new Functions(allowOverride: false);

            functions.Add("func 8", func);
            var storedFunc = functions.Get("func 8", 8);

            storedFunc.Should().Be(func);
        }

        [Fact]
        public void Functions_WithSameNumberOfParmeters()
        {
            var functions = new Functions(allowOverride: false);
            Expression<Func<decimal, decimal>> func1 = _ => 2m;
            Expression<Func<decimal, decimal>> func2 = _ => 3m;
            functions.Add("func1", func1);
            functions.Add("func2", func2);

            var storedFunc1 = functions.Get("func1", 1);
            var storedFunc2 = functions.Get("func2", 1);

            storedFunc1.Should().Be(func1);
            storedFunc2.Should().Be(func2);
        }

        [Fact]
        public void Functions_AllowOverride()
        {
            var functions = new Functions(allowOverride: true);
            Expression<Func<decimal, decimal>> func = x => 2m;
            functions.Add("func", _ => 1m);
            functions.Add("func", func);

            var storedFunc = functions.Get("func", 1);

            storedFunc.Should().Be(func);
        }

        [Fact]
        public void Functions_DoesNot_AllowOverride_ThrowsException()
        {
            var functions = new Functions(allowOverride: false);
            Expression<Func<decimal, decimal>> func = _ => 2m;

            functions.Add("func", _ => 1m);
            Action action = () => functions.Add("func", func);

            action.Should().Throw<ArgumentException>().WithMessage("An item with the same key has already been added. Key: func");
        }

        [Fact]
        public void Functions_Overload()
        {
            var functions = new Functions(allowOverride: false);
            Expression<Func<decimal>> func0 = () => 2m;
            Expression<Func<decimal, decimal>> func1 = _ => 2m;

            functions.Add("func", func0);
            functions.Add("func", func1);

            functions.Get("func", 0).Should().Be(func0);
            functions.Get("func", 1).Should().Be(func1);
        }

        [Fact]
        public void Function_NotFound_ByParameterCount_Throws()
        {
            var functions = new Functions(allowOverride: false);

            Action action = () => _ = functions.Get("My Func", 1);

            action.Should().Throw<InvalidOperationException>().WithMessage("There is no function 'My Func' with 1 parameters defined");
        }

        [Fact]
        public void Function_NotFound_ByName_Throws()
        {
            var functions = new Functions(allowOverride: false);
            functions.Add("My Func", () => 0m);

            Action action = () => _ = functions.Get("My Func", 1);

            action.Should().Throw<InvalidOperationException>().WithMessage("There is no function 'My Func' with 1 parameters defined");
        }
    }
}

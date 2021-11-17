using System;
using System.Linq.Expressions;
using FluentAssertions;
using Xunit;

namespace JustEvaluate.Tests
{
    public class FunctionsRegistryTests
    {
        [Fact]
        public void Function_With_0_Parameters_IsStored()
        {
            Expression<Func<decimal>> func = () => 1m;
            var functions = new FunctionsRegistry();

            functions.Add("func 0", func);
            var storedFunc = functions.Get("func 0", 0);

            storedFunc.Should().Be(func);
        }

        [Fact]
        public void Function_With_1_Parameters_IsStored()
        {
            Expression<Func<decimal, decimal>> func = (p1) => 1m;
            var functions = new FunctionsRegistry();

            functions.Add("func 1", func);
            var storedFunc = functions.Get("func 1", 1);

            storedFunc.Should().Be(func);
        }

        [Fact]
        public void Function_With_2_Parameters_IsStored()
        {
            Expression<Func<decimal, decimal, decimal>> func = (p1, p2) => 1m;
            var functions = new FunctionsRegistry();

            functions.Add("func 2", func);
            var storedFunc = functions.Get("func 2", 2);

            storedFunc.Should().Be(func);
        }

        [Fact]
        public void Function_With_3_Parameters_IsStored()
        {
            Expression<Func<decimal, decimal, decimal, decimal>> func = (p1, p2, p3) => 1m;
            var functions = new FunctionsRegistry();

            functions.Add("func 3", func);
            var storedFunc = functions.Get("func 3", 3);

            storedFunc.Should().Be(func);
        }

        [Fact]
        public void Function_With_4_Parameters_IsStored()
        {
            Expression<Func<decimal, decimal, decimal, decimal, decimal>> func = (p1, p2, p3, p4) => 1m;
            var functions = new FunctionsRegistry();

            functions.Add("func 4", func);
            var storedFunc = functions.Get("func 4", 4);

            storedFunc.Should().Be(func);
        }

        [Fact]
        public void Function_With_5_Parameters_IsStored()
        {
            Expression<Func<decimal, decimal, decimal, decimal, decimal, decimal>> func = (p1, p2, p3, p4, p5) => 1m;
            var functions = new FunctionsRegistry();

            functions.Add("func 5", func);
            var storedFunc = functions.Get("func 5", 5);

            storedFunc.Should().Be(func);
        }

        [Fact]
        public void Function_With_6_Parameters_IsStored()
        {
            Expression<Func<decimal, decimal, decimal, decimal, decimal, decimal, decimal>> func = (p1, p2, p3, p4, p5, p6) => 1m;
            var functions = new FunctionsRegistry();

            functions.Add("func 6", func);
            var storedFunc = functions.Get("func 6", 6);

            storedFunc.Should().Be(func);
        }

        [Fact]
        public void Function_With_7_Parameters_IsStored()
        {
            Expression<Func<decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal>> func = (p1, p2, p3, p4, p5, p6, p7) => 1m;
            var functions = new FunctionsRegistry();

            functions.Add("func 7", func);
            var storedFunc = functions.Get("func 7", 7);

            storedFunc.Should().Be(func);
        }

        [Fact]
        public void Function_With_8_Parameters_IsStored()
        {
            Expression<Func<decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal>> func = (p1, p2, p3, p4, p5, p6, p7, p8) => 1m;
            var functions = new FunctionsRegistry();

            functions.Add("func 8", func);
            var storedFunc = functions.Get("func 8", 8);

            storedFunc.Should().Be(func);
        }

        [Fact]
        public void Functions_WithSameNumberOfParmeters()
        {
            var functions = new FunctionsRegistry();
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
        public void Functions_AllowReplace()
        {
            var functions = new FunctionsRegistry();
            Expression<Func<decimal, decimal>> func = x => 2m;
            functions.Add("func", _ => 1m);
            functions.Add("func", func, allowReplace: true);

            var storedFunc = functions.Get("func", 1);

            storedFunc.Should().Be(func);
        }

        [Fact]
        public void Functions_DoesNot_AllowReplace_ThrowsException()
        {
            var functions = new FunctionsRegistry();
            Expression<Func<decimal, decimal>> func = _ => 2m;

            functions.Add("func", _ => 1m);
            Action action = () => functions.Add("func", func);

            action.Should().Throw<ArgumentException>().WithMessage("An item with the same key has already been added. Key: func");
        }

        [Fact]
        public void Functions_Overload()
        {
            var functions = new FunctionsRegistry();
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
            var functions = new FunctionsRegistry();

            Action action = () => _ = functions.Get("My Func", 1);

            action.Should().Throw<InvalidOperationException>().WithMessage("There is no function 'My Func' with 1 parameters defined");
        }

        [Fact]
        public void Function_NotFound_ByName_Throws()
        {
            var functions = new FunctionsRegistry();
            functions.Add("My Func", () => 0m);

            Action action = () => _ = functions.Get("My Func", 1);

            action.Should().Throw<InvalidOperationException>().WithMessage("There is no function 'My Func' with 1 parameters defined");
        }

        [Theory]
        [InlineData("not")]
        [InlineData("NOT")]
        [InlineData("if")]
        [InlineData("If")]
        public void Register_BuiltInFunctionName_Throws(string name)
        {
            var functions = new FunctionsRegistry();

            Action action = () => _ = functions.Add(name, () => 1m);

            action.Should().Throw<InvalidOperationException>()
                           .WithMessage($"'{name}' is reserved for built-in function and user defined function with the same name is not allowed");
        }

        [Theory]
        [InlineData("not")]
        [InlineData("NOT")]
        [InlineData("if")]
        [InlineData("If")]
        public void BuilInFunction_IsKnown_Returns_True(string name)
        {
            var functions = new FunctionsRegistry();
            functions.IsBuiltInFunction(name).Should().BeTrue();
        }

        [Fact]
        public void BuilInFunction_IsNotKnown_Returns_False()
        {
            var functions = new FunctionsRegistry();
            functions.IsBuiltInFunction("other").Should().BeFalse();
        }

        [Theory]
        [InlineData("not", 1)]
        [InlineData("NOT", 1)]
        [InlineData("if", 3)]
        [InlineData("If", 3)]
        public void BuiltInFunction_ArgumentCount(string name, int count)
        {
            var functions = new FunctionsRegistry();
            functions.BuiltInFunctionArgumentCount(name).Should().Be(count);
        }

        [Fact]
        public void BuiltInFunction_ArgumentCount_UnknownFunction_Thorws()
        {
            var functions = new FunctionsRegistry();
            const string name = "other";
            Action action = () => _ = functions.BuiltInFunctionArgumentCount(name);

            action.Should().Throw<ArgumentException>()
                           .WithMessage($"'{name}' is not a built-in function (Parameter 'name')");
        }

        [Fact]
        public void Alias_UnknownFunction_Thorws()
        {
            var functions = new FunctionsRegistry();

            Action action = () => _ = functions.AddFunctionAlias(function: "some function", alias: "alias");

            action.Should().Throw<InvalidOperationException>()
                           .WithMessage("There is no function 'some function' defined");
        }

        [Theory]
        [InlineData("if")]
        [InlineData("not")]
        public void Alias_IsBuiltFunction_Thorws(string builtInFunction)
        {
            var functions = new FunctionsRegistry().Add("some function", () => 1m);

            Action action = () => _ = functions.AddFunctionAlias(function: "some function", alias: builtInFunction);

            action.Should().Throw<InvalidOperationException>()
                           .WithMessage($"'{builtInFunction}' is reserved for built-in function and can not be used as alias");
        }

        [Fact]
        public void Alias_IsRegisteredFunction_Thorws()
        {
            var functions = new FunctionsRegistry().Add("some function", () => 1m)
                                                   .Add("some function 2", () => 1m);

            Action action = () => _ = functions.AddFunctionAlias(function: "some function 2", alias: "some function");

            action.Should().Throw<InvalidOperationException>()
                           .WithMessage("There is function 'some function' defined and can not be used as alias");
        }

        [Fact]
        public void Alias_AlreadyUsed_Thorws()
        {
            var functions = new FunctionsRegistry().Add("some function", () => 1m)
                                                   .Add("some function 2", () => 1m);

            Action action = () => _ = functions.AddFunctionAlias(function: "some function", alias: "some function alias")
                                               .AddFunctionAlias(function: "some function 2", alias: "some function alias");

            action.Should().Throw<InvalidOperationException>()
                           .WithMessage("There is already alias 'some function alias' registered for function 'some function 2'");
        }

        [Fact]
        public void AddFunction_SameAsAlias_Thorws()
        {
            var functions = new FunctionsRegistry().Add("some function", () => 1m)
                                                   .AddFunctionAlias(function: "some function", alias: "alias");

            Action action = () => _ = functions.Add("alias", () => 0m);

            action.Should().Throw<InvalidOperationException>()
                           .WithMessage("'alias' is already added as and alias and cannot be used as function name");
        }

        [Fact]
        public void GetOriginalName_NoAlias()
        {
            var functions = new FunctionsRegistry().Add("some function", () => 1m);

            functions.GetOriginalName("some function").Should().Be("some function");
        }

        [Fact]
        public void GetOriginalName_ByAlias()
        {
            var functions = new FunctionsRegistry().Add("some function", () => 1m).AddFunctionAlias(function: "some function", alias: "alias");

            functions.GetOriginalName("alias").Should().Be("some function");
        }

        [Fact]
        public void Alias_BuiltInFinction()
        {
            var functions = new FunctionsRegistry().AddFunctionAlias(function: "if", alias: "alias");

            functions.IsBuiltInFunction("alias").Should().Be(true);
        }
    }
}

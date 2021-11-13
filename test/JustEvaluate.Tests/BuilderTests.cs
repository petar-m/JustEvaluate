﻿using System;
using FluentAssertions;
using Xunit;

namespace JustEvaluate.Tests
{
    public class BuilderTests
    {
        public static Builder CreateBuilder() => new Builder(new FunctionsRegistry());

        [Theory]
        [InlineData("1", 1)]
        [InlineData("-1", -1)]
        [InlineData("+1", 1)]
        [InlineData("1+1", 2)]
        [InlineData("1+2-3", 0)]
        public void BasicOperations(string expression, decimal expected)
        {
            var parsed = new Parser().Parse(expression);
            var builder = CreateBuilder();

            Func<decimal> func = builder.Build(parsed);
            decimal result = func();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("(1)", 1)]
        [InlineData("((1))", 1)]
        [InlineData("(-1)", -1)]
        [InlineData("(+1)", 1)]
        [InlineData("((-1))", -1)]
        [InlineData("(((+1)))", 1)]
        [InlineData("2 - (3 * 4)", -10)]
        [InlineData("(2 - 3) * 4", -4)]
        [InlineData("((6 - 3)/3) * 4", 4)]
        [InlineData("((6 - (3/3)) * (4))", 20)]
        public void BasicOperationsWithBrackets(string expression, decimal expected)
        {
            var parsed = new Parser().Parse(expression);
            var builder = CreateBuilder();

            Func<decimal> func = builder.Build(parsed);
            decimal result = func();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("1-2*4", -7)]
        [InlineData("1+2*4", 9)]
        [InlineData("5/10*2", 1)]
        [InlineData("5*10/2", 25)]
        [InlineData("2 & 3 * 10", 1)]
        [InlineData("2 & 3 *  0", 0)]
        [InlineData("2 | 3 * 10", 1)]
        [InlineData("0 | 3 * 10", 1)]
        [InlineData("0 | 3 *  0", 0)]
        [InlineData("2 & 3 + 1", 1)]
        [InlineData("2 & 3 - 3", 0)]
        [InlineData("2 | 3 / 3 ", 1)]
        [InlineData("0 | 0 * 10", 0)]
        [InlineData("1 | 0 +  1", 1)]
        [InlineData("1 | 0 -  1", 1)]
        [InlineData("1 | 1 & 1", 1)]
        [InlineData("1 | 0 & 1", 1)]
        [InlineData("1 | 1 & 0", 1)]
        [InlineData("1 * 1 | 1 + 1 & 1 - 1", 1)]
        [InlineData("2 + 1 > 1", 1)]
        [InlineData("2 + 1 = 1", 0)]
        [InlineData("3 & 0 + 2 = 0 | 2 -3", 1)]
        public void OperatorPrecedence(string expression, decimal expected)
        {
            var parsed = new Parser().Parse(expression);
            var builder = CreateBuilder();

            Func<decimal> func = builder.Build(parsed);
            decimal result = func();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("1>0", 1)]
        [InlineData("1>=0", 1)]
        [InlineData("1>=1", 1)]
        [InlineData("1>=2", 0)]
        [InlineData("1<0", 0)]
        [InlineData("1<=0", 0)]
        [InlineData("1<=1", 1)]
        [InlineData("1<=2", 1)]
        [InlineData("1=1", 1)]
        [InlineData("1=2", 0)]
        [InlineData("2=1", 0)]
        [InlineData("1<>1", 0)]
        [InlineData("1<>2", 1)]
        [InlineData("2<>1", 1)]
        public void RelationalExpressions(string expression, decimal expected)
        {
            var parsed = new Parser().Parse(expression);
            var builder = CreateBuilder();

            Func<decimal> func = builder.Build(parsed);
            decimal result = func();

            result.Should().Be(expected);
        }

        [Fact]
        public void MismatchedBraces_NoClosing_Throws()
        {
            var tokens = new Token[] { new Token("("), new Token(1), new Token('+'), new Token("3") };
            var builder = CreateBuilder();

            Action action = () => builder.Build(tokens);

            action.Should().Throw<InvalidOperationException>().WithMessage("Mismatched brackets");
        }

        [Fact]
        public void MismatchedBraces_NoOpening_Throws()
        {
            var tokens = new Token[] { new Token(1), new Token('+'), new Token("3"), new Token(")") };
            var builder = CreateBuilder();

            Action action = () => builder.Build(tokens);

            action.Should().Throw<InvalidOperationException>().WithMessage("Mismatched brackets");
        }

        [Fact]
        public void MismatchedBraces_EndsWithOpening_Throws()
        {
            var tokens = new Token[] { new Token(1), new Token('+'), new Token("3"), new Token("(") };
            var builder = CreateBuilder();

            Action action = () => builder.Build(tokens);

            action.Should().Throw<InvalidOperationException>().WithMessage("Mismatched brackets");
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        [InlineData("*")]
        [InlineData("/")]
        [InlineData(">")]
        [InlineData(">=")]
        [InlineData("<")]
        [InlineData("<=")]
        [InlineData("=")]
        [InlineData("<>")]
        [InlineData("&")]
        [InlineData("|")]
        public void MissingOperand_Throws(string op)
        {
            var tokens = new Token[] { new Token(1), new Token(op)};
            var builder = CreateBuilder();

            Action action = () => builder.Build(tokens);

            action.Should().Throw<InvalidOperationException>().WithMessage("Insufficient data for calculation - missing operand");
        }

        [Fact]
        public void MissingOperator_Throws()
        {
            var tokens = new Token[] { new Token(1), new Token("3") };
            var builder = CreateBuilder();

            Action action = () => builder.Build(tokens);

            action.Should().Throw<InvalidOperationException>().WithMessage("Too many values supplied - missing operator");
        }

        [Fact]
        public void MismatchedBraces_StartsWithClosing_Throws()
        {
            var tokens = new Token[] { new Token(")"), new Token(1), new Token('+'), new Token("3") };
            var builder = CreateBuilder();

            Action action = () => builder.Build(tokens);

            action.Should().Throw<InvalidOperationException>().WithMessage("Mismatched brackets");
        }

        [Fact]
        public void ArgumentsUsage()
        {
            var arg = new Arguments { Height = 3.45m, Width = 4.155m };
            var parsed = new Parser().Parse("Width * Height");
            var builder = CreateBuilder();

            Func<Arguments, decimal> func = builder.Build<Arguments>(parsed);
            decimal result = func(arg);

            result.Should().Be(arg.Height * arg.Width);
        }

        [Fact]
        public void ArgumentsNames_CaseInsensitive()
        {
            var arg = new Arguments { Height = 3.45m, Width = 4.155m };
            var parsed = new Parser().Parse("width * HEIGHT");
            var builder = CreateBuilder();

            Func<Arguments, decimal> func = builder.Build<Arguments>(parsed);
            decimal result = func(arg);

            result.Should().Be(arg.Height * arg.Width);
        }

        [Fact]
        public void ArgumentName_PropertyDoesNotExist_Throws()
        {
            var arg = new Arguments { Height = 3.45m, Width = 4.155m };
            var parsed = new Parser().Parse("Area / 2");
            var builder = CreateBuilder();

            Action action = () => _ = builder.Build<Arguments>(parsed);

            action.Should().Throw<InvalidOperationException>().WithMessage("Argument type Arguments does not have property named 'Area'");
        }

        [Fact]
        public void ArgumentName_PropertyIsNotDecimal_Throws()
        {
            var arg = new Arguments { Name = "Rectangle" };
            var parsed = new Parser().Parse("Name * 2");
            var builder = CreateBuilder();

            Action action = () => _ = builder.Build<Arguments>(parsed);

            action.Should().Throw<InvalidOperationException>().WithMessage("Property named 'Name' is of type String, expected Decimal");
        }

        [Fact]
        public void Function_Name_CaseInsensitive()
        {
            var functions = new FunctionsRegistry();
            functions.Add("always one", () => 1m);

            var parsed = new Parser().Parse("Always ONE()");
            var builder = new Builder(functions);

            Func<decimal> func = builder.Build(parsed);
            decimal result = func();

            result.Should().Be(1m);
        }

        [Fact]
        public void Function_WithParameter()
        {
            var functions = new FunctionsRegistry();
            functions.Add("PlusOne", x => x + 1m);

            var parsed = new Parser().Parse("PlusOne(3)");
            var builder = new Builder(functions);

            Func<decimal> func = builder.Build(parsed);
            decimal result = func();

            result.Should().Be(4m);
        }

        [Fact]
        public void Function_WithParameterFunction()
        {
            var functions = new FunctionsRegistry();
            functions.Add("PlusOne", x => x + 1m);
            functions.Add("Half", x => x / 2m);

            var parsed = new Parser().Parse("Half(PlusOne(3))");
            var builder = new Builder(functions);

            Func<decimal> func = builder.Build(parsed);
            decimal result = func();

            result.Should().Be(2m);
        }

        [Fact]
        public void Function_WithParameterExpression()
        {
            var functions = new FunctionsRegistry();
            functions.Add("PlusOne", x => x + 1m);
            functions.Add("Half", x => x / 2m);

            var parsed = new Parser().Parse("Half((PlusOne(3) + 6) * 1.5)");
            var builder = new Builder(functions);

            Func<decimal> func = builder.Build(parsed);
            decimal result = func();

            result.Should().Be(7.5m);
        }

        [Fact]
        public void Function_WithMultipleParameterExpression()
        {
            var functions = new FunctionsRegistry();
            functions.Add("PlusOne", x => x + 1m);
            functions.Add("Calc", (x, y, z) => x * y - z);

            var parsed = new Parser().Parse("Calc( (PlusOne(3) + 6) * 1.5, (PlusOne(13) - 4) / 2 * (-1), (((1-2) * 4) / 2))");
            var builder = new Builder(functions);

            Func<decimal> func = builder.Build(parsed);
            decimal result = func();

            result.Should().Be(-73m);
        }

        [Fact]
        public void Function_WithMultipleParameterExpression_WithArguments()
        {
            var functions = new FunctionsRegistry();
            functions.Add("PlusOne", x => x + 1m);
            functions.Add("Calc", (x, y, z) => x * y - z);
            var arguments = new Arguments { Height = 6, Width = 4 };

            var parsed = new Parser().Parse("Calc( (PlusOne(3) + Height) * 1.5, (PlusOne(13) - Width) / 2 * (-1), (((1-2) * Width) / 2))");
            var builder = new Builder(functions);

            Func<Arguments, decimal> func = builder.Build<Arguments>(parsed);
            decimal result = func(arguments);

            result.Should().Be(-73m);
        }

        [Fact]
        public void Function_WithMultipleNestedFunctions()
        {
            var functions = new FunctionsRegistry();
            functions.Add("Plus One", x => x + 1m);
            functions.Add("Plus10", x => x + 10m);
            functions.Add("Sum", (x, y, z) => x + y + z);
            functions.Add("Mult", (x, y) => x * y);
            var arguments = new Arguments { Height = 6, Width = 4 };

            var parsed = new Parser().Parse(
                @"Plus10(Sum(10 / (Mult(Width, Height) - 4) / 2 - Plus One(0),
                             Mult(Sum(Plus One(Width-3), -1 * Plus10(Plus One((Height-Width)*2)), (3-1-Plus One(Sum(1,2,Sum(5,4,1))))), -1 ),
                             -(-(-Width+Width)) - Mult(width-Height, -2)   
                            )
                        )");
            var builder = new Builder(functions);

            Func<Arguments, decimal> func = builder.Build<Arguments>(parsed);
            decimal result = func(arguments);

            result.Should().Be(30.25m);
        }

        [Fact]
        public void BuiltInFunction_If_WithInvalidArgumentCount_Throws()
        {
            var parsed = new Parser().Parse("if(1)");

            var builder = CreateBuilder();

            Action action = () => _ = builder.Build<Arguments>(parsed);

            action.Should().Throw<InvalidOperationException>().WithMessage("Built-in function 'if' takes 3 arguments but invoked with 1");
        }

        [Fact]
        public void BuiltInFunction_Not_WithInvalidArgumentCount_Throws()
        {
            var parsed = new Parser().Parse("not(1,3)");

            var builder = CreateBuilder();

            Action action = () => _ = builder.Build<Arguments>(parsed);

            action.Should().Throw<InvalidOperationException>().WithMessage("Built-in function 'not' takes 1 arguments but invoked with 2");
        }
    }
}

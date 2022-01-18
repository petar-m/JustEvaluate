using System;
using System.Collections.Generic;
using FluentAssertions;
using JustEvaluate.UtilityFunctions;
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
            var tokens = new Token[] { new Token(1), new Token(op) };
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
        public void ArgumentsUsage_FromDictionary()
        {
            var arg = new Dictionary<string, decimal>() { { "Height", 3.45m }, { "Width", 4.155m } };
            var parsed = new Parser().Parse("Width * Height");
            var builder = CreateBuilder();

            var func = builder.Build<Dictionary<string, decimal>>(parsed);
            decimal result = func(arg);

            result.Should().Be(arg["Height"] * arg["Width"]);
        }

        [Fact]
        public void ArgumentsUsage_FromIDictionary()
        {
            IDictionary<string, decimal> arg = new Dictionary<string, decimal>() { { "Height", 3.45m }, { "Width", 4.155m } };
            var parsed = new Parser().Parse("Width * Height");
            var builder = CreateBuilder();

            var func = builder.Build<IDictionary<string, decimal>>(parsed);
            decimal result = func(arg);

            result.Should().Be(arg["Height"] * arg["Width"]);
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
        public void Arguments_FromDictionary_CaseInsensitive_If_DictionaryComparerIsCaseInsensitive()
        {
            IDictionary<string, decimal> arg = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase) { { "Height", 3.45m }, { "Width", 4.155m } };
            var parsed = new Parser().Parse("width * HEIGHT");
            var builder = CreateBuilder();

            var func = builder.Build<IDictionary<string, decimal>>(parsed);
            decimal result = func(arg);

            result.Should().Be(arg["Height"] * arg["Width"]);
        }

        [Fact]
        public void Arguments_FromIDictionary_CaseSensitive_If_DictionaryComparerIsCaseSensitive()
        {
            IDictionary<string, decimal> arg = new Dictionary<string, decimal>() { { "Height", 3.45m }, { "Width", 4.155m } };
            var parsed = new Parser().Parse("width * HEIGHT");
            var builder = CreateBuilder();

            var func = builder.Build<IDictionary<string, decimal>>(parsed);
            Action action = () => func(arg);
            
            action.Should().Throw<KeyNotFoundException>().WithMessage("The given key 'width' was not present in the dictionary.");
        }

        [Fact]
        public void Arguments_FromDictionary_CaseSensitive_If_DictionaryComparerIsCaseSensitive()
        {
            var arg = new Dictionary<string, decimal>() { { "Height", 3.45m }, { "Width", 4.155m } };
            var parsed = new Parser().Parse("width * HEIGHT");
            var builder = CreateBuilder();

            var func = builder.Build<Dictionary<string, decimal>>(parsed);
            Action action = () => func(arg);

            action.Should().Throw<KeyNotFoundException>().WithMessage("The given key 'width' was not present in the dictionary.");
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
        public void Function_Alias_CaseInsensitive()
        {
            var functions = new FunctionsRegistry();
            functions.Add("always one", () => 1m).AddFunctionAlias("always one", "only one");

            var parsed = new Parser().Parse("ONLY ONE()");
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
        public void Function_WithMultipleParameterExpression_WithArguments_FromDictionary()
        {
            var functions = new FunctionsRegistry();
            functions.Add("PlusOne", x => x + 1m);
            functions.Add("Calc", (x, y, z) => x * y - z);
            var arguments = new Dictionary<string, decimal> { { "Height", 6m }, { "Width", 4 } };

            var parsed = new Parser().Parse("Calc( (PlusOne(3) + Height) * 1.5, (PlusOne(13) - Width) / 2 * (-1), (((1-2) * Width) / 2))");
            var builder = new Builder(functions);

            Func<Dictionary<string, decimal>, decimal> func = builder.Build<Dictionary<string, decimal>>(parsed);
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

        public class Input1
        {
            [Alias("Value alias")]
            public decimal Value { get; set; }

            [Alias("Numeric Value")]
            public string Text { get; set; }
        }

        [Fact]
        public void Property_Alias_IsUsed()
        {
            var input = new Input1 { Value = 123.4m };
            var builder = CreateBuilder();
            var parser = new Parser();
            var tokens = parser.Parse(" Value alias + 1 ");

            var func = builder.Build<Input1>(tokens);

            func(input).Should().Be(input.Value + 1);
        }

        [Fact]
        public void Property_IsNotOfType_Decimal_Throws_AliasIncludedInMessage()
        {
            var builder = CreateBuilder();
            var parser = new Parser();
            var tokens = parser.Parse(" Numeric Value + 1 ");

            Action action = () => _ = builder.Build<Input1>(tokens);

            action.Should().Throw<InvalidOperationException>().WithMessage("Property named 'Text' (alias 'Numeric Value') is of type String, expected Decimal");
        }

        public class Input2
        {
            [Alias("Value")]
            public decimal Value { get; set; }
        }

        [Fact]
        public void Property_AliasIsSameAsPropertyName_Throws()
        {
            var builder = CreateBuilder();
            var parser = new Parser();
            var tokens = parser.Parse(" Value + 1 ");

            Action action = () => _ = builder.Build<Input2>(tokens);

            action.Should().Throw<InvalidOperationException>().WithMessage("Property aliases should be unique and different than property names, diplicates: Value");
        }

        public class Input3
        {
            [Alias("AnotherValue")]
            public decimal Value { get; set; }

            public decimal AnotherValue { get; set; }
        }

        [Fact]
        public void Property_AliasIsSameAsAnotherPropertyName_Throws()
        {
            var builder = CreateBuilder();
            var parser = new Parser();
            var tokens = parser.Parse(" Value + 1 ");

            Action action = () => _ = builder.Build<Input3>(tokens);

            action.Should().Throw<InvalidOperationException>().WithMessage("Property aliases should be unique and different than property names, diplicates: AnotherValue");
        }

        public class Input4
        {
            [Alias("AnotherValue")]
            public decimal Value { get; set; }

            [Alias("AnotherValue")]
            public decimal Value2 { get; set; }
        }

        [Fact]
        public void Property_AliasIsSameAsAnotherAlias_Throws()
        {
            var builder = CreateBuilder();
            var parser = new Parser();
            var tokens = parser.Parse(" Value + 1 ");

            Action action = () => _ = builder.Build<Input4>(tokens);

            action.Should().Throw<InvalidOperationException>().WithMessage("Property aliases should be unique and different than property names, diplicates: AnotherValue");
        }

        public class Input5
        {
            [Alias("Height")]
            public decimal X { get; set; }

            [Alias("Width")]
            public decimal Y { get; set; }

            public decimal MethodWithoutParameters() => 10;

            public decimal MethodOverload(decimal x) => x * 10;

            public decimal MethodOverload(decimal x, decimal y) => x * y *10;

            public decimal MethodX(string x) => 10;

            public string MethodX() => string.Empty;

            public decimal Method() => 1;
            
            public decimal method() => 1;
        }

        [Fact]
        public void Alias_PropertyIsUsed()
        {
            var builder = CreateBuilder();
            var parser = new Parser();
            var tokens = parser.Parse("Width * Height");

            var result = builder.Build<Input5>(tokens);

            result(new Input5 { X = 2, Y = 4 }).Should().Be(8);
        }

        [Fact]
        public void Alias_PropertyIsUsed_WhenFunctionArgument()
        {
            var builder = CreateBuilder();
            var parser = new Parser();
            var tokens = parser.Parse("not(Width * Height)");

            var result = builder.Build<Input5>(tokens);

            result(new Input5 { X = 2, Y = 4 }).Should().Be(0);
        }

        [Fact]
        public void BuiltInFunction_Alias()
        {
            var functions = new FunctionsRegistry().AddFunctionAlias(function: "not", alias: "alias");
            var builder = new Builder(functions);
            var parser = new Parser();

            var result = builder.Build(parser.Parse("alias(1)"));

            result().Should().Be(0);
        }

        [Fact]
        public void MethodCall()
        {
            var functions = new FunctionsRegistry();
            var builder = new Builder(functions);
            var parser = new Parser();

            var result = builder.Build<Input5>(parser.Parse("MethodWithoutParameters()"));

            result(new Input5()).Should().Be(10);
        }

        [Fact]
        public void MethodCall_CaseInsensitive()
        {
            var functions = new FunctionsRegistry();
            var builder = new Builder(functions);
            var parser = new Parser();

            var result = builder.Build<Input5>(parser.Parse("methoDwithoutpARAmeterS()"));

            result(new Input5()).Should().Be(10);
        }

        [Fact]
        public void MethodCall_OverloadResolvedByParametersCount()
        {
            var functions = new FunctionsRegistry();
            var builder = new Builder(functions);
            var parser = new Parser();

            var result = builder.Build<Input5>(parser.Parse("MethodOverload(2, 3)"));

            result(new Input5()).Should().Be(60);
        }

        [Fact]
        public void MethodCall_AmbiguousName_Throws()
        {
            var functions = new FunctionsRegistry();
            var builder = new Builder(functions);
            var parser = new Parser();

            Action action = () => _ = builder.Build<Input5>(parser.Parse("Method()"));

            action.Should().Throw<InvalidOperationException>().WithMessage("Ambiguous method '.Method' on type JustEvaluate.Tests.BuilderTests+Input5: can't decide between Method,method");
        }

        [Fact]
        public void MethodCall_MethodWithDecimalParametersNotFound_Throws()
        {
            var functions = new FunctionsRegistry();
            var builder = new Builder(functions);
            var parser = new Parser();

            Action action = () => _ = builder.Build<Input5>(parser.Parse("MethodX(1)"));

            action.Should().Throw<InvalidOperationException>().WithMessage("There is no function 'MethodX' with 1 parameters defined; No method '.MethodX' found on argument of type JustEvaluate.Tests.BuilderTests+Input5 taking 1 parameters of type decimal and returning decimal");
        }

        [Fact]
        public void MethodCall_MethodReturningDecimalNotFound_Throws()
        {
            var functions = new FunctionsRegistry();
            var builder = new Builder(functions);
            var parser = new Parser();

            Action action = () => _ = builder.Build<Input5>(parser.Parse("MethodX()"));

            action.Should().Throw<InvalidOperationException>().WithMessage("There is no function 'MethodX' with 0 parameters defined; No method '.MethodX' found on argument of type JustEvaluate.Tests.BuilderTests+Input5 taking 0 parameters of type decimal and returning decimal");
        }

        [Fact]
        public void MethodCall_ComplexArguments()
        {
            var functions = new FunctionsRegistry().AddMath();
            var builder = new Builder(functions);
            var parser = new Parser();

            var result = builder.Build<Input5>(parser.Parse("1 + Min(MethodOverload(MethodOverload(X, 1000)), MethodOverload(Max(X, Y)))"))(new Input5 { X = 2, Y = 3 });

            result.Should().Be(31);
        }

        [Fact]
        public void MethodCall_FunctionTakesPrecedence()
        {
            var functions = new FunctionsRegistry().Add("MethodOverload", x => -12.345m);
            var builder = new Builder(functions);
            var parser = new Parser();

            var result = builder.Build<Input5>(parser.Parse("MethodOverload(10)"))(new Input5 { X = 2, Y = 3 });

            result.Should().Be(-12.345m);
        }
    }
}

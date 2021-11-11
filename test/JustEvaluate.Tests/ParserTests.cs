using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace JustEvaluate.Tests
{
    public class ParserTests
    {
        private static readonly Parser _parser = new Parser();

        [Fact]
        public void Constant_SingleChar()
        {
            var input = "1";

            var result = _parser.Parse(input);

            result.Should().HaveCount(1);
            result.First().Assert(TokenType.Constant, input, 1, input);
        }

        [Fact]
        public void Tabs_Ignored()
        {
            var input = "1  +\t2";

            var result = _parser.Parse(input).ToArray();

            result.Should().HaveCount(3);
            result[0].Assert(TokenType.Constant, 1);
            result[1].Assert(TokenType.Add);
            result[2].Assert(TokenType.Constant, 2);
        }

        [Fact]
        public void NewLines_Ignored()
        {
            var input = @"
                           1
                           +
                           2";

            var result = _parser.Parse(input).ToArray();

            result.Should().HaveCount(3);
            result[0].Assert(TokenType.Constant, 1);
            result[1].Assert(TokenType.Add);
            result[2].Assert(TokenType.Constant, 2);
        }

        [Fact]
        public void NewLines_AllIgnored()
        {
            var input = "\r1\n+\r\n2";

            var result = _parser.Parse(input).ToArray();

            result.Should().HaveCount(3);
            result[0].Assert(TokenType.Constant, 1);
            result[1].Assert(TokenType.Add);
            result[2].Assert(TokenType.Constant, 2);
        }

        [Fact]
        public void UnaryMinus_Number()
        {
            var input = "-1.";

            var result = _parser.Parse(input).ToArray();

            result.Should().HaveCount(3);
            result[0].Assert(TokenType.Constant, -1);
            result[1].Assert(TokenType.Multipy);
            result[2].Assert(TokenType.Constant, 1);
        }

        [Fact]
        public void UnaryMinus_Name()
        {
            var input = "- abc";

            var result = _parser.Parse(input).ToArray();

            result.Should().HaveCount(3);
            result[0].Assert(TokenType.Constant, -1);
            result[1].Assert(TokenType.Multipy);
            result[2].Assert(TokenType.Name, "abc");
        }

        [Fact]
        public void UnaryMinus_Function()
        {
            var input = "- abc()";

            var result = _parser.Parse(input).ToArray();

            result.Should().HaveCount(3);
            result[0].Assert(TokenType.Constant, -1);
            result[1].Assert(TokenType.Multipy);
            result[2].Assert(TokenType.Function, "abc");
        }

        [Fact]
        public void UnaryMinus_Brackets()
        {
            var input = "-(abc)";

            var result = _parser.Parse(input).ToArray();

            result.Should().HaveCount(5);
            result[0].Assert(TokenType.Constant, -1);
            result[1].Assert(TokenType.Multipy, "*");
            result[2].Assert(TokenType.OpeningBracket, "(");
            result[3].Assert(TokenType.Name, "abc");
            result[4].Assert(TokenType.ClosingBracket, ")");
        }

        [Fact]
        public void UnaryMinus_FunctionArgument()
        {
            var input = "abc(-4)";

            var result = _parser.Parse(input).ToArray();

            result.Should().HaveCount(1);
            var abc = result[0];
            abc.Assert(TokenType.Function, "abc");
            var arguments = abc.FunctionArguments;
            arguments.Should().HaveCount(1);
            arguments[0].Should().HaveCount(3);
            arguments[0][0].Assert(TokenType.Constant, -1);
            arguments[0][1].Assert(TokenType.Multipy, "*");
            arguments[0][2].Assert(TokenType.Constant, 4);
        }

        [Fact]
        public void UnaryPlus_Number()
        {
            var input = "+1";

            var result = _parser.Parse(input).ToArray();

            result.Should().HaveCount(1);
            result[0].Assert(TokenType.Constant, 1);
        }

        [Fact]
        public void UnaryPlus_Name()
        {
            var input = "+ abc";

            var result = _parser.Parse(input).ToArray();

            result.Should().HaveCount(1);
            result[0].Assert(TokenType.Name, "abc");
        }

        [Fact]
        public void UnaryPlus_Function()
        {
            var input = "+abc()";

            var result = _parser.Parse(input).ToArray();

            result.Should().HaveCount(1);
            result[0].Assert(TokenType.Function, "abc");
        }

        [Fact]
        public void UnaryPlus_Brackets()
        {
            var input = "+(abc)";

            var result = _parser.Parse(input).ToArray();

            result.Should().HaveCount(3);
            result[0].Assert(TokenType.OpeningBracket, "(");
            result[1].Assert(TokenType.Name, "abc");
            result[2].Assert(TokenType.ClosingBracket, ")");
        }

        [Fact]
        public void UnaryPlus_FunctionArgument()
        {
            var input = " abc ( + 4 )";

            var result = _parser.Parse(input).ToArray();

            result.Should().HaveCount(1);
            var abc = result[0];
            abc.Assert(TokenType.Function, "abc");
            var arguments = abc.FunctionArguments;
            arguments.Should().HaveCount(1);
            arguments[0].Should().HaveCount(1);
            arguments[0][0].Assert(TokenType.Constant, 4);
        }

        [Fact]
        public void UnaryPlus_NonFirstFunctionArgument()
        {
            var input = " abc ( a, + 4 )";

            var result = _parser.Parse(input).ToArray();

            result.Should().HaveCount(1);
            var abc = result[0];
            abc.Assert(TokenType.Function, "abc");
            var arguments = abc.FunctionArguments;
            arguments.Should().HaveCount(2);

            arguments[0].Should().HaveCount(1);
            arguments[0][0].Assert(TokenType.Name, "a");

            arguments[1].Should().HaveCount(1);
            arguments[1][0].Assert(TokenType.Constant, 4);
        }

        [Fact]
        public void Brackets()
        {
            var input = " ( 1123.345 ) ";

            var result = _parser.Parse(input).ToArray();

            result.Should().HaveCount(3);
            result[0].Assert(TokenType.OpeningBracket);
            result[1].Assert(TokenType.Constant, 1123.345m);
            result[2].Assert(TokenType.ClosingBracket);
        }

        [Fact]
        public void Brackets_Expression()
        {
            var input = "1 * (2.56 - ( a + b) * 2.)";

            var result = _parser.Parse(input).ToArray();

            result.Should().HaveCount(13);
            result[0].Assert(TokenType.Constant, 1);
            result[1].Assert(TokenType.Multipy, "*");
            result[2].Assert(TokenType.OpeningBracket, "(");
            result[3].Assert(TokenType.Constant, 2.56m);
            result[4].Assert(TokenType.Subtract, "-");
            result[5].Assert(TokenType.OpeningBracket, "(");
            result[6].Assert(TokenType.Name, "a");
            result[7].Assert(TokenType.Add, "+");
            result[8].Assert(TokenType.Name, "b");
            result[9].Assert(TokenType.ClosingBracket, ")");
            result[10].Assert(TokenType.Multipy, "*");
            result[11].Assert(TokenType.Constant, 2);
            result[12].Assert(TokenType.ClosingBracket, ")");
        }

        [Theory]
        [InlineData("(")]
        [InlineData(")")]
        [InlineData("(()")]
        [InlineData("())")]
        [InlineData(")(")]
        public void Brackets_Invalid(string input)
        {
            Action action = () => _parser.Parse(input);

            action.Should().Throw<Exception>("Mismatched brackets");
        }

        [Theory]
        [InlineData("func(1),1")]
        [InlineData(",1")]
        [InlineData("1,")]
        [InlineData("1,1+3")]
        [InlineData("1+3,1")]
        public void MisplacedParameterSeparator_Throws(string input)
        {
            Action action = () => _parser.Parse(input).ToArray();

            action.Should().Throw<InvalidOperationException>("Misplaced function parameter separator");
        }

        [Fact]
        public void Function_WithoutArguments()
        {
            var input = "func()";

            var result = _parser.Parse(input).ToArray();

            result.Should().HaveCount(1);
            result[0].Assert(TokenType.Function, "func");
            result[0].FunctionArguments.Should().HaveCount(0);
        }

        [Fact]
        public void Function_MultipleArguments()
        {
            var input = "func(1,a,b)";

            var result = _parser.Parse(input).ToArray();

            result.Should().HaveCount(1);
            result[0].Assert(TokenType.Function, "func");
            var arguments = result[0].FunctionArguments;
            arguments.Should().HaveCount(3);

            arguments[0].Should().HaveCount(1);
            arguments[0][0].Assert(TokenType.Constant, 1);

            arguments[1].Should().HaveCount(1);
            arguments[1][0].Assert(TokenType.Name, "a");

            arguments[2].Should().HaveCount(1);
            arguments[2][0].Assert(TokenType.Name, "b");
        }

        [Fact]
        public void Function_ExpressionArgumentsMismatchedBrackets()
        {
            var input = "x( (1, 2))";

            Action action = () => _parser.Parse(input);

            action.Should().Throw<Exception>("Mismatched brackets");
        }

        [Theory]
        [InlineData("1>1", TokenType.GreaterThan)]
        [InlineData("1>=1", TokenType.GreaterOrEqualTo)]
        [InlineData("1=1", TokenType.EqualTo)]
        [InlineData("1<>1", TokenType.NotEqualTo)]
        [InlineData("1<1", TokenType.LessThan)]
        [InlineData("1<=1", TokenType.LessOrEqualTo)]
        public void BooleanAndRelational_IsParsed(string input, TokenType expected)
        {
            var result = _parser.Parse(input);

            result.Should().HaveCount(3);
            result.ToArray()[1].Type.Should().Be(expected);
        }

        [Fact]
        public void Function_ExpressionArguments()
        {
            var input = "func( 1 * a + 3 , -2 / ((a - 1) * (b+1)) , (-(b / 4)) )";

            var result = _parser.Parse(input).ToArray();

            result.Should().HaveCount(1);
            result[0].Assert(TokenType.Function, "func");
            var arguments = result[0].FunctionArguments;
            arguments.Should().HaveCount(3);

            arguments[0].Should().HaveCount(5);
            arguments[0][0].Assert(TokenType.Constant, 1);
            arguments[0][1].Assert(TokenType.Multipy);
            arguments[0][2].Assert(TokenType.Name, "a");
            arguments[0][3].Assert(TokenType.Add);
            arguments[0][4].Assert(TokenType.Constant, 3);

            arguments[1].Should().HaveCount(17);
            arguments[1][0].Assert(TokenType.Constant, -1);
            arguments[1][1].Assert(TokenType.Multipy);
            arguments[1][2].Assert(TokenType.Constant, 2);
            arguments[1][3].Assert(TokenType.Divide);
            arguments[1][4].Assert(TokenType.OpeningBracket);
            arguments[1][5].Assert(TokenType.OpeningBracket);
            arguments[1][6].Assert(TokenType.Name, "a");
            arguments[1][7].Assert(TokenType.Subtract);
            arguments[1][8].Assert(TokenType.Constant, 1);
            arguments[1][9].Assert(TokenType.ClosingBracket);
            arguments[1][10].Assert(TokenType.Multipy);
            arguments[1][11].Assert(TokenType.OpeningBracket);
            arguments[1][12].Assert(TokenType.Name, "b");
            arguments[1][13].Assert(TokenType.Add);
            arguments[1][14].Assert(TokenType.Constant, 1);
            arguments[1][15].Assert(TokenType.ClosingBracket);
            arguments[1][16].Assert(TokenType.ClosingBracket);

            arguments[2].Should().HaveCount(9);
            arguments[2][0].Assert(TokenType.OpeningBracket);
            arguments[2][1].Assert(TokenType.Constant, -1);
            arguments[2][2].Assert(TokenType.Multipy);
            arguments[2][3].Assert(TokenType.OpeningBracket);
            arguments[2][4].Assert(TokenType.Name, "b");
            arguments[2][5].Assert(TokenType.Divide);
            arguments[2][6].Assert(TokenType.Constant, 4);
            arguments[2][7].Assert(TokenType.ClosingBracket);
            arguments[2][8].Assert(TokenType.ClosingBracket);
        }

        [Fact]
        public void Function_FunctionArguments()
        {
            var input = @".8 / func(- f1(f2( (1+4)/2,  f3 ( f4 ()))) - f5(), 
                                     (f6() + f7()) / f8( (1 + 1) - f9() ),  
                                     f10(f11(a/b), f12( (f13((a) - (f14()) )) ) ) 
                                    )";

            var result = _parser.Parse(input).ToArray();

            result.Should().HaveCount(3);
            result[0].Assert(TokenType.Constant, .8m);
            result[1].Assert(TokenType.Divide);
            result[2].Assert(TokenType.Function, "func");

            var func = result[2];
            func.FunctionArguments.Should().HaveCount(3);

            func.FunctionArguments[0].Should().HaveCount(5);
            func.FunctionArguments[0][0].Assert(TokenType.Constant, -1);
            func.FunctionArguments[0][1].Assert(TokenType.Multipy);
            func.FunctionArguments[0][2].Assert(TokenType.Function, "f1");
            func.FunctionArguments[0][3].Assert(TokenType.Subtract);
            func.FunctionArguments[0][4].Assert(TokenType.Function, "f5");

            var f1 = func.FunctionArguments[0][2];
            f1.FunctionArguments.Should().HaveCount(1);
            f1.FunctionArguments[0][0].Assert(TokenType.Function, "f2");

            var f2 = f1.FunctionArguments[0][0];
            f2.FunctionArguments.Should().HaveCount(2);
            f2.FunctionArguments[0].Should().HaveCount(7);
            f2.FunctionArguments[0][0].Assert(TokenType.OpeningBracket);
            f2.FunctionArguments[0][1].Assert(TokenType.Constant, 1);
            f2.FunctionArguments[0][2].Assert(TokenType.Add);
            f2.FunctionArguments[0][3].Assert(TokenType.Constant, 4);
            f2.FunctionArguments[0][4].Assert(TokenType.ClosingBracket);
            f2.FunctionArguments[0][5].Assert(TokenType.Divide);
            f2.FunctionArguments[0][6].Assert(TokenType.Constant, 2);

            f2.FunctionArguments[1].Should().HaveCount(1);
            f2.FunctionArguments[1][0].Assert(TokenType.Function, "f3");

            var f3 = f2.FunctionArguments[1][0];
            f3.FunctionArguments.Should().HaveCount(1);
            var f4 = f3.FunctionArguments[0][0];
            f4.Assert(TokenType.Function, "f4");
            f4.FunctionArguments.Should().HaveCount(0);

            var f5 = func.FunctionArguments[0][4];
            f5.FunctionArguments.Should().HaveCount(0);

            func.FunctionArguments[1].Should().HaveCount(7);
            func.FunctionArguments[1][0].Assert(TokenType.OpeningBracket);
            func.FunctionArguments[1][1].Assert(TokenType.Function, "f6");
            func.FunctionArguments[1][2].Assert(TokenType.Add);
            func.FunctionArguments[1][3].Assert(TokenType.Function, "f7");
            func.FunctionArguments[1][4].Assert(TokenType.ClosingBracket);
            func.FunctionArguments[1][5].Assert(TokenType.Divide);
            func.FunctionArguments[1][6].Assert(TokenType.Function, "f8");

            var f6 = func.FunctionArguments[1][1];
            f6.FunctionArguments.Should().HaveCount(0);

            var f7 = func.FunctionArguments[1][3];
            f7.FunctionArguments.Should().HaveCount(0);

            var f8 = func.FunctionArguments[1][6];
            f8.FunctionArguments.Should().HaveCount(1);
            f8.FunctionArguments[0][0].Assert(TokenType.OpeningBracket);
            f8.FunctionArguments[0][1].Assert(TokenType.Constant, 1);
            f8.FunctionArguments[0][2].Assert(TokenType.Add);
            f8.FunctionArguments[0][3].Assert(TokenType.Constant, 1);
            f8.FunctionArguments[0][4].Assert(TokenType.ClosingBracket);
            f8.FunctionArguments[0][5].Assert(TokenType.Subtract);
            f8.FunctionArguments[0][6].Assert(TokenType.Function, "f9");

            var f9 = f8.FunctionArguments[0][6];
            f9.FunctionArguments.Should().HaveCount(0);

            func.FunctionArguments[2].Should().HaveCount(1);
            var f10 = func.FunctionArguments[2][0];
            f10.Assert(TokenType.Function, "f10");
            f10.FunctionArguments.Should().HaveCount(2);

            f10.FunctionArguments[0].Should().HaveCount(1);
            f10.FunctionArguments[0][0].Assert(TokenType.Function, "f11");

            var f11 = f10.FunctionArguments[0][0];
            f11.FunctionArguments.Should().HaveCount(1);
            f11.FunctionArguments[0][0].Assert(TokenType.Name, "a");
            f11.FunctionArguments[0][1].Assert(TokenType.Divide);
            f11.FunctionArguments[0][2].Assert(TokenType.Name, "b");

            f10.FunctionArguments[1].Should().HaveCount(1);
            f10.FunctionArguments[1][0].Assert(TokenType.Function, "f12");

            var f12 = f10.FunctionArguments[1][0];
            f12.FunctionArguments.Should().HaveCount(1);
            f12.FunctionArguments[0][0].Assert(TokenType.OpeningBracket);
            f12.FunctionArguments[0][1].Assert(TokenType.Function, "f13");
            f12.FunctionArguments[0][2].Assert(TokenType.ClosingBracket);

            var f13 = f12.FunctionArguments[0][1];
            f13.FunctionArguments.Should().HaveCount(1);
            f13.FunctionArguments[0].Should().HaveCount(7);
            f13.FunctionArguments[0][0].Assert(TokenType.OpeningBracket);
            f13.FunctionArguments[0][1].Assert(TokenType.Name, "a");
            f13.FunctionArguments[0][2].Assert(TokenType.ClosingBracket);
            f13.FunctionArguments[0][3].Assert(TokenType.Subtract);
            f13.FunctionArguments[0][4].Assert(TokenType.OpeningBracket);
            f13.FunctionArguments[0][5].Assert(TokenType.Function, "f14");
            f13.FunctionArguments[0][6].Assert(TokenType.ClosingBracket);

            var f14 = f13.FunctionArguments[0][5];
            f14.FunctionArguments.Should().HaveCount(0);
        }
    }
}

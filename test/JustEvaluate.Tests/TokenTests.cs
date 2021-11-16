using System;
using FluentAssertions;
using Xunit;

namespace JustEvaluate.Tests
{
    public class TokenTests
    {
        [Fact]
        public void Consrtuct_From_EmptyText()
        {
            var input = string.Empty;

            var token = new Token(input);

            token.Assert(TokenType.Empty, string.Empty, null, input);
        }

        [Fact]
        public void Consrtuct_From_WhiteSpace()
        {
            var input = " ";

            var token = new Token(input);

            token.Assert(TokenType.Empty, string.Empty, null, input);
        }

        [Fact]
        public void Consrtuct_From_WhiteSpaces()
        {
            var input = "   ";

            var token = new Token(input);

            token.Assert(TokenType.Empty, string.Empty, null, input);
        }

        [Theory(DisplayName = "Consrtuct_From_TerminalCharText")]
        [InlineData("-", TokenType.Subtract)]
        [InlineData("+", TokenType.Add)]
        [InlineData("/", TokenType.Divide)]
        [InlineData("*", TokenType.Multipy)]
        [InlineData("(", TokenType.OpeningBracket)]
        [InlineData(")", TokenType.ClosingBracket)]
        [InlineData("&", TokenType.And)]
        [InlineData("|", TokenType.Or)]
        [InlineData(",", TokenType.FunctionParameterSeparator)]
        [InlineData(">", TokenType.GreaterThan)]
        [InlineData("<", TokenType.LessThan)]
        [InlineData("=", TokenType.EqualTo)]
        [InlineData(" -", TokenType.Subtract)]
        [InlineData(" +", TokenType.Add)]
        [InlineData(" /", TokenType.Divide)]
        [InlineData(" *", TokenType.Multipy)]
        [InlineData(" (", TokenType.OpeningBracket)]
        [InlineData(" )", TokenType.ClosingBracket)]
        [InlineData(" &", TokenType.And)]
        [InlineData(" |", TokenType.Or)]
        [InlineData(" ,", TokenType.FunctionParameterSeparator)]
        [InlineData(" >", TokenType.GreaterThan)]
        [InlineData(" <", TokenType.LessThan)]
        [InlineData(" =", TokenType.EqualTo)]
        [InlineData("- ", TokenType.Subtract)]
        [InlineData("+ ", TokenType.Add)]
        [InlineData("/ ", TokenType.Divide)]
        [InlineData("* ", TokenType.Multipy)]
        [InlineData("( ", TokenType.OpeningBracket)]
        [InlineData(") ", TokenType.ClosingBracket)]
        [InlineData("& ", TokenType.And)]
        [InlineData("| ", TokenType.Or)]
        [InlineData(", ", TokenType.FunctionParameterSeparator)]
        [InlineData("> ", TokenType.GreaterThan)]
        [InlineData("< ", TokenType.LessThan)]
        [InlineData("= ", TokenType.EqualTo)]
        [InlineData(" - ", TokenType.Subtract)]
        [InlineData(" + ", TokenType.Add)]
        [InlineData(" / ", TokenType.Divide)]
        [InlineData(" * ", TokenType.Multipy)]
        [InlineData(" ( ", TokenType.OpeningBracket)]
        [InlineData(" ) ", TokenType.ClosingBracket)]
        [InlineData(" & ", TokenType.And)]
        [InlineData(" | ", TokenType.Or)]
        [InlineData(" , ", TokenType.FunctionParameterSeparator)]
        [InlineData(" > ", TokenType.GreaterThan)]
        [InlineData(" < ", TokenType.LessThan)]
        [InlineData(" = ", TokenType.EqualTo)]
        public void Consrtuct_From_TerminalCharText(string input, TokenType type)
        {
            var token = new Token(input);

            token.Assert(type, input.Trim(), null, input);
        }

        [Theory(DisplayName = "Consrtuct_From_TerminalCharSequenceText")]
        [InlineData("<>", TokenType.NotEqualTo)]
        [InlineData("<=", TokenType.LessOrEqualTo)]
        [InlineData(">=", TokenType.GreaterOrEqualTo)]
        public void Consrtuct_From_TerminalCharSequenceText(string input, TokenType type)
        {
            var token = new Token(input);

            token.Assert(type, input.Trim(), null, input);
        }

        [Theory(DisplayName = "Consrtuct_From_TerminalChar")]
        [InlineData('-', TokenType.Subtract)]
        [InlineData('+', TokenType.Add)]
        [InlineData('/', TokenType.Divide)]
        [InlineData('*', TokenType.Multipy)]
        //[InlineData(')', TokenType.OpeningBracket)] // this is breaking VS Test Explorer for some reason?
        [InlineData('(', TokenType.OpeningBracket)]
        [InlineData(',', TokenType.FunctionParameterSeparator)]
        public void Consrtuct_From_TerminalChar(char input, TokenType type)
        {
            var token = new Token(input);

            token.Assert(type, input.ToString(), null, input.ToString());
        }

        [Fact]
        public void Consrtuct_From_TerminalChar_ClosingBracket()
        {
            var token = new Token(')');

            token.Assert(TokenType.ClosingBracket, ")", null, ")");
        }

        [Fact]
        public void Consrtuct_From_TerminalChar_NonTerminalChar_Throws()
        {
            Action action = () => { Token t = new Token('f'); };

            action.Should().Throw<InvalidOperationException>();
        }

        [Theory(DisplayName = "Consrtuct_From_NumericString")]
        [InlineData("123", 123)]
        [InlineData(".123", .123)]
        [InlineData("0.123", .123)]
        [InlineData(" 123", 123)]
        [InlineData(" .123", .123)]
        [InlineData(" 0.123", .123)]
        [InlineData("123 ", 123)]
        [InlineData(".123 ", .123)]
        [InlineData("0.123 ", .123)]
        [InlineData(" 123 ", 123)]
        [InlineData(" .123 ", .123)]
        [InlineData(" 0.123 ", .123)]
        public void Consrtuct_From_NumericString(string input, decimal expected)
        {
            var token = new Token(input);

            token.Assert(TokenType.Constant, input.Trim(), expected, input);
        }

        [Theory(DisplayName = "Consrtuct_From_Name")]
        [InlineData("a123")]
        [InlineData("a 1 2  3")]
        [InlineData(" a 1 2  3  ")]
        [InlineData("_fdgfer")]
        public void Consrtuct_From_Name(string input)
        {
            var token = new Token(input);

            token.Assert(TokenType.Name, input.Trim(), null, input);
        }

        [Theory]
        [InlineData(-12.445, "-12.445")]
        [InlineData(12.445, "12.445")]
        [InlineData(0.445, "0.445")]
        [InlineData(-0.445, "-0.445")]
        [InlineData(12, "12")]
        [InlineData(-12, "-12")]
        public void Consrtuct_From_Decimal(decimal value, string textValue)
        {
            var token = new Token(value);

            token.Assert(TokenType.Constant, textValue, value, textValue);
        }

        [Theory]
        [InlineData("*", true)]
        [InlineData("/", true)]
        [InlineData("+", true)]
        [InlineData("-", true)]
        [InlineData(")", false)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("10", false)]
        public void Operator_IsRecognized(string text, bool isOperator)
        {
            var token = new Token(text);

            token.IsOperator.Should().Be(isOperator);
        }

        [Theory]
        [InlineData("*", false)]
        [InlineData("/", false)]
        [InlineData("+", true)]
        [InlineData("-", false)]
        [InlineData(")", false)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("10", false)]
        public void Addition_IsRecognized(string text, bool isAddition)
        {
            var token = new Token(text);

            token.IsAdd.Should().Be(isAddition);
        }

        [Theory]
        [InlineData("*", false)]
        [InlineData("/", false)]
        [InlineData("+", false)]
        [InlineData("-", true)]
        [InlineData(")", false)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("10", false)]
        public void Subtraction_IsRecognized(string text, bool isSubtraction)
        {
            var token = new Token(text);

            token.IsSubtract.Should().Be(isSubtraction);
        }

        [Theory]
        [InlineData("*", true)]
        [InlineData("/", false)]
        [InlineData("+", false)]
        [InlineData("-", false)]
        [InlineData(")", false)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("10", false)]
        public void Multiplication_IsRecognized(string text, bool isMultiplication)
        {
            var token = new Token(text);

            token.IsMultiply.Should().Be(isMultiplication);
        }

        [Theory]
        [InlineData("*", false)]
        [InlineData("/", true)]
        [InlineData("+", false)]
        [InlineData("-", false)]
        [InlineData(")", false)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("10", false)]
        public void Division_IsRecognized(string text, bool isDivision)
        {
            var token = new Token(text);

            token.IsDivide.Should().Be(isDivision);
        }

        [Theory]
        [InlineData("*", false)]
        [InlineData("/", false)]
        [InlineData("+", false)]
        [InlineData("-", false)]
        [InlineData(")", false)]
        [InlineData("(", true)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("10", false)]
        public void OpeningBracket_IsRecognized(string text, bool isOpeningBracket)
        {
            var token = new Token(text);

            token.IsOpeningBracket.Should().Be(isOpeningBracket);
        }

        [Theory]
        [InlineData("*", false)]
        [InlineData("/", false)]
        [InlineData("+", false)]
        [InlineData("-", false)]
        [InlineData(")", true)]
        [InlineData("(", false)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("10", false)]
        public void ClosingBracket_IsRecognized(string text, bool isClosingBracket)
        {
            var token = new Token(text);

            token.IsClosingBracket.Should().Be(isClosingBracket);
        }

        [Theory]
        [InlineData("*", false)]
        [InlineData("/", false)]
        [InlineData("+", false)]
        [InlineData("-", false)]
        [InlineData(")", false)]
        [InlineData("(", false)]
        [InlineData(",", false)]
        [InlineData("a", true)]
        [InlineData("10", false)]
        public void Name_IsRecognized(string text, bool isName)
        {
            var token = new Token(text);

            token.IsName.Should().Be(isName);
        }

        [Fact]
        public void Constant_IsRecognized()
        {
            var token = new Token(1);

            token.IsConstant.Should().BeTrue();
        }

        [Fact]
        public void Function_IsRecognized()
        {
            var token = new Token("aaa");
            token.ChangeToFunction();

            token.IsFunction.Should().BeTrue();
        }

        [Fact]
        public void ChangeToFunction_ChangesName()
        {
            var token = new Token("a a a");
            token.ChangeToFunction();

            token.IsFunction.Should().BeTrue();
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("1")]
        [InlineData("*")]
        [InlineData("+")]
        [InlineData("-")]
        [InlineData("/")]
        [InlineData("(")]
        [InlineData(")")]
        [InlineData(",")]
        public void ChangeToFunction_ChangesNameOnly(string text)
        {
            var token = new Token(text);

            Action action = () => token.ChangeToFunction();

            action.Should().Throw<Exception>($"Cannot change '{token.Type}' to function");
        }

        [Theory]
        [InlineData("+", "a")]
        [InlineData("-", "a")]
        [InlineData("*", "a")]
        [InlineData("/", "a")]
        [InlineData("a", "+")]
        [InlineData("a", "-")]
        [InlineData("a", "*")]
        [InlineData("a", "/")]
        [InlineData("a", ")")]
        public void LessOrEqualPrecendanceOver_WhenTokenIsNotOperator_Throws(string x, string y)
        {
            var token1 = new Token(x);
            var token2 = new Token(y);

            Action action = () => token1.LessOrEqualPrecendanceOver(token2);

            action.Should().Throw<InvalidOperationException>("Only operators support precendence");
        }

        [Theory]
        [InlineData("+", "+", true)]
        [InlineData("+", "-", true)]
        [InlineData("+", "*", true)]
        [InlineData("+", "/", true)]
        [InlineData("-", "+", true)]
        [InlineData("-", "-", true)]
        [InlineData("-", "*", true)]
        [InlineData("-", "/", true)]
        [InlineData("*", "+", false)]
        [InlineData("*", "-", false)]
        [InlineData("*", "*", true)]
        [InlineData("*", "/", true)]
        [InlineData("/", "+", false)]
        [InlineData("/", "-", false)]
        [InlineData("/", "*", true)]
        [InlineData("/", "/", true)]
        [InlineData("&", "-", true)]
        [InlineData("|", "-", true)]
        [InlineData("|", "&", true)]
        [InlineData(">", "-", true)]
        [InlineData("<", "-", true)]
        [InlineData(">=", "-", true)]
        [InlineData("<=", "-", true)]
        [InlineData("=", ">", true)]
        [InlineData("=", "<", true)]
        [InlineData("=", ">=", true)]
        [InlineData("=", "<=", true)]
        [InlineData("<>", ">", true)]
        [InlineData("<>", "<", true)]
        [InlineData("<>", ">=", true)]
        [InlineData("<>", "<=", true)]
        [InlineData("&", "=", true)]
        [InlineData("|", "=", true)]
        public void LessOrEqualPrecendanceOver(string x, string y, bool expected)
        {
            var token1 = new Token(x);
            var token2 = new Token(y);

            bool result = token1.LessOrEqualPrecendanceOver(token2);

            result.Should().Be(expected);
        }
    }
}

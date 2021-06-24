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

            token.Type.Should().Be(TokenType.Empty);
            token.Value.Should().BeEmpty();
            token.Text.Should().Be(input);
            token.NumericValue.Should().BeNull();
        }

        [Fact]
        public void Consrtuct_From_WhiteSpace()
        {
            var input = " ";

            var token = new Token(input);

            token.Type.Should().Be(TokenType.Empty);
            token.Value.Should().BeEmpty();
            token.Text.Should().Be(input);
            token.NumericValue.Should().BeNull();
        }

        [Fact]
        public void Consrtuct_From_WhiteSpaces()
        {
            var input = "   ";

            var token = new Token(input);

            token.Type.Should().Be(TokenType.Empty);
            token.Value.Should().BeEmpty();
            token.Text.Should().Be(input);
            token.NumericValue.Should().BeNull();
        }

        [Theory(DisplayName = "Consrtuct_From_TerminalCharText")]
        [InlineData("-", TokenType.Subtract)]
        [InlineData("+", TokenType.Add)]
        [InlineData("/", TokenType.Divide)]
        [InlineData("*", TokenType.Multipy)]
        [InlineData("(", TokenType.OpeningBracket)]
        [InlineData(")", TokenType.ClosingBracket)]
        [InlineData(",", TokenType.FunctionParameterSeparator)]
        [InlineData(" -", TokenType.Subtract)]
        [InlineData(" +", TokenType.Add)]
        [InlineData(" /", TokenType.Divide)]
        [InlineData(" *", TokenType.Multipy)]
        [InlineData(" (", TokenType.OpeningBracket)]
        [InlineData(" )", TokenType.ClosingBracket)]
        [InlineData(" ,", TokenType.FunctionParameterSeparator)]
        [InlineData("- ", TokenType.Subtract)]
        [InlineData("+ ", TokenType.Add)]
        [InlineData("/ ", TokenType.Divide)]
        [InlineData("* ", TokenType.Multipy)]
        [InlineData("( ", TokenType.OpeningBracket)]
        [InlineData(") ", TokenType.ClosingBracket)]
        [InlineData(", ", TokenType.FunctionParameterSeparator)]
        [InlineData(" - ", TokenType.Subtract)]
        [InlineData(" + ", TokenType.Add)]
        [InlineData(" / ", TokenType.Divide)]
        [InlineData(" * ", TokenType.Multipy)]
        [InlineData(" ( ", TokenType.OpeningBracket)]
        [InlineData(" ) ", TokenType.ClosingBracket)]
        [InlineData(" , ", TokenType.FunctionParameterSeparator)]
        public void Consrtuct_From_TerminalCharText(string input, TokenType type)
        {
            var token = new Token(input);

            token.Type.Should().Be(type);
            token.Value.Should().Be(input.Trim());
            token.Text.Should().Be(input);
            token.NumericValue.Should().BeNull();
        }

        [Theory(DisplayName = "Consrtuct_From_TerminalChar")]
        [InlineData('-', TokenType.Subtract)]
        [InlineData('+', TokenType.Add)]
        [InlineData('/', TokenType.Divide)]
        [InlineData('*', TokenType.Multipy)]
        [InlineData('(', TokenType.OpeningBracket)]
        [InlineData(')', TokenType.ClosingBracket)]
        [InlineData(',', TokenType.FunctionParameterSeparator)]
        public void Consrtuct_From_TerminalChar(char input, TokenType type)
        {
            var token = new Token(input);

            token.Type.Should().Be(type);
            token.Value.Should().Be(input.ToString());
            token.Text.Should().Be(input.ToString());
            token.NumericValue.Should().BeNull();
        }

        [Fact]
        public void Consrtuct_From_TerminalChar_NonTerminalChar_Throws()
        {
            System.Action sss = () => { Token t = new Token('f'); };

            sss.Should().Throw<InvalidOperationException>();
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

            token.Type.Should().Be(TokenType.Constant);
            token.Value.Should().Be(input.Trim());
            token.Text.Should().Be(input);
            token.NumericValue.Should().Be(expected);
        }

        [Theory(DisplayName = "Consrtuct_From_Name")]
        [InlineData("a123")]
        [InlineData("a 1 2  3")]
        [InlineData(" a 1 2  3  ")]
        [InlineData("_fdgfer")]
        public void Consrtuct_From_Name(string input)
        {
            var token = new Token(input);

            token.Type.Should().Be(TokenType.Name);
            token.Value.Should().Be(input.Trim());
            token.Text.Should().Be(input);
            token.NumericValue.Should().BeNull();
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

            token.Type.Should().Be(TokenType.Constant);
            token.Value.Should().Be(textValue);
            token.Text.Should().Be(textValue);
            token.NumericValue.Should().Be(value);
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
    }
}


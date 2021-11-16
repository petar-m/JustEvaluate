using System;
using FluentAssertions;
using Xunit;

namespace JustEvaluate.Tests
{
    public class CharExtensionsTests
    {
        [Fact]
        public void TerminalCharToTokenType_WhenNotTerminal_Throws()
        {
            var nonTerminal = 'a';

            Action action = () => nonTerminal.TerminalCharToTokenType();

            action.Should().Throw<InvalidOperationException>($"Character '{nonTerminal}' is not terminal");
        }

        [Theory]
        [InlineData("+", TokenType.Add)]
        [InlineData("-", TokenType.Subtract)]
        [InlineData("*", TokenType.Multipy)]
        [InlineData("/", TokenType.Divide)]
        [InlineData("(", TokenType.OpeningBracket)]
        [InlineData(")", TokenType.ClosingBracket)]
        [InlineData(",", TokenType.FunctionParameterSeparator)]
        [InlineData("|", TokenType.Or)]
        [InlineData("&", TokenType.And)]
        [InlineData(">", TokenType.GreaterThan)]
        [InlineData("<", TokenType.LessThan)]
        [InlineData("=", TokenType.EqualTo)]
        public void TerminalChar_ConvertedToTokenType(string symbol, TokenType tokenType)
        {
            var c = symbol[0];

            var result = c.TerminalCharToTokenType();

            result.Should().Be(tokenType);
        }

        [Theory]
        [InlineData(">", true)]
        [InlineData("<", true)]
        [InlineData("=", false)]
        [InlineData("!", false)]
        [InlineData("&", false)]
        public void TerminalChar_IsPossibleTerminalSequence(string symbol, bool expected)
        {
            var c = symbol[0];

            var result = c.IsPossibleTerminalSequence();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(">", true)]
        [InlineData("=", true)]
        [InlineData("<", false)]
        [InlineData("!", false)]
        [InlineData("&", false)]
        public void TerminalChar_IsNextInTerminalSequence(string symbol, bool expected)
        {
            var c = symbol[0];

            var result = c.IsNextInTerminalSequence();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("+", true)]
        [InlineData("-", true)]
        [InlineData("*", true)]
        [InlineData("/", true)]
        [InlineData("(", true)]
        [InlineData(")", true)]
        [InlineData(",", true)]
        [InlineData("a", false)]
        [InlineData("1", false)]
        [InlineData("_", false)]
        [InlineData("#", false)]
        public void TerminalChar_IsRecognized(string symbol, bool expected)
        {
            var c = symbol[0];

            var result = c.IsTerminalChar();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("+", true)]
        [InlineData("-", true)]
        [InlineData("*", true)]
        [InlineData("/", true)]
        [InlineData("(", false)]
        [InlineData(")", false)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("1", false)]
        [InlineData("_", false)]
        [InlineData("!", false)]
        [InlineData(".", false)]
        public void Operator_IsRecognized(string symbol, bool expected)
        {
            var c = symbol[0];

            var result = c.IsOperator();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("+", false)]
        [InlineData("-", false)]
        [InlineData("*", false)]
        [InlineData("/", false)]
        [InlineData("(", true)]
        [InlineData(")", true)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("1", false)]
        [InlineData("_", false)]
        [InlineData("!", false)]
        [InlineData(".", false)]
        public void Bracket_IsRecognized(string symbol, bool expected)
        {
            var c = symbol[0];

            var result = c.IsBracket();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("+", false)]
        [InlineData("-", false)]
        [InlineData("*", false)]
        [InlineData("/", false)]
        [InlineData("(", true)]
        [InlineData(")", false)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("1", false)]
        [InlineData("_", false)]
        [InlineData("!", false)]
        [InlineData(".", false)]
        public void OpeningBracket_IsRecognized(string symbol, bool expected)
        {
            var c = symbol[0];

            var result = c.IsOpeningBracket();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("+", false)]
        [InlineData("-", false)]
        [InlineData("*", false)]
        [InlineData("/", false)]
        [InlineData("(", false)]
        [InlineData(")", true)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("1", false)]
        [InlineData("_", false)]
        [InlineData("!", false)]
        [InlineData(".", false)]
        public void ClosingBracket_IsRecognized(string symbol, bool expected)
        {
            var c = symbol[0];

            var result = c.IsClosingBracket();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("+", true)]
        [InlineData("-", false)]
        [InlineData("*", false)]
        [InlineData("/", false)]
        [InlineData("(", false)]
        [InlineData(")", false)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("1", false)]
        [InlineData("_", false)]
        [InlineData("!", false)]
        [InlineData(".", false)]
        public void Plus_IsRecognized(string symbol, bool expected)
        {
            var c = symbol[0];

            var result = c.IsPlus();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("+", false)]
        [InlineData("-", true)]
        [InlineData("*", false)]
        [InlineData("/", false)]
        [InlineData("(", false)]
        [InlineData(")", false)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("1", false)]
        [InlineData("_", false)]
        [InlineData("!", false)]
        [InlineData(".", false)]
        public void Minus_IsRecognized(string symbol, bool expected)
        {
            var c = symbol[0];

            var result = c.IsMinus();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("+", false)]
        [InlineData("-", false)]
        [InlineData("*", true)]
        [InlineData("/", false)]
        [InlineData("(", false)]
        [InlineData(")", false)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("1", false)]
        [InlineData("_", false)]
        [InlineData("!", false)]
        [InlineData(".", false)]
        public void Multiply_IsRecognized(string symbol, bool expected)
        {
            var c = symbol[0];

            var result = c.IsMiltiply();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("+", false)]
        [InlineData("-", false)]
        [InlineData("*", false)]
        [InlineData("/", true)]
        [InlineData("(", false)]
        [InlineData(")", false)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("1", false)]
        [InlineData("_", false)]
        [InlineData("!", false)]
        [InlineData(".", false)]
        public void Divide_IsRecognized(string symbol, bool expected)
        {
            var c = symbol[0];

            var result = c.IsDivide();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("+", false)]
        [InlineData("-", false)]
        [InlineData("*", false)]
        [InlineData("/", false)]
        [InlineData("(", false)]
        [InlineData(")", false)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("1", true)]
        [InlineData("_", false)]
        [InlineData("!", false)]
        [InlineData(".", true)]
        public void NumericPart_IsRecognized(string symbol, bool expected)
        {
            var c = symbol[0];

            var result = c.IsNumericPart();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("+", false)]
        [InlineData("-", false)]
        [InlineData("*", false)]
        [InlineData("/", false)]
        [InlineData("(", false)]
        [InlineData(")", false)]
        [InlineData(",", true)]
        [InlineData("a", false)]
        [InlineData("1", false)]
        [InlineData("_", false)]
        [InlineData("!", false)]
        [InlineData(".", false)]
        public void FunctionParameterSeparator_IsRecognized(string symbol, bool expected)
        {
            var c = symbol[0];

            var result = c.IsFunctionParameterSeparator();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("+", false)]
        [InlineData("-", false)]
        [InlineData("*", false)]
        [InlineData("/", false)]
        [InlineData("(", false)]
        [InlineData(")", false)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("1", false)]
        [InlineData("_", false)]
        [InlineData("!", false)]
        [InlineData(".", true)]
        public void DecimalSeparator_IsRecognized(string symbol, bool expected)
        {
            var c = symbol[0];

            var result = c.IsDecimalSeparator();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("+", false)]
        [InlineData("-", false)]
        [InlineData("*", false)]
        [InlineData("/", false)]
        [InlineData("(", false)]
        [InlineData(")", false)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("1", false)]
        [InlineData("_", false)]
        [InlineData("!", false)]
        [InlineData(".", false)]
        [InlineData("=", true)]
        public void EqualTo_IsRecognized(string symbol, bool expected)
        {
            var c = symbol[0];

            var result = c.IsEqualsTo();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("+", false)]
        [InlineData("-", false)]
        [InlineData("*", false)]
        [InlineData("/", false)]
        [InlineData("(", false)]
        [InlineData(")", false)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("1", false)]
        [InlineData("_", false)]
        [InlineData("!", false)]
        [InlineData(".", false)]
        [InlineData(">", true)]
        public void GreaterThan_IsRecognized(string symbol, bool expected)
        {
            var c = symbol[0];

            var result = c.IsGreaterThan();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("+", false)]
        [InlineData("-", false)]
        [InlineData("*", false)]
        [InlineData("/", false)]
        [InlineData("(", false)]
        [InlineData(")", false)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("1", false)]
        [InlineData("_", false)]
        [InlineData("!", false)]
        [InlineData(".", false)]
        [InlineData("<", true)]
        public void LessThan_IsRecognized(string symbol, bool expected)
        {
            var c = symbol[0];

            var result = c.IsLessThan();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("+", false)]
        [InlineData("-", false)]
        [InlineData("*", false)]
        [InlineData("/", false)]
        [InlineData("(", false)]
        [InlineData(")", false)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("1", false)]
        [InlineData("_", false)]
        [InlineData("!", false)]
        [InlineData(".", false)]
        [InlineData("&", true)]
        public void And_IsRecognized(string symbol, bool expected)
        {
            var c = symbol[0];

            var result = c.IsAnd();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("+", false)]
        [InlineData("-", false)]
        [InlineData("*", false)]
        [InlineData("/", false)]
        [InlineData("(", false)]
        [InlineData(")", false)]
        [InlineData(",", false)]
        [InlineData("a", false)]
        [InlineData("1", false)]
        [InlineData("_", false)]
        [InlineData("!", false)]
        [InlineData(".", false)]
        [InlineData("|", true)]
        public void Or_IsRecognized(string symbol, bool expected)
        {
            var c = symbol[0];

            var result = c.IsOr();

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData('>', '=', TokenType.GreaterOrEqualTo)]
        [InlineData('<', '=', TokenType.LessOrEqualTo)]
        [InlineData('<', '>', TokenType.NotEqualTo)]
        public void TerminalSequenceToTokenType(char c1, char c2, TokenType expected)
        {
            var chars = new[] { c1, c2 };

            var result = chars.TerminalSequenceToTokenType();

            result.Should().Be(expected);
        }

        [Fact]
        public void TerminalSequenceToTokenType_WhenLessThan2Chars_Throws()
        {
            Action action = () => new char[] { 'a' }.TerminalSequenceToTokenType();

            action.Should().Throw<ArgumentException>("Terminal sequence expected length is 2");
        }

        [Fact]
        public void TerminalSequenceToTokenType_WhenMoreThan2Chars_Throws()
        {
            Action action = () => new char[] { 'a', 'b', 'c' }.TerminalSequenceToTokenType();

            action.Should().Throw<ArgumentException>("Terminal sequence expected length is 2");
        }

        [Fact]
        public void TerminalSequenceToTokenType_WhenUnknown_Throws()
        {
            var chars = new[] { 'a', 'b' };
            Action action = () => chars.TerminalSequenceToTokenType();

            action.Should().Throw<ArgumentException>($"Invalid terminal sequence expected '{string.Join(string.Empty, chars)}'");
        }
    }
}

using FluentAssertions;

namespace JustEvaluate.Tests
{
    public static class AssertExtensions
    {
        public static void Assert(this Token token, TokenType type, string value, decimal? numericValue, string text)
        {
            token.Type.Should().Be(type);
            token.Value.Should().Be(value);
            token.Text.Should().Be(text);
            token.NumericValue.Should().Be(numericValue);
        }

        public static void Assert(this Token token, TokenType type, string value)
        {
            token.Type.Should().Be(type);
            token.Value.Should().Be(value);
        }

        public static void Assert(this Token token, TokenType type, decimal? numericValue)
        {
            token.Type.Should().Be(type);
            token.NumericValue.Should().Be(numericValue);
        }

        public static void Assert(this Token token, TokenType type) => token.Type.Should().Be(type);
    }
}

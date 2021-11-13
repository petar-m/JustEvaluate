using FluentAssertions;
using Xunit;

namespace JustEvaluate.Tests
{
    public partial class FunctionsRegistryExtensionsTests
    {
        [Theory]
        [InlineData(5, 10, 20, 0)]
        [InlineData(10, 10, 20, 0)]
        [InlineData(15, 10, 20, 1)]
        [InlineData(20, 10, 20, 0)]
        [InlineData(25, 10, 20, 0)]
        public void Between(decimal x, decimal y, decimal z, decimal expected)
        {
            var input = new Input { X = x, Y = y, Z = z };
            _setup.Evaluator.Evaluate("Between(x, y, z)", input).Should().Be(expected);
        }

        [Theory]
        [InlineData(5, 10, 20, 0)]
        [InlineData(10, 10, 20, 1)]
        [InlineData(15, 10, 20, 1)]
        [InlineData(20, 10, 20, 0)]
        [InlineData(25, 10, 20, 0)]
        public void BetweenLeftInclusive(decimal x, decimal y, decimal z, decimal expected)
        {
            var input = new Input { X = x, Y = y, Z = z };
            _setup.Evaluator.Evaluate("BetweenLeftInclusive(x, y, z)", input).Should().Be(expected);
        }

        [Theory]
        [InlineData(5, 10, 20, 0)]
        [InlineData(10, 10, 20, 0)]
        [InlineData(15, 10, 20, 1)]
        [InlineData(20, 10, 20, 1)]
        [InlineData(25, 10, 20, 0)]
        public void BetweenRightInclusive(decimal x, decimal y, decimal z, decimal expected)
        {
            var input = new Input { X = x, Y = y, Z = z };
            _setup.Evaluator.Evaluate("BetweenRightInclusive(x, y, z)", input).Should().Be(expected);
        }

        [Theory]
        [InlineData(5, 10, 20, 0)]
        [InlineData(10, 10, 20, 1)]
        [InlineData(15, 10, 20, 1)]
        [InlineData(20, 10, 20, 1)]
        [InlineData(25, 10, 20, 0)]
        public void BetweenInclusive(decimal x, decimal y, decimal z, decimal expected)
        {
            var input = new Input { X = x, Y = y, Z = z };
            _setup.Evaluator.Evaluate("BetweenInclusive(x, y, z)", input).Should().Be(expected);
        }
    }
}

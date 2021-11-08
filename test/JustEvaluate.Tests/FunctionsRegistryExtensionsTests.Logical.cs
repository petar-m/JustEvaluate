using FluentAssertions;
using Xunit;

namespace JustEvaluate.Tests
{
    public partial class FunctionsRegistryExtensionsTests
    {
        [Theory]
        [InlineData(10, 20, 0)]
        [InlineData(20, 10, 1)]
        [InlineData(10, 10, 0)]
        public void GreaterThan(decimal x, decimal y, decimal expected)
        {
            var input = new Input { X = x, Y = y };
            _setup.Evaluator.Evaluate("GreaterThan(x, y)", input).Should().Be(expected);
        }

        [Theory]
        [InlineData(10, 20, 0)]
        [InlineData(20, 10, 1)]
        [InlineData(10, 10, 1)]
        public void GreaterThanOrEqual(decimal x, decimal y, decimal expected)
        {
            var input = new Input { X = x, Y = y };
            _setup.Evaluator.Evaluate("GreaterThanOrEqual(x, y)", input).Should().Be(expected);
        }

        [Theory]
        [InlineData(10, 20, 1)]
        [InlineData(20, 10, 0)]
        [InlineData(10, 10, 0)]
        public void LessThan(decimal x, decimal y, decimal expected)
        {
            var input = new Input { X = x, Y = y };
            _setup.Evaluator.Evaluate("LessThan(x, y)", input).Should().Be(expected);
        }

        [Theory]
        [InlineData(10, 20, 1)]
        [InlineData(20, 10, 0)]
        [InlineData(10, 10, 1)]
        public void LessThanOrEqual(decimal x, decimal y, decimal expected)
        {
            var input = new Input { X = x, Y = y };
            _setup.Evaluator.Evaluate("LessThanOrEqual(x, y)", input).Should().Be(expected);
        }

        [Theory]
        [InlineData(10, 20, 0)]
        [InlineData(20, 10, 0)]
        [InlineData(10, 10, 1)]
        public void EqualTo(decimal x, decimal y, decimal expected)
        {
            var input = new Input { X = x, Y = y };
            _setup.Evaluator.Evaluate("EqualTo(x, y)", input).Should().Be(expected);
        }

        [Theory]
        [InlineData(10, 20, 1)]
        [InlineData(20, 10, 1)]
        [InlineData(10, 10, 0)]
        public void NotEqualTo(decimal x, decimal y, decimal expected)
        {
            var input = new Input { X = x, Y = y };
            _setup.Evaluator.Evaluate("NotEqualTo(x, y)", input).Should().Be(expected);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(0, 1)]
        public void Not(decimal x, decimal expected)
        {
            var input = new Input { X = x, };
            _setup.Evaluator.Evaluate("Not(x)", input).Should().Be(expected);
        }

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

        [Theory]
        [InlineData(11, 21, -3, 21)]
        [InlineData(-12, 22, 0, 22)]
        [InlineData(0, 21, -3, -3)]
        public void If(decimal x, decimal y, decimal z, decimal expected)
        {
            var input = new Input { X = x, Y = y, Z = z };
            _setup.Evaluator.Evaluate("If(x, y, z)", input).Should().Be(expected);
        }
    }
}

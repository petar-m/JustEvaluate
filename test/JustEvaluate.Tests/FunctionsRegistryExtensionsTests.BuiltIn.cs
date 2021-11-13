using FluentAssertions;
using Xunit;

namespace JustEvaluate.Tests
{
    public partial class FunctionsRegistryExtensionsTests
    {
        [Theory]
        [InlineData(1, 0)]
        [InlineData(0, 1)]
        [InlineData(10.89, 0)]
        public void Not(decimal x, decimal expected)
        {
            var input = new Input { X = x, };
            _setup.Evaluator.Evaluate("Not(x)", input).Should().Be(expected);
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

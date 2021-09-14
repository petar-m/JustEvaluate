using FluentAssertions;
using Xunit;

namespace JustEvaluate.Tests
{
    public partial class FunctionExtensionsTests
    {
        [Theory]
        [InlineData(10, 20, 10)]
        [InlineData(20, 10, 10)]
        [InlineData(10, 10, 10)]
        public void Min_2_Arguments(decimal x, decimal y, decimal expected)
        {
            var input = new Input { X = x, Y = y };
            _setup.Evaluator.Evaluate("Min(x, y)", input).Should().Be(expected);
        }

        [Theory]
        [InlineData(10, 20, 30, 10)]
        [InlineData(30, 20, 10, 10)]
        [InlineData(10, 10, 10, 10)]
        public void Min_3_Arguments(decimal x, decimal y, decimal z, decimal expected)
        {
            var input = new Input { X = x, Y = y, Z = z };
            _setup.Evaluator.Evaluate("Min(x, y, z)", input).Should().Be(expected);
        }

        [Theory]
        [InlineData(10, 20, 20)]
        [InlineData(20, 10, 20)]
        [InlineData(10, 10, 10)]
        public void Max_2_Arguments(decimal x, decimal y, decimal expected)
        {
            var input = new Input { X = x, Y = y };
            _setup.Evaluator.Evaluate("Max(x, y)", input).Should().Be(expected);
        }

        [Theory]
        [InlineData(1.29, 1)]
        [InlineData(1.50, 2)]
        [InlineData(1.69, 2)]
        public void Round(decimal x, decimal expected)
        {
            var input = new Input { X = x };
            _setup.Evaluator.Evaluate("Round(x)", input).Should().Be(expected);
        }

        [Theory]
        [InlineData(1.295, 1, 1.3)]
        [InlineData(1.295, 2, 1.3)]
        [InlineData(1.295, 2.1, 1.3)]
        [InlineData(1.295, 2.9, 1.3)]
        [InlineData(1.50, 0, 2)]
        [InlineData(1.69, 1, 1.7)]
        public void Round_With_Precision(decimal x, decimal y, decimal expected)
        {
            var input = new Input { X = x, Y = y };
            _setup.Evaluator.Evaluate("Round(x, y)", input).Should().Be(expected);
        }

        [Theory]
        [InlineData(1.29, 1)]
        [InlineData(1.50, 1)]
        [InlineData(1.69, 1)]
        [InlineData(-1.29, -2)]
        [InlineData(-1.50, -2)]
        [InlineData(-1.69, -2)]
        public void Floor(decimal x, decimal expected)
        {
            var input = new Input { X = x };
            _setup.Evaluator.Evaluate("Floor(x)", input).Should().Be(expected);
        }

        [Theory]
        [InlineData(1.29, 2)]
        [InlineData(1.50, 2)]
        [InlineData(1.69, 2)]
        [InlineData(-1.29, -1)]
        [InlineData(-1.50, -1)]
        [InlineData(-1.69, -1)]
        public void Ceiling(decimal x, decimal expected)
        {
            var input = new Input { X = x };
            _setup.Evaluator.Evaluate("Ceiling(x)", input).Should().Be(expected);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(4, 2)]
        [InlineData(9, 3)]
        public void Sqrt(decimal x, decimal expected)
        {
            var input = new Input { X = x };
            _setup.Evaluator.Evaluate("Sqrt(x)", input).Should().Be(expected);
        }

        [Theory]
        [InlineData(-3, 0, 1)]
        [InlineData(2, 2, 4)]
        [InlineData(9, .5, 3)]
        public void Pow(decimal x, decimal y, decimal expected)
        {
            var input = new Input { X = x, Y = y };
            _setup.Evaluator.Evaluate("Pow(x, y)", input).Should().Be(expected);
        }

        [Theory]
        [InlineData(1.4, 1.4)]
        [InlineData(-4.2, 4.2)]
        [InlineData(0, 0)]
        public void Abs(decimal x, decimal expected)
        {
            var input = new Input { X = x };
            _setup.Evaluator.Evaluate("Abs(x)", input).Should().Be(expected);
        }
    }
}

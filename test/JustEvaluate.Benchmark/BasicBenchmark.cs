using System;
using BenchmarkDotNet.Attributes;
using JustEvaluate.UtilityFunctions;

namespace JustEvaluate.Benchmark
{
    [SimpleJob]
    public class BasicBenchmark
    {
        private Evaluator _evaluator;
        private Func<Input, decimal> _function;
        private readonly string formula = "Amount * 10 / 100 + Sqrt(Amount)";

        [GlobalSetup]
        public void Setup()
        {
            var expressionCache = new CompiledExpressionsCache();
            _evaluator = new Evaluator(new Parser(), new Builder(new FunctionsRegistry().AddMath().AddLogical()), expressionCache);

            // force caching
            _evaluator.Evaluate(formula, new Input { Year = 0 });

            _function = expressionCache.Get<Input>(formula);
        }

        [Benchmark]
        public decimal UsingEvaluator() => _evaluator.Evaluate(formula, new Input {Year = 123});

        [Benchmark]
        public decimal UsingCompiledExpression() => _function(new Input { Year = 123 });

        [Benchmark(Baseline = true)]
        public decimal Coded() => Calculate(new Input { Year = 123 });

        public decimal Calculate(Input input) => input.Year * 10 / 100 + (decimal)Math.Sqrt((double)input.Year);
    }

    public class Input
    {
        public decimal Year { get; set; }
    }
}

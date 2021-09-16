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
            _evaluator.Evaluate(formula, new Input { Amount = 0 });

            _function = expressionCache.Get<Input>(formula);
        }

        [Params(40, 1098887)]
        public decimal Amount { get; set; }

        [Benchmark]
        public decimal UsingEvaluator() => _evaluator.Evaluate(formula, new Input {Amount = Amount });

        [Benchmark]
        public decimal UsingCompiledExpression() => _function(new Input { Amount = Amount });

        [Benchmark(Baseline = true)]
        public decimal Coded()
        {
            return Amount * 10 / 100 + (decimal)Math.Sqrt((double)Amount);
        }
    }

    public class Input
    {
        public decimal Amount { get; set; }
    }
}

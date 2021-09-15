using System;
using BenchmarkDotNet.Attributes;
using JustEvaluate.UtilityFunctions;

namespace JustEvaluate.Benchmark
{

    [SimpleJob]
    public class BasicBenchmark2
    {
        private Evaluator _evaluator;
        private Func<Input, decimal> _function;
        private readonly string formula = "EqualTo(Year / 4, Floor(Year / 4)) * If(NotEqualTo(Year / 100, Floor(Year / 100)), 1, EqualTo(Year / 400, Floor(Year / 400)))";

        [GlobalSetup]
        public void Setup()
        {
            var expressionCache = new CompiledExpressionsCache();
            _evaluator = new Evaluator(new Parser(), new Builder(new FunctionsRegistry().AddMath().AddLogical()), expressionCache);

            // force caching
            _evaluator.Evaluate(formula, new Input { Year = 0 });

            _function = expressionCache.Get<Input>(formula);
        }

        [Params(1986, 1988, 1900, 2000)]
        public int Year { get; set; }

        [Benchmark]
        public decimal UsingEvaluator() => _evaluator.Evaluate(formula, new Input { Year = Year });

        [Benchmark]
        public decimal UsingCompiledExpression() => _function(new Input { Year = Year });

        [Benchmark(Baseline = true)]
        public decimal Coded() => Calculate(new Input { Year = Year });

        public decimal Calculate(Input input) => input.Year % 4 == 0 && (input.Year % 100 != 0 || input.Year % 400 == 0) ? 1 : 0;
    }
}

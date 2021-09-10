using System;
using BenchmarkDotNet.Attributes;

namespace JustEvaluate.Benchmark
{
    [SimpleJob]
    public class BasicBenchmark
    {
        private readonly Evaluator _evaluator = new Evaluator(new Parser(), new Builder(new Functions()), new ExpressionCache());
        private readonly Helper _helper = new Helper(1, 1);
        private Func<decimal> function;
        private const string expression = "1 + 1";

        [GlobalSetup]
        public void Setup()
        {
            _ = _evaluator.Evaluate("1 + 1");
            function = new Builder(new Functions()).Build(new Parser().Parse(expression));
        }

        [Benchmark]
        public decimal Evaluated() => _evaluator.Evaluate("1 + 1");

        [Benchmark]
        public decimal EvaluatedWithoutCache() => function();

        //[Benchmark]
        //public decimal EvaluatedNoWarmUp() => new Evaluator(new Parser(), new Builder(new Functions()), new ExpressionCache()).Evaluate("1 + 1");

        [Benchmark(Baseline = true)]
        public decimal Coded() => _helper.Basic();
    }
}

using BenchmarkDotNet.Running;

namespace JustEvaluate.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<BasicBenchmark>();
        }
    }

    public class Helper
    {
        private readonly decimal _x;
        private readonly decimal _y;

        public Helper(decimal x, decimal y)
        {
            _x = x;
            _y = y;
        }

        public decimal Basic() => _x + _y;
    }
}

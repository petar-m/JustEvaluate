using BenchmarkDotNet.Running;

namespace JustEvaluate.Benchmark
{
    static class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<BasicBenchmark2>();
        }
    }
}

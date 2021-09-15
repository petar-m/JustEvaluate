using System;
using JustEvaluate.UtilityFunctions;
using Microsoft.Extensions.DependencyInjection;

namespace JustEvaluate.Examples
{
    static partial class Program
    {
        static void Main()
        {
            // Create an instance - use a global one to take advantage of cached compiled expressions
            var evaluator = new Evaluator(new Parser(), new Builder(new FunctionsRegistry().AddMath().AddLogical()), new CompiledExpressionsCache());

            // Or use IoC container
            var serviceProvider = new ServiceCollection()
                                               .AddSingleton<Parser>()
                                               .AddSingleton<Builder>()
                                               .AddSingleton<CompiledExpressionsCache>()
                                               .AddSingleton(new FunctionsRegistry().AddMath().AddLogical())
                                               .AddSingleton<Evaluator>()
                                               // Sample classes
                                               .AddTransient<LeapYearChecker>()
                                               .AddTransient<FahrenheitToCelsiusConverter>()
                                               .AddTransient<Commission>()
                                               .BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var converter = scope.ServiceProvider.GetRequiredService<FahrenheitToCelsiusConverter>();
            for(int i = -50; i < 120; i += 10)
            {
                Console.WriteLine($"{i}F={converter.Convert(i):#.##}C");
            }

            var checker = scope.ServiceProvider.GetRequiredService<LeapYearChecker>();
            for(int i = 1890; i < 2020; i++)
            {
                if(checker.IsLeapYear1(i))
                {
                    Console.Write($"{i}, ");
                }
            }
            Console.WriteLine();
            for(int i = 1890; i < 2020; i++)
            {
                if(checker.IsLeapYear2(i))
                {
                    Console.Write($"{i}, ");
                }
            }
            Console.WriteLine();
            for(int i = 1890; i < 2020; i++)
            {
                if(checker.IsLeapYear3(i))
                {
                    Console.Write($"{i}, ");
                }
            }
            Console.WriteLine();

            var commission = scope.ServiceProvider.GetRequiredService<Commission>();
            foreach(var amount in new[] { 0, 500, 50000, 200000 })
            {
                Console.WriteLine($"amount = {amount} comission={commission.Calculate(amount)}");
            }
        }
    }
}

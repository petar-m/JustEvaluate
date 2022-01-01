using System;
using JustEvaluate.UtilityFunctions;
using Microsoft.Extensions.DependencyInjection;
using static System.Console;

namespace JustEvaluate.Examples
{
    static partial class Program
    {
        static void Main()
        {
            // Create an instance - use a global one to take advantage of cached compiled expressions
            var evaluator = new Evaluator(new Parser(), new Builder(new FunctionsRegistry().AddMath()), new CompiledExpressionsCache());

            // Or use IoC container
            var serviceProvider = new ServiceCollection()
                                               .AddSingleton<Parser>()
                                               .AddSingleton<Builder>()
                                               .AddSingleton<CompiledExpressionsCache>()
                                               .AddSingleton(new FunctionsRegistry().AddMath()
                                                                                    .Add("DividesByWithoutReminder", (x, y) => (decimal)Math.IEEERemainder((double)x, (double)y) == 0m ? 1 : 0))
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
                WriteLine($"{i}F={converter.Convert(i):#.##}C");
            }

            var checker = scope.ServiceProvider.GetRequiredService<LeapYearChecker>();
            WriteLine("System.DateTime.IsLeapYear()");
            for(int i = 1890; i < 2020; i++)
            {
                if(checker.IsLeapYear(i))
                {
                    Write($"{i}, ");
                }
            }
            WriteLine("\ncustom1 ---------------------------------------");
            for(int i = 1890; i < 2020; i++)
            {
                if(checker.IsLeapYear1(i))
                {
                    Write($"{i}, ");
                }
            }
            WriteLine("\ncustom2 ---------------------------------------");
            for(int i = 1890; i < 2020; i++)
            {
                if(checker.IsLeapYear2(i))
                {
                    Write($"{i}, ");
                }
            }
            WriteLine();

            var commission = scope.ServiceProvider.GetRequiredService<Commission>();
            foreach(var amount in new[] { 0, 500, 50000, 200000 })
            {
                WriteLine($"amount = {amount} comission={commission.CalculateUsingAnonimousTypeArgument(amount)}");
                WriteLine($"amount = {amount} comission={commission.CalculateUsingDictionaryArgument(amount)}");
            }
        }
    }
}

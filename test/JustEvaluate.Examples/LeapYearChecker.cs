using System;

namespace JustEvaluate.Examples
{
    public class LeapYearChecker
    {
        public class Input
        {
            public decimal Year { get; set; }
        }

        private readonly Evaluator _evaluator;

        public LeapYearChecker(Evaluator evaluator) => _evaluator = evaluator;

        public bool IsLeapYear(int year) => DateTime.IsLeapYear(year);

        public bool IsLeapYear1(int year)
        {
            var input = new Input { Year = year };

            // We can use some unility functions like Floor
            const string isLeapYear = "Year / 4 = Floor(Year / 4) & (Year / 100 <> Floor(Year / 100) | Year / 100 = Floor(Year / 100) & Year / 400 = Floor(Year / 400))";
            return _evaluator.Evaluate(isLeapYear, input) == 1m;
        }

        public bool IsLeapYear2(int year)
        {
            var input = new Input { Year = year };

            // Or user-defined function (check the IoC registration in Main)
            const string isLeapYear = "DividesByWithoutReminder(Year, 4) & If(Not(DividesByWithoutReminder(Year, 100)), 1, DividesByWithoutReminder(Year ,400))";
            return _evaluator.Evaluate(isLeapYear, input) == 1m;
        }
    }
}

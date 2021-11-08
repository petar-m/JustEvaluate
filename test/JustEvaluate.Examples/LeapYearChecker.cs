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

        public bool IsLeapYear1(int year)
        {
            var input = new Input { Year = year };

            // We can use some unility functions - EqualTo, NotEqualTo, Floor, Or. All of them return 0 or 1.
            const string isLeapYear = "EqualTo(Year / 4, Floor(Year / 4)) * (NotEqualTo(Year / 100, Floor(Year / 100)) | EqualTo(Year / 100, Floor(Year / 100)) * EqualTo(Year / 400, Floor(Year / 400)))";
            return _evaluator.Evaluate(isLeapYear, input) == 1m;
        }

        public bool IsLeapYear2(int year)
        {
            var input = new Input { Year = year };

            // Or we can use utility function If() 
            const string isLeapYear = "EqualTo(Year / 4, Floor(Year / 4)) * If(NotEqualTo(Year / 100, Floor(Year / 100)), 1, EqualTo(Year / 400, Floor(Year / 400)))";
            return _evaluator.Evaluate(isLeapYear, input) == 1m;
        }

        public bool IsLeapYear3(int year)
        {
            var input = new Input { Year = year };

            // Or we can define our own function
            _evaluator.FunctionsRegistry.Add("DividesByWithoutReminder", (x, y) => (decimal)Math.IEEERemainder((double)x, (double)y) == 0m ? 1 : 0, true);
            const string isLeapYear = "DividesByWithoutReminder(Year, 4) * If(Not(DividesByWithoutReminder(Year, 100)), 1, DividesByWithoutReminder(Year ,400))";
            return _evaluator.Evaluate(isLeapYear, input) == 1m;
        }
    }
}

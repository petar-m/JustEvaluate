namespace JustEvaluate.Examples
{
    public class FahrenheitToCelsiusConverter
    {
        private const string Formula = "(DegreesInFahrenheit - 32) / 1.8";
        private readonly Evaluator _evaluator;

        public FahrenheitToCelsiusConverter(Evaluator evaluator) => _evaluator = evaluator;

        public decimal Convert(decimal degreesInFahrenheit) => _evaluator.Evaluate(Formula, new { DegreesInFahrenheit = degreesInFahrenheit });
    }
}

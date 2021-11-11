namespace JustEvaluate.Examples
{
    public class Commission
    {
        private readonly Evaluator _evaluator;

        public Commission(Evaluator evaluator) => _evaluator = evaluator;

        public decimal Calculate(decimal amount)
        {

            const string commission =
                @"
(Amount > 0)
* If(Amount >= 1 & Amount <= 1000, 5, 1)
* If(Amount > 1000 & Amount < 100000, 100 + Amount * 2 / 100 , 1)
* If(Amount >= 100000, amount * 2.5 / 100, 1)
";

            return _evaluator.Evaluate(commission, new { amount });
        }
    }
}

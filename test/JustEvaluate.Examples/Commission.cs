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
  If(LessThanOrEqual(Amount, 0), 0, 1)
* If(BetweenInclusive(Amount, 1, 1000), 5, 1)
* If(Between(Amount, 1000, 100000), 100 + Amount * 2 / 100 , 1)
* If(GreaterThanOrEqual(Amount, 100000), amount * 2.5 / 100, 1)
";
            return _evaluator.Evaluate(commission, new { amount });
        }
    }
}

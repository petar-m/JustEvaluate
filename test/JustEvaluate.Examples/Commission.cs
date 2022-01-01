using System;
using System.Collections.Generic;

namespace JustEvaluate.Examples
{
    public class Commission
    {
        private readonly Evaluator _evaluator;
        const string commission = @"
(Amount > 0)
* If(Amount >= 1 & Amount <= 1000, 5, 1)
* If(Amount > 1000 & Amount < 100000, 100 + Amount * 2 / 100 , 1)
* If(Amount >= 100000, amount * 2.5 / 100, 1)
";

        public Commission(Evaluator evaluator) => _evaluator = evaluator;

        public decimal CalculateUsingAnonimousTypeArgument(decimal amount) => _evaluator.Evaluate(commission, new { amount });

        public decimal CalculateUsingDictionaryArgument(decimal amount) => _evaluator.Evaluate(commission, new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase) { { "amount", amount } });
    }
}

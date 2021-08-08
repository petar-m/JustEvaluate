using System;
using System.Collections.Generic;

namespace JustEvaluate
{
    public class ExpressionCache
    {
        private Dictionary<string, Func<decimal>> _parameterlessExpressions = new Dictionary<string, Func<decimal>>();

        private Dictionary<(string, Type), object> _expressions = new Dictionary<(string, Type), object>();

        public void Add(string expression, Func<decimal> compiledExpression) => _parameterlessExpressions[expression] = compiledExpression;

        public void Add<TArg>(string expression, Func<TArg, decimal> compiledExpression) => _expressions[(expression, typeof(TArg))] = compiledExpression;

        public Func<decimal> Get(string expression)
        {
            if(_parameterlessExpressions.TryGetValue(expression, out var function))
            {
                return function;
            }
            return null;
        }

        public Func<TArg, decimal> Get<TArg>(string expression)
        {
            if(_expressions.TryGetValue((expression, typeof(TArg)), out var function))
            {
                return (Func<TArg, decimal>)function;
            }
            return null;
        }
    }
}

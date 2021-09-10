using System;
using System.Collections.Generic;

namespace JustEvaluate
{
    // TODO: unbounded cache!
    public class ExpressionCache
    {
        private readonly Dictionary<string, Func<decimal>> _parameterlessExpressions = new Dictionary<string, Func<decimal>>();

        private readonly Dictionary<(string, Type), object> _expressions = new Dictionary<(string, Type), object>();

        public virtual void Add(string expression, Func<decimal> compiledExpression) => _parameterlessExpressions[expression] = compiledExpression;

        public virtual void Add<TArg>(string expression, Func<TArg, decimal> compiledExpression) => _expressions[(expression, typeof(TArg))] = compiledExpression;

        public virtual Func<decimal> Get(string expression)
        {
            if(_parameterlessExpressions.TryGetValue(expression, out var function))
            {
                return function;
            }
            return null;
        }

        public virtual Func<TArg, decimal> Get<TArg>(string expression)
        {
            if(_expressions.TryGetValue((expression, typeof(TArg)), out var function))
            {
                return (Func<TArg, decimal>)function;
            }
            return null;
        }
    }
}

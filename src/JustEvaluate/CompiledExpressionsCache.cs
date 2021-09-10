using System;
using System.Collections.Generic;

namespace JustEvaluate
{
    // TODO: unbounded cache!
    public class CompiledExpressionsCache
    {
        private readonly Dictionary<string, Func<decimal>> _parameterlessExpressions = new Dictionary<string, Func<decimal>>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, Dictionary<Type, object>> _expressions = new Dictionary<string, Dictionary<Type, object>>(StringComparer.OrdinalIgnoreCase);

        public virtual void Add(string expression, Func<decimal> compiledExpression) => _parameterlessExpressions[expression] = compiledExpression;

        public virtual void Add<TArg>(string expression, Func<TArg, decimal> compiledExpression)
        {
            if(_expressions.TryGetValue(expression, out Dictionary<Type, object> items))
            {
                items[typeof(TArg)] = compiledExpression;
            }
            else
            {
                _expressions[expression] = new Dictionary<Type, object> { { typeof(TArg), compiledExpression } };
            }
        }

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
            if(_expressions.TryGetValue(expression, out Dictionary<Type, object> items) && items.TryGetValue(typeof(TArg), out var function))
            {
                return (Func<TArg, decimal>)function;
            }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace JustEvaluate
{
    public class Functions
    {
        private readonly bool _allowOverride;

        public Functions(bool allowOverride)
        {
            _allowOverride = allowOverride;
        }

        private readonly Dictionary<int, Dictionary<string, LambdaExpression>> _functions = new Dictionary<int, Dictionary<string, LambdaExpression>>();

        public void Add(string name, Expression<Func<decimal>> function) => AddInternal(name, function);

        public void Add(string name, Expression<Func<decimal, decimal>> function) => AddInternal(name, function);

        public void Add(string name, Expression<Func<decimal, decimal, decimal>> function) => AddInternal(name, function);

        public void Add(string name, Expression<Func<decimal, decimal, decimal, decimal>> function) => AddInternal(name, function);

        public void Add(string name, Expression<Func<decimal, decimal, decimal, decimal, decimal>> function) => AddInternal(name, function);

        public void Add(string name, Expression<Func<decimal, decimal, decimal, decimal, decimal, decimal>> function) => AddInternal(name, function);

        public void Add(string name, Expression<Func<decimal, decimal, decimal, decimal, decimal, decimal, decimal>> function) => AddInternal(name, function);

        public void Add(string name, Expression<Func<decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal>> function) => AddInternal(name, function);

        public void Add(string name, Expression<Func<decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal>> function) => AddInternal(name, function);

        public LambdaExpression Get(string name, int parametersCount)
        {
            if(!_functions.TryGetValue(parametersCount, out var functions) || !functions.TryGetValue(name, out var function))
            {
                throw new InvalidOperationException($"There is no function '{name}' with {parametersCount} parameters defined");
            }

            return function;
        }

        private void AddInternal(string name, LambdaExpression expression)
        {
            int argumentsCount = expression.Parameters.Count;

            if(!_functions.TryGetValue(argumentsCount, out var functions))
            {
                _functions.Add(argumentsCount, new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase) { { name, expression } });
            }
            else if(_allowOverride)
            {
                functions[name] = expression;
            }
            else
            {
                functions.Add(name, expression);
            }
        }
    }
}

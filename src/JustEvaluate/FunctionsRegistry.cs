using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace JustEvaluate
{
    public class FunctionsRegistry
    {
        private readonly Dictionary<int, Dictionary<string, LambdaExpression>> _functions = new Dictionary<int, Dictionary<string, LambdaExpression>>();

        public FunctionsRegistry Add(string name, Expression<Func<decimal>> function, bool allowReplace = false) => AddInternal(name, function, allowReplace);

        public FunctionsRegistry Add(string name, Expression<Func<decimal, decimal>> function, bool allowReplace = false) => AddInternal(name, function, allowReplace);

        public FunctionsRegistry Add(string name, Expression<Func<decimal, decimal, decimal>> function, bool allowReplace = false) => AddInternal(name, function, allowReplace);

        public FunctionsRegistry Add(string name, Expression<Func<decimal, decimal, decimal, decimal>> function, bool allowReplace = false) => AddInternal(name, function, allowReplace);

        public FunctionsRegistry Add(string name, Expression<Func<decimal, decimal, decimal, decimal, decimal>> function, bool allowReplace = false) => AddInternal(name, function, allowReplace);

        public FunctionsRegistry Add(string name, Expression<Func<decimal, decimal, decimal, decimal, decimal, decimal>> function, bool allowReplace = false) => AddInternal(name, function, allowReplace);

        public FunctionsRegistry Add(string name, Expression<Func<decimal, decimal, decimal, decimal, decimal, decimal, decimal>> function, bool allowReplace = false) => AddInternal(name, function, allowReplace);

        public FunctionsRegistry Add(string name, Expression<Func<decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal>> function, bool allowReplace = false) => AddInternal(name, function, allowReplace);

        public FunctionsRegistry Add(string name, Expression<Func<decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal>> function, bool allowReplace = false) => AddInternal(name, function, allowReplace);

        public LambdaExpression Get(string name, int parametersCount)
        {
            if(!_functions.TryGetValue(parametersCount, out var functions) || !functions.TryGetValue(name, out var function))
            {
                throw new InvalidOperationException($"There is no function '{name}' with {parametersCount} parameters defined");
            }

            return function;
        }

        private FunctionsRegistry AddInternal(string name, LambdaExpression expression, bool allowReplace)
        {
            int argumentsCount = expression.Parameters.Count;

            if(!_functions.TryGetValue(argumentsCount, out var functions))
            {
                _functions.Add(argumentsCount, new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase) { { name, expression } });
            }
            else if(allowReplace)
            {
                functions[name] = expression;
            }
            else
            {
                functions.Add(name, expression);
            }
            return this;
        }
    }
}

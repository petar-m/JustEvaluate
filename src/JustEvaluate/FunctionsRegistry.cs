using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace JustEvaluate
{
    public class FunctionsRegistry
    {
        private static readonly Dictionary<string, int> _builtInFunctions = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "if", 3}, { "not", 1}
        };

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

        public static bool IsBuiltInFunction(string name) => _builtInFunctions.ContainsKey(name);

        public static int BuiltInFunctionArgumentCount(string name)
        {
            if(!IsBuiltInFunction(name))
            {
                throw new ArgumentException($"'{name}' is not a built-in function", nameof(name));
            }

            return _builtInFunctions[name];
        }

        private FunctionsRegistry AddInternal(string name, LambdaExpression expression, bool allowReplace)
        {
            if(IsBuiltInFunction(name))
            {
                throw new InvalidOperationException($"'{name}' is reserved for built-in function and user defined function with the same name is not allowed");
            }

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

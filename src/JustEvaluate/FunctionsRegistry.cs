using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly Dictionary<string, string> _aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public FunctionsRegistry Add(string name, Expression<Func<decimal>> function, bool allowReplace = false) => AddInternal(name, function, allowReplace);

        public FunctionsRegistry Add(string name, Expression<Func<decimal, decimal>> function, bool allowReplace = false) => AddInternal(name, function, allowReplace);

        public FunctionsRegistry Add(string name, Expression<Func<decimal, decimal, decimal>> function, bool allowReplace = false) => AddInternal(name, function, allowReplace);

        public FunctionsRegistry Add(string name, Expression<Func<decimal, decimal, decimal, decimal>> function, bool allowReplace = false) => AddInternal(name, function, allowReplace);

        public FunctionsRegistry Add(string name, Expression<Func<decimal, decimal, decimal, decimal, decimal>> function, bool allowReplace = false) => AddInternal(name, function, allowReplace);

        public FunctionsRegistry Add(string name, Expression<Func<decimal, decimal, decimal, decimal, decimal, decimal>> function, bool allowReplace = false) => AddInternal(name, function, allowReplace);

        public FunctionsRegistry Add(string name, Expression<Func<decimal, decimal, decimal, decimal, decimal, decimal, decimal>> function, bool allowReplace = false) => AddInternal(name, function, allowReplace);

        public FunctionsRegistry Add(string name, Expression<Func<decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal>> function, bool allowReplace = false) => AddInternal(name, function, allowReplace);

        public FunctionsRegistry Add(string name, Expression<Func<decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal, decimal>> function, bool allowReplace = false) => AddInternal(name, function, allowReplace);

        public string GetOriginalName(string name) => _aliases.ContainsKey(name) ? _aliases[name] : name;

        public LambdaExpression Get(string name, int parametersCount)
        {
            var function = TryGet(name, parametersCount);

            if(function is null)
            {
                throw new InvalidOperationException($"There is no function '{name}' with {parametersCount} parameters defined");
            }

            return function;
        }

        public LambdaExpression TryGet(string name, int parametersCount)
        {
            name = GetOriginalName(name);

            if(!_functions.TryGetValue(parametersCount, out var functions) || !functions.TryGetValue(name, out var function))
            {
                return null;
            }

            return function;
        }

        public bool IsBuiltInFunction(string name) => _builtInFunctions.ContainsKey(GetOriginalName(name));

        public int BuiltInFunctionArgumentCount(string name)
        {
            name = GetOriginalName(name);
            if(!IsBuiltInFunction(name))
            {
                throw new ArgumentException($"'{name}' is not a built-in function", nameof(name));
            }

            return _builtInFunctions[name];
        }

        public FunctionsRegistry AddFunctionAlias(string function, string alias)
        {
            if(!_builtInFunctions.ContainsKey(function) && !_functions.Values.Any(x => x.ContainsKey(function)))
            {
                throw new InvalidOperationException($"There is no function '{function}' defined");
            }

            if(_builtInFunctions.ContainsKey(alias))
            {
                throw new InvalidOperationException($"'{alias}' is reserved for built-in function and can not be used as alias");
            }

            if(_functions.Values.Any(x => x.ContainsKey(alias)))
            {
                throw new InvalidOperationException($"There is function '{alias}' defined and can not be used as alias");
            }

            if(_aliases.ContainsKey(alias))
            {
                throw new InvalidOperationException($"There is already alias '{alias}' registered for function '{function}'");
            }

            _aliases.Add(alias, function);

            return this;
        }

        private FunctionsRegistry AddInternal(string name, LambdaExpression expression, bool allowReplace)
        {
            if(IsBuiltInFunction(name))
            {
                throw new InvalidOperationException($"'{name}' is reserved for built-in function and user defined function with the same name is not allowed");
            }

            if(_aliases.ContainsKey(name))
            {
                throw new InvalidOperationException($"'{name}' is already added as and alias and cannot be used as function name");
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

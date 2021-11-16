using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace JustEvaluate
{
    public class Builder
    {
        private static readonly Expression Zero = Expression.Constant(0m, typeof(decimal));
        private static readonly Expression One = Expression.Constant(1m, typeof(decimal));

        public Builder(FunctionsRegistry functions)
        {
            FunctionsRegistry = functions;
        }

        public FunctionsRegistry FunctionsRegistry { get; }

        public virtual Func<decimal> Build(IEnumerable<Token> tokens)
        {
            ValidateBuiltInFunctions(tokens);
            Token[] postfixTokens = ConvertToPostfix(tokens);
            MapPropertyNames<object>(postfixTokens);

            Expression expression = CalculatePostfix<object>(postfixTokens, null);

            return Expression.Lambda<Func<decimal>>(expression).Compile(preferInterpretation: false);
        }

        public virtual Func<TArg, decimal> Build<TArg>(IEnumerable<Token> tokens)
        {
            ValidateBuiltInFunctions(tokens);
            var postfixTokens = ConvertToPostfix(tokens);
            MapPropertyNames<TArg>(postfixTokens);

            var parameter = Expression.Parameter(typeof(TArg));
            Expression expression = CalculatePostfix<TArg>(postfixTokens, parameter);
            return Expression.Lambda<Func<TArg, decimal>>(expression, parameter).Compile(preferInterpretation: false);
        }

        private void ValidateBuiltInFunctions(IEnumerable<Token> tokens)
        {
            foreach(Token token in tokens)
            {
                if(token.IsFunction && FunctionsRegistry.IsBuiltInFunction(token.Value) && FunctionsRegistry.BuiltInFunctionArgumentCount(token.Value) != token.FunctionArguments.Count)
                {
                    throw new InvalidOperationException($"Built-in function '{token.Value}' takes {FunctionsRegistry.BuiltInFunctionArgumentCount(token.Value)} arguments but invoked with {token.FunctionArguments.Count}");
                }
            }
        }

        private static void MapPropertyNames<TArg>(Token[] tokens)
        {
            var properties = typeof(TArg).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                         .SelectMany(x => x.GetCustomAttributes(typeof(AliasAttribute), inherit: true)
                                                           .Cast<AliasAttribute>()
                                                           .Select(a => new { a.Name, PropertyInfo = x }).Concat(new[] { new { x.Name, PropertyInfo = x } }))
                                         .ToList();
            var duplicates = properties.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
                                        .Where(x => x.Count() > 1)
                                        .Select(x => x.Key)
                                        .ToArray();
            if(duplicates.Length > 0)
            {
                throw new InvalidOperationException($"Property aliases should be unique and different than property names, diplicates: {string.Join(", ", duplicates)}");
            }

            Dictionary<string, PropertyInfo> propertyInfos = properties.ToDictionary(x => x.Name, x => x.PropertyInfo, StringComparer.OrdinalIgnoreCase);
            for(int i = 0; i < tokens.Length; i++)
            {
                if(tokens[i].IsName)
                {
                    if(propertyInfos.TryGetValue(tokens[i].Value, out var propertyInfo))
                    {
                        if(propertyInfo.PropertyType != typeof(decimal))
                        {
                            var alias = tokens[i].Value.Equals(propertyInfo.Name, StringComparison.OrdinalIgnoreCase) ? string.Empty : $" (alias '{tokens[i].Value}')";
                            throw new InvalidOperationException($"Property named '{ propertyInfo.Name }'{ alias } is of type {propertyInfo.PropertyType.Name}, expected {typeof(decimal).Name}");
                        }
                        tokens[i].ChangeValueTo(propertyInfo.Name);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Argument type {typeof(TArg).Name} does not have property named '{tokens[i].Value}'");
                    }
                }
            }
        }

        private static Token[] ConvertToPostfix(IEnumerable<Token> tokens)
        {
            var output = new Queue<Token>();
            var operators = new Stack<Token>();
            foreach(Token token in tokens)
            {
                if(token.IsOperator)
                {
                    while(operators.Count > 0)
                    {
                        Token op = operators.Peek();
                        if(op.IsOperator && token.LessOrEqualPrecendanceOver(op))
                        {
                            _ = operators.Pop();
                            output.Enqueue(op);
                        }
                        else
                        {
                            break;
                        }
                    }

                    operators.Push(token);
                }
                else if(token.IsOpeningBracket)
                {
                    operators.Push(token);
                }
                else if(token.IsClosingBracket)
                {
                    if(operators.Count == 0)
                    {
                        throw new InvalidOperationException("Mismatched brackets");
                    }

                    Token op = operators.Pop();
                    while(!op.IsOpeningBracket && operators.Count > 0)
                    {
                        output.Enqueue(op);
                        op = operators.Pop();
                    }

                    if(!op.IsOpeningBracket)
                    {
                        throw new InvalidOperationException("Mismatched brackets");
                    }
                }
                else if(token.IsConstant || token.IsFunction || token.IsName)
                {
                    output.Enqueue(token);
                }
            }

            while(operators.Count > 0)
            {
                Token o = operators.Pop();
                if(o.IsOpeningBracket || o.IsClosingBracket)
                {
                    throw new InvalidOperationException("Mismatched brackets");
                }

                output.Enqueue(o);
            }

            return output.ToArray();
        }

        private Expression CalculatePostfix<TParam>(IEnumerable<Token> postfix, ParameterExpression param)
        {
            var calcStack = new Stack<Expression>();
            foreach(Token token in postfix)
            {
                if(token.IsConstant || token.IsName || token.IsFunction)
                {
                    if(token.IsConstant)
                    {
                        calcStack.Push(Expression.Constant(token.NumericValue.Value, typeof(decimal)));
                    }
                    else if(token.IsName)
                    {
                        calcStack.Push(Expression.Property(param, token.Value));
                    }
                    else if(token.IsFunction)
                    {
                        var arguments = new Expression[token.FunctionArguments.Count];

                        for(int i = 0; i < token.FunctionArguments.Count; i++)
                        {
                            arguments[i] = CalculatePostfix<TParam>(ConvertToPostfix(token.FunctionArguments[i]), param);
                        }

                        if(FunctionsRegistry.IsBuiltInFunction(token.Value))
                        {
                            calcStack.Push(CreateExpressionFromBuiltInFunction(FunctionsRegistry.GetOriginalName(token.Value), arguments));
                        }
                        else
                        {
                            var functionCall = FunctionsRegistry.Get(token.Value, token.FunctionArguments.Count);
                            calcStack.Push(Expression.Invoke(functionCall, arguments));
                        }
                    }
                }
                else if(token.IsOperator)
                {
                    if(calcStack.Count < 2)
                    {
                        throw new InvalidOperationException("Insufficient data for calculation - missing operand");
                    }

                    Expression op1 = calcStack.Pop();
                    Expression op2 = calcStack.Pop();
                    calcStack.Push(Calculate(token, op2, op1));
                }
            }

            if(calcStack.Count != 1)
            {
                throw new InvalidOperationException("Too many values supplied - missing operator");
            }

            return calcStack.Pop();
        }

        private static Expression Calculate(Token token, Expression op1, Expression op2)
        {
            switch(token.Type)
            {
                case TokenType.Add:
                    return Expression.Add(op1, op2);
                case TokenType.Multipy:
                    return Expression.Multiply(op1, op2);
                case TokenType.Divide:
                    return Expression.Divide(op1, op2);
                case TokenType.Subtract:
                    return Expression.Subtract(op1, op2);
                case TokenType.And:
                case TokenType.Or:
                    return CalculateBoolean(op1, op2, token.Type);
                case TokenType.EqualTo:
                case TokenType.NotEqualTo:
                case TokenType.GreaterThan:
                case TokenType.GreaterOrEqualTo:
                case TokenType.LessThan:
                case TokenType.LessOrEqualTo:
                    return CalculateRelational(op1, op2, token.Type);
                default:
                    throw new InvalidOperationException($"Unknown operator '{token.Type}'");
            }
        }

        private static Expression CalculateBoolean(Expression op1, Expression op2, TokenType tokenType)
        {
            var left = Expression.NotEqual(op1, Zero);
            var right = Expression.NotEqual(op2, Zero);
            BinaryExpression test;
            switch(tokenType)
            {
                case TokenType.And:
                    test = Expression.AndAlso(left, right);
                    break;
                case TokenType.Or:
                    test = Expression.OrElse(left, right);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown boolean operator '{tokenType}'");
            }

            return Expression.Condition(test, One, Zero, typeof(decimal));
        }

        private static Expression CalculateRelational(Expression op1, Expression op2, TokenType tokenType)
        {
            BinaryExpression test;
            switch(tokenType)
            {
                case TokenType.EqualTo:
                    test = Expression.Equal(op1, op2);
                    break;
                case TokenType.NotEqualTo:
                    test = Expression.NotEqual(op1, op2);
                    break;
                case TokenType.GreaterThan:
                    test = Expression.GreaterThan(op1, op2);
                    break;
                case TokenType.GreaterOrEqualTo:
                    test = Expression.GreaterThanOrEqual(op1, op2);
                    break;
                case TokenType.LessThan:
                    test = Expression.LessThan(op1, op2);
                    break;
                case TokenType.LessOrEqualTo:
                    test = Expression.LessThanOrEqual(op1, op2);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown boolean operator '{tokenType}'");
            }

            return Expression.Condition(test, One, Zero, typeof(decimal));
        }

        private Expression CreateExpressionFromBuiltInFunction(string name, Expression[] arguments)
        {
            if(string.Equals("if", name, StringComparison.OrdinalIgnoreCase))
            {
                var test = Expression.NotEqual(arguments[0], Zero);
                return Expression.Condition(test, arguments[1], arguments[2], typeof(decimal));
            }

            if(string.Equals("not", name, StringComparison.OrdinalIgnoreCase))
            {
                var test = Expression.NotEqual(arguments[0], Zero);
                return Expression.Condition(test, Zero, One, typeof(decimal));
            }

            throw new NotImplementedException($"Built-in function '{name}' is not supported.");
        }
    }
}

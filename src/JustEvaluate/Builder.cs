using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace JustEvaluate
{
    public class Builder
    {
        private readonly Functions _functions;

        public Builder(Functions functions)
        {
            _functions = functions;
        }

        public Func<decimal> Build(IEnumerable<Token> tokens)
        {
            Token[] postfixTokens = ConvertToPostfix(tokens);

            Expression expression = CalculatePostfix<object>(postfixTokens, null);

            return Expression.Lambda<Func<decimal>>(expression).Compile();
        }

        public Func<TArg, decimal> Build<TArg>(IEnumerable<Token> tokens)
        {
            var postfixTokens = ConvertToPostfix(tokens);
            MapPropertyNames<TArg>(postfixTokens);

            var parameter = Expression.Parameter(typeof(TArg));
            Expression expression = CalculatePostfix<TArg>(postfixTokens, parameter);
            return Expression.Lambda<Func<TArg, decimal>>(expression, parameter).Compile();
        }

        private void MapPropertyNames<TArg>(Token[] tokens)
        {
            // TODO: Functions supprot all kinds of names, maybe an attribute can achieve the same for properties
            // currently only casing difference is supported
            var properties = typeof(TArg).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                                         .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
            for(int i = 0; i < tokens.Length; i++)
            {
                if(tokens[i].IsName)
                {
                    if(properties.TryGetValue(tokens[i].Value, out var propertyInfo))
                    {
                        if(propertyInfo.PropertyType != typeof(decimal))
                        {
                            throw new InvalidOperationException($"Property named '{tokens[i].Value}' is of type {propertyInfo.PropertyType.Name}, expected {typeof(decimal).Name}");
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

        private Token[] ConvertToPostfix(IEnumerable<Token> tokens)
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
                        var functionCall = _functions.Get(token.Value, token.FunctionArguments.Count);
                        var arguments = new Expression[token.FunctionArguments.Count];

                        for(int i = 0; i < token.FunctionArguments.Count; i++)
                        {
                            arguments[i] = CalculatePostfix<TParam>(ConvertToPostfix(token.FunctionArguments[i]), param);
                        }

                        calcStack.Push(Expression.Invoke(functionCall, arguments));
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

        private Expression Calculate(Token token, Expression op1, Expression op2)
        {
            if(token.IsAdd)
            {
                return Expression.Add(op1, op2);
            }

            if(token.IsMultiply)
            {
                return Expression.Multiply(op1, op2);
            }

            if(token.IsDivide)
            {
                return Expression.Divide(op1, op2);
            }

            if(token.IsSubtract)
            {
                return Expression.Subtract(op1, op2);
            }

            throw new InvalidOperationException(token.Value);
        }
    }
}

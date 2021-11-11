using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace JustEvaluate
{
    [DebuggerDisplay("{Type} | {Text} | {Value} | {NumericValue}")]
    public class Token
    {
        private static readonly NumberFormatInfo _decimalFormat = new NumberFormatInfo { NumberDecimalSeparator = "." };
        private static readonly HashSet<TokenType> _operators = new HashSet<TokenType>
        {
            TokenType.Add,
            TokenType.Multipy,
            TokenType.Divide,
            TokenType.Subtract,
            TokenType.And,
            TokenType.Or,
            TokenType.EqualTo,
            TokenType.NotEqualTo,
            TokenType.LessOrEqualTo,
            TokenType.LessThan,
            TokenType.GreaterOrEqualTo,
            TokenType.GreaterThan
        };
        private static readonly Dictionary<TokenType, int> _operatorPrecedence = new Dictionary<TokenType, int>
        {
            { TokenType.Add, 5 },
            { TokenType.Multipy, 6 },
            { TokenType.Divide, 6 },
            { TokenType.Subtract, 5 },
            { TokenType.And, 2 },
            { TokenType.Or,1 },
            { TokenType.EqualTo, 3 },
            { TokenType.NotEqualTo, 3 },
            { TokenType.LessOrEqualTo, 4 },
            { TokenType.LessThan, 4 },
            { TokenType.GreaterOrEqualTo, 4 },
            { TokenType.GreaterThan,4 }
        };

        public Token(string text)
        {
            Text = text;
            var trimmedText = text.Trim();
            Value = trimmedText;

            if(text.Length == 0 || trimmedText.Length == 0)
            {
                Type = TokenType.Empty;
            }
            else if(trimmedText[0].IsTerminalChar())
            {
                if(trimmedText.Length == 1)
                {
                    Type = trimmedText[0].TerminalCharToTokenType();
                }
                else
                {
                    Type = trimmedText.ToCharArray().TerminalSequenceToTokenType();
                }
            }
            else if(trimmedText[0].IsNumericPart())
            {
                Type = TokenType.Constant;
                NumericValue = decimal.Parse(trimmedText, NumberStyles.Any, _decimalFormat);
            }
            else
            {
                Type = TokenType.Name;
            }
        }

        public Token(decimal value)
        {
            Type = TokenType.Constant;
            Text = value.ToString(CultureInfo.InvariantCulture);
            Value = Text;
            NumericValue = value;
        }

        public Token(char c)
        {
            Type = c.TerminalCharToTokenType();
            Text = c.ToString();
            Value = Text;
        }

        public string Text { get; }

        public string Value { get; private set; }

        public decimal? NumericValue { get; }

        public TokenType Type { get; private set; }

        public List<List<Token>> FunctionArguments { get; } = new List<List<Token>>();

        public bool IsOperator => _operators.Contains(Type);

        public bool IsAdd => Type == TokenType.Add;

        public bool IsSubtract => Type == TokenType.Subtract;

        public bool IsMultiply => Type == TokenType.Multipy;

        public bool IsDivide => Type == TokenType.Divide;

        public bool IsOpeningBracket => Type == TokenType.OpeningBracket;

        public bool IsClosingBracket => Type == TokenType.ClosingBracket;

        public bool IsConstant => Type == TokenType.Constant;

        public bool IsFunction => Type == TokenType.Function;

        public bool IsFunctionParameterSeparator => Type == TokenType.FunctionParameterSeparator;

        public bool IsName => Type == TokenType.Name;

        public bool IsEmpty => Type == TokenType.Empty;

        public bool LessOrEqualPrecendanceOver(Token token)
        {
            if(!IsOperator || !token.IsOperator)
            {
                throw new InvalidOperationException("Only operators support precendence");
            }

            return _operatorPrecedence[Type] <= _operatorPrecedence[token.Type];
        }

        public void ChangeToFunction()
        {
            if(!IsName)
            {
                throw new InvalidOperationException($"Cannot change '{Type}' to function");
            }

            Type = TokenType.Function;
        }

        public void ChangeValueTo(string value) => Value = value;
    }
}

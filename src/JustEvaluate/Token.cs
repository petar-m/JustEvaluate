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
                Type = trimmedText[0].TerminalCharToTokenType();
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

        public string Value { get; }

        public decimal? NumericValue { get; }

        public TokenType Type { get; private set; }

        public List<List<Token>> FunctionArguments { get; } = new List<List<Token>>();

        public bool IsOperator => Type == TokenType.Add || Type == TokenType.Multipy || Type == TokenType.Divide || Type == TokenType.Subtract;

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

        public bool LessOrEqualPrecendanceOver(Token token) => Type <= token.Type;

        public void ChangeToFunction()
        {
            if(!IsName)
            {
                throw new InvalidOperationException($"Cannot change '{Type}' to function");
            }

            Type = TokenType.Function;
        }
    }
}

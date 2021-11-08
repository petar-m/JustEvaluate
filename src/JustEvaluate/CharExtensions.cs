using System;

namespace JustEvaluate
{
    public static class CharExtensions
    {
        public static TokenType TerminalCharToTokenType(this char c)
        {
            if(c.IsMinus())
            {
                return TokenType.Subtract;
            }
            else if(c.IsPlus())
            {
                return TokenType.Add;
            }
            else if(c.IsDivide())
            {
                return TokenType.Divide;
            }
            else if(c.IsMiltiply())
            {
                return TokenType.Multipy;
            }
            else if(c.IsOpeningBracket())
            {
                return TokenType.OpeningBracket;
            }
            else if(c.IsClosingBracket())
            {
                return TokenType.ClosingBracket;
            }
            else if(c.IsFunctionParameterSeparator())
            {
                return TokenType.FunctionParameterSeparator;
            }
            else if(c.IsAnd())
            {
                return TokenType.And;
            }
            else if(c.IsOr())
            {
                return TokenType.Or;
            }

            throw new InvalidOperationException($"Character '{c}' is not terminal");
        }

        public static bool IsTerminalChar(this char c) => c.IsOperator() || c.IsBracket() || c.IsFunctionParameterSeparator() || c.IsAnd() || c.IsOr();

        public static bool IsOperator(this char c) => c.IsPlus() || c.IsMiltiply() || c.IsDivide() || c.IsMinus();

        public static bool IsBracket(this char c) => c.IsOpeningBracket() || c.IsClosingBracket();

        public static bool IsOpeningBracket(this char c) => c == '(';

        public static bool IsClosingBracket(this char c) => c == ')';

        public static bool IsMinus(this char c) => c == '-';

        public static bool IsPlus(this char c) => c == '+';

        public static bool IsDivide(this char c) => c == '/';

        public static bool IsMiltiply(this char c) => c == '*';

        public static bool IsNumericPart(this char c) => char.IsDigit(c) || c.IsDecimalSeparator();

        public static bool IsFunctionParameterSeparator(this char c) => c == ',';

        public static bool IsDecimalSeparator(this char c) => c == '.';

        public static bool IsAnd(this char c) => c == '&';

        public static bool IsOr(this char c) => c == '|';
    }
}

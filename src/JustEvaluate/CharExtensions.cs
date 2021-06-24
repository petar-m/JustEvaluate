using System;

namespace JustEvaluate
{
    public static class CharExtensions
    {
        public static TokenType ToTokenType(this char c)
        {
            switch (c)
            {
                case '-': return TokenType.Subtract;
                case '+': return TokenType.Add;
                case '/': return TokenType.Divide ;
                case '*': return TokenType.Multipy ;
                case '(': return TokenType.OpeningBracket ;
                case ')': return TokenType.ClosingBracket ;
                case ',': return TokenType.FunctionParameterSeparator;
                default: throw new InvalidOperationException($"No TokenType for char '{c}'");
            }
        }

        public static bool IsTerminalChar(this char c) => c.IsOperator() || c.IsBracket() || c.IsFunctionParameterSeparator();

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
    }
}

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
            else if(c.IsEqualsTo())
            {
                return TokenType.EqualTo;
            }
            else if(c.IsGreaterThan())
            {
                return TokenType.GreaterThan;
            }
            else if(c.IsLessThan())
            {
                return TokenType.LessThan;
            }

            throw new InvalidOperationException($"Character '{c}' is not terminal");
        }

        public static TokenType TerminalSequenceToTokenType(this char[] chars)
        {
            if(chars.Length != 2)
            {
                throw new ArgumentException("Terminal sequence expected length is 2");
            }

            if(chars[0] == '<' && chars[1] == '>')
            {
                return TokenType.NotEqualTo;
            }

            if(chars[0] <= '<' && chars[1] == '=')
            {
                return TokenType.LessOrEqualTo;
            }

            if(chars[0] <= '>' && chars[1] == '=')
            {
                return TokenType.GreaterOrEqualTo;
            }

            throw new ArgumentException($"Invalid terminal sequence expected '{string.Join(string.Empty, chars)}'");
        }

        public static bool IsTerminalChar(this char c) => c.IsOperator() || c.IsBracket() || c.IsFunctionParameterSeparator();

        public static bool IsOperator(this char c) => c.IsPlus() || c.IsMiltiply() || c.IsDivide() || c.IsMinus() || c.IsAnd() || c.IsOr() || c.IsEqualsTo() || c.IsLessThan() || c.IsGreaterThan();

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

        public static bool IsEqualsTo(this char c) => c == '=';

        public static bool IsGreaterThan(this char c) => c == '>';

        public static bool IsLessThan(this char c) => c == '<';

        public static bool IsPossibleTerminalSequence(this char c) => c.IsLessThan() || c.IsGreaterThan();

        public static bool IsNextInTerminalSequence(this char c) => c.IsGreaterThan() || c.IsEqualsTo();
    }
}

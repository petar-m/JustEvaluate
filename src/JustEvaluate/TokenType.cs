namespace JustEvaluate
{
    public enum TokenType
    {
        Or = 0,
        And = 1,
        NotEqualTo = 2,
        EqualTo = 3,
        LessThan = 4,
        LessOrEqualTo = 5,
        GreaterThan = 6,
        GreaterOrEqualTo = 7,
        Subtract = 8,
        Add = 9,
        Divide = 10,
        Multipy = 11,
        OpeningBracket = 12,
        ClosingBracket = 13,
        Constant = 14,
        Function = 15,
        FunctionParameterSeparator = 16,
        Name = 17,
        Empty = 18
    }
}

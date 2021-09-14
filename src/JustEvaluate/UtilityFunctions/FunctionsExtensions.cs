using static System.Math;

namespace JustEvaluate.UtilityFunctions
{
    public static class FunctionsExtensions
    {
        public static void AddLogical(this Functions functions, bool allowReplace = false)
        {
            functions.Add("GreaterThan", (x, y) => x > y ? 1 : 0, allowReplace);
            functions.Add("GreaterThanOrEqual", (x, y) => x >= y ? 1 : 0, allowReplace);
            functions.Add("LessThan", (x, y) => x < y ? 1 : 0, allowReplace);
            functions.Add("LessThanOrEqual", (x, y) => x <= y ? 1 : 0, allowReplace);
            functions.Add("EqualTo", (x, y) => x == y ? 1 : 0, allowReplace);
            functions.Add("NotEqualTo", (x, y) => x != y ? 1 : 0, allowReplace);
            functions.Add("Not", x => x == 0 ? 1 : 0, allowReplace);
            functions.Add("Between", (x, y, z) => x > y && x < z ? 1 : 0, allowReplace);
            functions.Add("BetweenLeftInclusive", (x, y, z) => x >= y && x < z ? 1 : 0, allowReplace);
            functions.Add("BetweenRightInclusive", (x, y, z) => x > y && x <= z ? 1 : 0, allowReplace);
            functions.Add("BetweenInclusive", (x, y, z) => x >= y && x <= z ? 1 : 0, allowReplace);
            functions.Add("Or", (x, y) => x != 0 || y != 0 ? 1 : 0, allowReplace);
            functions.Add("Or", (x, y, z) => x != 0 || y != 0 || z != 0 ? 1 : 0, allowReplace);
            functions.Add("And", (x, y) => x != 0 && y != 0 ? 1 : 0, allowReplace);
            functions.Add("And", (x, y, z) => x != 0 && y != 0 && z != 0 ? 1 : 0, allowReplace);
        }

        public static void AddMath(this Functions functions, bool allowReplace = false)
        {
            functions.Add("Min", (x, y) => Min(x, y), allowReplace);
            functions.Add("Min", (x, y, z) => Min(x, Min(y, z)), allowReplace);
            functions.Add("Max", (x, y) => Max(x, y), allowReplace);
            functions.Add("Max", (x, y, z) => Max(x, Max(y, z)), allowReplace);
            functions.Add("Round", x => Round(x), allowReplace);
            functions.Add("Round", (x, y) => Round(x, (int)y), allowReplace);
            functions.Add("Floor", x => Floor(x), allowReplace);
            functions.Add("Ceiling", x => Ceiling(x), allowReplace);
            functions.Add("Sqrt", x => (decimal)Sqrt((double)x), allowReplace);
            functions.Add("Pow", (x, y) => (decimal)Pow((double)x, (double)y), allowReplace);
            functions.Add("Abs", x => Abs(x), allowReplace);
        }
    }
}

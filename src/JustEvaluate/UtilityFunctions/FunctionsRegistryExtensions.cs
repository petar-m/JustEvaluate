using static System.Math;

namespace JustEvaluate.UtilityFunctions
{
    public static class FunctionsRegistryExtensions
    {
        public static FunctionsRegistry AddLogical(this FunctionsRegistry functions, bool allowReplace = false)
            => functions.Add("GreaterThan", (x, y) => x > y ? 1 : 0, allowReplace)
                        .Add("GreaterThanOrEqual", (x, y) => x >= y ? 1 : 0, allowReplace)
                        .Add("LessThan", (x, y) => x < y ? 1 : 0, allowReplace)
                        .Add("LessThanOrEqual", (x, y) => x <= y ? 1 : 0, allowReplace)
                        .Add("EqualTo", (x, y) => x == y ? 1 : 0, allowReplace)
                        .Add("NotEqualTo", (x, y) => x != y ? 1 : 0, allowReplace)
                        .Add("Not", x => x == 0 ? 1 : 0, allowReplace)
                        .Add("Between", (x, y, z) => x > y && x < z ? 1 : 0, allowReplace)
                        .Add("BetweenLeftInclusive", (x, y, z) => x >= y && x < z ? 1 : 0, allowReplace)
                        .Add("BetweenRightInclusive", (x, y, z) => x > y && x <= z ? 1 : 0, allowReplace)
                        .Add("BetweenInclusive", (x, y, z) => x >= y && x <= z ? 1 : 0, allowReplace)
                        .Add("Or", (x, y) => x != 0 || y != 0 ? 1 : 0, allowReplace)
                        .Add("Or", (x, y, z) => x != 0 || y != 0 || z != 0 ? 1 : 0, allowReplace)
                        .Add("And", (x, y) => x != 0 && y != 0 ? 1 : 0, allowReplace)
                        .Add("And", (x, y, z) => x != 0 && y != 0 && z != 0 ? 1 : 0, allowReplace)
                        .Add("If", (x, y, z) => x != 0 ? y : z);

        public static FunctionsRegistry AddMath(this FunctionsRegistry functions, bool allowReplace = false)
            => functions.Add("Min", (x, y) => Min(x, y), allowReplace)
                        .Add("Min", (x, y, z) => Min(x, Min(y, z)), allowReplace)
                        .Add("Max", (x, y) => Max(x, y), allowReplace)
                        .Add("Max", (x, y, z) => Max(x, Max(y, z)), allowReplace)
                        .Add("Round", x => Round(x), allowReplace)
                        .Add("Round", (x, y) => Round(x, (int)y), allowReplace)
                        .Add("Floor", x => Floor(x), allowReplace)
                        .Add("Ceiling", x => Ceiling(x), allowReplace)
                        .Add("Sqrt", x => (decimal)Sqrt((double)x), allowReplace)
                        .Add("Pow", (x, y) => (decimal)Pow((double)x, (double)y), allowReplace)
                        .Add("Abs", x => Abs(x), allowReplace);
    }
}

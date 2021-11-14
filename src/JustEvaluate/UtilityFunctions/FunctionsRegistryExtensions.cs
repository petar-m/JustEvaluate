using static System.Math;

namespace JustEvaluate.UtilityFunctions
{
    public static class FunctionsRegistryExtensions
    {
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

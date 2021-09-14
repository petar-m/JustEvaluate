using JustEvaluate.UtilityFunctions;
using Xunit;

namespace JustEvaluate.Tests
{
    public partial class FunctionExtensionsTests : IClassFixture<FunctionExtensionsTests.EvaluatorSetup>
    {
        private readonly EvaluatorSetup _setup;

        public FunctionExtensionsTests(EvaluatorSetup setup) => _setup = setup;

        public class EvaluatorSetup
        {
            public EvaluatorSetup()
            {
                var functions = new Functions();
                functions.AddLogical(allowReplace: false);
                functions.AddMath(allowReplace: false);

                Evaluator = new Evaluator(new Parser(), new Builder(functions), new CompiledExpressionsCache());
            }

            public Evaluator Evaluator { get; }
        }

        public class Input
        {
            public decimal X { get; set; }
            public decimal Y { get; set; }
            public decimal Z { get; set; }
        }
    }
}

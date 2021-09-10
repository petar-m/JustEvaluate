using System;

namespace JustEvaluate
{
    public class Evaluator
    {
        private readonly Parser _parser;
        private readonly Builder _builder;
        private readonly CompiledExpressionsCache _expressionCache;

        public Evaluator(Parser parser, Builder builder, CompiledExpressionsCache expressionCache)
        {
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
            _expressionCache = expressionCache ?? throw new ArgumentNullException(nameof(expressionCache));
        }

        public decimal Evaluate(string input)
        {
            Func<decimal> function = _expressionCache.Get(input);
            if(function is null)
            {
                var tokenizedInput = _parser.Parse(input);
                function = _builder.Build(tokenizedInput);
                _expressionCache.Add(input, function);
            }

            return function();
        }

        public decimal Evaluate<TArg>(string input, TArg argument)
        {
            Func<TArg, decimal> function = _expressionCache.Get<TArg>(input);
            if(function is null)
            {
                var tokenizedInput = _parser.Parse(input);
                function = _builder.Build<TArg>(tokenizedInput);
                _expressionCache.Add(input, function);
            }

            return function(argument);
        }
    }
}

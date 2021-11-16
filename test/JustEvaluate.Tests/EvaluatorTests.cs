using System;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace JustEvaluate.Tests
{
    public class EvaluatorTests
    {
        [Fact]
        public void Constructor_ParserIsNull_Throws()
        {
            Action action = () => new Evaluator(null, new Builder(new FunctionsRegistry()), new CompiledExpressionsCache());

            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("parser");
        }

        [Fact]
        public void Constructor_BuilderIsNull_Throws()
        {
            Action action = () => new Evaluator(new Parser(), null, new CompiledExpressionsCache());

            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("builder");
        }

        [Fact]
        public void Constructor_ExpressionCacheIsNull_Throws()
        {
            Action action = () => new Evaluator(new Parser(), new Builder(new FunctionsRegistry()), null);

            action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("expressionCache");
        }

        [Fact]
        public void Evaluate_WithNoArgument_WhenNotCached_ParsedBuiltCachedInvoked()
        {
            var input = "1 + 1";
            var tokens = new Token[] { };

            var parser = A.Fake<Parser>();
            A.CallTo(() => parser.Parse(input)).Returns(tokens);

            var compiled = A.Fake<Func<decimal>>();
            A.CallTo(() => compiled.Invoke()).Returns(0);

            var builder = A.Fake<Builder>();
            A.CallTo(() => builder.Build(tokens)).Returns(compiled);

            var expressionCache = A.Fake<CompiledExpressionsCache>();
            A.CallTo(() => expressionCache.Get(input)).Returns(null);

            var evaluator = new Evaluator(parser, builder, expressionCache);
            _ = evaluator.Evaluate(input);

            A.CallTo(() => parser.Parse(input)).MustHaveHappenedOnceExactly();
            A.CallTo(() => builder.Build(tokens)).MustHaveHappenedOnceExactly();
            A.CallTo(() => expressionCache.Add(input, compiled)).MustHaveHappenedOnceExactly();
            A.CallTo(() => compiled.Invoke()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Evaluate_WithNoArgument_WhenCached_Invoked()
        {
            var input = "1 + 1";
            var tokens = new Token[] { };

            var parser = A.Fake<Parser>();
            A.CallTo(() => parser.Parse(input)).Returns(tokens);

            var compiled = A.Fake<Func<decimal>>();
            A.CallTo(() => compiled.Invoke()).Returns(0);

            var builder = A.Fake<Builder>();
            A.CallTo(() => builder.Build(tokens)).Returns(compiled);

            var expressionCache = A.Fake<CompiledExpressionsCache>();
            A.CallTo(() => expressionCache.Get(input)).Returns(compiled);

            var evaluator = new Evaluator(parser, builder, expressionCache);
            _ = evaluator.Evaluate(input);

            A.CallTo(() => expressionCache.Get(input)).MustHaveHappenedOnceExactly();
            A.CallTo(() => compiled.Invoke()).MustHaveHappenedOnceExactly();

            A.CallTo(() => parser.Parse(input)).MustNotHaveHappened();
            A.CallTo(() => builder.Build(tokens)).MustNotHaveHappened();
            A.CallTo(() => expressionCache.Add(input, compiled)).MustNotHaveHappened();
        }

        [Fact]
        public void Evaluate_WithArgument_WhenNotCached_ParsedBuiltCachedInvoked()
        {
            var argument = new Arguments();
            var input = "Width * Height";
            var tokens = Array.Empty<Token>();

            var parser = A.Fake<Parser>();
            A.CallTo(() => parser.Parse(input)).Returns(tokens);

            var compiled = A.Fake<Func<Arguments, decimal>>();
            A.CallTo(() => compiled.Invoke(argument)).Returns(0);

            var builder = A.Fake<Builder>();
            A.CallTo(() => builder.Build<Arguments>(tokens)).Returns(compiled);

            var expressionCache = A.Fake<CompiledExpressionsCache>();
            A.CallTo(() => expressionCache.Get<Arguments>(input)).Returns(null);

            var evaluator = new Evaluator(parser, builder, expressionCache);
            _ = evaluator.Evaluate(input, argument);

            A.CallTo(() => parser.Parse(input)).MustHaveHappenedOnceExactly();
            A.CallTo(() => builder.Build<Arguments>(tokens)).MustHaveHappenedOnceExactly();
            A.CallTo(() => expressionCache.Add(input, compiled)).MustHaveHappenedOnceExactly();
            A.CallTo(() => compiled.Invoke(argument)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Evaluate_Argument_WhenCached_Invoked()
        {
            var argument = new Arguments();
            var input = "Width * Height";
            var tokens = Array.Empty<Token>();

            var parser = A.Fake<Parser>();

            var compiled = A.Fake<Func<Arguments, decimal>>();
            A.CallTo(() => compiled.Invoke(argument)).Returns(0);

            var builder = A.Fake<Builder>();

            var expressionCache = A.Fake<CompiledExpressionsCache>();
            A.CallTo(() => expressionCache.Get<Arguments>(input)).Returns(compiled);

            var evaluator = new Evaluator(parser, builder, expressionCache);
            _ = evaluator.Evaluate(input, argument);

            A.CallTo(() => expressionCache.Get<Arguments>(input)).MustHaveHappenedOnceExactly();
            A.CallTo(() => compiled.Invoke(argument)).MustHaveHappenedOnceExactly();

            A.CallTo(() => parser.Parse(input)).MustNotHaveHappened();
            A.CallTo(() => builder.Build<Arguments>(tokens)).MustNotHaveHappened();
        }
    }
}

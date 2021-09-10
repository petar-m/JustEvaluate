using System;
using FluentAssertions;
using Xunit;

namespace JustEvaluate.Tests
{
    public class ExpressionCacheTests
    {
        [Fact]
        public void Added_Expression_IsStored()
        {
            var name = "expression name";
            Func<decimal> func = () => 1;
            CompiledExpressionsCache cache = new CompiledExpressionsCache();
            cache.Add(name, func);

            var cached = cache.Get(name);

            cached.Should().BeSameAs(func);
        }

        [Fact]
        public void ExpressionName_IsCaseInsensitive()
        {
            var name = "expression name";
            Func<decimal> func = () => 1;
            CompiledExpressionsCache cache = new CompiledExpressionsCache();
            cache.Add(name, func);

            var cached = cache.Get(name.ToUpper());

            cached.Should().BeSameAs(func);
        }

        [Fact]
        public void Expression_WithGivenName_NotFound_Returns_Null()
        {
            var name = "expression name";
            Func<decimal> func = () => 1;
            CompiledExpressionsCache cache = new CompiledExpressionsCache();
            cache.Add(name, func);

            var cached = cache.Get("another name");

            cached.Should().BeNull();
        }

        [Fact]
        public void Added_ExpressionWithTypeArgument_IsStored()
        {
            var name = "expression name";
            Func<Arguments, decimal> func = _ => 1;
            CompiledExpressionsCache cache = new CompiledExpressionsCache();
            cache.Add(name, func);

            var cached = cache.Get<Arguments>(name);

            cached.Should().BeSameAs(func);
        }

        [Fact]
        public void ExpressionWithTypeArgument_Name_IsCaseInsensitive()
        {
            var name = "expression name";
            Func<Arguments, decimal> func = _ => 1;
            CompiledExpressionsCache cache = new CompiledExpressionsCache();
            cache.Add(name, func);

            var cached = cache.Get<Arguments>(name.ToUpper());

            cached.Should().BeSameAs(func);
        }

        [Fact]
        public void SameExpression_DifferentTypeArgument_IsStored()
        {
            var name = "expression name";
            Func<Arguments, decimal> func = _ => 1;
            Func<string, decimal> func2 = _ => 2;
            CompiledExpressionsCache cache = new CompiledExpressionsCache();
            cache.Add(name, func);
            cache.Add(name, func2);

            var cached = cache.Get<Arguments>(name);
            var cached2 = cache.Get<string>(name);

            cached.Should().BeSameAs(func);
            cached2.Should().BeSameAs(func2);
        }

        [Fact]
        public void ExpressionWithTypeArgument_WithGivenName_NotFound_Returns_Null()
        {
            var name = "expression name";
            Func<Arguments, decimal> func = _ => 1;
            CompiledExpressionsCache cache = new CompiledExpressionsCache();
            cache.Add(name, func);

            var cached = cache.Get<Arguments>("another name");

            cached.Should().BeNull();
        }

        [Fact]
        public void ExpressionWithTypeArgument_WithGivenName_DifferentTypeArgument_NotFound_Returns_Null()
        {
            var name = "expression name";
            Func<Arguments, decimal> func = _ => 1;
            CompiledExpressionsCache cache = new CompiledExpressionsCache();
            cache.Add(name, func);

            var cached = cache.Get<string>(name);

            cached.Should().BeNull();
        }
    }
}

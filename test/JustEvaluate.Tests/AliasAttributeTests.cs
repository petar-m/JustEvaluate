using System;
using FluentAssertions;
using Xunit;

namespace JustEvaluate.Tests
{
    public class AliasAttributeTests
    {
        [Fact]
        public void Alias_Null_Trows()
        {
            Action action = () => _ = new AliasAttribute(null);

            _ = action.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("name");
        }

        [Fact]
        public void Alias_Empty_Trows()
        {
            Action action = () => _ = new AliasAttribute(string.Empty);

            _ = action.Should().Throw<ArgumentException>().WithMessage("Alias can not be empty or white space (Parameter 'name')");
        }

        [Fact]
        public void Alias_Whitespace_Trows()
        {
            Action action = () => _ = new AliasAttribute(" ");

            _ = action.Should().Throw<ArgumentException>().WithMessage("Alias can not be empty or white space (Parameter 'name')");
        }

        [Fact]
        public void Alias_Whitespaces_Trows()
        {
            Action action = () => _ = new AliasAttribute("  ");

            _ = action.Should().Throw<ArgumentException>().WithMessage("Alias can not be empty or white space (Parameter 'name')");
        }

        [Theory]
        [InlineData("> Name", ">")]
        [InlineData("Name <", "<")]
        [InlineData("Name & other", "&")]
        [InlineData("Name , other", ",")]
        [InlineData("Name * other -", "* -")]
        public void Alias_ContainingTerminalChar_Trows(string name, string terminalChars)
        {
            Action action = () => _ = new AliasAttribute(name);

            _ = action.Should().Throw<ArgumentException>().WithMessage($"Alias can not contain terminal chars: {terminalChars} (Parameter 'name')");
        }

        [Fact]
        public void Alias_Name_IsTrimmed()
        {
            _ = new AliasAttribute(" x ").Name.Should().Be("x");
        }
    }
}

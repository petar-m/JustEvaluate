using System;
using System.Linq;

namespace JustEvaluate
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class AliasAttribute : Attribute
    {
        public AliasAttribute(string name)
        {
            if(name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if(string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Alias can not be empty or white space", nameof(name));
            }

            var terminalChars = name.Where(x => x.IsTerminalChar()).ToArray();
            if(terminalChars.Length > 0)
            {
                throw new ArgumentException($"Alias can not contain terminal chars: {string.Join(" ", terminalChars)}", nameof(name));
            }

            Name = name.Trim();
        }

        public string Name { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JustEvaluate
{
    public class ParserOptions
    {
        public bool EnableOrAsText { get; set; }

        public bool EnableAndAsText { get; set; }
    }

    public class Parser
    {
        private readonly ParserOptions _options;

        private static readonly Lazy<Regex> _matchAnd = new Lazy<Regex>(() => new Regex(" and ", RegexOptions.Compiled | RegexOptions.IgnoreCase));
        private static readonly Lazy<Regex> _matchOr = new Lazy<Regex>(() => new Regex(" or ", RegexOptions.Compiled | RegexOptions.IgnoreCase));

        public Parser()
        {
        }

        public Parser(ParserOptions options)
        {
            _options = options;
        }

        public virtual IEnumerable<Token> Parse(string input)
        {
            var builder = new StringBuilder(input);
            var cleanedInput = builder.Replace("\r", string.Empty)
                                      .Replace("\n", string.Empty)
                                      .Replace("\t", string.Empty)
                                      .ToString();

            if(_options?.EnableAndAsText == true)
            {
                cleanedInput = _matchAnd.Value.Replace(cleanedInput, " & ");
            }

            if(_options?.EnableOrAsText == true)
            {
                cleanedInput = _matchOr.Value.Replace(cleanedInput, " | ");
            }

            var tokens = ParseToTokens(cleanedInput);
            ValidateBrackets(tokens);
            tokens = RemoveEmpty(tokens);
            tokens = ConvertUnaryOperationsToBinary(tokens);
            tokens = Functions(tokens);
            return tokens;
        }

        private List<Token> ParseToTokens(string input)
        {
            var tokens = new List<Token>();
            int length = 0;
            int start = 0;
            for(int i = 0; i < input.Length; i++)
            {
                length++;
                char current = input[i];
                if(current.IsTerminalChar())
                {
                    tokens.Add(new Token(input.Substring(start, length - 1)));
                    if(current.IsPossibleTerminalSequence() && i < input.Length - 1 && input[i + 1].IsNextInTerminalSequence())
                    {
                        tokens.Add(new Token(new string(new char[] { current, input[i + 1] })));
                        length = 0;
                        start = i + 2;
                        i++;
                    }
                    else
                    {
                        tokens.Add(new Token(current));
                        length = 0;
                        start = i + 1;
                    }
                }
            }

            if(start <= input.Length - 1)
            {
                tokens.Add(new Token(input.Substring(start, length)));
            }

            return tokens;
        }

        private List<Token> RemoveEmpty(List<Token> tokens)
        {
            for(int i = tokens.Count - 1; i >= 0; i--)
            {
                if(tokens[i].IsEmpty)
                {
                    tokens.Remove(tokens[i]);
                }
            }

            return tokens;
        }

        private List<Token> Functions(List<Token> tokens)
        {
            var functionsStack = new Stack<Token>();
            var bracesStack = new Stack<int>();
            var removeTokens = new List<Token>();

            for(int i = 0; i < tokens.Count; i++)
            {
                if(tokens[i].IsOpeningBracket)
                {
                    if(i > 0 && tokens[i - 1].IsName)
                    {
                        tokens[i - 1].ChangeToFunction();
                        functionsStack.Push(tokens[i - 1]);
                        removeTokens.Add(tokens[i]);
                        tokens[i - 1].FunctionArguments.Add(new List<Token>());
                        bracesStack.Push(0);
                    }
                    else if(functionsStack.Count > 0)
                    {
                        functionsStack.Peek().FunctionArguments.Last().Add(tokens[i]);
                        bracesStack.Push(bracesStack.Pop() + 1);
                        removeTokens.Add(tokens[i]);
                    }
                }
                else if(tokens[i].IsFunctionParameterSeparator)
                {
                    if(bracesStack.Count > 0 && bracesStack.Peek() > 0)
                    {
                        throw new Exception("Mismatched brackets");
                    }

                    removeTokens.Add(tokens[i]);
                    if(functionsStack.Count == 0)
                    {
                        throw new InvalidOperationException("Misplaced function parameter separator");
                    }

                    functionsStack.Peek().FunctionArguments.Add(new List<Token>());
                }
                else if(tokens[i].IsClosingBracket)
                {
                    if(functionsStack.Count > 0)
                    {
                        int braceCount = bracesStack.Count > 0 ? bracesStack.Pop() : 0;
                        if(braceCount == 0)
                        {
                            // end of function
                            var func = functionsStack.Pop();

                            var lastArg = func.FunctionArguments.Last();
                            if(lastArg.Count == 0)
                            {
                                func.FunctionArguments.Remove(lastArg);
                            }

                            removeTokens.Add(tokens[i]);
                        }
                        else
                        {
                            functionsStack.Peek().FunctionArguments.Last().Add(tokens[i]);
                            bracesStack.Push(braceCount - 1);
                            removeTokens.Add(tokens[i]);
                        }
                    }
                }
                else if(functionsStack.Count > 0)
                {
                    functionsStack.Peek().FunctionArguments.Last().Add(tokens[i]);
                    removeTokens.Add(tokens[i]);
                }
            }

            foreach(var token in removeTokens)
            {
                tokens.Remove(token);
            }
            return tokens;
        }

        private void ValidateBrackets(List<Token> tokens)
        {
            int count = 0;
            for(int i = 0; i < tokens.Count; i++)
            {
                if(tokens[i].IsOpeningBracket)
                {
                    count++;
                }
                else if(tokens[i].IsClosingBracket)
                {
                    count--;
                    if(count < 0)
                    {
                        throw new Exception("Mismatched brackets");
                    }
                }
            }

            if(count != 0)
            {
                throw new Exception("Mismatched brackets");
            }
        }

        private List<Token> ConvertUnaryOperationsToBinary(List<Token> tokens)
        {
            for(int i = tokens.Count - 1; i >= 0; i--)
            {
                var token = tokens[i];
                if((token.IsAdd || token.IsSubtract) && (i == 0 || tokens[i - 1].IsOpeningBracket || tokens[i - 1].IsFunctionParameterSeparator))
                {
                    if(token.IsAdd)
                    {
                        tokens.RemoveAt(i);
                    }
                    else
                    {
                        tokens[i] = new Token("*");
                        tokens.Insert(i, new Token(-1));
                    }
                }
            }

            return tokens;
        }
    }
}

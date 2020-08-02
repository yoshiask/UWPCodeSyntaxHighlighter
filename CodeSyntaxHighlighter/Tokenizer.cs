using CodeSyntaxHighlighter.TextMate;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CodeSyntaxHighlighter
{
    public class Tokenizer
    {
        public LanguageGrammar Grammar { get; set; }

        public Tokenizer(LanguageGrammar grammar)
        {
            Grammar = grammar;
        }

        public List<Token> Tokenize(string text)
        {
            List<Token> tokens = new List<Token>(text.Length);
            foreach (var pattern in Grammar.Patterns)
            {
                tokens.AddRange(MatchPattern(pattern, text));
            }
            return tokens;
        }

        public List<Token> MatchPattern(Pattern pattern, string text)
        {
            List<Token> tokens = new List<Token>();
            List<Token> subTokens = new List<Token>();
            string refName = pattern.Include;

            if (pattern.Match != null)
            {
                MatchCollection matches = Regex.Matches(text, pattern.Match);
                foreach (Match match in matches)
                {
                    tokens.AddRange(HandleCaptures(pattern, match));

                    if (pattern.Patterns != null)
                    {
                        foreach (Pattern subPattern in pattern.Patterns)
                        {
                            tokens.AddRange(MatchPattern(subPattern, text));
                        }
                    }
                    return tokens;
                }
                return tokens;
            }

            else if (pattern.Begin != null && pattern.End != null)
            {
                // Look for matches of Begin
                MatchCollection begins = Regex.Matches(text, pattern.Begin);
                int lastEndMatch = -1;
                foreach (Match begin in begins)
                {
                    if (begin.Index <= lastEndMatch)
                        continue;

                    // Look for matches of End that are after the captured Begin
                    string textAfterBegin = text.Substring(begin.Index + 1);
                    Match end = Regex.Match(textAfterBegin, pattern.End);
                    Token curToken;
                    string content;
                    if (end == null)
                    {
                        // TODO: Handle this properly, HandleCapture overrides this

                        // The TextMate docs say that if there's no end match,
                        // capture all of the remaining text
                        content = begin.Value + textAfterBegin;
                        curToken = new Token()
                        {
                            Text = content,
                            Name = pattern.Name,
                            Start = begin.Index,
                            //Length = text.Length - begin.Index
                        };
                    }
                    else
                    {
                        content = text.Substring(begin.Index, Math.Min(end.Index + 2, text.Length - begin.Index));
                        curToken = new Token()
                        {
                            Text = content,
                            Name = pattern.Name,
                            Start = begin.Index,
                            //Length = end.Index + 1
                        };
                        lastEndMatch = end.Index + begin.Index + 1;
                    }
                    tokens.AddRange(HandleCaptures(pattern, begin, end, content));

                    foreach (Pattern subPattern in pattern.Patterns)
                    {
                        subTokens.AddRange(MatchPattern(subPattern, curToken.Text));
                    }
                    tokens.AddRange(subTokens);
                }

                return tokens;
            }

            else if (refName != null)
            {
                // Don't forget to remove the pound symbol!
                if (refName.StartsWith("#"))
                    refName = refName.Substring(1);

                if (Grammar.Repository.ContainsKey(refName))
                    return MatchPattern(Grammar.Repository[refName], text);
            }

            if (pattern.Patterns != null)
            {
                foreach (Pattern subPattern in pattern.Patterns)
                {
                    tokens.AddRange(MatchPattern(subPattern, text));
                }
                return tokens;
            }

            // If we've reached this point, then nothing matched
            // and tokens is empty
            return tokens;
        }

        private List<Token> HandleCaptures(Pattern pattern, Match match)
        {
            var tokens = new List<Token>();
            for (int i = 0; i < match.Groups.Count; i++)
            {
                if (pattern.Captures != null && pattern.Captures.ContainsKey(i.ToString()))
                {
                    tokens.Add(new Token(match, pattern.Captures[i.ToString()]));
                }
                else
                {
                    tokens.Add(new Token(match, pattern.Name));
                }
            }
            return tokens;
        }

        private List<Token> HandleCaptures(Pattern pattern, Match beginMatch, Match endMatch, string content)
        {
            var tokens = new List<Token>();

            if (pattern.BeginCaptures != null && pattern.BeginCaptures.ContainsKey("0"))
            {
                tokens.Add(new Token(beginMatch, pattern.BeginCaptures["0"].Name));
            }
            else
            {
                tokens.Add(new Token(endMatch, pattern.Name));
            }

            if (pattern.EndCaptures != null && pattern.EndCaptures.ContainsKey("0"))
            {
                tokens.Add(new Token(endMatch, pattern.EndCaptures["0"].Name));
            }
            else
            {
                tokens.Add(new Token(endMatch, pattern.Name));
            }

            tokens.Add(new Token()
            {
                Text = content,
                Name = pattern.Name,
                Start = beginMatch.Index + 1,
                //Length = content.Length
            });

            return tokens;
        }
    }
}

using CodeSyntaxHighlighter.TextMate;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodeSyntaxHighlighter
{
    public class Token : Range
    {
        public string Text { get; set; }
        public string Name { get; set; }

        public Token(Match x, string patternName)
        {
            Text = x.Value;
            Name = patternName;
            Start = x.Index;
            End = Start + x.Length;
        }

        public Token() { }
        public Token(Token copy) : base(copy)
		{
            Text = copy.Text;
            Name = copy.Name;
		}

        public override string ToString()
        {
            return Name + ": " + Text;
        }

        private (Dictionary<string, string>, int)? _style;
        public (Dictionary<string, string>, int) GetStyle(LanguageTheme theme)
		{
            if (!_style.HasValue)
                _style = Theme.GetStyleForScope(theme, Name);
            return _style.Value;
		}

        public static List<Token> Collapse(IEnumerable<Token> me)
        {
            List<Token> orderdList = me.OrderBy(r => r.Start).ToList();
            List<Token> newList = new List<Token>();

            int max = orderdList[0].End;
            int min = orderdList[0].Start;

            foreach (var item in orderdList.Skip(1))
            {
                if (item.End > max && item.Start > max)
                {
                    newList.Add(new Token { Start = min, End = max, Name = item.Name });
                    min = item.Start;
                }
                max = max > item.End ? max : item.End;
            }
            newList.Add(new Token { Start = min, End = max, Name = orderdList[0].Name });

            return newList;
        }
    }

    public class Range
	{
        public int Start { get; set; }
        public int End { get; set; }

        public Range() { }
        public Range(int start, int end)
		{
            Start = start;
            End = end;
		}
        public Range(Range copy)
		{
            Start = copy.Start;
            End = copy.End;
		}

        public static List<T> Collapse<T>(IEnumerable<T> me) where T : Range, new()
        {
            List<T> orderdList = me.OrderBy(r => r.Start).ToList();
            List<T> newList = new List<T>();

            int max = orderdList[0].End;
            int min = orderdList[0].Start;

            foreach (var item in orderdList.Skip(1))
            {
                if (item.End > max && item.Start > max)
                {
                    newList.Add(new T { Start = min, End = max });
                    min = item.Start;
                }
                max = max > item.End ? max : item.End;
            }
            newList.Add(new T { Start = min, End = max });

            return newList;
        }
    }
}

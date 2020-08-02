using CodeSyntaxHighlighter.TextMate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace CodeSyntaxHighlighter
{
	/// <summary>
	/// Class for displaying tokens in a text control
	/// </summary>
	public class Colorizer
	{
		public LanguageTheme Theme { get; set; }
        private List<TextSpan> Texts { get; set; }

		public Colorizer(LanguageTheme theme)
		{
			Theme = theme;
		}

		public Paragraph ColorizeNaive(List<Token> tokens, string text)
		{
            Texts = new List<TextSpan>(text.Length);
            for (int i = 0; i < text.Length; i++)
			{
                Texts.Add(null);
			}

            Texts[0] = new TextSpan(text.Length, CodeSyntaxHighlighter.Theme.GetBoxStyle(Theme), 0);
            CollapseTokens(tokens);

            // TODO: How do I highlight the actual text?

            /// const			         true
            /// string      "isPrivate"
            /// object      "isPrivate": true
            ///             "isPrivate": true


            var p = new Paragraph();
            foreach (Token token in tokens)
            {
                var run = new Run()
                {
                    Text = token.Text
                };
                (Dictionary<string, string> settings, int order) = CodeSyntaxHighlighter.Theme.GetStyleForScope(Theme, token.Name);
                if (settings != null)
                {
                    if (settings.ContainsKey("foreground"))
                    {
                        run.Foreground = GetBrushFromHex(settings["foreground"]);
                    }
                    if (settings.ContainsKey("background"))
                    {
                        // Runs don't have a background
                        //run.Background = GetSolidColorBrush(settings["background"]);
                    }
                    if (settings.ContainsKey("fontSize"))
                    {
                        run.FontSize = Int32.Parse(settings["fontSize"]);
                    }
                    if (settings.ContainsKey("fontStyle"))
                    {
                        switch (settings["fontStyle"])
                        {
                            case "italic":
                                run.FontStyle = FontStyle.Italic;
                                break;
                            case "oblique":
                                run.FontStyle = FontStyle.Oblique;
                                break;
                            case "normal":
                                run.FontStyle = FontStyle.Normal;
                                break;
                        }
                    }
                    if (settings.ContainsKey("fontWeight"))
                    {
                        switch (settings["fontWeight"])
                        {
                            case "extrablack":
                                run.FontWeight = FontWeights.ExtraBlack;
                                break;
                            case "black":
                                run.FontWeight = FontWeights.Black;
                                break;
                            case "extrabold":
                                run.FontWeight = FontWeights.ExtraBold;
                                break;
                            case "bold":
                                run.FontWeight = FontWeights.Bold;
                                break;
                            case "semibold":
                                run.FontWeight = FontWeights.SemiBold;
                                break;
                            case "medium":
                                run.FontWeight = FontWeights.Medium;
                                break;
                            case "normal":
                                run.FontWeight = FontWeights.Normal;
                                break;
                            case "semilight":
                                run.FontWeight = FontWeights.SemiLight;
                                break;
                            case "light":
                                run.FontWeight = FontWeights.Light;
                                break;
                            case "extralight":
                                run.FontWeight = FontWeights.ExtraLight;
                                break;
                            case "thin":
                                run.FontWeight = FontWeights.Thin;
                                break;
                        }
                    }
                }
                p.Inlines.Add(run);
            }
            return p;
        }

        public Paragraph Colorize(List<Token> tokens, string text)
        {
            Texts = new List<TextSpan>(text.Length);
            for (int i = 0; i < text.Length; i++)
            {
                Texts.Add(null);
            }

            Texts[0] = new TextSpan(text.Length, CodeSyntaxHighlighter.Theme.GetBoxStyle(Theme), 0);
            List<Token> collapsed = new List<Token>();
            try
			{
                collapsed = Token.Collapse(tokens.OrderByDescending(t => t.GetStyle(Theme).Item2));
                //CollapseTokens(tokens);
            }
            catch
			{

			}

            var p = new Paragraph();
            for (int i = 0; i < collapsed.Count; i++)
            {
                Token token = collapsed[i];
                if (token == null)
                    continue;
                var run = new Run()
                {
                    Text = text.Substring(token.Start, token.End - token.Start)
                };
                (Dictionary<string, string> style, int order) = token.GetStyle(Theme);
                if (style != null)
                {
                    if (style.ContainsKey("foreground"))
                    {
                        run.Foreground = GetBrushFromHex(style["foreground"]);
                    }
                    if (style.ContainsKey("background"))
                    {
                        // Runs don't have a background
                        //run.Background = GetSolidColorBrush(settings["background"]);
                    }
                    if (style.ContainsKey("fontSize"))
                    {
                        run.FontSize = Int32.Parse(style["fontSize"]);
                    }
                    if (style.ContainsKey("fontStyle"))
                    {
                        switch (style["fontStyle"])
                        {
                            case "italic":
                                run.FontStyle = FontStyle.Italic;
                                break;
                            case "oblique":
                                run.FontStyle = FontStyle.Oblique;
                                break;
                            case "normal":
                                run.FontStyle = FontStyle.Normal;
                                break;
                        }
                    }
                    if (style.ContainsKey("fontWeight"))
                    {
                        switch (style["fontWeight"])
                        {
                            case "extrablack":
                                run.FontWeight = FontWeights.ExtraBlack;
                                break;
                            case "black":
                                run.FontWeight = FontWeights.Black;
                                break;
                            case "extrabold":
                                run.FontWeight = FontWeights.ExtraBold;
                                break;
                            case "bold":
                                run.FontWeight = FontWeights.Bold;
                                break;
                            case "semibold":
                                run.FontWeight = FontWeights.SemiBold;
                                break;
                            case "medium":
                                run.FontWeight = FontWeights.Medium;
                                break;
                            case "normal":
                                run.FontWeight = FontWeights.Normal;
                                break;
                            case "semilight":
                                run.FontWeight = FontWeights.SemiLight;
                                break;
                            case "light":
                                run.FontWeight = FontWeights.Light;
                                break;
                            case "extralight":
                                run.FontWeight = FontWeights.ExtraLight;
                                break;
                            case "thin":
                                run.FontWeight = FontWeights.Thin;
                                break;
                        }
                    }
                }
                p.Inlines.Add(run);
            }
            return p;

            //var p = new Paragraph();
            for (int i = 0; i < Texts.Count; i++)
            {
                TextSpan span = Texts[i];
                if (span == null)
                    continue;
                var run = new Run()
                {
                    Text = text.Substring(i, span.End - i)
                };
                if (span.Style != null)
                {
                    if (span.Style.ContainsKey("foreground"))
                    {
                        run.Foreground = GetBrushFromHex(span.Style["foreground"]);
                    }
                    if (span.Style.ContainsKey("background"))
                    {
                        // Runs don't have a background
                        //run.Background = GetSolidColorBrush(settings["background"]);
                    }
                    if (span.Style.ContainsKey("fontSize"))
                    {
                        run.FontSize = Int32.Parse(span.Style["fontSize"]);
                    }
                    if (span.Style.ContainsKey("fontStyle"))
                    {
                        switch (span.Style["fontStyle"])
                        {
                            case "italic":
                                run.FontStyle = FontStyle.Italic;
                                break;
                            case "oblique":
                                run.FontStyle = FontStyle.Oblique;
                                break;
                            case "normal":
                                run.FontStyle = FontStyle.Normal;
                                break;
                        }
                    }
                    if (span.Style.ContainsKey("fontWeight"))
                    {
                        switch (span.Style["fontWeight"])
                        {
                            case "extrablack":
                                run.FontWeight = FontWeights.ExtraBlack;
                                break;
                            case "black":
                                run.FontWeight = FontWeights.Black;
                                break;
                            case "extrabold":
                                run.FontWeight = FontWeights.ExtraBold;
                                break;
                            case "bold":
                                run.FontWeight = FontWeights.Bold;
                                break;
                            case "semibold":
                                run.FontWeight = FontWeights.SemiBold;
                                break;
                            case "medium":
                                run.FontWeight = FontWeights.Medium;
                                break;
                            case "normal":
                                run.FontWeight = FontWeights.Normal;
                                break;
                            case "semilight":
                                run.FontWeight = FontWeights.SemiLight;
                                break;
                            case "light":
                                run.FontWeight = FontWeights.Light;
                                break;
                            case "extralight":
                                run.FontWeight = FontWeights.ExtraLight;
                                break;
                            case "thin":
                                run.FontWeight = FontWeights.Thin;
                                break;
                        }
                    }
                }
                p.Inlines.Add(run);
            }
            return p;
        }

        private void CollapseTokens(IList<Token> tokens)
		{
            // TODO: Write an algorithm that can easily and efficiently
            // apply settings over a given range

            // At this point, Texts should contain one big TextSpan
            // that covers all text and the default settings

            foreach (var token in tokens)
			{
                (Dictionary<string, string> settings, int order) = CodeSyntaxHighlighter.Theme.GetStyleForScope(Theme, token.Name);
                SetSpan(token.Start, token.End - 1, settings, order);
			}
		}

        private void SetSpan(int start, int end, Dictionary<string, string> settings, int order = 0)
		{
            // Check for any overlap with other spans
            int previousStart = Texts.FindLastIndex(
                0, Math.Min(start + 1, Texts.Count),
                ts => ts != null
            );

            if (previousStart < 0)
			{

			}

            var previous = Texts[previousStart];
            if (previous.End >= start)
			{
                Debug.WriteLine("BRANCH: Overlap with previous span");
                // Previous span overlaps with current span
                if (previous.Order > order)
                {
                    // Previous span takes priority, splice current.
                    Debug.WriteLine("BRANCH: Previous span takes priority");

                    // We don't have to deal with the first half, since we
                    // already know that it shouldn't be applied.
                    SetSpan(previous.End + 1, end, settings, order);
                    Debug.WriteLine("END: Status unknown, possibly spliced\n");
				}
                else
				{
                    // Current span takes priority
                    Debug.WriteLine("BRANCH: Current span takes priority");

                    // Splice the previous span at the start of the current span
                    SpliceAt(start, keepA: false);

                    int nextStart = Texts.FindLastIndex(start, end - start, ts => ts != null);
                    if (nextStart != -1)
					{
                        var next = Texts[nextStart];
                        // TODO: There could easily be more than one span
                        // that starts and possibly ends within the range
                        // we're looking at.

                        // Span overlaps, check priority
                        Debug.WriteLine("BRANCH: Overlap with next span");
                        if (next.Order > order)
						{
                            // Next span takes priority, splice current.
                            Debug.WriteLine("BRANCH: Next span takes priority");
                            Texts[start] = new TextSpan(nextStart - 1, settings, order);
                            Debug.WriteLine("END: Current spliced, spanB\n");
                            return;
                        } 
                        else
						{
                            // Current span takes priority, splice next.
                            Debug.WriteLine("BRANCH: Current span takes priority");
                            SpliceAt(end, keepB: false);

                            Texts[start] = new TextSpan(end, settings, order);
                            Debug.WriteLine("END: Next spliced\n");
                            return;
						}
                    }
                    else
					{
                        // There are no spans under the current span,
                        // we're good to go!
                        Debug.WriteLine("BRANCH: No spans overlap");
                        Texts[start] = new TextSpan(end, settings, order);
                        Debug.WriteLine("END: Nothing spliced\n");
                        return;
                    }
                }
            }
		}

        /// <summary>
        /// Splits any spans underneath the location index. The second span starts
        /// at <paramref name="location"/>, and the first ends at
        /// <c><paramref name="location"/> - 1</c>
        /// </summary>
        /// <param name="location">Index to cut at</param>
        private void SpliceAt(int location, bool keepA = true, bool keepB = true)
		{
            int spanStart = Texts.FindLastIndex(0, location + 1, ts => ts != null);
            var span = Texts[spanStart];
            if (keepA)
                Texts[spanStart] = new TextSpan(location - 1, span.Style, span.Order);
            else
                Texts[spanStart] = null;
            
            if (keepB)
                Texts[location] = new TextSpan(span.End, span.Style, span.Order);
            else
                Texts[location] = null;
        }

        private class TextSpan
		{
            // Start index is stored by the index of the item in the Texts property
            //public int Start { get; set; }

            /// <summary>
            /// Inclusive end index
            /// </summary>
            public int End { get; set; }
            //public int Length { get; set; }
            public Dictionary<string, string> Style { get; set; }
            public int Order { get; set; }

            public TextSpan(int end, Dictionary<string, string> style, int order)
			{
                Order = order;
                End = end;
                Style = style;
			}
		}

        public static SolidColorBrush GetBrushFromHex(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            byte a = 255, r, g, b;

            if (hex.Length == 6)
            {
                r = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
                g = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
                b = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            }
            else
            {
                a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
                r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
                g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
                b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
            }
            return new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
        }
    }
}

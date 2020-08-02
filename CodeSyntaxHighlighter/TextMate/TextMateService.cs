using System;

namespace CodeSyntaxHighlighter.TextMate
{
    public interface IGrammar
    {
        /// <summary>
        /// Tokenize <paramref name="lineText"/> using previous line state <paramref name="prevState"/>.
        /// </summary>
        ITokenizeLineResult tokenizeLine(string lineText, StackElement prevState);

        /// <summary>
        /// Tokenize <paramref name="lineText"/> using previous line state <paramref name="prevState"/>.
        /// The result contains the tokens in binary format, resolved with the following information:
        ///     - language
        ///     - token type(regex, string, comment, other)
        ///     - font style
        ///     - foreground color
        ///     - background color
        ///     e.g. for getting the languageId: `(metadata & MetadataConsts.LANGUAGEID_MASK) >>> MetadataConsts.LANGUAGEID_OFFSET`
        /// </summary>
        ITokenizeLineResult2 tokenizeLine2(string lineText, StackElement prevState);
    }
    public interface ITokenizeLineResult
    {
        IToken[] tokens { get; }
        /// <summary>
        ///  The <c>prevState</c> to be passed on to the next line tokenization.
        /// </summary>
        StackElement ruleStack { get; }
    }

    /// <summary>
    /// Helpers to manage the "collapsed" metadata of an entire StackElement stack.
    /// The following assumptions have been made:
    ///     - languageId< 256 => needs 8 bits
    ///     - unique color count< 512 => needs 9 bits
    /// 
    /// The binary format is:
    /// -------------------------------------------
    /// 3322 2222 2222 1111 1111 1100 0000 0000
    /// 1098 7654 3210 9876 5432 1098 7654 3210
    /// -------------------------------------------
    /// xxxx xxxx xxxx xxxx xxxx xxxx xxxx xxxx
    /// bbbb bbbb bfff ffff ffFF FTTT LLLL LLLL
    /// -------------------------------------------
    ///     - L = LanguageId (8 bits)
    ///     - T = StandardTokenType(3 bits)
    ///     - F = FontStyle(3 bits)
    ///     - f = foreground color(9 bits)
    ///     - b = background color(9 bits)
    /// </summary>
    public enum MetadataConsts
    {
        LANGUAGEID_MASK = 255,
        TOKEN_TYPE_MASK = 1792,
        FONT_STYLE_MASK = 14336,
        FOREGROUND_MASK = 8372224,
        BACKGROUND_MASK = 4286578688,
        LANGUAGEID_OFFSET = 0,
        TOKEN_TYPE_OFFSET = 8,
        FONT_STYLE_OFFSET = 11,
        FOREGROUND_OFFSET = 14,
        BACKGROUND_OFFSET = 23,
    }
    public interface ITokenizeLineResult2
    {
        /// <summary>
        /// The tokens in binary format. Each token occupies two array indices. For token i:
        ///     - at offset 2*i => startIndex
        ///     - at offset 2*i + 1 => metadata
        /// </summary>
        UInt32[] tokens { get; }
        /// <summary>
        /// The <c>prevState</c> to be passed on to the next line tokenization.
        /// </summary>
        StackElement ruleStack { get; }
    }
    public interface IToken
    {
        int startIndex { get; }
        int endIndex { get; }
        string[] scopes { get; }
    }

    public interface StackElement
    {
        void _stackElementBrand();
        int depth { get; }
        StackElement clone();
        bool Equals(StackElement other);
    }
}

using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace CodeSyntaxHighlighter
{
    public partial class Grammar
    {
        private static TextMate.LanguageGrammar _grammarCSharp;
        public static async Task<TextMate.LanguageGrammar> GetCSharpGrammar()
        {
            if (_grammarCSharp == null)
            {
                var tmFile = await StorageFile.GetFileFromApplicationUriAsync(
                    new Uri(@"ms-appx:///CodeSyntaxHighlighter/Grammars/csharp.tmLanguage")
                );
                string tmLanguage = await FileIO.ReadTextAsync(tmFile);
                _grammarCSharp = LoadGrammar(tmLanguage);
            }
            return _grammarCSharp;
        }


        private static TextMate.LanguageGrammar _grammarJson;
        public static async Task<TextMate.LanguageGrammar> GetJsonGrammar()
        {
            if (_grammarJson == null)
            {
                var tmFile = await StorageFile.GetFileFromApplicationUriAsync(
                    new Uri(@"ms-appx:///CodeSyntaxHighlighter/Grammars/json.tmLanguage")
                );
                string tmLanguage = await FileIO.ReadTextAsync(tmFile);
                _grammarJson = LoadGrammar(tmLanguage);
            }
            return _grammarJson;
        }


        private static TextMate.LanguageGrammar _grammarC;
        public static async Task<TextMate.LanguageGrammar> GetCGrammar()
        {
            if (_grammarC == null)
            {
                var tmFile = await StorageFile.GetFileFromApplicationUriAsync(
                    new Uri(@"ms-appx:///CodeSyntaxHighlighter/Grammars/c.tmLanguage")
                );
                string tmLanguage = await FileIO.ReadTextAsync(tmFile);
                _grammarC = LoadGrammar(tmLanguage);
            }
            return _grammarC;
        }


        private static TextMate.LanguageGrammar _grammarCpp;
        public static async Task<TextMate.LanguageGrammar> GetCppGrammar()
        {
            if (_grammarCpp == null)
            {
                var tmFile = await StorageFile.GetFileFromApplicationUriAsync(
                    new Uri(@"ms-appx:///CodeSyntaxHighlighter/Grammars/cpp.tmLanguage")
                );
                string tmLanguage = await FileIO.ReadTextAsync(tmFile);
                _grammarCpp = LoadGrammar(tmLanguage);
            }
            return _grammarCpp;
        }


        private static TextMate.LanguageGrammar _grammarLaTeX;
        public static async Task<TextMate.LanguageGrammar> GetLaTeXGrammar()
        {
            if (_grammarLaTeX == null)
            {
                var tmFile = await StorageFile.GetFileFromApplicationUriAsync(
                    new Uri(@"ms-appx:///CodeSyntaxHighlighter/Grammars/latex.tmLanguage")
                );
                string tmLanguage = await FileIO.ReadTextAsync(tmFile);
                _grammarLaTeX = LoadGrammar(tmLanguage);
            }
            return _grammarLaTeX;
        }
    }
}

using CodeSyntaxHighlighter.TextMate;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace CodeSyntaxHighlighter
{
	public partial class Theme
	{
        private static LanguageTheme _themeMonokai;
        public static async Task<LanguageTheme> GetMonokaiTheme()
        {
            if (_themeMonokai == null)
            {
                var tmFile = await StorageFile.GetFileFromApplicationUriAsync(
                    new Uri(@"ms-appx:///CodeSyntaxHighlighter/Themes/Monokai.tmTheme")
                );
                string tmTheme = await FileIO.ReadTextAsync(tmFile);
                _themeMonokai = LoadTheme(tmTheme);
            }
            return _themeMonokai;
        }

        private static LanguageTheme _themeHomebrew;
        public static async Task<LanguageTheme> GetHomebrewTheme()
        {
            if (_themeHomebrew == null)
            {
                var tmFile = await StorageFile.GetFileFromApplicationUriAsync(
                    new Uri(@"ms-appx:///CodeSyntaxHighlighter/Themes/Homebrew.tmTheme")
                );
                string tmTheme = await FileIO.ReadTextAsync(tmFile);
                _themeHomebrew = LoadTheme(tmTheme);
            }
            return _themeHomebrew;
        }

        private static LanguageTheme _themeVSCodeLightPlus;
        public static async Task<LanguageTheme> GetVSCodeLightPlusTheme()
        {
            if (_themeVSCodeLightPlus == null)
            {
                var tmFile = await StorageFile.GetFileFromApplicationUriAsync(
                    new Uri(@"ms-appx:///CodeSyntaxHighlighter/Themes/vscode_light_plus.tmTheme")
                );
                string tmTheme = await FileIO.ReadTextAsync(tmFile);
                _themeVSCodeLightPlus = LoadTheme(tmTheme);
            }
            return _themeVSCodeLightPlus;
        }

        private static LanguageTheme _themeVSCodeDarkPlus;
        public static async Task<LanguageTheme> GetVSCodeDarkPlusTheme()
        {
            // TODO: VS Code theme use "tokenColors" instead of "settings",
            // and scope is an array of strings instead of a single string
            if (_themeVSCodeDarkPlus == null)
            {
                var tmFile = await StorageFile.GetFileFromApplicationUriAsync(
                    new Uri(@"ms-appx:///CodeSyntaxHighlighter/Themes/vscode_dark_plus.tmTheme")
                );
                string tmTheme = await FileIO.ReadTextAsync(tmFile);
                _themeVSCodeDarkPlus = LoadTheme(tmTheme);
            }
            return _themeVSCodeDarkPlus;
        }
    }
}

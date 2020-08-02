using CodeSyntaxHighlighter;
using CodeSyntaxHighlighter.TextMate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CSHSampleApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        LanguageGrammar grammar;
        LanguageTheme theme;
        Tokenizer tokenizer;
        Colorizer colorizer;

        public MainPage()
        {
            this.InitializeComponent();
            //Do();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            grammar = await Grammar.GetJsonGrammar();
            theme = await Theme.GetHomebrewTheme();
            colorizer = new Colorizer(theme);

            // Set the proper theme for the display box
            var settings = Theme.GetBoxStyle(theme);
            if (settings != null)
            {
                if (settings.ContainsKey("foreground"))
                {
                    display.Foreground = Colorizer.GetBrushFromHex(settings["foreground"]);
                }
                if (settings.ContainsKey("background"))
                {
                    displayGrid.Background = Colorizer.GetBrushFromHex(settings["background"]);
                }
                if (settings.ContainsKey("fontSize"))
                {
                    display.FontSize = Int32.Parse(settings["fontSize"]);
                }
                if (settings.ContainsKey("fontStyle"))
                {
                    switch (settings["fontStyle"])
                    {
                        case "italic":
                            display.FontStyle = FontStyle.Italic;
                            break;
                        case "oblique":
                            display.FontStyle = FontStyle.Oblique;
                            break;
                        case "normal":
                            display.FontStyle = FontStyle.Normal;
                            break;
                    }
                }
                if (settings.ContainsKey("fontWeight"))
                {
                    switch (settings["fontWeight"])
                    {
                        case "extrablack":
                            display.FontWeight = FontWeights.ExtraBlack;
                            break;
                        case "black":
                            display.FontWeight = FontWeights.Black;
                            break;
                        case "extrabold":
                            display.FontWeight = FontWeights.ExtraBold;
                            break;
                        case "bold":
                            display.FontWeight = FontWeights.Bold;
                            break;
                        case "semibold":
                            display.FontWeight = FontWeights.SemiBold;
                            break;
                        case "medium":
                            display.FontWeight = FontWeights.Medium;
                            break;
                        case "normal":
                            display.FontWeight = FontWeights.Normal;
                            break;
                        case "semilight":
                            display.FontWeight = FontWeights.SemiLight;
                            break;
                        case "light":
                            display.FontWeight = FontWeights.Light;
                            break;
                        case "extralight":
                            display.FontWeight = FontWeights.ExtraLight;
                            break;
                        case "thin":
                            display.FontWeight = FontWeights.Thin;
                            break;
                    }
                }
                if (settings.ContainsKey("selection"))
				{
                    display.SelectionHighlightColor = Colorizer.GetBrushFromHex(settings["selection"]);
				}
            }
        }

        private void editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            tokenizeButton_Click(null, null);
        }

        private void tokenizeButton_Click(object sender, RoutedEventArgs e)
        {
            tokenizer = new Tokenizer(grammar);
            display.Blocks.Clear();

            var tokens = tokenizer.Tokenize(editor.Text);
            // Now colorize the text
            display.Blocks.Add(colorizer.Colorize(
                tokens,
                editor.Text
            ));

            //Debug.Write("\r\n");
            //foreach (var token in tokens)
			//{
            //    Debug.WriteLine($"[ {token.Name}: {token.Text} ]");
            //}
        }

        //public async void Do()
        //{
        //    var savePicker = new Windows.Storage.Pickers.FileSavePicker();
        //    savePicker.SuggestedStartLocation =
        //        Windows.Storage.Pickers.PickerLocationId.Desktop;
        //    savePicker.FileTypeChoices.Add("C# Source File", new List<string>() { ".cs" });
        //    Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
        //    await Windows.Storage.FileIO.WriteTextAsync(file, await CodeSyntaxHighlighter.Grammar.Parse());
        //}
    }
}

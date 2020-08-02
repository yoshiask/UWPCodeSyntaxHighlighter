using CodeSyntaxHighlighter.TextMate;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CodeSyntaxHighlighter
{
	public partial class Theme
	{
		public static LanguageTheme LoadTheme(string tmThemeName)
		{
			return JsonConvert.DeserializeObject<LanguageTheme>(tmThemeName);
		}

		public static (Dictionary<string, string>, int) GetStyleForScope(LanguageTheme theme, string name)
		{
			if (theme == null || theme.Settings == null)
			{
				return (null, -1);
			}

			string[] selectors = name.Split('.');
			foreach (var sett in theme.Settings)
			{
				if (sett.Scope == null)
					continue;

				string[] scopes = sett.Scope.Split('.');

				// If the theme's selector is more specific than the name,
				// it can't possibly match
				if (scopes.Length > selectors.Length)
					return (new Dictionary<string, string>(), -1);

				bool isMatch = true;
				int selectorMatchCount = 0;
				for (int i = 0; i < scopes.Length; i++)
				{
					if (scopes[i] != selectors[i] && scopes[i] != "*")
					{
						isMatch = false;
						break;
					}
					selectorMatchCount++;
				}

				if (isMatch)
					return (sett.Settings, selectorMatchCount);
			}
			return (new Dictionary<string, string>(), -1);
		}
	
		public static Dictionary<string, string> GetBoxStyle(LanguageTheme theme)
		{
			return theme?.Settings?.Find(s => s.Scope == null)?.Settings;
		}
	}
}

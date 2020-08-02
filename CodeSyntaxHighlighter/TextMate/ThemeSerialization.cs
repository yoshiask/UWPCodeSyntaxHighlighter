using Newtonsoft.Json;
using System.Collections.Generic;

namespace CodeSyntaxHighlighter.TextMate
{
	public class LanguageTheme
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("scope")]
		public string Scope { get; set; }

		[JsonProperty("semanticClass")]
		public string SemanticClass { get; set; }

		[JsonProperty("uuid")]
		public string UUID { get; set; }

		[JsonProperty("author")]
		public string Author { get; set; }

		[JsonProperty("comment")]
		public string Comment { get; set; }

		[JsonProperty("settings")]
		public List<ThemeSettings> Settings { get; set; }
	}

	public class ThemeSettings
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("scope")]
		public string Scope { get; set; }

		[JsonProperty("settings")]
		public Dictionary<string, string> Settings { get; set; }
	}
}

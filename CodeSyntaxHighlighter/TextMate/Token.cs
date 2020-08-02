using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSyntaxHighlighter.TextMate
{
	public class Token
	{
		public virtual void _tokenBrand() { }

		public int offset;
		public string type;
		public string language;

		Token(int offset, string type, string language)
		{
			this.offset = offset | 0;
			this.type = type;
			this.language = language;
		}

		public override string ToString()
		{
			return '(' + this.offset + ", " + this.type + ')';
		}
	}

	public class TokenizationResult
	{
		public virtual void _tokenizationResultBrand() { }

		public readonly Token[] tokens;
		public readonly IState endState;

		TokenizationResult(Token[] tokens, IState endState)
		{
			this.tokens = tokens;
			this.endState = endState;
		}
	}

	public class TokenizationResult2
	{
		public virtual void _tokenizationResult2Brand() { }

		/// <summary>
		/// The tokens in binary format. Each token occupies two array indices. For token i:
		/// - at offset 2*i => startIndex
		/// - at offset 2*i + 1 => metadata
		/// </summary>
		public readonly UInt32[] tokens;
		public readonly IState endState;

		TokenizationResult2(UInt32[] tokens, IState endState)
		{
			this.tokens = tokens;
			this.endState = endState;
		}
	}
}

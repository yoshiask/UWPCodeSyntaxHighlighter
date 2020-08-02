using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeSyntaxHighlighter.TextMate;

namespace CodeSyntaxHighlighter
{
    public class Tokenization
    {
        private readonly _grammar IGrammar;
        private readonly bool _containsEmbeddedLanguages;
        private readonly List<bool> _seenLanguages;
        private readonly StackElement _initialState;

        public delegate void OnDidEncounterLanguageHandler(LanguageId value);
        public static event OnDidEncounterLanguageHandler OnDidEncounterLanguage;

        public Tokenization(IGrammar grammar, StackElement initialState, bool containsEmbeddedLanguages)
        {
            //super();
            this._grammar = grammar;
            this._initialState = initialState;
            this._containsEmbeddedLanguages = containsEmbeddedLanguages;
            this._seenLanguages = new List<bool>();
        }

        public IState getInitialState() {
		    return this._initialState;
	    }

        public TokenizationResult2 tokenize2(string line, StackElement state)
        {
            var textMateResult = this._grammar.tokenizeLine2(line, state);

            if (this._containsEmbeddedLanguages)
            {
                var seenLanguages = this._seenLanguages;
                var tokens = textMateResult.tokens;

                // Must check if any of the embedded languages was hit
                for (int i = 0, len = (tokens.length >> 1); i < len; i++)
                {
                    var metadata = tokens[(i << 1) + 1];
                    var languageId = TokenMetadata.getLanguageId(metadata);

                    if (!seenLanguages[languageId])
                    {
                        seenLanguages[languageId] = true;
                        OnDidEncounterLanguage?.Invoke(languageId);
                    }
                }
            }

            StackElement endState;
            // try to save an object if possible
            if (state.Equals(textMateResult.ruleStack))
            {
                endState = state;
            }
            else
            {
                endState = textMateResult.ruleStack;
            }

            return new TokenizationResult2(textMateResult.tokens, endState);
        }
    }
}

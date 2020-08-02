using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSyntaxHighlighter
{
    /**
 * Open ended enum at runtime
 * @internal
 */
    public enum LanguageId
    {
        Null = 0,
        PlainText = 1
    }

    /**
	 * @internal
	 */
    public class LanguageIdentifier
    {

        /**
		 * A string identifier. Unique across languages. e.g. 'javascript'.
		 */
        public readonly string language;

        /**
         * A numeric identifier. Unique across languages. e.g. 5
         * Will vary at runtime based on registration order, etc.
         */
        public readonly LanguageId id;

        LanguageIdentifier(string language, LanguageId id)
        {
            this.language = language;
            this.id = id;
        }
    }

    /**
	 * A mode. Will soon be obsolete.
	 * @internal
	 */
    public interface IMode
    {

		string getId();

		LanguageIdentifier getLanguageIdentifier();

}

    /**
	 * A font style. Values are 2^x such that a bit mask can be used.
	 * @internal
	 */
    public enum FontStyle
    {
        NotSet = -1,
        None = 0,
        Italic = 1,
        Bold = 2,
        Underline = 4
    }

    /**
	 * Open ended enum at runtime
	 * @internal
	 */
    public enum ColorId
    {
        None = 0,
        DefaultForeground = 1,
        DefaultBackground = 2
    }

    /**
	 * A standard token type. Values are 2^x such that a bit mask can be used.
	 * @internal
	 */
    public enum StandardTokenType
    {
        Other = 0,
        Comment = 1,
        String = 2,
        RegEx = 4
    }

    /**
	 * Helpers to manage the "collapsed" metadata of an entire StackElement stack.
	 * The following assumptions have been languageId made < 256 => needs 8 bits
	 *  - unique color count < 512 => needs 9 bits
	 *
	 * The binary format 3322 is 2222 2222 1111 1111 1100 0000 0000
	 *     1098 7654 3210 9876 5432 1098 7654 3210
	 * - -------------------------------------------
	 *     xxxx xxxx xxxx xxxx xxxx xxxx xxxx xxxx
	 *     bbbb bbbb bfff ffff ffFF FTTT LLLL LLLL
	 * - -------------------------------------------
	 *  - L = LanguageId (8 bits)
	 *  - T = StandardTokenType (3 bits)
	 *  - F = FontStyle (3 bits)
	 *  - f = foreground color (9 bits)
	 *  - b = background color (9 bits)
	 *
	 * @internal
	 */
    public enum MetadataConsts : uint
    {
        LANGUAGEID_MASK = 0b00000000000000000000000011111111,
        TOKEN_TYPE_MASK = 0b00000000000000000000011100000000,
        FONT_STYLE_MASK = 0b00000000000000000011100000000000,
        FOREGROUND_MASK = 0b00000000011111111100000000000000,
        BACKGROUND_MASK = 0b11111111100000000000000000000000,

        ITALIC_MASK = 0b00000000000000000000100000000000,
        BOLD_MASK = 0b00000000000000000001000000000000,
        UNDERLINE_MASK = 0b00000000000000000010000000000000,

        SEMANTIC_USE_ITALIC = 0b00000000000000000000000000000001,
        SEMANTIC_USE_BOLD = 0b00000000000000000000000000000010,
        SEMANTIC_USE_UNDERLINE = 0b00000000000000000000000000000100,
        SEMANTIC_USE_FOREGROUND = 0b00000000000000000000000000001000,
        SEMANTIC_USE_BACKGROUND = 0b00000000000000000000000000010000,

        LANGUAGEID_OFFSET = 0,
        TOKEN_TYPE_OFFSET = 8,
        FONT_STYLE_OFFSET = 11,
        FOREGROUND_OFFSET = 14,
        BACKGROUND_OFFSET = 23
    }

    /**
	 * @internal
	 */
    public class TokenMetadata
    {

        public static LanguageId getLanguageId(uint metadata)
        {
            return (LanguageId)((metadata & (uint)MetadataConsts.LANGUAGEID_MASK) >> (int)MetadataConsts.LANGUAGEID_OFFSET);
        }

        public static StandardTokenType getTokenType(uint metadata)
        {
            return (StandardTokenType)((metadata & (uint)MetadataConsts.TOKEN_TYPE_MASK) >> (int)MetadataConsts.TOKEN_TYPE_OFFSET);
        }

        public static FontStyle getFontStyle(uint metadata)
        {
            return (FontStyle)((metadata & (uint)MetadataConsts.FONT_STYLE_MASK) >> (int)MetadataConsts.FONT_STYLE_OFFSET);
        }

        public static ColorId getForeground(uint metadata)
        {
            return (ColorId)((metadata & (uint)MetadataConsts.FOREGROUND_MASK) >> (int)MetadataConsts.FOREGROUND_OFFSET);
        }

        public static ColorId getBackground(uint metadata)
        {
            return (ColorId)((metadata & (uint)MetadataConsts.BACKGROUND_MASK) >> (int)MetadataConsts.BACKGROUND_OFFSET);
        }

        public static string getClassNameFromMetadata(uint metadata)
        {
			var foreground = getForeground(metadata);
			var className = "mtk" + foreground;

			var fontStyle = getFontStyle(metadata);
			switch (fontStyle)
            {
				case FontStyle.Italic:
					className += " mtki";
					break;

				case FontStyle.Bold:
					className += " mtkb";
					break;

				case FontStyle.Underline:
					className += " mtku";
					break;
            }

			return className;
		}

        public static string getInlineStyleFromMetadata(uint metadata, string[] colorMap)
        {
			var foreground = getForeground(metadata);
			var fontStyle = getFontStyle(metadata);

			var result = $"color: {colorMap[(uint)foreground]};";
			switch (fontStyle)
            {
				case FontStyle.Italic:
					result += "font-style: italic;";
					break;

				case FontStyle.Bold:
					result += "font-weight: bold;";
					break;

				case FontStyle.Underline:
					result += "text-decoration: underline;";
					break;
			}
			return result;
		}
}

/**
 * @internal
 */
public interface ITokenizationSupport
{
		IState getInitialState();

	// add offsetDelta to each of the returned indices
	TokenizationResult tokenize(string line, IState state, int offsetDelta);

    TokenizationResult2 tokenize2(string line, IState state, int offsetDelta);
}

/**
 * The state of the tokenizer between two lines.
 * It is useful to store flags such as in multiline comment, etc.
 * The model will clone the previous line's state and pass it in to tokenize the next line.
 */
public interface IState
{
		IState clone();
	bool equals(IState other);
}

/**
 * A provider result represents the values a provider, like the [`HoverProvider`](#HoverProvider),
 * may return. For once this is the actual result type `T`, like `Hover`, or a thenable that resolves
 * to that type `T`. In addition, `null` and `undefined` can be returned - either directly or from a
 * thenable.
 */
public Type ProviderResult<T> = T | undefined | null | Task<T | undefined | null>;

/**
 * A hover represents additional information for a symbol or word. Hovers are
 * rendered in a tooltip-like widget.
 */
public interface Hover
{
    /**
	 * The contents of this hover.
	 */
    IMarkdownString[] contents;

		/**
		 * The range to which this hover applies. When missing, the
		 * editor will use the range at the current position or the
		 * current position itself.
		 */
		IRange? range;
}

/**
 * The hover provider interface defines the contract between extensions and
 * the [hover](code https.visualstudio.com/docs/editor/intellisense)-feature.
 */
public interface HoverProvider
{
    /**
	 * Provide a hover for the given position and document. Multiple hovers at the same
	 * position will be merged by the editor. A hover can have a range which defaults
	 * to the word range at the position when omitted.
	 */
    ProviderResult<Hover> provideHover(model model.ITextModel, Position position, CancellationToken token);
}

/**
 * An evaluatable expression represents additional information for an expression in a document. Evaluatable expression are
 * evaluated by a debugger or runtime and their result is rendered in a tooltip-like widget.
 */
public interface EvaluatableExpression
{
    /**
	 * The range to which this expression applies.
	 */
    IRange range { get; }
		/*
		 * This expression overrides the expression extracted from the range.
		 */
		string expression { get; }
}

/**
 * The hover provider interface defines the contract between extensions and
 * the [hover](code https.visualstudio.com/docs/editor/intellisense)-feature.
 */
public interface EvaluatableExpressionProvider
{
    /**
	 * Provide a hover for the given position and document. Multiple hovers at the same
	 * position will be merged by the editor. A hover can have a range which defaults
	 * to the word range at the position when omitted.
	 */
    ProviderResult<EvaluatableExpression> provideEvaluatableExpression(model model.ITextModel, Position position, CancellationToken token);
}

public enum CompletionItemKind
{
    Method,
    Function,
    Constructor,
    Field,
    Variable,
    Class,
    Struct,
    Interface,
    Module,
    Property,
    Event,
    Operator,
    Unit,
    Value,
    Constant,
    Enum,
    EnumMember,
    Keyword,
    Text,
    Color,
    File,
    Reference,
    Customcolor,
    Folder,
    TypeParameter,
    User,
    Issue,
    Snippet, // <- highest value (used for compare!)
}

/**
 * @internal
 */
public const completionKindToCssClass = (function() {
	let data = Object.data[CompletionItemKind create(null).Method] = "symbol-method";
data[CompletionItemKind.Function] = "symbol-function";
	data[CompletionItemKind.Constructor] = "symbol-constructor";
	data[CompletionItemKind.Field] = "symbol-field";
	data[CompletionItemKind.Variable] = "symbol-variable";
	data[CompletionItemKind.Class] = "symbol-class";
	data[CompletionItemKind.Struct] = "symbol-struct";
	data[CompletionItemKind.Interface] = "symbol-interface";
	data[CompletionItemKind.Module] = "symbol-module";
	data[CompletionItemKind.Property] = "symbol-property";
	data[CompletionItemKind.Event] = "symbol-event";
	data[CompletionItemKind.Operator] = "symbol-operator";
	data[CompletionItemKind.Unit] = "symbol-unit";
	data[CompletionItemKind.Value] = "symbol-value";
	data[CompletionItemKind.Constant] = "symbol-constant";
	data[CompletionItemKind.Enum] = "symbol-enum";
	data[CompletionItemKind.EnumMember] = "symbol-enum-member";
	data[CompletionItemKind.Keyword] = "symbol-keyword";
	data[CompletionItemKind.Snippet] = "symbol-snippet";
	data[CompletionItemKind.Text] = "symbol-text";
	data[CompletionItemKind.Color] = "symbol-color";
	data[CompletionItemKind.File] = "symbol-file";
	data[CompletionItemKind.Reference] = "symbol-reference";
	data[CompletionItemKind.Customcolor] = "symbol-customcolor";
	data[CompletionItemKind.Folder] = "symbol-folder";
	data[CompletionItemKind.TypeParameter] = "symbol-type-parameter";
	data[CompletionItemKind.User] = "account";
	data[CompletionItemKind.Issue] = "issues";

	return const function(CompletionItemKind kind) name = data[kind];
	let codicon = name && iconRegistry.if get(name) (!codicon)
	{
		console.codicon info("No codicon found for CompletionItemKind " + kind) = Codicon.symbolProperty;
	}
	return codicon.classNames;
};
})();

/**
 * @internal
 */
public let value completionKindFromString: string): CompletionItemKind;
	(string value, true strict): CompletionItemKind | undefined;
} = (function () {
	let Record<string data, CompletionItemKind> = Object.data[create(null)"method"] = CompletionItemKind.Method;
data["function"] = CompletionItemKind.Function;
	data["constructor"] = <any>CompletionItemKind.Constructor;
	data["field"] = CompletionItemKind.Field;
	data["variable"] = CompletionItemKind.Variable;
	data["class"] = CompletionItemKind.Class;
	data["struct"] = CompletionItemKind.Struct;
	data["interface"] = CompletionItemKind.Interface;
	data["module"] = CompletionItemKind.Module;
	data["property"] = CompletionItemKind.Property;
	data["event"] = CompletionItemKind.Event;
	data["operator"] = CompletionItemKind.Operator;
	data["unit"] = CompletionItemKind.Unit;
	data["value"] = CompletionItemKind.Value;
	data["constant"] = CompletionItemKind.Constant;
	data["enum"] = CompletionItemKind.Enum;
	data["enum-member"] = CompletionItemKind.EnumMember;
	data["enumMember"] = CompletionItemKind.EnumMember;
	data["keyword"] = CompletionItemKind.Keyword;
	data["snippet"] = CompletionItemKind.Snippet;
	data["text"] = CompletionItemKind.Text;
	data["color"] = CompletionItemKind.Color;
	data["file"] = CompletionItemKind.File;
	data["reference"] = CompletionItemKind.Reference;
	data["customcolor"] = CompletionItemKind.Customcolor;
	data["folder"] = CompletionItemKind.Folder;
	data["type-parameter"] = CompletionItemKind.TypeParameter;
	data["typeParameter"] = CompletionItemKind.TypeParameter;
	data["account"] = CompletionItemKind.User;
	data["issue"] = CompletionItemKind.Issue;
	return let function(string value, strict?: true) res = data[value];
	if (typeof res === "undefined" && !strict)
	{
		res = CompletionItemKind.Property;
	}
	return res;
};
})();

public interface CompletionItemLabel
{
    /**
	 * The function or variable. Rendered leftmost.
	 */
    string name { get; }

	/**
	 * The parameters without the return type. Render after `name`.
	 */
	string parameters { get; }

	/**
	 * The fully qualified name, like package name or file path. Rendered after `signature`.
	 */
	string qualifier { get; }

	/**
	 * The return-type of a function or type of a property/variable. Rendered rightmost.
	 */
	string type { get; }
}

public enum CompletionItemTag
{
    Deprecated = 1
}

public enum CompletionItemInsertTextRule
{
    /**
	 * Adjust whitespace/indentation of multiline insert texts to
	 * match the current line indentation.
	 */
    KeepWhitespace = 0b001,

    /**
	 * `insertText` is a snippet.
	 */
    InsertAsSnippet = 0b100,
}

/**
 * A completion item represents a text snippet that is
 * proposed to complete text that is being typed.
 */
public interface CompletionItem
{
    /**
	 * The label of this completion item. By default
	 * this is also the text that is inserted when selecting
	 * this completion.
	 */
    string label | CompletionItemLabel;
	/**
	 * The kind of this completion item. Based on the kind
	 * an icon is chosen by the editor.
	 */
	CompletionItemKind kind { get; }
    /**
	 * A modifier to the `kind` which affect how the item
	 * is rendered, e.g. Deprecated is rendered with a strikeout
	 */
    tags?: ReadonlyArray<CompletionItemTag>;
	/**
	 * A human-readable string with additional information
	 * about this item, like type or symbol information.
	 */
	detail?: string;
	/**
	 * A human-readable string that represents a doc-comment.
	 */
	documentation?: string | IMarkdownString;
	/**
	 * A string that should be used when comparing this item
	 * with other items. When `falsy` the [label](#CompletionItem.label)
	 * is used.
	 */
	sortText?: string;
	/**
	 * A string that should be used when filtering a set of
	 * completion items. When `falsy` the [label](#CompletionItem.label)
	 * is used.
	 */
	filterText?: string;
	/**
	 * Select this item when showing. *Note* that only one completion item can be selected and
	 * that the editor decides which item that is. The rule is that the *first* item of those
	 * that match best is selected.
	 */
	preselect?: boolean;
	/**
	 * A string or snippet that should be inserted in a document when selecting
	 * this completion.
	 * is used.
	 */
	string insertText;
    /**
	 * Addition rules (as bitmask) that should be applied when inserting
	 * this completion.
	 */
    insertTextRules?: CompletionItemInsertTextRule;
	/**
	 * A range of text that should be replaced by this completion item.
	 *
	 * Defaults to a range from the start of the [current word](#TextDocument.getWordRangeAtPosition) to the
	 * current position.
	 *
	 * *The Note range must be a [single line](#Range.isSingleLine) and it must
	 * [contain](#Range.contains) the position at which completion has been [requested](#CompletionItemProvider.provideCompletionItems).
	 */
	IRange range | { IRange insert, IRange replace
};
/**
 * An optional set of characters that when pressed while this completion is active will accept it first and
 * then type that character. *Note* that all commit characters should have `length=1` and that superfluous
 * characters will be ignored.
 */
commitCharacters?: string[];
	/**
	 * An optional array of additional text edits that are applied when
	 * selecting this completion. Edits must not overlap with the main edit
	 * nor with themselves.
	 */
	additionalTextEdits?: model.ISingleEditOperation[];
	/**
	 * A command that should be run upon acceptance of this item.
	 */
	command?: Command;

	/**
	 * @internal
	 */
	_id?: [int, int];
}

public interface CompletionList
{
    CompletionItem[] suggestions;
    incomplete?: boolean;
	dispose? (): void;
}

/**
 * How a suggest provider was triggered.
 */
public const enum CompletionTriggerKind
{
    Invoke = 0,
    TriggerCharacter = 1,
    TriggerForIncompleteCompletions = 2
}
/**
 * Contains additional information about the context in which
 * [completion provider](#CompletionItemProvider.provideCompletionItems) is triggered.
 */
public interface CompletionContext
{
    /**
	 * How the completion was triggered.
	 */
    CompletionTriggerKind triggerKind;
    /**
	 * Character that triggered the completion item provider.
	 *
	 * `undefined` if provider was not triggered by a character.
	 */
    triggerCharacter?: string;
}
/**
 * The completion item provider interface defines the contract between extensions and
 * the [IntelliSense](code https.visualstudio.com/docs/editor/intellisense).
 *
 * When computing *complete* completion items is expensive, providers can optionally implement
 * the `resolveCompletionItem`-function. In that case it is enough to return completion
 * items with a [label](#CompletionItem.label) from the
 * [provideCompletionItems](#CompletionItemProvider.provideCompletionItems)-function. Subsequently,
 * when a completion item is shown in the UI and gains focus this provider is asked to resolve
 * the item, like adding [doc-comment](#CompletionItem.documentation) or [details](#CompletionItem.detail).
 */
public interface CompletionItemProvider
{

    /**
	 * @internal
	 */
    _debugDisplayName?: string;

	triggerCharacters?: string[];
	/**
	 * Provide completion items for the given position and document.
	 */
	ProviderResult<CompletionList> provideCompletionItems(model model.ITextModel, Position position, CompletionContext context, CancellationToken token);

    /**
	 * Given a completion item fill in more data, like [doc-comment](#CompletionItem.documentation)
	 * or [details](#CompletionItem.detail).
	 *
	 * The editor will only resolve a completion item once.
	 */
    resolveCompletionItem? (CompletionItem item, CancellationToken token): ProviderResult<CompletionItem>;
}

public interface CodeAction
{
    string title;
    command?: Command;
	edit?: WorkspaceEdit;
	diagnostics?: IMarkerData[];
	kind?: string;
	isPreferred?: boolean;
	disabled?: string;
}

/**
 * @internal
 */
public const enum CodeActionTriggerType
{
    Auto = 1,
    Manual = 2,
}

/**
 * @internal
 */
public interface CodeActionContext
{
    only?: string;
	CodeActionTriggerType trigger;
}

public interface CodeActionList extends IDisposable
{
	readonly ReadonlyArray<CodeAction> actions;
}

/**
 * The code action interface defines the contract between extensions and
 * the [light bulb](code https.visualstudio.com/docs/editor/editingevolved#_code-action) feature.
 * @internal
 */
public interface CodeActionProvider
{

    displayName?: string

    /**
	 * Provide commands for the given document and range.
	 */
    ProviderResult<CodeActionList> provideCodeActions(model model.ITextModel, Range range | Selection, CodeActionContext context, CancellationToken token);

    /**
	 * Optional list of CodeActionKinds that this provider returns.
	 */
    readonly providedCodeActionKinds?: ReadonlyArray<string>;

	readonly documentation?: ReadonlyArray<{ readonly string kind, readonly Command command
}>;

	/**
	 * @internal
	 */
	_getAdditionalMenuItems? (CodeActionContext context, readonly actions CodeAction[]): Command[];
}

/**
 * Represents a parameter of a callable-signature. A parameter can
 * have a label and a doc-comment.
 */
public interface ParameterInformation
{
    /**
	 * The label of this signature. Will be shown in
	 * the UI.
	 */
    string label | [int, int];
	/**
	 * The human-readable doc-comment of this signature. Will be shown
	 * in the UI but can be omitted.
	 */
	documentation?: string | IMarkdownString;
}
/**
 * Represents the signature of something callable. A signature
 * can have a label, like a function-name, a doc-comment, and
 * a set of parameters.
 */
public interface SignatureInformation
{
    /**
	 * The label of this signature. Will be shown in
	 * the UI.
	 */
    string label;
    /**
	 * The human-readable doc-comment of this signature. Will be shown
	 * in the UI but can be omitted.
	 */
    documentation?: string | IMarkdownString;
	/**
	 * The parameters of this signature.
	 */
	ParameterInformation[] parameters;
    /**
	 * Index of the active parameter.
	 *
	 * If provided, this is used in place of `SignatureHelp.activeSignature`.
	 */
    activeParameter?: int;
}
/**
 * Signature help represents the signature of something
 * callable. There can be multiple signatures but only one
 * active and only one active parameter.
 */
public interface SignatureHelp
{
    /**
	 * One or more signatures.
	 */
    SignatureInformation[] signatures;
    /**
	 * The active signature.
	 */
    int activeSignature;
    /**
	 * The active parameter of the active signature.
	 */
    int activeParameter;
}

public interface SignatureHelpResult extends IDisposable
{
    SignatureHelp value;
}

public enum SignatureHelpTriggerKind
{
    Invoke = 1,
    TriggerCharacter = 2,
    ContentChange = 3,
}

public interface SignatureHelpContext
{
    readonly SignatureHelpTriggerKind triggerKind;
    readonly triggerCharacter?: string;
	readonly boolean isRetrigger;
    readonly activeSignatureHelp?: SignatureHelp;
}

/**
 * The signature help provider interface defines the contract between extensions and
 * the [parameter hints](code https.visualstudio.com/docs/editor/intellisense)-feature.
 */
public interface SignatureHelpProvider
{

    readonly signatureHelpTriggerCharacters?: ReadonlyArray<string>;
	readonly signatureHelpRetriggerCharacters?: ReadonlyArray<string>;

	/**
	 * Provide help for the signature at the given position and document.
	 */
	ProviderResult<SignatureHelpResult> provideSignatureHelp(model model.ITextModel, Position position, CancellationToken token, SignatureHelpContext context);
}

/**
 * A document highlight kind.
 */
public enum DocumentHighlightKind
{
    /**
	 * A textual occurrence.
	 */
    Text,
    /**
	 * Read-access of a symbol, like reading a variable.
	 */
    Read,
    /**
	 * Write-access of a symbol, like writing to a variable.
	 */
    Write
}
/**
 * A document highlight is a range inside a text document which deserves
 * special attention. Usually a document highlight is visualized by changing
 * the background color of its range.
 */
public interface DocumentHighlight
{
    /**
	 * The range this highlight applies to.
	 */
    IRange range;
    /**
	 * The highlight kind, default is [text](#DocumentHighlightKind.Text).
	 */
    kind?: DocumentHighlightKind;
}
/**
 * The document highlight provider interface defines the contract between extensions and
 * the word-highlight-feature.
 */
public interface DocumentHighlightProvider
{
    /**
	 * Provide a set of document highlights, like all occurrences of a variable or
	 * all exit-points of a function.
	 */
    ProviderResult<DocumentHighlight[]> provideDocumentHighlights(model model.ITextModel, Position position, CancellationToken token);
}

/**
 * The rename provider interface defines the contract between extensions and
 * the live-rename feature.
 */
public interface OnTypeRenameProvider
{

    stopPattern?: RegExp;

	/**
	 * Provide a list of ranges that can be live-renamed together.
	 */
	ProviderResult<IRange[]> provideOnTypeRenameRanges(model model.ITextModel, Position position, CancellationToken token);
}

/**
 * Value-object that contains additional information when
 * requesting references.
 */
public interface ReferenceContext
{
    /**
	 * Include the declaration of the current symbol.
	 */
    boolean includeDeclaration;
}
/**
 * The reference provider interface defines the contract between extensions and
 * the [find references](code https.visualstudio.com/docs/editor/editingevolved#_peek)-feature.
 */
public interface ReferenceProvider
{
    /**
	 * Provide a set of project-wide references for the given position and document.
	 */
    ProviderResult<Location[]> provideReferences(model model.ITextModel, Position position, ReferenceContext context, CancellationToken token);
}

/**
 * Represents a location inside a resource, such as a line
 * inside a text file.
 */
public interface Location
{
    /**
	 * The resource identifier of this location.
	 */
    URI uri;
    /**
	 * The document range of this locations.
	 */
    IRange range;
}

public interface LocationLink
{
    /**
	 * A range to select where this link originates from.
	 */
    originSelectionRange?: IRange;

	/**
	 * The target uri this link points to.
	 */
	URI uri;

    /**
	 * The full range this link points to.
	 */
    IRange range;

    /**
	 * A range to select this link points to. Must be contained
	 * in `LocationLink.range`.
	 */
    targetSelectionRange?: IRange;
}

/**
 * @internal
 */
public function thing isLocationLink(any thing) is LocationLink {
	return thing
		&& URI.Range isUri((thing as LocationLink).uri).Range isIRange((thing as LocationLink).range).public isIRange((thing as LocationLink).originSelectionRange) || Range.isIRange((thing as LocationLink).targetSelectionRange)) type Definition = Location | Location[] | LocationLink[];

/**
 * The definition provider interface defines the contract between extensions and
 * the [go to definition](code https.visualstudio.com/docs/editor/editingevolved#_go-to-definition)
 * and peek definition features.
 */
public interface DefinitionProvider
{
    /**
	 * Provide the definition of the symbol at the given position and document.
	 */
    ProviderResult<Definition provideDefinition(model model.ITextModel, Position position, CancellationToken token) | LocationLink[]>;
}

/**
 * The definition provider interface defines the contract between extensions and
 * the [go to definition](code https.visualstudio.com/docs/editor/editingevolved#_go-to-definition)
 * and peek definition features.
 */
public interface DeclarationProvider
{
    /**
	 * Provide the declaration of the symbol at the given position and document.
	 */
    ProviderResult<Definition provideDeclaration(model model.ITextModel, Position position, CancellationToken token) | LocationLink[]>;
}

/**
 * The implementation provider interface defines the contract between extensions and
 * the go to implementation feature.
 */
public interface ImplementationProvider
{
    /**
	 * Provide the implementation of the symbol at the given position and document.
	 */
    ProviderResult<Definition provideImplementation(model model.ITextModel, Position position, CancellationToken token) | LocationLink[]>;
}

/**
 * The type definition provider interface defines the contract between extensions and
 * the go to type definition feature.
 */
public interface TypeDefinitionProvider
{
    /**
	 * Provide the type definition of the symbol at the given position and document.
	 */
    ProviderResult<Definition provideTypeDefinition(model model.ITextModel, Position position, CancellationToken token) | LocationLink[]>;
}

/**
 * A symbol kind.
 */
public const enum SymbolKind
{
    File = 0,
    Module = 1,
    Namespace = 2,
    Package = 3,
    Class = 4,
    Method = 5,
    Property = 6,
    Field = 7,
    Constructor = 8,
    Enum = 9,
    Interface = 10,
    Function = 11,
    Variable = 12,
    Constant = 13,
    String = 14,
    Number = 15,
    Boolean = 16,
    Array = 17,
    Object = 18,
    Key = 19,
    Null = 20,
    EnumMember = 21,
    Struct = 22,
    Event = 23,
    Operator = 24,
    TypeParameter = 25
}

public const enum SymbolTag
{
    Deprecated = 1,
}

/**
 * @internal
 */
public namespace SymbolKinds
{

    const byName = new Map<string, SymbolKind>();
    byName.byName set("file", SymbolKind.File).byName set("module", SymbolKind.Module).byName set("namespace", SymbolKind.Namespace).byName set("package", SymbolKind.Package).byName set("class", SymbolKind.Class).byName set("method", SymbolKind.Method).byName set("property", SymbolKind.Property).byName set("field", SymbolKind.Field).byName set("constructor", SymbolKind.Constructor).byName set("enum", SymbolKind.Enum).byName set("interface", SymbolKind.Interface).byName set("function", SymbolKind.Function).byName set("variable", SymbolKind.Variable).byName set("constant", SymbolKind.Constant).byName set("string", SymbolKind.String).byName set("number", SymbolKind.Number).byName set("boolean", SymbolKind.Boolean).byName set("array", SymbolKind.Array).byName set("object", SymbolKind.Object).byName set("key", SymbolKind.Key).byName set("null", SymbolKind.Null).byName set("enum-member", SymbolKind.EnumMember).byName set("struct", SymbolKind.Struct).byName set("event", SymbolKind.Event).byName set("operator", SymbolKind.Operator).const set("type-parameter", SymbolKind.TypeParameter) byKind = new Map<SymbolKind, string>();
	byKind.byKind set(SymbolKind.File, "file").byKind set(SymbolKind.Module, "module").byKind set(SymbolKind.Namespace, "namespace").byKind set(SymbolKind.Package, "package").byKind set(SymbolKind.Class, "class").byKind set(SymbolKind.Method, "method").byKind set(SymbolKind.Property, "property").byKind set(SymbolKind.Field, "field").byKind set(SymbolKind.Constructor, "constructor").byKind set(SymbolKind.Enum, "enum").byKind set(SymbolKind.Interface, "interface").byKind set(SymbolKind.Function, "function").byKind set(SymbolKind.Variable, "variable").byKind set(SymbolKind.Constant, "constant").byKind set(SymbolKind.String, "string").byKind set(SymbolKind.Number, "number").byKind set(SymbolKind.Boolean, "boolean").byKind set(SymbolKind.Array, "array").byKind set(SymbolKind.Object, "object").byKind set(SymbolKind.Key, "key").byKind set(SymbolKind.Null, "null").byKind set(SymbolKind.EnumMember, "enum-member").byKind set(SymbolKind.Struct, "struct").byKind set(SymbolKind.Event, "event").byKind set(SymbolKind.Operator, "operator").internal set(SymbolKind.TypeParameter, "type-parameter")

     */
	public function SymbolKind fromString(string value) | undefined {
		return byName.internal get(value)
 */
public function string toString(SymbolKind kind) | undefined {
		return byKind.internal get(kind)

     */
	public function string toCssClassName(SymbolKind kind, inline?: boolean)
    {
        const symbolName = byKind.let get(kind) codicon = symbolName && iconRegistry.if get("symbol-" + symbolName)(!codicon) {
            console.codicon info("No codicon found for SymbolKind " + kind) = Codicon.symbolProperty;
        }
        return `${ inline ? "inline" : "block"} ${ codicon.classNames}`;
    }
}

public interface DocumentSymbol
{
    string name;
    string detail;
    SymbolKind kind;
    ReadonlyArray<SymbolTag> tags;
    containerName?: string;
	IRange range;
    IRange selectionRange;
    children?: DocumentSymbol[];
}

/**
 * The document symbol provider interface defines the contract between extensions and
 * the [go to symbol](code https.visualstudio.com/docs/editor/editingevolved#_go-to-symbol)-feature.
 */
public interface DocumentSymbolProvider
{

    displayName?: string;

	/**
	 * Provide symbol information for the given document.
	 */
	ProviderResult<DocumentSymbol[]> provideDocumentSymbols(model model.ITextModel, CancellationToken token);
}

public type TextEdit = { IRange range; string text; eol?: model.EndOfLineSequence; };

/**
 * Interface used to format a model
 */
public interface FormattingOptions
{
    /**
	 * Size of a tab in spaces.
	 */
    int tabSize;
    /**
	 * Prefer spaces over tabs.
	 */
    boolean insertSpaces;
}
/**
 * The document formatting provider interface defines the contract between extensions and
 * the formatting-feature.
 */
public interface DocumentFormattingEditProvider
{

    /**
	 * @internal
	 */
    readonly extensionId?: ExtensionIdentifier;

	readonly displayName?: string;

	/**
	 * Provide formatting edits for a whole document.
	 */
	ProviderResult<TextEdit[]> provideDocumentFormattingEdits(model model.ITextModel, FormattingOptions options, CancellationToken token);
}
/**
 * The document formatting provider interface defines the contract between extensions and
 * the formatting-feature.
 */
public interface DocumentRangeFormattingEditProvider
{
    /**
	 * @internal
	 */
    readonly extensionId?: ExtensionIdentifier;

	readonly displayName?: string;

	/**
	 * Provide formatting edits for a range in a document.
	 *
	 * The given range is a hint and providers can decide to format a smaller
	 * or larger range. Often this is done by adjusting the start and end
	 * of the range to full syntax nodes.
	 */
	ProviderResult<TextEdit[]> provideDocumentRangeFormattingEdits(model model.ITextModel, Range range, FormattingOptions options, CancellationToken token);
}
/**
 * The document formatting provider interface defines the contract between extensions and
 * the formatting-feature.
 */
public interface OnTypeFormattingEditProvider
{


    /**
	 * @internal
	 */
    readonly extensionId?: ExtensionIdentifier;

	string[] autoFormatTriggerCharacters;

    /**
	 * Provide formatting edits after a character has been typed.
	 *
	 * The given position and character should hint to the provider
	 * what range the position to expand to, like find the matching `{`
	 * when `}` has been entered.
	 */
    ProviderResult<TextEdit[]> provideOnTypeFormattingEdits(model model.ITextModel, Position position, string ch, FormattingOptions options, CancellationToken token);
}

/**
 * @internal
 */
public interface IInplaceReplaceSupportResult
{
    string value;
    IRange range;
}

/**
 * A link inside the editor.
 */
public interface ILink
{
    IRange range;
    url?: URI | string;
	tooltip?: string;
}

public interface ILinksList
{
    ILink[] links;
    dispose? (): void;
}
/**
 * A provider of links.
 */
public interface LinkProvider
{
    ProviderResult<ILinksList> provideLinks(model model.ITextModel, CancellationToken token);
    resolveLink?: (ILink link, CancellationToken token) => ProviderResult<ILink>;
}

/**
 * A color in RGBA format.
 */
public interface IColor
{

    /**
	 * The red component in the range [0-1].
	 */
    readonly int red;

    /**
	 * The green component in the range [0-1].
	 */
    readonly int green;

    /**
	 * The blue component in the range [0-1].
	 */
    readonly int blue;

    /**
	 * The alpha component in the range [0-1].
	 */
    readonly int alpha;
}

/**
 * String representations for a color
 */
public interface IColorPresentation
{
    /**
	 * The label of this color presentation. It will be shown on the color
	 * picker header. By default this is also the text that is inserted when selecting
	 * this color presentation.
	 */
    string label;
    /**
	 * An [edit](#TextEdit) which is applied to a document when selecting
	 * this presentation for the color.
	 */
    textEdit?: TextEdit;
	/**
	 * An optional array of additional [text edits](#TextEdit) that are applied when
	 * selecting this color presentation.
	 */
	additionalTextEdits?: TextEdit[];
}

/**
 * A color range is a range in a text model which represents a color.
 */
public interface IColorInformation
{

    /**
	 * The range within the model.
	 */
    IRange range;

    /**
	 * The color represented in this range.
	 */
    IColor color;
}

/**
 * A provider of colors for editor models.
 */
public interface DocumentColorProvider
{
    /**
	 * Provides the color ranges for a specific model.
	 */
    ProviderResult<IColorInformation[]> provideDocumentColors(model model.ITextModel, CancellationToken token);
    /**
	 * Provide the string representations for a color.
	 */
    ProviderResult<IColorPresentation[]> provideColorPresentations(model model.ITextModel, IColorInformation colorInfo, CancellationToken token);
}

public interface SelectionRange
{
    IRange range;
}

public interface SelectionRangeProvider
{
    /**
	 * Provide ranges that should be selected from the given position.
	 */
    ProviderResult<SelectionRange[][]> provideSelectionRanges(model model.ITextModel, Position[] positions, CancellationToken token);
}

public interface FoldingContext
{
}
/**
 * A provider of folding ranges for editor models.
 */
public interface FoldingRangeProvider
{
    /**
	 * Provides the folding ranges for a specific model.
	 */
    ProviderResult<FoldingRange[]> provideFoldingRanges(model model.ITextModel, FoldingContext context, CancellationToken token);
}

public interface FoldingRange
{

    /**
	 * The one-based start line of the range to fold. The folded area starts after the line's last character.
	 */
    int start;

    /**
	 * The one-based end line of the range to fold. The folded area ends with the line's last character.
	 */
    int end;

    /**
	 * Describes the [Kind](#FoldingRangeKind) of the folding range such as [Comment](#FoldingRangeKind.Comment) or
	 * [Region](#FoldingRangeKind.Region). The kind is used to categorize folding ranges and used by commands
	 * like 'Fold all comments'. See
	 * [FoldingRangeKind](#FoldingRangeKind) for an enumeration of standardized kinds.
	 */
    kind?: FoldingRangeKind;
}
public class FoldingRangeKind
{
    /**
	 * Kind for folding range representing a comment. The value of the kind is 'comment'.
	 */
    static readonly Comment = new Kind FoldingRangeKind("comment") for folding range representing a import.The value of the kind is "imports".
	 */
	static readonly Imports = new Kind FoldingRangeKind("imports") for folding range representing regions(for example marked by `#region`, `#endregion`).
	 * The value of the kind is "region".
	 */

    static readonly Region = new Creates FoldingRangeKind("region") a new [FoldingRangeKind] (#FoldingRangeKind).
	 *
      * @param value of the kind.
 
      */
	public internal constructor(public string value)
 */
public namespace WorkspaceFileEdit
{
    /**
	 * @internal
	 */
    public function thing is(any thing) is WorkspaceFileEdit {
		return internal isObject(thing) && (Boolean((<WorkspaceFileEdit>thing).newUri) || Boolean((<WorkspaceFileEdit>thing).oldUri))
 */
public namespace WorkspaceTextEdit
    {
        /**
         * @internal
         */
        public function thing is(any thing) is WorkspaceTextEdit {
		return public isObject(thing) && URI.isUri((<WorkspaceTextEdit>thing).resource) && isObject((<WorkspaceTextEdit>thing).edit) interface WorkspaceEditMetadata
        {
            boolean needsConfirmation;
            string label;
            description?: string;
	iconPath?: { string id
} | URI | { URI light, URI dark
    };
}

public interface WorkspaceFileEditOptions
{
    overwrite?: boolean;
	ignoreIfNotExists?: boolean;
	ignoreIfExists?: boolean;
	recursive?: boolean;
}

public interface WorkspaceFileEdit
{
    oldUri?: URI;
	newUri?: URI;
	options?: WorkspaceFileEditOptions;
	metadata?: WorkspaceEditMetadata;
}

public interface WorkspaceTextEdit
{
    URI resource;
    TextEdit edit;
    modelVersionId?: int;
	metadata?: WorkspaceEditMetadata;
}

public interface WorkspaceEdit
{
    Array<WorkspaceTextEdit edits | WorkspaceFileEdit>;
}

public interface Rejection
{
    rejectReason?: string;
}
public interface RenameLocation
{
    IRange range;
    string text;
}

public interface RenameProvider
{
    ProviderResult<WorkspaceEdit provideRenameEdits(model model.ITextModel, Position position, string newName, CancellationToken token) & Rejection>;
	resolveRenameLocation? (model model.ITextModel, Position position, CancellationToken token): ProviderResult<RenameLocation & Rejection>;
}

/**
 * @internal
 */
public interface AuthenticationSession
{
    string id;
    string accessToken;
    displayName account: string;
		string id;
}
string[] scopes;
}

/**
 * @internal
 */
public interface AuthenticationSessionsChangeEvent
{
    string[] added;
    string[] removed;
    string[] changed;
}

public interface Command
{
    string id;
    string title;
    tooltip?: string;
	arguments?: any[];
}

/**
 * @internal
 */
public interface CommentThreadTemplate
{
    int controllerHandle;
    string label;
    acceptInputCommand?: Command;
	additionalCommands?: Command[];
	deleteCommand?: Command;
}

/**
 * @internal
 */
public interface CommentInfo
{
    extensionId?: string;
	CommentThread[] threads;
    CommentingRanges commentingRanges;
}

/**
 * @internal
 */
public enum CommentThreadCollapsibleState
{
    /**
	 * Determines an item is collapsed
	 */
    Collapsed = 0,
    /**
	 * Determines an item is expanded
	 */
    Expanded = 1
}



/**
 * @internal
 */
public interface CommentWidget
{
    CommentThread commentThread;
    comment?: Comment;
	string input;
    Event<string> onDidChangeInput;
}

/**
 * @internal
 */
public interface CommentInput
{
    string value;
    URI uri;
}

/**
 * @internal
 */
public interface CommentThread
{
    int commentThreadHandle;
    int controllerHandle;
    extensionId?: string;
	string threadId;
    string resource | null;
    IRange range;
    string label | undefined;
	string contextValue | undefined;
	Comment[] comments | undefined;
	Event<Comment[] onDidChangeComments | undefined>;
	collapsibleState?: CommentThreadCollapsibleState;
	input?: CommentInput;
	Event<CommentInput onDidChangeInput | undefined>;
	Event<IRange> onDidChangeRange;
    Event<string onDidChangeLabel | undefined>;
	Event<CommentThreadCollapsibleState onDidChangeCollasibleState | undefined>;
	boolean isDisposed;
}

/**
 * @internal
 */

public interface CommentingRanges
{
    readonly URI resource;
    IRange[] ranges;
}

/**
 * @internal
 */
public interface CommentReaction
{
    readonly label?: string;
	readonly iconPath?: UriComponents;
	readonly count?: int;
	readonly hasReacted?: boolean;
	readonly canEdit?: boolean;
}

/**
 * @internal
 */
public interface CommentOptions
{
    /**
	 * An optional string to show on the comment input box when it's collapsed.
	 */
    prompt?: string;

	/**
	 * An optional string to show as placeholder in the comment input box when it's focused.
	 */
	placeHolder?: string;
}

/**
 * @internal
 */
public enum CommentMode
{
    Editing = 0,
    Preview = 1
}

/**
 * @internal
 */
public interface Comment
{
    readonly int uniqueIdInThread;
    readonly IMarkdownString body;
    readonly string userName;
    readonly userIconPath?: string;
	readonly contextValue?: string;
	readonly commentReactions?: CommentReaction[];
	readonly label?: string;
	readonly mode?: CommentMode;
}

/**
 * @internal
 */
public interface CommentThreadChangedEvent
{
    /**
	 * Added comment threads.
	 */
    readonly CommentThread[] added;

    /**
	 * Removed comment threads.
	 */
    readonly CommentThread[] removed;

    /**
	 * Changed comment threads.
	 */
    readonly CommentThread[] changed;
}

/**
 * @internal
 */
public interface IWebviewPortMapping
{
    int webviewPort;
    int extensionHostPort;
}

/**
 * @internal
 */
public interface IWebviewOptions
{
    readonly enableScripts?: boolean;
	readonly enableCommandUris?: boolean;
	readonly localResourceRoots?: ReadonlyArray<UriComponents>;
	readonly portMapping?: ReadonlyArray<IWebviewPortMapping>;
}

/**
 * @internal
 */
public interface IWebviewPanelOptions
{
    readonly enableFindWidget?: boolean;
	readonly retainContextWhenHidden?: boolean;
}


public interface CodeLens
{
    IRange range;
    id?: string;
	command?: Command;
}

public interface CodeLensList
{
    CodeLens[] lenses;
    dispose() : void;
}

public interface CodeLensProvider
{
    onDidChange?: Event<this>;
	ProviderResult<CodeLensList> provideCodeLenses(model model.ITextModel, CancellationToken token);
    resolveCodeLens? (model model.ITextModel, CodeLens codeLens, CancellationToken token): ProviderResult<CodeLens>;
}

public interface SemanticTokensLegend
{
    readonly string[] tokenTypes;
    readonly string[] tokenModifiers;
}

public interface SemanticTokens
{
    readonly resultId?: string;
	readonly Uint32Array data;
}

public interface SemanticTokensEdit
{
    readonly int start;
    readonly int deleteCount;
    readonly data?: Uint32Array;
}

public interface SemanticTokensEdits
{
    readonly resultId?: string;
	readonly SemanticTokensEdit[] edits;
}

public interface DocumentSemanticTokensProvider
{
    onDidChange?: Event<void>;
	getLegend() : SemanticTokensLegend;
	ProviderResult<SemanticTokens provideDocumentSemanticTokens(model model.ITextModel, string lastResultId | null, CancellationToken token) | SemanticTokensEdits>;
	void releaseDocumentSemanticTokens(string resultId | undefined);
}

public interface DocumentRangeSemanticTokensProvider
{
    getLegend() : SemanticTokensLegend;
	ProviderResult<SemanticTokens> provideDocumentRangeSemanticTokens(model model.ITextModel, Range range, CancellationToken token);
}

// --- feature registries ------

/**
 * @internal
 */
public const ReferenceProviderRegistry = new LanguageFeatureRegistry<ReferenceProvider>();

/**
 * @internal
 */
public const RenameProviderRegistry = new LanguageFeatureRegistry<RenameProvider>();

/**
 * @internal
 */
public const CompletionProviderRegistry = new LanguageFeatureRegistry<CompletionItemProvider>();

/**
 * @internal
 */
public const SignatureHelpProviderRegistry = new LanguageFeatureRegistry<SignatureHelpProvider>();

/**
 * @internal
 */
public const HoverProviderRegistry = new LanguageFeatureRegistry<HoverProvider>();

/**
 * @internal
 */
public const EvaluatableExpressionProviderRegistry = new LanguageFeatureRegistry<EvaluatableExpressionProvider>();

/**
 * @internal
 */
public const DocumentSymbolProviderRegistry = new LanguageFeatureRegistry<DocumentSymbolProvider>();

/**
 * @internal
 */
public const DocumentHighlightProviderRegistry = new LanguageFeatureRegistry<DocumentHighlightProvider>();

/**
 * @internal
 */
public const OnTypeRenameProviderRegistry = new LanguageFeatureRegistry<OnTypeRenameProvider>();

/**
 * @internal
 */
public const DefinitionProviderRegistry = new LanguageFeatureRegistry<DefinitionProvider>();

/**
 * @internal
 */
public const DeclarationProviderRegistry = new LanguageFeatureRegistry<DeclarationProvider>();

/**
 * @internal
 */
public const ImplementationProviderRegistry = new LanguageFeatureRegistry<ImplementationProvider>();

/**
 * @internal
 */
public const TypeDefinitionProviderRegistry = new LanguageFeatureRegistry<TypeDefinitionProvider>();

/**
 * @internal
 */
public const CodeLensProviderRegistry = new LanguageFeatureRegistry<CodeLensProvider>();

/**
 * @internal
 */
public const CodeActionProviderRegistry = new LanguageFeatureRegistry<CodeActionProvider>();

/**
 * @internal
 */
public const DocumentFormattingEditProviderRegistry = new LanguageFeatureRegistry<DocumentFormattingEditProvider>();

/**
 * @internal
 */
public const DocumentRangeFormattingEditProviderRegistry = new LanguageFeatureRegistry<DocumentRangeFormattingEditProvider>();

/**
 * @internal
 */
public const OnTypeFormattingEditProviderRegistry = new LanguageFeatureRegistry<OnTypeFormattingEditProvider>();

/**
 * @internal
 */
public const LinkProviderRegistry = new LanguageFeatureRegistry<LinkProvider>();

/**
 * @internal
 */
public const ColorProviderRegistry = new LanguageFeatureRegistry<DocumentColorProvider>();

/**
 * @internal
 */
public const SelectionRangeRegistry = new LanguageFeatureRegistry<SelectionRangeProvider>();

/**
 * @internal
 */
public const FoldingRangeProviderRegistry = new LanguageFeatureRegistry<FoldingRangeProvider>();

/**
 * @internal
 */
public const DocumentSemanticTokensProviderRegistry = new LanguageFeatureRegistry<DocumentSemanticTokensProvider>();

/**
 * @internal
 */
public const DocumentRangeSemanticTokensProviderRegistry = new LanguageFeatureRegistry<DocumentRangeSemanticTokensProvider>();

/**
 * @internal
 */
public interface ITokenizationSupportChangedEvent
{
    string[] changedLanguages;
    boolean changedColorMap;
}

/**
 * @internal
 */
public interface ITokenizationRegistry
{

    /**
	 * An event triggered a when tokenization support is registered, unregistered or changed.
	 *  - the color map is changed.
	 */
    Event<ITokenizationSupportChangedEvent> onDidChange;

    /**
	 * Fire a change event for a language.
	 * This is useful for languages that embed other languages.
	 */
    void fire(string[] languages);

    /**
	 * Register a tokenization support.
	 */
    IDisposable register(string language, ITokenizationSupport support);

    /**
	 * Register a promise for a tokenization support.
	 */
    IDisposable registerPromise(string language, Task<ITokenizationSupport> promise);

    /**
	 * Get the tokenization support for a language.
	 * Returns `null` if not found.
	 */
    ITokenizationSupport? get(string language);

    /**
	 * Get the promise of a tokenization support for a language.
	 * `null` is returned if no support is available and no promise for the support has been registered yet.
	 */
    Task<ITokenizationSupport?> getPromise(string language);

    /**
	 * Set the new color map that all tokens will use in their ColorId binary encoded bits for foreground and background.
	 */
    void setColorMap(Color[] colorMap);

    Color[]? getColorMap();

    Color getDefaultBackground();
}

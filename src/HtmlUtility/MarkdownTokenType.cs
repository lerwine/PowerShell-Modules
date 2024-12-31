namespace HtmlUtility;

public enum MarkdownTokenType
{
    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.MarkdownObject"/>, except for <see cref="Markdig.Renderers.Html.HtmlAttributes"/>.
    /// </summary>
    /// <remarks>
    /// Includes all enum types except: <see cref="HtmlAttributes"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.MarkdownObject))]
    Any,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.Block"/>.
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="Abbreviation"/>, <see cref="BlankLineBlock"/>, <see cref="CodeBlock"/>, <see cref="ContainerBlock"/>, <see cref="CustomContainer"/>, <see cref="DefinitionItem"/>, <see cref="DefinitionList"/>, <see cref="DefinitionTerm"/>,
    ///     <see cref="EmptyBlock"/>, <see cref="FencedCodeBlock"/>, <see cref="Figure"/>, <see cref="FigureCaption"/>, <see cref="FooterBlock"/>, <see cref="Footnote"/>, <see cref="FootnoteGroup"/>, <see cref="FootnoteLinkReferenceDefinition"/>,
    ///     <see cref="HeadingBlock"/>, <see cref="HeadingLinkReferenceDefinition"/>, <see cref="HtmlBlock"/>, <see cref="LeafBlock"/>, <see cref="LinkReferenceDefinition"/>, <see cref="LinkReferenceDefinitionGroup"/>, <see cref="ListBlock"/>,
    ///     <see cref="ListItemBlock"/>, <see cref="MarkdownDocument"/>, <see cref="MathBlock"/>, <see cref="ParagraphBlock"/>, <see cref="QuoteBlock"/>, <see cref="Table"/>, <see cref="TableCell"/>, <see cref="TableRow"/>,
    ///     <see cref="ThematicBreakBlock"/>, <see cref="YamlFrontMatterBlock"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Block))]
    Block,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.BlankLineBlock"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.BlankLineBlock))]
    BlankLineBlock,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.ContainerBlock"/>.
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="CustomContainer"/>, <see cref="DefinitionItem"/>, <see cref="DefinitionList"/>, <see cref="Figure"/>, <see cref="FooterBlock"/>, <see cref="Footnote"/>, <see cref="FootnoteGroup"/>,
    ///     <see cref="LinkReferenceDefinitionGroup"/>, <see cref="ListBlock"/>, <see cref="ListItemBlock"/>, <see cref="MarkdownDocument"/>, <see cref="QuoteBlock"/>, <see cref="Table"/>, <see cref="TableCell"/>, <see cref="TableRow"/>
    /// Included by: <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.ContainerBlock))]
    ContainerBlock,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.CustomContainers.CustomContainer"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.CustomContainers.CustomContainer))]
    CustomContainer,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.DefinitionLists.DefinitionItem"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.DefinitionLists.DefinitionItem))]
    DefinitionItem,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.DefinitionLists.DefinitionList"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.DefinitionLists.DefinitionList))]
    DefinitionList,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.Figures.Figure"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Figures.Figure))]
    Figure,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.Footers.FooterBlock"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Footers.FooterBlock))]
    FooterBlock,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.Footnotes.Footnote"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Footnotes.Footnote))]
    Footnote,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.Footnotes.FootnoteGroup"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Footnotes.FootnoteGroup))]
    FootnoteGroup,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.LinkReferenceDefinitionGroup"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.LinkReferenceDefinitionGroup))]
    LinkReferenceDefinitionGroup,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.ListBlock"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.ListBlock))]
    ListBlock,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.ListItemBlock"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.ListItemBlock))]
    ListItemBlock,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.MarkdownDocument"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.MarkdownDocument))]
    MarkdownDocument,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.QuoteBlock"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.QuoteBlock))]
    QuoteBlock,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.Tables.Table"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Tables.Table))]
    Table,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.Tables.TableCell"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Tables.TableCell))]
    TableCell,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.Tables.TableRow"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Tables.TableRow))]
    TableRow,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.LeafBlock"/>.
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="Abbreviation"/>, <see cref="CodeBlock"/>, <see cref="DefinitionTerm"/>, <see cref="EmptyBlock"/>, <see cref="FencedCodeBlock"/>, <see cref="FigureCaption"/>, <see cref="FootnoteLinkReferenceDefinition"/>,
    ///     <see cref="HeadingBlock"/>, <see cref="HeadingLinkReferenceDefinition"/>, <see cref="HtmlBlock"/>, <see cref="LinkReferenceDefinition"/>, <see cref="MathBlock"/>, <see cref="ParagraphBlock"/>, <see cref="ThematicBreakBlock"/>,
    ///     <see cref="YamlFrontMatterBlock"/>
    /// Included by: <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.LeafBlock))]
    LeafBlock,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.Abbreviations.Abbreviation"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Abbreviations.Abbreviation))]
    Abbreviation,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.CodeBlock"/>.
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="FencedCodeBlock"/>, <see cref="MathBlock"/>, <see cref="YamlFrontMatterBlock"/>
    /// Included by: <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.CodeBlock))]
    CodeBlock,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.FencedCodeBlock"/>.
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="MathBlock"/>
    /// Included by: <see cref="CodeBlock"/>, <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.FencedCodeBlock))]
    FencedCodeBlock,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.Mathematics.MathBlock"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="FencedCodeBlock"/>, <see cref="CodeBlock"/>, <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Mathematics.MathBlock))]
    MathBlock,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.Yaml.YamlFrontMatterBlock"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="CodeBlock"/>, <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Yaml.YamlFrontMatterBlock))]
    YamlFrontMatterBlock,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.DefinitionLists.DefinitionTerm"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.DefinitionLists.DefinitionTerm))]
    DefinitionTerm,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.EmptyBlock"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.EmptyBlock))]
    EmptyBlock,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.Figures.FigureCaption"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Figures.FigureCaption))]
    FigureCaption,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.HeadingBlock"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.HeadingBlock))]
    HeadingBlock,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.HtmlBlock"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.HtmlBlock))]
    HtmlBlock,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.LinkReferenceDefinition"/>.
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="FootnoteLinkReferenceDefinition"/>, <see cref="HeadingLinkReferenceDefinition"/>
    /// Included by: <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.LinkReferenceDefinition))]
    LinkReferenceDefinition,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.Footnotes.FootnoteLinkReferenceDefinition"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LinkReferenceDefinition"/>, <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Footnotes.FootnoteLinkReferenceDefinition))]
    FootnoteLinkReferenceDefinition,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LinkReferenceDefinition"/>, <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition))]
    HeadingLinkReferenceDefinition,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.ParagraphBlock"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.ParagraphBlock))]
    ParagraphBlock,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.ThematicBreakBlock"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.ThematicBreakBlock))]
    ThematicBreakBlock,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.Inlines.Inline"/>.
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="AbbreviationInline"/>, <see cref="AutolinkInline"/>, <see cref="CodeInline"/>, <see cref="ContainerInline"/>, <see cref="CustomContainerInline"/>, <see cref="DelimiterInline"/>, <see cref="EmojiInline"/>,
    ///     <see cref="EmphasisDelimiterInline"/>, <see cref="EmphasisInline"/>, <see cref="FootnoteLink"/>, <see cref="HtmlEntityInline"/>, <see cref="HtmlInline"/>, <see cref="JiraLink"/>, <see cref="LeafInline"/>, <see cref="LineBreakInline"/>,
    ///     <see cref="LinkDelimiterInline"/>, <see cref="LinkInline"/>, <see cref="LiteralInline"/>, <see cref="MathInline"/>, <see cref="PipeTableDelimiterInline"/>, <see cref="SmartyPant"/>, <see cref="TaskList"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.Inline))]
    Inline,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.Inlines.ContainerInline"/>.
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="CustomContainerInline"/>, <see cref="DelimiterInline"/>, <see cref="EmphasisDelimiterInline"/>, <see cref="EmphasisInline"/>, <see cref="JiraLink"/>, <see cref="LinkDelimiterInline"/>, <see cref="LinkInline"/>,
    ///     <see cref="PipeTableDelimiterInline"/>
    /// Included by: <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.ContainerInline))]
    ContainerInline,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.Footnotes.FootnoteLink"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Footnotes.FootnoteLink))]
    FootnoteLink,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.Inlines.LeafInline"/>.
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="AbbreviationInline"/>, <see cref="AutolinkInline"/>, <see cref="CodeInline"/>, <see cref="EmojiInline"/>, <see cref="HtmlEntityInline"/>, <see cref="HtmlInline"/>, <see cref="LineBreakInline"/>, <see cref="LiteralInline"/>,
    ///     <see cref="MathInline"/>, <see cref="SmartyPant"/>, <see cref="TaskList"/>
    /// Included by: <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.LeafInline))]
    LeafInline,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.Abbreviations.AbbreviationInline"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Abbreviations.AbbreviationInline))]
    AbbreviationInline,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.Inlines.AutolinkInline"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.AutolinkInline))]
    AutolinkInline,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.Inlines.CodeInline"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.CodeInline))]
    CodeInline,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.Inlines.HtmlEntityInline"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.HtmlEntityInline))]
    HtmlEntityInline,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.Inlines.HtmlInline"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.HtmlInline))]
    HtmlInline,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.Inlines.LineBreakInline"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.LineBreakInline))]
    LineBreakInline,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.Inlines.LiteralInline"/>.
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="EmojiInline"/>
    /// Included by: <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.LiteralInline))]
    LiteralInline,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.Emoji.EmojiInline"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LiteralInline"/>, <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Emoji.EmojiInline))]
    EmojiInline,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.Mathematics.MathInline"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Mathematics.MathInline))]
    MathInline,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.SmartyPants.SmartyPant"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.SmartyPants.SmartyPant))]
    SmartyPant,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.TaskLists.TaskList"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.TaskLists.TaskList))]
    TaskList,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.Inlines.DelimiterInline"/>.
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="EmphasisDelimiterInline"/>, <see cref="LinkDelimiterInline"/>, <see cref="PipeTableDelimiterInline"/>
    /// Included by: <see cref="ContainerInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.DelimiterInline))]
    DelimiterInline,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.Inlines.EmphasisDelimiterInline"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="DelimiterInline"/>, <see cref="ContainerInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.EmphasisDelimiterInline))]
    EmphasisDelimiterInline,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.Inlines.LinkDelimiterInline"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="DelimiterInline"/>, <see cref="ContainerInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.LinkDelimiterInline))]
    LinkDelimiterInline,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.Tables.PipeTableDelimiterInline"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="DelimiterInline"/>, <see cref="ContainerInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Tables.PipeTableDelimiterInline))]
    PipeTableDelimiterInline,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.Inlines.EmphasisInline"/>.
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="CustomContainerInline"/>
    /// Included by: <see cref="ContainerInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.EmphasisInline))]
    EmphasisInline,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.CustomContainers.CustomContainerInline"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="EmphasisInline"/>, <see cref="ContainerInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.CustomContainers.CustomContainerInline))]
    CustomContainerInline,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Renderers.Html.HtmlAttributes"/>.
    /// </summary>
    [ReflectionType(typeof(Markdig.Renderers.Html.HtmlAttributes))]
    HtmlAttributes,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Syntax.Inlines.LinkInline"/>.
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="JiraLink"/>
    /// Included by: <see cref="ContainerInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.LinkInline))]
    LinkInline,

    /// <summary>
    /// Matches objects of type <see cref="Markdig.Extensions.JiraLinks.JiraLink"/>.
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LinkInline"/>, <see cref="ContainerInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.JiraLinks.JiraLink))]
    JiraLink
}

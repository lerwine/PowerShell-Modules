namespace HtmlUtility;

public enum MarkdownTokenType
{
    /// <summary>
    /// Objects of type Markdig.Syntax.Block
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
    /// Objects of type Markdig.Syntax.BlankLineBlock
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.BlankLineBlock))]
    BlankLineBlock,

    /// <summary>
    /// Objects of type Markdig.Syntax.ContainerBlock
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="CustomContainer"/>, <see cref="DefinitionItem"/>, <see cref="DefinitionList"/>, <see cref="Figure"/>, <see cref="FooterBlock"/>, <see cref="Footnote"/>, <see cref="FootnoteGroup"/>,
    ///     <see cref="LinkReferenceDefinitionGroup"/>, <see cref="ListBlock"/>, <see cref="ListItemBlock"/>, <see cref="MarkdownDocument"/>, <see cref="QuoteBlock"/>, <see cref="Table"/>, <see cref="TableCell"/>, <see cref="TableRow"/>
    /// Included by: <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.ContainerBlock))]
    ContainerBlock,

    /// <summary>
    /// Objects of type Markdig.Extensions.CustomContainers.CustomContainer
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.CustomContainers.CustomContainer))]
    CustomContainer,

    /// <summary>
    /// Objects of type Markdig.Extensions.DefinitionLists.DefinitionItem
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.DefinitionLists.DefinitionItem))]
    DefinitionItem,

    /// <summary>
    /// Objects of type Markdig.Extensions.DefinitionLists.DefinitionList
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.DefinitionLists.DefinitionList))]
    DefinitionList,

    /// <summary>
    /// Objects of type Markdig.Extensions.Figures.Figure
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Figures.Figure))]
    Figure,

    /// <summary>
    /// Objects of type Markdig.Extensions.Footers.FooterBlock
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Footers.FooterBlock))]
    FooterBlock,

    /// <summary>
    /// Objects of type Markdig.Extensions.Footnotes.Footnote
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Footnotes.Footnote))]
    Footnote,

    /// <summary>
    /// Objects of type Markdig.Extensions.Footnotes.FootnoteGroup
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Footnotes.FootnoteGroup))]
    FootnoteGroup,

    /// <summary>
    /// Objects of type Markdig.Syntax.LinkReferenceDefinitionGroup
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.LinkReferenceDefinitionGroup))]
    LinkReferenceDefinitionGroup,

    /// <summary>
    /// Objects of type Markdig.Syntax.ListBlock
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.ListBlock))]
    ListBlock,

    /// <summary>
    /// Objects of type Markdig.Syntax.ListItemBlock
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.ListItemBlock))]
    ListItemBlock,

    /// <summary>
    /// Objects of type Markdig.Syntax.MarkdownDocument
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.MarkdownDocument))]
    MarkdownDocument,

    /// <summary>
    /// Objects of type Markdig.Syntax.QuoteBlock
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.QuoteBlock))]
    QuoteBlock,

    /// <summary>
    /// Objects of type Markdig.Extensions.Tables.Table
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Tables.Table))]
    Table,

    /// <summary>
    /// Objects of type Markdig.Extensions.Tables.TableCell
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Tables.TableCell))]
    TableCell,

    /// <summary>
    /// Objects of type Markdig.Extensions.Tables.TableRow
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="ContainerBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Tables.TableRow))]
    TableRow,

    /// <summary>
    /// Objects of type Markdig.Syntax.LeafBlock
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
    /// Objects of type Markdig.Extensions.Abbreviations.Abbreviation
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Abbreviations.Abbreviation))]
    Abbreviation,

    /// <summary>
    /// Objects of type Markdig.Syntax.CodeBlock
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="FencedCodeBlock"/>, <see cref="MathBlock"/>, <see cref="YamlFrontMatterBlock"/>
    /// Included by: <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.CodeBlock))]
    CodeBlock,

    /// <summary>
    /// Objects of type Markdig.Syntax.FencedCodeBlock
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="MathBlock"/>
    /// Included by: <see cref="CodeBlock"/>, <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.FencedCodeBlock))]
    FencedCodeBlock,

    /// <summary>
    /// Objects of type Markdig.Extensions.Mathematics.MathBlock
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="FencedCodeBlock"/>, <see cref="CodeBlock"/>, <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Mathematics.MathBlock))]
    MathBlock,

    /// <summary>
    /// Objects of type Markdig.Extensions.Yaml.YamlFrontMatterBlock
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="CodeBlock"/>, <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Yaml.YamlFrontMatterBlock))]
    YamlFrontMatterBlock,

    /// <summary>
    /// Objects of type Markdig.Extensions.DefinitionLists.DefinitionTerm
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.DefinitionLists.DefinitionTerm))]
    DefinitionTerm,

    /// <summary>
    /// Objects of type Markdig.Syntax.EmptyBlock
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.EmptyBlock))]
    EmptyBlock,

    /// <summary>
    /// Objects of type Markdig.Extensions.Figures.FigureCaption
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Figures.FigureCaption))]
    FigureCaption,

    /// <summary>
    /// Objects of type Markdig.Syntax.HeadingBlock
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.HeadingBlock))]
    HeadingBlock,

    /// <summary>
    /// Objects of type Markdig.Syntax.HtmlBlock
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.HtmlBlock))]
    HtmlBlock,

    /// <summary>
    /// Objects of type Markdig.Syntax.LinkReferenceDefinition
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="FootnoteLinkReferenceDefinition"/>, <see cref="HeadingLinkReferenceDefinition"/>
    /// Included by: <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.LinkReferenceDefinition))]
    LinkReferenceDefinition,

    /// <summary>
    /// Objects of type Markdig.Extensions.Footnotes.FootnoteLinkReferenceDefinition
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LinkReferenceDefinition"/>, <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Footnotes.FootnoteLinkReferenceDefinition))]
    FootnoteLinkReferenceDefinition,

    /// <summary>
    /// Objects of type Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LinkReferenceDefinition"/>, <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition))]
    HeadingLinkReferenceDefinition,

    /// <summary>
    /// Objects of type Markdig.Syntax.ParagraphBlock
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.ParagraphBlock))]
    ParagraphBlock,

    /// <summary>
    /// Objects of type Markdig.Syntax.ThematicBreakBlock
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafBlock"/>, <see cref="Block"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.ThematicBreakBlock))]
    ThematicBreakBlock,

    /// <summary>
    /// Objects of type Markdig.Syntax.Inlines.Inline
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="AbbreviationInline"/>, <see cref="AutolinkInline"/>, <see cref="CodeInline"/>, <see cref="ContainerInline"/>, <see cref="CustomContainerInline"/>, <see cref="DelimiterInline"/>, <see cref="EmojiInline"/>,
    ///     <see cref="EmphasisDelimiterInline"/>, <see cref="EmphasisInline"/>, <see cref="FootnoteLink"/>, <see cref="HtmlEntityInline"/>, <see cref="HtmlInline"/>, <see cref="JiraLink"/>, <see cref="LeafInline"/>, <see cref="LineBreakInline"/>,
    ///     <see cref="LinkDelimiterInline"/>, <see cref="LinkInline"/>, <see cref="LiteralInline"/>, <see cref="MathInline"/>, <see cref="PipeTableDelimiterInline"/>, <see cref="SmartyPant"/>, <see cref="TaskList"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.Inline))]
    Inline,

    /// <summary>
    /// Objects of type Markdig.Syntax.Inlines.ContainerInline
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="CustomContainerInline"/>, <see cref="DelimiterInline"/>, <see cref="EmphasisDelimiterInline"/>, <see cref="EmphasisInline"/>, <see cref="JiraLink"/>, <see cref="LinkDelimiterInline"/>, <see cref="LinkInline"/>,
    ///     <see cref="PipeTableDelimiterInline"/>
    /// Included by: <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.ContainerInline))]
    ContainerInline,

    /// <summary>
    /// Objects of type Markdig.Extensions.Footnotes.FootnoteLink
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Footnotes.FootnoteLink))]
    FootnoteLink,

    /// <summary>
    /// Objects of type Markdig.Syntax.Inlines.LeafInline
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="AbbreviationInline"/>, <see cref="AutolinkInline"/>, <see cref="CodeInline"/>, <see cref="EmojiInline"/>, <see cref="HtmlEntityInline"/>, <see cref="HtmlInline"/>, <see cref="LineBreakInline"/>, <see cref="LiteralInline"/>,
    ///     <see cref="MathInline"/>, <see cref="SmartyPant"/>, <see cref="TaskList"/>
    /// Included by: <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.LeafInline))]
    LeafInline,

    /// <summary>
    /// Objects of type Markdig.Extensions.Abbreviations.AbbreviationInline
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Abbreviations.AbbreviationInline))]
    AbbreviationInline,

    /// <summary>
    /// Objects of type Markdig.Syntax.Inlines.AutolinkInline
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.AutolinkInline))]
    AutolinkInline,

    /// <summary>
    /// Objects of type Markdig.Syntax.Inlines.CodeInline
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.CodeInline))]
    CodeInline,

    /// <summary>
    /// Objects of type Markdig.Syntax.Inlines.HtmlEntityInline
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.HtmlEntityInline))]
    HtmlEntityInline,

    /// <summary>
    /// Objects of type Markdig.Syntax.Inlines.HtmlInline
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.HtmlInline))]
    HtmlInline,

    /// <summary>
    /// Objects of type Markdig.Syntax.Inlines.LineBreakInline
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.LineBreakInline))]
    LineBreakInline,

    /// <summary>
    /// Objects of type Markdig.Syntax.Inlines.LiteralInline
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="EmojiInline"/>
    /// Included by: <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.LiteralInline))]
    LiteralInline,

    /// <summary>
    /// Objects of type Markdig.Extensions.Emoji.EmojiInline
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LiteralInline"/>, <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Emoji.EmojiInline))]
    EmojiInline,

    /// <summary>
    /// Objects of type Markdig.Extensions.Mathematics.MathInline
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Mathematics.MathInline))]
    MathInline,

    /// <summary>
    /// Objects of type Markdig.Extensions.SmartyPants.SmartyPant
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.SmartyPants.SmartyPant))]
    SmartyPant,

    /// <summary>
    /// Objects of type Markdig.Extensions.TaskLists.TaskList
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LeafInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.TaskLists.TaskList))]
    TaskList,

    /// <summary>
    /// Objects of type Markdig.Syntax.Inlines.DelimiterInline
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="EmphasisDelimiterInline"/>, <see cref="LinkDelimiterInline"/>, <see cref="PipeTableDelimiterInline"/>
    /// Included by: <see cref="ContainerInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.DelimiterInline))]
    DelimiterInline,

    /// <summary>
    /// Objects of type Markdig.Syntax.Inlines.EmphasisDelimiterInline
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="DelimiterInline"/>, <see cref="ContainerInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.EmphasisDelimiterInline))]
    EmphasisDelimiterInline,

    /// <summary>
    /// Objects of type Markdig.Syntax.Inlines.LinkDelimiterInline
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="DelimiterInline"/>, <see cref="ContainerInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.LinkDelimiterInline))]
    LinkDelimiterInline,

    /// <summary>
    /// Objects of type Markdig.Extensions.Tables.PipeTableDelimiterInline
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="DelimiterInline"/>, <see cref="ContainerInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.Tables.PipeTableDelimiterInline))]
    PipeTableDelimiterInline,

    /// <summary>
    /// Objects of type Markdig.Syntax.Inlines.EmphasisInline
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="CustomContainerInline"/>
    /// Included by: <see cref="ContainerInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.EmphasisInline))]
    EmphasisInline,

    /// <summary>
    /// Objects of type Markdig.Extensions.CustomContainers.CustomContainerInline
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="EmphasisInline"/>, <see cref="ContainerInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.CustomContainers.CustomContainerInline))]
    CustomContainerInline,

    /// <summary>
    /// Objects of type Markdig.Renderers.Html.HtmlAttributes
    /// </summary>
    [ReflectionType(typeof(Markdig.Renderers.Html.HtmlAttributes))]
    HtmlAttributes,

    /// <summary>
    /// Objects of type Markdig.Syntax.Inlines.LinkInline
    /// </summary>
    /// <remarks>
    /// Includes: <see cref="JiraLink"/>
    /// Included by: <see cref="ContainerInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Syntax.Inlines.LinkInline))]
    LinkInline,

    /// <summary>
    /// Objects of type Markdig.Extensions.JiraLinks.JiraLink
    /// </summary>
    /// <remarks>
    /// Included by: <see cref="LinkInline"/>, <see cref="ContainerInline"/>, <see cref="Inline"/>
    /// </remarks>
    [ReflectionType(typeof(Markdig.Extensions.JiraLinks.JiraLink))]
    JiraLink
}

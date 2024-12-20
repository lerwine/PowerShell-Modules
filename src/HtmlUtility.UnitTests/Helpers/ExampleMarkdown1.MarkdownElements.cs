using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace HtmlUtility.UnitTests.Helpers;

public static partial class ExampleMarkdown1
{
    class MarkdownElements
    {
        internal MarkdownDocument Document { get; init; }

        // #region Private fields

        private HeadingBlock? _element0;
        private HtmlAttributes? _element0_Attributes;
        private LiteralInline? _element0_0;
        private HtmlBlock? _element1;
        private ParagraphBlock? _element2;
        private LiteralInline? _element2_0;
        private ParagraphBlock? _element3;
        private LinkInline? _element3_0;
        private LiteralInline? _element3_0_0;
        private ParagraphBlock? _element4;
        private LiteralInline? _element4_0;
        private HtmlEntityInline? _element4_1;
        private LineBreakInline? _element4_2;
        private HtmlAttributes? _element4_2_Attributes;
        private ParagraphBlock? _element5;
        private LiteralInline? _element5_0;
        private Markdig.Extensions.Footnotes.FootnoteLink? _element5_1;
        private LiteralInline? _element5_2;
        private Markdig.Extensions.Footnotes.FootnoteLink? _element5_3;
        private LiteralInline? _element5_4;
        private LinkInline? _element5_5;
        private LiteralInline? _element5_5_0;
        private LiteralInline? _element5_6;
        private HeadingBlock? _element6;
        private HtmlAttributes? _element6_Attributes;
        private LiteralInline? _element6_0;
        private HeadingBlock? _element7;
        private HtmlAttributes? _element7_Attributes;
        private LiteralInline? _element7_0;
        private ParagraphBlock? _element8;
        private LinkInline? _element8_0;
        private HtmlAttributes? _element8_0_Attributes;
        private LiteralInline? _element8_0_0;
        private LiteralInline? _element8_1;
        private Markdig.Extensions.Emoji.EmojiInline? _element8_2;
        private ParagraphBlock? _element9;
        private LiteralInline? _element9_0;
        private LinkInline? _element9_1;
        private LiteralInline? _element9_1_0;
        private LiteralInline? _element9_2;
        private ParagraphBlock? _element10;
        private AutolinkInline? _element10_0;
        private LineBreakInline? _element10_1;
        private AutolinkInline? _element10_2;
        private LineBreakInline? _element10_3;
        private LinkInline? _element10_4;
        private LiteralInline? _element10_4_0;
        private ParagraphBlock? _element11;
        private LiteralInline? _element11_0;
        private LineBreakInline? _element11_1;
        private LiteralInline? _element11_2;
        private ParagraphBlock? _element12;
        private LiteralInline? _element12_0;
        private LineBreakInline? _element12_1;
        private LiteralInline? _element12_2;
        private ListBlock? _element13;
        private HtmlAttributes? _element13_Attributes;
        private ListItemBlock? _element13_0;
        private HtmlAttributes? _element13_0_Attributes;
        private ParagraphBlock? _element13_0_0;
        private Markdig.Extensions.TaskLists.TaskList? _element13_0_0_0;
        private LiteralInline? _element13_0_0_1;
        private ListItemBlock? _element13_1;
        private HtmlAttributes? _element13_1_Attributes;
        private ParagraphBlock? _element13_1_0;
        private Markdig.Extensions.TaskLists.TaskList? _element13_1_0_0;
        private LiteralInline? _element13_1_0_1;
        private ListItemBlock? _element13_2;
        private ParagraphBlock? _element13_2_0;
        private LiteralInline? _element13_2_0_0;
        private ListItemBlock? _element13_3;
        private ParagraphBlock? _element13_3_0;
        private LiteralInline? _element13_3_0_0;
        private ListBlock? _element14;
        private ListItemBlock? _element14_0;
        private ParagraphBlock? _element14_0_0;
        private LiteralInline? _element14_0_0_0;
        private ListItemBlock? _element14_1;
        private ParagraphBlock? _element14_1_0;
        private LiteralInline? _element14_1_0_0;
        private QuoteBlock? _element15;
        private ParagraphBlock? _element15_0;
        private LiteralInline? _element15_0_0;
        private LiteralInline? _element15_0_1;
        private LineBreakInline? _element15_0_2;
        private LiteralInline? _element15_0_3;
        private QuoteBlock? _element16;
        private ParagraphBlock? _element16_0;
        private LiteralInline? _element16_0_0;
        private LineBreakInline? _element16_0_1;
        private EmphasisInline? _element16_0_2;
        private LiteralInline? _element16_0_2_0;
        private ParagraphBlock? _element16_1;
        private LiteralInline? _element16_1_0;
        private ParagraphBlock? _element17;
        private LiteralInline? _element17_0;
        private EmphasisInline? _element17_1;
        private LiteralInline? _element17_1_0;
        private Markdig.Extensions.Abbreviations.AbbreviationInline? _element17_1_1;
        private LiteralInline? _element17_1_2;
        private LiteralInline? _element17_1_3;
        private LineBreakInline? _element17_2;
        private LiteralInline? _element17_3;
        private Markdig.Extensions.Abbreviations.AbbreviationInline? _element17_4;
        private LiteralInline? _element17_5;
        private LiteralInline? _element17_6;
        private QuoteBlock? _element18;
        private ParagraphBlock? _element18_0;
        private LiteralInline? _element18_0_0;
        private LiteralInline? _element18_0_1;
        private LineBreakInline? _element18_0_2;
        private LiteralInline? _element18_0_3;
        private ParagraphBlock? _element18_1;
        private LiteralInline? _element18_1_0;
        private FencedCodeBlock? _element18_2;
        private HtmlAttributes? _element18_2_Attributes;
        private ParagraphBlock? _element18_3;
        private CodeInline? _element18_3_0;
        private Markdig.Extensions.Tables.Table? _element19;
        private Markdig.Extensions.Tables.TableRow? _element19_0;
        private Markdig.Extensions.Tables.TableCell? _element19_0_0;
        private ParagraphBlock? _element19_0_0_0;
        private LiteralInline? _element19_0_0_0_0;
        private Markdig.Extensions.Tables.TableCell? _element19_0_1;
        private ParagraphBlock? _element19_0_1_0;
        private LiteralInline? _element19_0_1_0_0;
        private Markdig.Extensions.Tables.TableRow? _element19_1;
        private Markdig.Extensions.Tables.TableCell? _element19_1_0;
        private ParagraphBlock? _element19_1_0_0;
        private LiteralInline? _element19_1_0_0_0;
        private Markdig.Extensions.Tables.TableCell? _element19_1_1;
        private ParagraphBlock? _element19_1_1_0;
        private LiteralInline? _element19_1_1_0_0;
        private Markdig.Extensions.Tables.TableRow? _element19_2;
        private Markdig.Extensions.Tables.TableCell? _element19_2_0;
        private ParagraphBlock? _element19_2_0_0;
        private LiteralInline? _element19_2_0_0_0;
        private Markdig.Extensions.Tables.TableCell? _element19_2_1;
        private ParagraphBlock? _element19_2_1_0;
        private LiteralInline? _element19_2_1_0_0;
        private HeadingBlock? _element20;
        private HtmlAttributes? _element20_Attributes;
        private LiteralInline? _element20_0;
        private Markdig.Extensions.Tables.Table? _element21;
        private Markdig.Extensions.Tables.TableRow? _element21_0;
        private Markdig.Extensions.Tables.TableCell? _element21_0_0;
        private ParagraphBlock? _element21_0_0_0;
        private LiteralInline? _element21_0_0_0_0;
        private Markdig.Extensions.Tables.TableCell? _element21_0_1;
        private ParagraphBlock? _element21_0_1_0;
        private LiteralInline? _element21_0_1_0_0;
        private Markdig.Extensions.Tables.TableCell? _element21_0_2;
        private ParagraphBlock? _element21_0_2_0;
        private LiteralInline? _element21_0_2_0_0;
        private Markdig.Extensions.Tables.TableRow? _element21_1;
        private Markdig.Extensions.Tables.TableCell? _element21_1_0;
        private ParagraphBlock? _element21_1_0_0;
        private LiteralInline? _element21_1_0_0_0;
        private Markdig.Extensions.Tables.TableCell? _element21_1_1;
        private ParagraphBlock? _element21_1_1_0;
        private LiteralInline? _element21_1_1_0_0;
        private Markdig.Extensions.Tables.TableCell? _element21_1_2;
        private ParagraphBlock? _element21_1_2_0;
        private LiteralInline? _element21_1_2_0_0;
        private Markdig.Extensions.Tables.TableRow? _element21_2;
        private Markdig.Extensions.Tables.TableCell? _element21_2_0;
        private ParagraphBlock? _element21_2_0_0;
        private LiteralInline? _element21_2_0_0_0;
        private Markdig.Extensions.Tables.TableCell? _element21_2_1;
        private ParagraphBlock? _element21_2_1_0;
        private LiteralInline? _element21_2_1_0_0;
        private Markdig.Extensions.Tables.TableCell? _element21_2_2;
        private ParagraphBlock? _element21_2_2_0;
        private LiteralInline? _element21_2_2_0_0;
        private Markdig.Extensions.Figures.Figure? _element22;
        private ParagraphBlock? _element22_0;
        private LinkInline? _element22_0_0;
        private LiteralInline? _element22_0_0_0;
        private Markdig.Extensions.Figures.FigureCaption? _element22_1;
        private LiteralInline? _element22_1_0;
        private ParagraphBlock? _element23;
        private LinkInline? _element23_0;
        private LiteralInline? _element23_0_0;
        private ParagraphBlock? _element24;
        private LiteralInline? _element24_0;
        private EmphasisInline? _element24_1;
        private LiteralInline? _element24_1_0;
        private LiteralInline? _element24_2;
        private EmphasisInline? _element24_3;
        private LiteralInline? _element24_3_0;
        private LiteralInline? _element24_4;
        private Markdig.Extensions.Mathematics.MathInline? _element24_5;
        private HtmlAttributes? _element24_5_Attributes;
        private ParagraphBlock? _element25;
        private LiteralInline? _element25_0;
        private Markdig.Extensions.Mathematics.MathInline? _element25_1;
        private HtmlAttributes? _element25_1_Attributes;
        private LiteralInline? _element25_2;
        private Markdig.Extensions.Mathematics.MathInline? _element25_3;
        private HtmlAttributes? _element25_3_Attributes;
        private LiteralInline? _element25_4;
        private ParagraphBlock? _element26;
        private EmphasisInline? _element26_0;
        private LiteralInline? _element26_0_0;
        private LineBreakInline? _element26_1;
        private Markdig.Extensions.Mathematics.MathInline? _element26_2;
        private HtmlAttributes? _element26_2_Attributes;
        private ParagraphBlock? _element27;
        private LiteralInline? _element27_0;
        private CodeInline? _element27_1;
        private LiteralInline? _element27_2;
        private ThematicBreakBlock? _element28;
        private ParagraphBlock? _element29;
        private EmphasisInline? _element29_0;
        private LiteralInline? _element29_0_0;
        private LiteralInline? _element29_1;
        private EmphasisInline? _element29_2;
        private LiteralInline? _element29_2_0;
        private LiteralInline? _element29_3;
        private FencedCodeBlock? _element30;
        private HtmlAttributes? _element30_Attributes;
        private FencedCodeBlock? _element31;
        private HtmlAttributes? _element31_Attributes;
        private FencedCodeBlock? _element32;
        private HtmlAttributes? _element32_Attributes;
        private FencedCodeBlock? _element33;
        private HtmlAttributes? _element33_Attributes;
        private Markdig.Extensions.DefinitionLists.DefinitionList? _element34;
        private Markdig.Extensions.DefinitionLists.DefinitionItem? _element34_0;
        private Markdig.Extensions.DefinitionLists.DefinitionTerm? _element34_0_0;
        private LiteralInline? _element34_0_0_0;
        private ParagraphBlock? _element34_0_1;
        private LiteralInline? _element34_0_1_0;
        private LineBreakInline? _element34_0_1_1;
        private LiteralInline? _element34_0_1_2;
        private Markdig.Extensions.DefinitionLists.DefinitionItem? _element34_1;
        private Markdig.Extensions.DefinitionLists.DefinitionTerm? _element34_1_0;
        private LiteralInline? _element34_1_0_0;
        private ParagraphBlock? _element34_1_1;
        private LiteralInline? _element34_1_1_0;
        private Markdig.Extensions.Footers.FooterBlock? _element35;
        private ParagraphBlock? _element35_0;
        private LiteralInline? _element35_0_0;
        private LineBreakInline? _element35_0_1;
        private LiteralInline? _element35_0_2;
        private LinkReferenceDefinitionGroup? _element36;
        private Markdig.Extensions.Footnotes.FootnoteLinkReferenceDefinition? _element36_0;
        private LinkReferenceDefinition? _element36_1;
        private Markdig.Extensions.Footnotes.FootnoteLinkReferenceDefinition? _element36_2;
        private Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition? _element36_3;
        private Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition? _element36_4;
        private Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition? _element36_5;
        private Markdig.Extensions.Footnotes.FootnoteGroup? _element37;
        private Markdig.Extensions.Footnotes.Footnote? _element37_0;
        private ParagraphBlock? _element37_0_0;
        private LiteralInline? _element37_0_0_0;
        private Markdig.Extensions.Footnotes.FootnoteLink? _element37_0_0_1;
        private Markdig.Extensions.Footnotes.Footnote? _element37_1;
        private ParagraphBlock? _element37_1_0;
        private LiteralInline? _element37_1_0_0;
        private Markdig.Extensions.Footnotes.FootnoteLink? _element37_1_0_1;

        // #endregion

        /*
            elements.Element0, elements.Element0_Attributes, elements.Element0_0, elements.Element1, elements.Element2, elements.Element2_0, elements.Element3, elements.Element3_0,
            elements.Element3_0_0, elements.Element4, elements.Element4_0, elements.Element4_1, elements.Element4_2, elements.Element4_2_Attributes
        */
        internal HeadingBlock Element0 => _element0 ??= (HeadingBlock)Document[0];
        internal HtmlAttributes Element0_Attributes => _element0_Attributes ??= Element0.GetAttributes();
        internal LiteralInline Element0_0 => _element0_0 ??= (LiteralInline)Element0.Inline!.FirstChild!;

        internal HtmlBlock Element1 => _element1 ??= (HtmlBlock)Document[1];

        internal ParagraphBlock Element2 => _element2 ??= (ParagraphBlock)Document[2];
        internal LiteralInline Element2_0 => _element2_0 ??= (LiteralInline)Element2.Inline!.FirstChild!;

        internal ParagraphBlock Element3 => _element3 ??= (ParagraphBlock)Document[3];
        internal LinkInline Element3_0 => _element3_0 ??= (LinkInline)Element3.Inline!.FirstChild!;
        internal LiteralInline Element3_0_0 => _element3_0_0 ??= (LiteralInline)Element3_0.FirstChild!;

        internal ParagraphBlock Element4 => _element4 ??= (ParagraphBlock)Document[4];
        internal LiteralInline Element4_0 => _element4_0 ??= (LiteralInline)Element4.Inline!.FirstChild!;
        internal HtmlEntityInline Element4_1 => _element4_1 ??= (HtmlEntityInline)Element4_0.NextSibling!;
        internal LineBreakInline Element4_2 => _element4_2 ??= (LineBreakInline)Element4.Inline!.LastChild!;
        internal HtmlAttributes Element4_2_Attributes => _element4_2_Attributes ??= Element4_2.GetAttributes();

        internal ParagraphBlock Element5 => _element5 ??= (ParagraphBlock)Document[5];
        internal LiteralInline Element5_0 => _element5_0 ??= (LiteralInline)Element5.Inline!.FirstChild!;
        internal Markdig.Extensions.Footnotes.FootnoteLink Element5_1 => _element5_1 ??= (Markdig.Extensions.Footnotes.FootnoteLink)Element5_0.NextSibling!;
        internal LiteralInline Element5_2 => _element5_2 ??= (LiteralInline)Element5_1.NextSibling!;
        internal Markdig.Extensions.Footnotes.FootnoteLink Element5_3 => _element5_3 ??= (Markdig.Extensions.Footnotes.FootnoteLink)Element5_2.NextSibling!;
        internal LiteralInline Element5_4 => _element5_4 ??= (LiteralInline)Element5_5.PreviousSibling!;
        internal LinkInline Element5_5 => _element5_5 ??= (LinkInline)Element5_6.PreviousSibling!;
        internal LiteralInline Element5_5_0 => _element5_5_0 ??= (LiteralInline)Element5_5.FirstChild!;
        internal LiteralInline Element5_6 => _element5_6 ??= (LiteralInline)Element5.Inline!.LastChild!;

        internal HeadingBlock Element6 => _element6 ??= (HeadingBlock)Document[6];
        internal HtmlAttributes Element6_Attributes => _element6_Attributes ??= Element6.GetAttributes();
        internal LiteralInline Element6_0 => _element6_0 ??= (LiteralInline)Element6.Inline!.FirstChild!;

        internal HeadingBlock Element7 => _element7 ??= (HeadingBlock)Document[7];
        internal HtmlAttributes Element7_Attributes => _element7_Attributes ??= Element7.GetAttributes();
        internal LiteralInline Element7_0 => _element7_0 ??= (LiteralInline)Element7.Inline!.FirstChild!;

        internal ParagraphBlock Element8 => _element8 ??= (ParagraphBlock)Document[8];
        internal LinkInline Element8_0 => _element8_0 ??= (LinkInline)Element8.Inline!.FirstChild!;
        internal HtmlAttributes Element8_0_Attributes => _element8_0_Attributes ??= Element8_0.GetAttributes();
        internal LiteralInline Element8_0_0 => _element8_0_0 ??= (LiteralInline)Element8_0.FirstChild!;
        internal LiteralInline Element8_1 => _element8_1 ??= (LiteralInline)Element8_0.NextSibling!;
        internal Markdig.Extensions.Emoji.EmojiInline Element8_2 => _element8_2 ??= (Markdig.Extensions.Emoji.EmojiInline)Element8.Inline!.LastChild!;

        internal ParagraphBlock Element9 => _element9 ??= (ParagraphBlock)Document[9];
        internal LiteralInline Element9_0 => _element9_0 ??= (LiteralInline)Element9.Inline!.FirstChild!;
        internal LinkInline Element9_1 => _element9_1 ??= (LinkInline)Element9_0.NextSibling!;
        internal LiteralInline Element9_1_0 => _element9_1_0 ??= (LiteralInline)Element9_1.FirstChild!;
        internal LiteralInline Element9_2 => _element9_2 ??= (LiteralInline)Element9.Inline!.LastChild!;

        internal ParagraphBlock Element10 => _element10 ??= (ParagraphBlock)Document[10];
        internal AutolinkInline Element10_0 => _element10_0 ??= (AutolinkInline)Element10.Inline!.FirstChild!;
        internal LineBreakInline Element10_1 => _element10_1 ??= (LineBreakInline)Element10_0.NextSibling!;
        internal AutolinkInline Element10_2 => _element10_2 ??= (AutolinkInline)Element10_1.NextSibling!;
        internal LineBreakInline Element10_3 => _element10_3 ??= (LineBreakInline)Element10_4.PreviousSibling!;
        internal LinkInline Element10_4 => _element10_4 ??= (LinkInline)Element10.Inline!.LastChild!;
        internal LiteralInline Element10_4_0 => _element10_4_0 ??= (LiteralInline)Element10_4.FirstChild!;

        internal ParagraphBlock Element11 => _element11 ??= (ParagraphBlock)Document[11];
        internal LiteralInline Element11_0 => _element11_0 ??= (LiteralInline)Element11.Inline!.FirstChild!;
        internal LineBreakInline Element11_1 => _element11_1 ??= (LineBreakInline)Element11_0.NextSibling!;
        internal LiteralInline Element11_2 => _element11_2 ??= (LiteralInline)Element11.Inline!.LastChild!;

        internal ParagraphBlock Element12 => _element12 ??= (ParagraphBlock)Document[12];
        internal LiteralInline Element12_0 => _element12_0 ??= (LiteralInline)Element12.Inline!.FirstChild!;
        internal LineBreakInline Element12_1 => _element12_1 ??= (LineBreakInline)Element12_0.NextSibling!;
        internal LiteralInline Element12_2 => _element12_2 ??= (LiteralInline)Element12.Inline!.LastChild!;

        public ListBlock Element13 => _element13 ??= (ListBlock)Document[13];
        public HtmlAttributes Element13_Attributes => _element13_Attributes ??= Element13.GetAttributes();
        public ListItemBlock Element13_0 => _element13_0 ??= (ListItemBlock)Element13[0];
        public HtmlAttributes Element13_0_Attributes => _element13_0_Attributes ??= Element13_0.GetAttributes();
        public ParagraphBlock Element13_0_0 => _element13_0_0 ??= (ParagraphBlock)Element13_0[0];
        public Markdig.Extensions.TaskLists.TaskList Element13_0_0_0 => _element13_0_0_0 ??= (Markdig.Extensions.TaskLists.TaskList)Element13_0_0.Inline!.FirstChild!;
        public LiteralInline Element13_0_0_1 => _element13_0_0_1 ??= (LiteralInline)Element13_0_0.Inline!.LastChild!;
        public ListItemBlock Element13_1 => _element13_1 ??= (ListItemBlock)Element13[1];
        public HtmlAttributes Element13_1_Attributes => _element13_1_Attributes ??= Element13_1.GetAttributes();
        public ParagraphBlock Element13_1_0 => _element13_1_0 ??= (ParagraphBlock)Element13_1[0];
        public Markdig.Extensions.TaskLists.TaskList Element13_1_0_0 => _element13_1_0_0 ??= (Markdig.Extensions.TaskLists.TaskList)Element13_1_0.Inline!.FirstChild!;
        public LiteralInline Element13_1_0_1 => _element13_1_0_1 ??= (LiteralInline)Element13_1_0.Inline!.LastChild!;
        public ListItemBlock Element13_2 => _element13_2 ??= (ListItemBlock)Element13[2];
        public ParagraphBlock Element13_2_0 => _element13_2_0 ??= (ParagraphBlock)Element13_2[0];
        public LiteralInline Element13_2_0_0 => _element13_2_0_0 ??= (LiteralInline)Element13_2_0.Inline!.FirstChild!;
        public ListItemBlock Element13_3 => _element13_3 ??= (ListItemBlock)Element13[3];
        public ParagraphBlock Element13_3_0 => _element13_3_0 ??= (ParagraphBlock)Element13_3[0];
        public LiteralInline Element13_3_0_0 => _element13_3_0_0 ??= (LiteralInline)Element13_3_0.Inline!.FirstChild!;

        public ListBlock Element14 => _element14 ??= (ListBlock)Document[14];
        public ListItemBlock Element14_0 => _element14_0 ??= (ListItemBlock)Element14[0];
        public ParagraphBlock Element14_0_0 => _element14_0_0 ??= (ParagraphBlock)Element14_0[0];
        public LiteralInline Element14_0_0_0 => _element14_0_0_0 ??= (LiteralInline)Element14_0_0.Inline!.FirstChild!;
        public ListItemBlock Element14_1 => _element14_1 ??= (ListItemBlock)Element14[1];
        public ParagraphBlock Element14_1_0 => _element14_1_0 ??= (ParagraphBlock)Element14_1[0];
        public LiteralInline Element14_1_0_0 => _element14_1_0_0 ??= (LiteralInline)Element14_1_0.Inline!.FirstChild!;

        public QuoteBlock Element15 => _element15 ??= (QuoteBlock)Document[15];
        public ParagraphBlock Element15_0 => _element15_0 ??= (ParagraphBlock)Element15[0];
        public LiteralInline Element15_0_0 => _element15_0_0 ??= (LiteralInline)Element15_0.Inline!.FirstChild!;
        public LiteralInline Element15_0_1 => _element15_0_1 ??= (LiteralInline)Element15_0_0.NextSibling!;
        public LineBreakInline Element15_0_2 => _element15_0_2 ??= (LineBreakInline)Element15_0_3.PreviousSibling!;
        public LiteralInline Element15_0_3 => _element15_0_3 ??= (LiteralInline)Element15_0.Inline!.LastChild!;

        public QuoteBlock Element16 => _element16 ??= (QuoteBlock)Document[16];
        public ParagraphBlock Element16_0 => _element16_0 ??= (ParagraphBlock)Element16[0];
        public LiteralInline Element16_0_0 => _element16_0_0 ??= (LiteralInline)Element16_0.Inline!.FirstChild!;
        public LineBreakInline Element16_0_1 => _element16_0_1 ??= (LineBreakInline)Element16_0_0.NextSibling!;
        public EmphasisInline Element16_0_2 => _element16_0_2 ??= (EmphasisInline)Element16_0.Inline!.LastChild!;
        public LiteralInline Element16_0_2_0 => _element16_0_2_0 ??= (LiteralInline)Element16_0_2.FirstChild!;
        public ParagraphBlock Element16_1 => _element16_1 ??= (ParagraphBlock)Element16[1];
        public LiteralInline Element16_1_0 => _element16_1_0 ??= (LiteralInline)Element16_1.Inline!.FirstChild!;

        public ParagraphBlock Element17 => _element17 ??= (ParagraphBlock)Document[17];
        public LiteralInline Element17_0 => _element17_0 ??= (LiteralInline)Element17.Inline!.FirstChild!;
        public EmphasisInline Element17_1 => _element17_1 ??= (EmphasisInline)Element17_0.NextSibling!;
        public LiteralInline Element17_1_0 => _element17_1_0 ??= (LiteralInline)Element17_1.FirstChild!;
        public Markdig.Extensions.Abbreviations.AbbreviationInline Element17_1_1 => _element17_1_1 ??= (Markdig.Extensions.Abbreviations.AbbreviationInline)Element17_1_0.NextSibling!;
        public LiteralInline Element17_1_2 => _element17_1_2 ??= (LiteralInline)Element17_1_3.PreviousSibling!;
        public LiteralInline Element17_1_3 => _element17_1_3 ??= (LiteralInline)Element17_1.LastChild!;
        public LineBreakInline Element17_2 => _element17_2 ??= (LineBreakInline)Element17_1.NextSibling!;
        public LiteralInline Element17_3 => _element17_3 ??= (LiteralInline)Element17_2.NextSibling!;
        public Markdig.Extensions.Abbreviations.AbbreviationInline Element17_4 => _element17_4 ??= (Markdig.Extensions.Abbreviations.AbbreviationInline)Element17_5.PreviousSibling!;
        public LiteralInline Element17_5 => _element17_5 ??= (LiteralInline)Element17_6.PreviousSibling!;
        public LiteralInline Element17_6 => _element17_6 ??= (LiteralInline)Element17.Inline!.LastChild!;

        public QuoteBlock Element18 => _element18 ??= (QuoteBlock)Document[18];
        public ParagraphBlock Element18_0 => _element18_0 ??= (ParagraphBlock)Element18[0];
        public LiteralInline Element18_0_0 => _element18_0_0 ??= (LiteralInline)Element18_0.Inline!.FirstChild!;
        public LiteralInline Element18_0_1 => _element18_0_1 ??= (LiteralInline)Element18_0_0.NextSibling!;
        public LineBreakInline Element18_0_2 => _element18_0_2 ??= (LineBreakInline)Element18_0_3.PreviousSibling!;
        public LiteralInline Element18_0_3 => _element18_0_3 ??= (LiteralInline)Element18_0.Inline!.LastChild!;
        public ParagraphBlock Element18_1 => _element18_1 ??= (ParagraphBlock)Element18[1];
        public LiteralInline Element18_1_0 => _element18_1_0 ??= (LiteralInline)Element18_1.Inline!.FirstChild!;
        public FencedCodeBlock Element18_2 => _element18_2 ??= (FencedCodeBlock)Element18[2];
        public HtmlAttributes Element18_2_Attributes => _element18_2_Attributes ?? Element18_2.GetAttributes();
        public ParagraphBlock Element18_3 => _element18_3 ??= (ParagraphBlock)Document[19];
        public CodeInline Element18_3_0 => _element18_3_0 ?? (CodeInline)Element18_3.Inline!.FirstChild!;

        public Markdig.Extensions.Tables.Table Element19 => _element19 ??= (Markdig.Extensions.Tables.Table)Document[19];
        public Markdig.Extensions.Tables.TableRow Element19_0 => _element19_0 ??= (Markdig.Extensions.Tables.TableRow)Element19[0];
        public Markdig.Extensions.Tables.TableCell Element19_0_0 => _element19_0_0 ??= (Markdig.Extensions.Tables.TableCell)Element19_0[0];
        public ParagraphBlock Element19_0_0_0 => _element19_0_0_0 ??= (ParagraphBlock)Element19_0_0[0];
        public LiteralInline Element19_0_0_0_0 => _element19_0_0_0_0 ??= (LiteralInline)Element19_0_0_0.Inline!.FirstChild!;
        public Markdig.Extensions.Tables.TableCell Element19_0_1 => _element19_0_1 ??= (Markdig.Extensions.Tables.TableCell)Element19_0[1];
        public ParagraphBlock Element19_0_1_0 => _element19_0_1_0 ??= (ParagraphBlock)Element19_0_1[0];
        public LiteralInline Element19_0_1_0_0 => _element19_0_1_0_0 ??= (LiteralInline)Element19_0_1_0.Inline!.FirstChild!;
        public Markdig.Extensions.Tables.TableRow Element19_1 => _element19_1 ??= (Markdig.Extensions.Tables.TableRow)Element19[1];
        public Markdig.Extensions.Tables.TableCell Element19_1_0 => _element19_1_0 ??= (Markdig.Extensions.Tables.TableCell)Element19_1[0];
        public ParagraphBlock Element19_1_0_0 => _element19_1_0_0 ??= (ParagraphBlock)Element19_1_0[0];
        public LiteralInline Element19_1_0_0_0 => _element19_1_0_0_0 ??= (LiteralInline)Element19_1_0_0.Inline!.FirstChild!;
        public Markdig.Extensions.Tables.TableCell Element19_1_1 => _element19_1_1 ??= (Markdig.Extensions.Tables.TableCell)Element19_1[1];
        public ParagraphBlock Element19_1_1_0 => _element19_1_1_0 ??= (ParagraphBlock)Element19_1_1[0];
        public LiteralInline Element19_1_1_0_0 => _element19_1_1_0_0 ??= (LiteralInline)Element19_1_1_0.Inline!.FirstChild!;
        public Markdig.Extensions.Tables.TableRow Element19_2 => _element19_2 ??= (Markdig.Extensions.Tables.TableRow)Element19[2];
        public Markdig.Extensions.Tables.TableCell Element19_2_0 => _element19_2_0 ??= (Markdig.Extensions.Tables.TableCell)Element19_2[0];
        public ParagraphBlock Element19_2_0_0 => _element19_2_0_0 ??= (ParagraphBlock)Element19_2_0[0];
        public LiteralInline Element19_2_0_0_0 => _element19_2_0_0_0 ??= (LiteralInline)Element19_2_0_0.Inline!.FirstChild!;
        public Markdig.Extensions.Tables.TableCell Element19_2_1 => _element19_2_1 ??= (Markdig.Extensions.Tables.TableCell)Element19_2[1];
        public ParagraphBlock Element19_2_1_0 => _element19_2_1_0 ??= (ParagraphBlock)Element19_2_1[0];
        public LiteralInline Element19_2_1_0_0 => _element19_2_1_0_0 ??= (LiteralInline)Element19_2_1_0.Inline!.FirstChild!;

        public HeadingBlock Element20 => _element20 ??= (HeadingBlock)Document[20];
        public HtmlAttributes Element20_Attributes => _element20_Attributes ??= Element20.GetAttributes();
        public LiteralInline Element20_0 => _element20_0 ??= (LiteralInline)Element20.Inline!.FirstChild!;

        public Markdig.Extensions.Tables.Table Element21 => _element21 ??= (Markdig.Extensions.Tables.Table)Document[21];
        public Markdig.Extensions.Tables.TableRow Element21_0 => _element21_0 ??= (Markdig.Extensions.Tables.TableRow)Element21[0];
        public Markdig.Extensions.Tables.TableCell Element21_0_0 => _element21_0_0 ??= (Markdig.Extensions.Tables.TableCell)Element21_0[0];
        public ParagraphBlock Element21_0_0_0 => _element21_0_0_0 ??= (ParagraphBlock)Element21_0_0[0];
        public LiteralInline Element21_0_0_0_0 => _element21_0_0_0_0 ??= (LiteralInline)Element21_0_0_0.Inline!.FirstChild!;
        public Markdig.Extensions.Tables.TableCell Element21_0_1 => _element21_0_1 ??= (Markdig.Extensions.Tables.TableCell)Element21_0[1];
        public ParagraphBlock Element21_0_1_0 => _element21_0_1_0 ??= (ParagraphBlock)Element21_0_1[0];
        public LiteralInline Element21_0_1_0_0 => _element21_0_1_0_0 ??= (LiteralInline)Element21_0_1_0.Inline!.FirstChild!;
        public Markdig.Extensions.Tables.TableCell Element21_0_2 => _element21_0_2 ??= (Markdig.Extensions.Tables.TableCell)Element21_0[2];
        public ParagraphBlock Element21_0_2_0 => _element21_0_2_0 ??= (ParagraphBlock)Element21_0_2[0];
        public LiteralInline Element21_0_2_0_0 => _element21_0_2_0_0 ??= (LiteralInline)Element21_0_2_0.Inline!.FirstChild!;
        public Markdig.Extensions.Tables.TableRow Element21_1 => _element21_1 ??= (Markdig.Extensions.Tables.TableRow)Element21[1];
        public Markdig.Extensions.Tables.TableCell Element21_1_0 => _element21_1_0 ??= (Markdig.Extensions.Tables.TableCell)Element21_1[0];
        public ParagraphBlock Element21_1_0_0 => _element21_1_0_0 ??= (ParagraphBlock)Element21_1_0[0];
        public LiteralInline Element21_1_0_0_0 => _element21_1_0_0_0 ??= (LiteralInline)Element21_1_0_0.Inline!.FirstChild!;
        public Markdig.Extensions.Tables.TableCell Element21_1_1 => _element21_1_1 ??= (Markdig.Extensions.Tables.TableCell)Element21_1[1];
        public ParagraphBlock Element21_1_1_0 => _element21_1_1_0 ??= (ParagraphBlock)Element21_1_1[0];
        public LiteralInline Element21_1_1_0_0 => _element21_1_1_0_0 ??= (LiteralInline)Element21_1_1_0.Inline!.FirstChild!;
        public Markdig.Extensions.Tables.TableCell Element21_1_2 => _element21_1_2 ??= (Markdig.Extensions.Tables.TableCell)Element21_1[2];
        public ParagraphBlock Element21_1_2_0 => _element21_1_2_0 ??= (ParagraphBlock)Element21_1_2[0];
        public LiteralInline Element21_1_2_0_0 => _element21_1_2_0_0 ??= (LiteralInline)Element21_1_2_0.Inline!.FirstChild!;
        public Markdig.Extensions.Tables.TableRow Element21_2 => _element21_2 ??= (Markdig.Extensions.Tables.TableRow)Element21[2];
        public Markdig.Extensions.Tables.TableCell Element21_2_0 => _element21_2_0 ??= (Markdig.Extensions.Tables.TableCell)Element21_2[0];
        public ParagraphBlock Element21_2_0_0 => _element21_2_0_0 ??= (ParagraphBlock)Element21_2_0[0];
        public LiteralInline Element21_2_0_0_0 => _element21_2_0_0_0 ??= (LiteralInline)Element21_2_0_0.Inline!.FirstChild!;
        public Markdig.Extensions.Tables.TableCell Element21_2_1 => _element21_2_1 ??= (Markdig.Extensions.Tables.TableCell)Element21_2[1];
        public ParagraphBlock Element21_2_1_0 => _element21_2_1_0 ??= (ParagraphBlock)Element21_2_1[0];
        public LiteralInline Element21_2_1_0_0 => _element21_2_1_0_0 ??= (LiteralInline)Element21_2_1_0.Inline!.FirstChild!;
        public Markdig.Extensions.Tables.TableCell Element21_2_2 => _element21_2_2 ??= (Markdig.Extensions.Tables.TableCell)Element21_2[2];
        public ParagraphBlock Element21_2_2_0 => _element21_2_2_0 ??= (ParagraphBlock)Element21_2_2[0];
        public LiteralInline Element21_2_2_0_0 => _element21_2_2_0_0 ??= (LiteralInline)Element21_2_2_0.Inline!.FirstChild!;

        public Markdig.Extensions.Figures.Figure Element22 => _element22 ??= (Markdig.Extensions.Figures.Figure)Document[22];
        public ParagraphBlock Element22_0 => _element22_0 ??= (ParagraphBlock)Element22[0];
        public LinkInline Element22_0_0 => _element22_0_0 ??= (LinkInline)Element22_0.Inline!.FirstChild!;
        public LiteralInline Element22_0_0_0 => _element22_0_0_0 ??= (LiteralInline)Element22_0_0.FirstChild!;
        public Markdig.Extensions.Figures.FigureCaption Element22_1 => _element22_1 ??= (Markdig.Extensions.Figures.FigureCaption)Element22[1];
        public LiteralInline Element22_1_0 => _element22_1_0 ??= (LiteralInline)Element22_1.Inline!.FirstChild!;

        public ParagraphBlock Element23 => _element23 ??= (ParagraphBlock)Document[23];
        public LinkInline Element23_0 => _element23_0 ??= (LinkInline)Element23.Inline!.FirstChild!;
        public LiteralInline Element23_0_0 => _element23_0_0 ??= (LiteralInline)Element23_0.FirstChild!;

        public ParagraphBlock Element24 => _element24 ??= (ParagraphBlock)Document[24];
        public LiteralInline Element24_0 => _element24_0 ??= (LiteralInline)Element24.Inline!.FirstChild!;
        public EmphasisInline Element24_1 => _element24_1 ??= (EmphasisInline)Element24_0.NextSibling!;
        public LiteralInline Element24_1_0 => _element24_1_0 ??= (LiteralInline)Element24_1.FirstChild!;
        public LiteralInline Element24_2 => _element24_2 ??= (LiteralInline)Element24_1.NextSibling!;
        public EmphasisInline Element24_3 => _element24_3 ??= (EmphasisInline)Element24_4.PreviousSibling!;
        public LiteralInline Element24_3_0 => _element24_3_0 ??= (LiteralInline)Element24_3.FirstChild!;
        public LiteralInline Element24_4 => _element24_4 ??= (LiteralInline)Element24_5.PreviousSibling!;
        public Markdig.Extensions.Mathematics.MathInline Element24_5 => _element24_5 ??= (Markdig.Extensions.Mathematics.MathInline)Element24.Inline!.LastChild!;
        public HtmlAttributes Element24_5_Attributes => _element24_5_Attributes ??= Element24_5.GetAttributes();

        public ParagraphBlock Element25 => _element25 ??= (ParagraphBlock)Document[25];
        public LiteralInline Element25_0 => _element25_0 ??= (LiteralInline)Element25.Inline!.FirstChild!;
        public Markdig.Extensions.Mathematics.MathInline Element25_1 => _element25_1 ??= (Markdig.Extensions.Mathematics.MathInline)Element25_0.NextSibling!;
        public HtmlAttributes Element25_1_Attributes => _element25_1_Attributes ??= Element25_1.GetAttributes();
        public LiteralInline Element25_2 => _element25_2 ??= (LiteralInline)Element25_1.NextSibling!;
        public Markdig.Extensions.Mathematics.MathInline Element25_3 => _element25_3 ??= (Markdig.Extensions.Mathematics.MathInline)Element25_4.PreviousSibling!;
        public HtmlAttributes Element25_3_Attributes => _element25_3_Attributes ??= Element25_3.GetAttributes();
        public LiteralInline Element25_4 => _element25_4 ??= (LiteralInline)Element25.Inline!.LastChild!;

        public ParagraphBlock Element26 => _element26 ??= (ParagraphBlock)Document[26];
        public EmphasisInline Element26_0 => _element26_0 ??= (EmphasisInline)Element26.Inline!.FirstChild!;
        public LiteralInline Element26_0_0 => _element26_0_0 ??= (LiteralInline)Element26_0.FirstChild!;
        public LineBreakInline Element26_1 => _element26_1 ??= (LineBreakInline)Element26_0.NextSibling!;
        public Markdig.Extensions.Mathematics.MathInline Element26_2 => _element26_2 ??= (Markdig.Extensions.Mathematics.MathInline)Element26.Inline!.LastChild!;
        public HtmlAttributes Element26_2_Attributes => _element26_2_Attributes ??= Element26_2.GetAttributes();

        public ParagraphBlock Element27 => _element27 ??= (ParagraphBlock)Document[27];
        public LiteralInline Element27_0 => _element27_0 ??= (LiteralInline)Element27.Inline!.FirstChild!;
        public CodeInline Element27_1 => _element27_1 ??= (CodeInline)Element27_0.NextSibling!;
        public LiteralInline Element27_2 => _element27_2 ??= (LiteralInline)Element27.Inline!.LastChild!;

        public ThematicBreakBlock Element28 => _element28 ??= (ThematicBreakBlock)Document[28];

        public ParagraphBlock Element29 => _element29 ??= (ParagraphBlock)Document[29];
        public EmphasisInline Element29_0 => _element29_0 ??= (EmphasisInline)Element29.Inline!.FirstChild!;
        public LiteralInline Element29_0_0 => _element29_0_0 ??= (LiteralInline)Element29_0.FirstChild!;
        public LiteralInline Element29_1 => _element29_1 ??= (LiteralInline)Element29_0.NextSibling!;
        public EmphasisInline Element29_2 => _element29_2 ??= (EmphasisInline)Element29_3.PreviousSibling!;
        public LiteralInline Element29_2_0 => _element29_2_0 ??= (LiteralInline)Element29_2.FirstChild!;
        public LiteralInline Element29_3 => _element29_3 ??= (LiteralInline)Element29.Inline!.LastChild!;

        public FencedCodeBlock Element30 => _element30 ??= (FencedCodeBlock)Document[30];
        public HtmlAttributes Element30_Attributes => _element30_Attributes ??= Element30.GetAttributes();

        public FencedCodeBlock Element31 => _element31 ??= (FencedCodeBlock)Document[31];
        public HtmlAttributes Element31_Attributes => _element31_Attributes ??= Element31.GetAttributes();

        public FencedCodeBlock Element32 => _element32 ??= (FencedCodeBlock)Document[32];
        public HtmlAttributes Element32_Attributes => _element32_Attributes ??= Element32.GetAttributes();

        public FencedCodeBlock Element33 => _element33 ??= (FencedCodeBlock)Document[33];
        public HtmlAttributes Element33_Attributes => _element33_Attributes ??= Element33.GetAttributes();

        /*
            elements.Element0, elements.Element0_Attributes, elements.Element0_0, elements.Element1, elements.Element2, elements.Element2_0, elements.Element3, elements.Element3_0,
            elements.Element3_0_0, elements.Element4, elements.Element4_0, elements.Element4_1, elements.Element4_2, elements.Element4_2_Attributes, elements.Element5, elements.Element5_0,
            elements.Element5_1, elements.Element5_2, elements.Element5_3, elements.Element5_4, elements.Element5_5, elements.Element5_5_0, elements.Element5_6, elements.Element6,
            elements.Element6_Attributes, elements.Element6_0, elements.Element7, elements.Element7_Attributes, elements.Element7_0, elements.Element8, elements.Element8_0,
            elements.Element8_0_Attributes, elements.Element8_0_0, elements.Element8_1, elements.Element8_2, elements.Element9, elements.Element9_0, elements.Element9_1, elements.Element9_1_0,
            elements.Element9_2, elements.Element10, elements.Element10_0, elements.Element10_1, elements.Element10_2, elements.Element10_3, elements.Element10_4, elements.Element10_4_0,
            elements.Element11, elements.Element11_0, elements.Element11_1, elements.Element11_2, elements.Element12, elements.Element12_0, elements.Element12_1, elements.Element12_2,
            elements.Element13, elements.Element13_Attributes, elements.Element13_0, elements.Element13_0_Attributes, elements.Element13_0_0, elements.Element13_0_0_0, elements.Element13_0_0_1,
            elements.Element13_1, elements.Element13_1_Attributes, elements.Element13_1_0, elements.Element13_1_0_0, elements.Element13_1_0_1, elements.Element13_2, elements.Element13_2_0,
            elements.Element13_2_0_0, elements.Element13_3, elements.Element13_3_0, elements.Element13_3_0_0, elements.Element14, elements.Element14_0, elements.Element14_0_0,
            elements.Element14_0_0_0, elements.Element14_1, elements.Element14_1_0, elements.Element14_1_0_0, elements.Element15, elements.ParagraphBlock, elements.Element15_0_0,
            elements.Element15_0_1, elements.Element15_0_2, elements.Element15_0_3, elements.Element16, elements.Element16_0, elements.Element16_0_0, elements.Element16_0_1,
            elements.Element16_0_2, elements.Element16_0_2_0, elements.Element16_1, elements.Element16_1_0, elements.Element17, elements.Element17_0, elements.Element17_1,
            elements.Element17_1_0, elements.Element17_1_1, elements.Element17_1_2, elements.Element17_1_3, elements.Element17_2, elements.Element17_3, elements.Element17_4,
            elements.Element17_5, elements.Element17_6, elements.Element18, elements.Element18_0, elements.Element18_0_0, elements.Element18_0_1, elements.Element18_0_2, elements.Element18_0_3,
            elements.Element18_1, elements.Element18_1_0, elements.Element18_2, elements.Element18_2_Attributes, elements.Element18_3, elements.Element18_3_0, elements.Element19,
            elements.Element19_0, elements.Element19_0_0, elements.Element19_0_0_0, elements.Element19_0_0_0_0, elements.Element19_0_1, elements.Element19_0_1_0, elements.Element19_0_1_0_0,
            elements.Element19_1, elements.Element19_1_0, elements.Element19_1_0_0, elements.Element19_1_0_0_0, elements.Element19_1_1, elements.Element19_1_1_0, elements.Element19_1_1_0_0,
            elements.Element19_2, elements.Element19_2_0, elements.Element19_2_0_0, elements.Element19_2_0_0_0, elements.Element19_2_1, elements.Element19_2_1_0, elements.Element19_2_1_0_0,
            elements.Element20, elements.Element20_Attributes, elements.Element20_0, elements.Element21, elements.Element21_0, elements.Element21_0_0, elements.Element21_0_0_0,
            elements.Element21_0_0_0_0, elements.Element21_0_1, elements.Element21_0_1_0, elements.Element21_0_1_0_0, elements.Element21_0_2, elements.Element21_0_2_0,
            elements.Element21_0_2_0_0, elements.Element21_1, elements.Element21_1_0, elements.Element21_1_0_0, elements.Element21_1_0_0_0, elements.Element21_1_1, elements.Element21_1_1_0,
            elements.Element21_1_1_0_0, elements.Element21_1_2, elements.Element21_1_2_0, elements.Element21_1_2_0_0, elements.Element21_2, elements.Element21_2_0, elements.Element21_2_0_0,
            elements.Element21_2_0_0_0, elements.Element21_2_1, elements.Element21_2_1_0, elements.Element21_2_1_0_0, elements.Element21_2_2, elements.Element21_2_2_0,
            elements.Element21_2_2_0_0, elements.Element22, elements.Element22_0, elements.Element22_0_0, elements.Element22_0_0_0, elements.Element22_1, elements.Element22_1_0,
            elements.Element23, elements.Element23_0, elements.Element23_0_0, elements.Element24, elements.Element24_0, elements.Element24_1, elements.Element24_1_0, elements.Element24_2,
            elements.Element24_3, elements.Element24_3_0, elements.Element24_4, elements.Element24_5, elements.Element24_5_Attributes, elements.Element25, elements.Element25_0,
            elements.Element25_1, elements.Element25_1_Attributes, elements.Element25_2, elements.Element25_3, elements.Element25_3_Attributes, elements.Element25_4, elements.Ele
            elements.Element27_1, elements.Element27_2, elements.Element28, elements.Element29, elements.Element29_0, elements.Element29_0_0, elements.Element29_1, elements.Element29_2,
            elements.Element29_2_0, elements.Element29_3, elements.Element30, elements.Element30_Attributes, elements.Element31, elements.Element31_Attributes, elements.Element32,
            elements.Element32_Attributes, elements.Element33, elements.Element33_Attributes, elements.Element34, elements.Element34_0, elements.Element34_0_0, elements.Element34_0_0_0,
            elements.Element34_0_1, elements.Element34_0_1_0, elements.Element34_0_1_1, elements.Element34_0_1_2, elements.Element34_1, elements.Element34_1_0, elements.Element34_1_0_0,
            elements.Element34_1_1, elements.Element34_1_1_0, elements.Element35, elements.Element35_0, elements.Element35_0_0, elements.Element35_0_1, elements.Element35_0_2,
            elements.Element36, elements.Element36_0, elements.Element36_1, elements.Element36_2, elements.Element36_3, elements.Element36_4, elements.Element36_5, elements.Element37,
            elements.Element37_0, elements.Element37_0_0, elements.Element37_0_0_0, elements.Element37_0_0_1, elements.Element37_1, elements.Element37_1_0, elements.Element37_1_0_0, elements.Element37_1_0_1
        */
        public Markdig.Extensions.DefinitionLists.DefinitionList Element34 => _element34 ??= (Markdig.Extensions.DefinitionLists.DefinitionList)Document[34];
        public Markdig.Extensions.DefinitionLists.DefinitionItem Element34_0 => _element34_0 ??= (Markdig.Extensions.DefinitionLists.DefinitionItem)Element34[0];
        public Markdig.Extensions.DefinitionLists.DefinitionTerm Element34_0_0 => _element34_0_0 ??= (Markdig.Extensions.DefinitionLists.DefinitionTerm)Element34_0[0];
        public LiteralInline Element34_0_0_0 => _element34_0_0_0 ??= (LiteralInline)Element34_0_0.Inline!.FirstChild!;
        public ParagraphBlock Element34_0_1 => _element34_0_1 ??= (ParagraphBlock)Element34_0[1];
        public LiteralInline Element34_0_1_0 => _element34_0_1_0 ??= (LiteralInline)Element34_0_1.Inline!.FirstChild!;
        public LineBreakInline Element34_0_1_1 => _element34_0_1_1 ??= (LineBreakInline)Element34_0_1_0.NextSibling!;
        public LiteralInline Element34_0_1_2 => _element34_0_1_2 ??= (LiteralInline)Element34_0_1.Inline!.LastChild!;
        public Markdig.Extensions.DefinitionLists.DefinitionItem Element34_1 => _element34_1 ??= (Markdig.Extensions.DefinitionLists.DefinitionItem)Element34[1];
        public Markdig.Extensions.DefinitionLists.DefinitionTerm Element34_1_0 => _element34_1_0 ??= (Markdig.Extensions.DefinitionLists.DefinitionTerm)Element34_1[0];
        public LiteralInline Element34_1_0_0 => _element34_1_0_0 ??= (LiteralInline)Element34_1_0.Inline!.FirstChild!;
        public ParagraphBlock Element34_1_1 => _element34_1_1 ??= (ParagraphBlock)Element34_1[1];
        public LiteralInline Element34_1_1_0 => _element34_1_1_0 ??= (LiteralInline)Element34_1_1.Inline!.FirstChild!;

        public Markdig.Extensions.Footers.FooterBlock Element35 => _element35 ??= (Markdig.Extensions.Footers.FooterBlock)Document[35];
        public ParagraphBlock Element35_0 => _element35_0 ??= (ParagraphBlock)Element35[0];
        public LiteralInline Element35_0_0 => _element35_0_0 ??= (LiteralInline)Element35_0.Inline!.FirstChild!;
        public LineBreakInline Element35_0_1 => _element35_0_1 ??= (LineBreakInline)Element35_0_0.NextSibling!;
        public LiteralInline Element35_0_2 => _element35_0_2 ??= (LiteralInline)Element35_0.Inline!.LastChild!;

        public LinkReferenceDefinitionGroup Element36 => _element36 ??= (LinkReferenceDefinitionGroup)Document[36];
        public Markdig.Extensions.Footnotes.FootnoteLinkReferenceDefinition Element36_0 => _element36_0 ??= (Markdig.Extensions.Footnotes.FootnoteLinkReferenceDefinition)Element36[0];
        public LinkReferenceDefinition Element36_1 => _element36_1 ??= (LinkReferenceDefinition)Element36[1];
        public Markdig.Extensions.Footnotes.FootnoteLinkReferenceDefinition Element36_2 => _element36_2 ??= (Markdig.Extensions.Footnotes.FootnoteLinkReferenceDefinition)Element36[2];
        public Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition Element36_3 => _element36_3 ??= (Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition)Element36[3];
        public Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition Element36_4 => _element36_4 ??= (Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition)Element36[4];
        public Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition Element36_5 => _element36_5 ??= (Markdig.Extensions.AutoIdentifiers.HeadingLinkReferenceDefinition)Element36[5];

        public Markdig.Extensions.Footnotes.FootnoteGroup Element37 => _element37 ??= (Markdig.Extensions.Footnotes.FootnoteGroup)Document[37];
        public Markdig.Extensions.Footnotes.Footnote Element37_0 => _element37_0 ??= (Markdig.Extensions.Footnotes.Footnote)Element37[0];
        public ParagraphBlock Element37_0_0 => _element37_0_0 ??= (ParagraphBlock)Element37_0[0];
        public LiteralInline Element37_0_0_0 => _element37_0_0_0 ??= (LiteralInline)Element37_0_0.Inline!.FirstChild!;
        public Markdig.Extensions.Footnotes.FootnoteLink Element37_0_0_1 => _element37_0_0_1 ??= (Markdig.Extensions.Footnotes.FootnoteLink)Element37_0_0.Inline!.LastChild!;
        public Markdig.Extensions.Footnotes.Footnote Element37_1 => _element37_1 ??= (Markdig.Extensions.Footnotes.Footnote)Element37[1];
        public ParagraphBlock Element37_1_0 => _element37_1_0 ??= (ParagraphBlock)Element37_1[0];
        public LiteralInline Element37_1_0_0 => _element37_1_0_0 ??= (LiteralInline)Element37_1_0.Inline!.FirstChild!;
        public Markdig.Extensions.Footnotes.FootnoteLink Element37_1_0_1 => _element37_1_0_1 ??= (Markdig.Extensions.Footnotes.FootnoteLink)Element37_1_0.Inline!.LastChild!;

        internal MarkdownElements(MarkdownDocument markdownDocument)
        {
            Document = markdownDocument;
        }
    }
}
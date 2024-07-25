using SpeakLink.RichText;
using UIKit;

namespace SpeakLink.Controls.MaciOS.SpanStyle.Paragraph;

public class NumberListToolbarSpanStyle : ParagraphListToolbarSpanStyle
{
    public override NSTextListMarkerFormats MarkerFormat => NSTextListMarkerFormats.Decimal;

    public NumberListToolbarSpanStyle(SpeakLinkRichTextView richTextView) : base(richTextView)
    {
    }

    public override RichEditorStyle RichEditorStyle => RichEditorStyle.NumberList;
}
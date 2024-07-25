using SpeakLink.RichText;
using UIKit;

namespace SpeakLink.Controls.MaciOS.SpanStyle.Paragraph;

public class BulletListToolbarSpanStyle : ParagraphListToolbarSpanStyle
{
    public override NSTextListMarkerFormats MarkerFormat => NSTextListMarkerFormats.Circle;

    public BulletListToolbarSpanStyle(SpeakLinkRichTextView richTextView) : base(richTextView)
    {
    }

    public override RichEditorStyle RichEditorStyle => RichEditorStyle.BulletList;
}
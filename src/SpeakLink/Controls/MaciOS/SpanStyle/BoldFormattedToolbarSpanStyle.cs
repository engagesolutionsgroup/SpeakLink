using SpeakLink.RichText;
using UIKit;

namespace SpeakLink.Controls.MaciOS.SpanStyle;

public class BoldFormattedToolbarSpanStyle : TraitFormattedToolbarSpanStyle
{
    public BoldFormattedToolbarSpanStyle(SpeakLinkRichTextView richTextView)
        : base(richTextView)
    {
    }
    public override RichEditorStyle RichEditorStyle => RichEditorStyle.Bold;
    public override UIFontDescriptorSymbolicTraits SymbolicTrait => UIFontDescriptorSymbolicTraits.Bold;
}
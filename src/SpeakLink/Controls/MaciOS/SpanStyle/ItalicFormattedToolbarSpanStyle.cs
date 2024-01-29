using SpeakLink.RichText;
using UIKit;

namespace SpeakLink.Controls.MaciOS.SpanStyle;

public class ItalicFormattedToolbarSpanStyle : TraitFormattedToolbarSpanStyle
{
    public override RichEditorStyle RichEditorStyle => RichEditorStyle.Italic;
    
    public ItalicFormattedToolbarSpanStyle(SpeakLinkRichTextView richTextView)
        : base(richTextView)
    {
    }

    public override UIFontDescriptorSymbolicTraits SymbolicTrait => UIFontDescriptorSymbolicTraits.Italic;
}
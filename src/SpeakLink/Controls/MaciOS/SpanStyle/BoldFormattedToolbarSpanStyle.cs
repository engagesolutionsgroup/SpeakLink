using Foundation;
using SpeakLink.RichText;
using UIKit;

namespace SpeakLink.Controls.MaciOS.SpanStyle;

public class BoldFormattedToolbarSpanStyle : TraitFormattedToolbarSpanStyle
{
    private static readonly NSString OriginalFontKey = new NSString("NSOriginalFont");
    
    public BoldFormattedToolbarSpanStyle(SpeakLinkRichTextView richTextView)
        : base(richTextView)
    {
    }

    public override RichEditorStyle RichEditorStyle => RichEditorStyle.Bold;
    public override UIFontDescriptorSymbolicTraits SymbolicTrait => UIFontDescriptorSymbolicTraits.Bold;

    public override bool CheckSpanForRange(NSRange? selectedRange)
    {
        if (selectedRange == null)
            return false;

        bool hasTrait = true;

        // Loop through the characters in the selected range
        RichTextView.AttributedText.EnumerateAttributes(
            selectedRange.Value,
            NSAttributedStringEnumeration.None,
            (NSDictionary attrs, NSRange range, ref bool stop) =>
            {
                // Skip attributes that only contain HKWMentionAttribute
                if (attrs.ContainsKey(new NSString(SpeakLinkMentionTextView.HkwMentionAttributeName))
                    && selectedRange.Value.Location == range.Location && selectedRange.Value.Length == range.Length)
                {
                    hasTrait = false;
                    return;
                }
                
                // Check for bold attribute
                var originalFont = attrs[OriginalFontKey] as UIFont;
                var font = originalFont ?? attrs[UIStringAttributeKey.Font] as UIFont;
                

                if (font != null)
                {
                    if ((font.FontDescriptor.SymbolicTraits & SymbolicTrait) != 0)
                        return;
                    else
                    {
                        hasTrait = false;
                        stop = true;
                    }
                }
                else
                {
                    hasTrait = false;
                    stop = true;
                }
            });

        return hasTrait;
    }
}
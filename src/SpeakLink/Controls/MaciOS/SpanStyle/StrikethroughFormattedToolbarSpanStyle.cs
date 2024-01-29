using Foundation;
using LinkedIn.Hakawai;
using SpeakLink.RichText;
using UIKit;

namespace SpeakLink.Controls.MaciOS.SpanStyle;

public class StrikethroughFormattedToolbarSpanStyle : FormattedToolbarSpanStyle
{
    public StrikethroughFormattedToolbarSpanStyle(SpeakLinkRichTextView richTextView) : base(richTextView)
    {
    }

    public override RichEditorStyle RichEditorStyle => RichEditorStyle.Strikethrough;

    public override void ApplyStyle(NSRange range, HKWTextView textView, bool newState)
    {
        RichTextView.TransformTextAtRange(range, inputString =>
            AddAttributeToString(inputString, UIStringAttributeKey.StrikethroughStyle, NSNumber.FromInt32(newState ? (int)NSUnderlineStyle.Single : 0)));
        RichTextView.SelectedRange = range;
    }

    public override void UpdateTypingAttributes(NSDictionary? typingAttributes)
    {
        var inputAttributes = RichTextView.CustomTypingAttributes;
        inputAttributes[UIStringAttributeKey.StrikethroughStyle] =
            NSNumber.FromInt32(Checked ? (int)NSUnderlineStyle.Single : 0);
        RichTextView.CustomTypingAttributes = inputAttributes;
    }

    public override bool CheckSpanForRange(NSRange? selectedRange)
    {
        if (selectedRange == null)
            return false;
    
        bool hasUnderline = true;
        RichTextView.AttributedText.EnumerateAttribute(UIStringAttributeKey.StrikethroughStyle, selectedRange.Value,
            NSAttributedStringEnumeration.LongestEffectiveRangeNotRequired, Callback);

        void Callback(NSObject value, NSRange range, ref bool stop)
        {
            if (value == null || ((NSNumber)value).Int32Value == (int)NSUnderlineStyle.None)
            {
                hasUnderline = false;
                stop = true; // Stop the enumeration
            }
        }

        return hasUnderline;
    }
}
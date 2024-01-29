using Foundation;
using LinkedIn.Hakawai;
using SpeakLink.RichText;
using UIKit;

namespace SpeakLink.Controls.MaciOS.SpanStyle;

public class UnderlineFormattedToolbarSpanStyle : FormattedToolbarSpanStyle
{
    public UnderlineFormattedToolbarSpanStyle(SpeakLinkRichTextView richTextView) : base(richTextView)
    {
    }

    public override RichEditorStyle RichEditorStyle => RichEditorStyle.Underline;

    public override void ApplyStyle(NSRange range, HKWTextView textView, bool newState)
    {
        RichTextView.TransformTextAtRange(range, inputString =>
            AddAttributeToString(inputString, UIStringAttributeKey.UnderlineStyle,
                NSNumber.FromInt32(newState ? (int)NSUnderlineStyle.Single : 0)));
        RichTextView.SelectedRange = range;
    }

    public override void UpdateTypingAttributes(NSDictionary? typingAttributes)
    {
        var currentTypingAttributes = RichTextView.CustomTypingAttributes;
        currentTypingAttributes[UIStringAttributeKey.UnderlineStyle] =
            NSNumber.FromInt32(Checked ? (int)NSUnderlineStyle.Single : 0);
        RichTextView.CustomTypingAttributes = currentTypingAttributes;
    }

    public override bool CheckSpanForRange(NSRange? selectedRange)
    {
        if (selectedRange == null)
            return false;

        bool hasUnderline = true;
        RichTextView.AttributedText.EnumerateAttribute(UIStringAttributeKey.UnderlineStyle, selectedRange.Value,
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
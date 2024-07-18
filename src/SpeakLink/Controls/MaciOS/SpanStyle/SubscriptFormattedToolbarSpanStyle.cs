using Foundation;
using LinkedIn.Hakawai;
using SpeakLink.RichText;
using UIKit;

namespace SpeakLink.Controls.MaciOS.SpanStyle;

public class SubscriptFormattedToolbarSpanStyle : FormattedToolbarSpanStyle
{
    public SubscriptFormattedToolbarSpanStyle(SpeakLinkRichTextView richTextView) : base(richTextView)
    {
    }

    public override RichEditorStyle RichEditorStyle => RichEditorStyle.Subscript;

    public override void ApplyStyle(NSRange range, HKWTextView textView, bool newState)
    {
        RichTextView.TransformTextAtRange(range, inputString =>
        {
            AddAttributeToString(inputString, UIStringAttributeKey.BaselineOffset,
                new NSNumber(newState ? (textView.Font!.PointSize - 5) : 0));
            AddAttributeToString(inputString, UIStringAttributeKey.Font, textView.Font!.WithSize(textView.Font.PointSize-5));
            return inputString;

        });
        
        RichTextView.SelectedRange = range;
    }

    public override void UpdateTypingAttributes(NSDictionary? typingAttributes)
    {
        var currentTypingAttributes = RichTextView.CustomTypingAttributes;
        currentTypingAttributes[UIStringAttributeKey.BaselineOffset] =
            new NSNumber(Checked ? RichTextView.Font!.PointSize - 5 : 0);
        currentTypingAttributes[UIStringAttributeKey.Font] = RichTextView.Font!.WithSize(RichTextView.Font.PointSize - 5);
        RichTextView.CustomTypingAttributes = currentTypingAttributes;
    }

    public override bool CheckSpanForRange(NSRange? selectedRange)
    {
        if (selectedRange == null)
            return false;

        bool hasSubscript = true;
        RichTextView.AttributedText.EnumerateAttribute(UIStringAttributeKey.BaselineOffset, selectedRange.Value,
            NSAttributedStringEnumeration.LongestEffectiveRangeNotRequired, Callback);
        bool accessed = false;

        void Callback(NSObject value, NSRange range, ref bool stop)
        {
            if (value == null || ((NSNumber)value).Int32Value == 0)
            {
                hasSubscript = false;
                stop = true; // Stop the enumeration
            }

            accessed = true;
        }

        return accessed && hasSubscript;
    }
}

public class SuperscriptFormattedToolbarSpanStyle : FormattedToolbarSpanStyle
{
    public SuperscriptFormattedToolbarSpanStyle(SpeakLinkRichTextView richTextView) : base(richTextView)
    {
    }

    public override RichEditorStyle RichEditorStyle => RichEditorStyle.Superscript;

    public override void ApplyStyle(NSRange range, HKWTextView textView, bool newState)
    {
        RichTextView.TransformTextAtRange(range, inputString =>
        {
            AddAttributeToString(inputString, UIStringAttributeKey.BaselineOffset,
                new NSNumber(newState ? (textView.Font!.PointSize + 5) : 0));
            AddAttributeToString(inputString, UIStringAttributeKey.Font, textView.Font!.WithSize(textView.Font.PointSize-5));
            return inputString;
        });
    }

    public override void UpdateTypingAttributes(NSDictionary? typingAttributes)
    {
        var currentTypingAttributes = RichTextView.CustomTypingAttributes;
        currentTypingAttributes[UIStringAttributeKey.BaselineOffset] =
            new NSNumber(Checked ? RichTextView.Font!.PointSize + 5 : 0);
        currentTypingAttributes[UIStringAttributeKey.Font] = RichTextView.Font!.WithSize(RichTextView.Font.PointSize - 5);
        RichTextView.CustomTypingAttributes = currentTypingAttributes;
    }

    public override bool CheckSpanForRange(NSRange? selectedRange)
    {
        if (selectedRange == null)
            return false;

        bool hasSubscript = true;
        RichTextView.AttributedText.EnumerateAttribute(UIStringAttributeKey.BaselineOffset, selectedRange.Value,
            NSAttributedStringEnumeration.LongestEffectiveRangeNotRequired, Callback);
        bool accessed = false;

        void Callback(NSObject value, NSRange range, ref bool stop)
        {
            if (value == null || ((NSNumber)value).Int32Value == 0)
            {
                hasSubscript = false;
                stop = true; // Stop the enumeration
            }

            accessed = true;
        }

        return accessed && hasSubscript;
    }
}
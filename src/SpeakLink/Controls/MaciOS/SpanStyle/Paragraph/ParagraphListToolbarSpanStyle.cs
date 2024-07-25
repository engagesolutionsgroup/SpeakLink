using Foundation;
using LinkedIn.Hakawai;
using UIKit;

namespace SpeakLink.Controls.MaciOS.SpanStyle.Paragraph;
#pragma warning disable CA1416
public abstract class ParagraphListToolbarSpanStyle : FormattedToolbarSpanStyle
{
    public abstract NSTextListMarkerFormats MarkerFormat { get; }

    public ParagraphListToolbarSpanStyle(SpeakLinkRichTextView richTextView) :
        base(richTextView)
    {
    }

    public override void ApplyStyle(NSRange range, HKWTextView textView, bool newState)
    {
        textView.TransformTextAtRange(range, inputString =>
        {
            var result = new NSMutableAttributedString(inputString);
            var inputStringRange = new NSRange(0, inputString.Length);
            if (newState)
            {
                result.AddAttribute(UIStringAttributeKey.ParagraphStyle, new NSMutableParagraphStyle()
                {
                    HeadIndent = 15f,
                    FirstLineHeadIndent = 15f,
                    TextLists = [new NSTextList(NSTextListMarkerFormats.Box, NSTextListOptions.PrependEnclosingMarker)]
                }, inputStringRange);
            }
            else
            {
                result.RemoveAttribute(UIStringAttributeKey.ParagraphStyle, inputStringRange);
                result.AddAttribute(UIStringAttributeKey.ParagraphStyle, NSParagraphStyle.Default, inputStringRange);
            }

            return result;
        });
    }

    public override void UpdateTypingAttributes(NSDictionary? typingAttributes)
    {
        var inputAttributes = RichTextView.CustomTypingAttributes;
        var existingParagraphStyle =
            (inputAttributes[UIStringAttributeKey.ParagraphStyle] as NSParagraphStyle)?.MutableCopy() as
            NSMutableParagraphStyle;
        if (Checked)
        {
            if (existingParagraphStyle is { TextLists.Length: 0 })
            {
                existingParagraphStyle.TextLists = [new NSTextList(MarkerFormat, NSTextListOptions.None)];
                existingParagraphStyle.HeadIndent = 15f;
                existingParagraphStyle.FirstLineHeadIndent = 15f;
                inputAttributes[UIStringAttributeKey.ParagraphStyle] = existingParagraphStyle;
            }
            else if (existingParagraphStyle is { TextLists: [not null] })
            {
                var defaultParagraphStyle = (NSMutableParagraphStyle)NSParagraphStyle.Default.MutableCopy();
                defaultParagraphStyle.TextLists =
                    [new NSTextList(MarkerFormat, NSTextListOptions.None)];
                defaultParagraphStyle.HeadIndent = 15f;
                defaultParagraphStyle.FirstLineHeadIndent = 15f;
                inputAttributes[UIStringAttributeKey.ParagraphStyle] = defaultParagraphStyle;
            }
            else if(existingParagraphStyle == null)
            {
                var defaultParagraphStyle = (NSMutableParagraphStyle)NSParagraphStyle.Default.MutableCopy();
                defaultParagraphStyle.TextLists =
                    [new NSTextList(MarkerFormat, NSTextListOptions.None)];
                defaultParagraphStyle.HeadIndent = 15f;
                defaultParagraphStyle.FirstLineHeadIndent = 15f;
                inputAttributes[UIStringAttributeKey.ParagraphStyle] = defaultParagraphStyle;
            }
        }
        else if (existingParagraphStyle != null)
        {
            existingParagraphStyle.TextLists = [];
            existingParagraphStyle.HeadIndent = 0;
            existingParagraphStyle.FirstLineHeadIndent = 0;
            inputAttributes[UIStringAttributeKey.ParagraphStyle] = existingParagraphStyle;
        }

        RichTextView.CustomTypingAttributes = inputAttributes;
    }

    public override bool CheckSpanForRange(NSRange? selectedRange)
    {
        if (selectedRange == null)
            return false;

        bool hasList = false;
        RichTextView.AttributedText.EnumerateAttribute(UIStringAttributeKey.ParagraphStyle, selectedRange.Value,
            NSAttributedStringEnumeration.LongestEffectiveRangeNotRequired, Callback);

        void Callback(NSObject value, NSRange range, ref bool stop)
        {
            if (value is NSParagraphStyle paragraphStyle)
            {
                hasList = paragraphStyle.TextLists.Length > 0;
                stop = true;
            }
        }

        return hasList;
    }
}
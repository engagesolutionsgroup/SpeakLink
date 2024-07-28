using Foundation;
using LinkedIn.Hakawai;
using UIKit;

namespace SpeakLink.Controls.MaciOS.SpanStyle.Paragraph;
#pragma warning disable CA1416
public abstract class ParagraphListToolbarSpanStyle : FormattedToolbarSpanStyle
{
    public abstract NSTextListMarkerFormats MarkerFormat { get; }

    public const int IndentSize = 15;

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
                    HeadIndent = IndentSize,
                    FirstLineHeadIndent = IndentSize,
                    TextLists = [new NSTextList(MarkerFormat, NSTextListOptions.PrependEnclosingMarker)]
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
                existingParagraphStyle.HeadIndent = IndentSize;
                existingParagraphStyle.FirstLineHeadIndent = IndentSize;
                inputAttributes[UIStringAttributeKey.ParagraphStyle] = existingParagraphStyle;
            }
            else if (existingParagraphStyle is { TextLists: { Length: > 0 } textLists }
                     && textLists[0].MarkerFormat != MarkerFormat)
            {
                var defaultParagraphStyle = (NSMutableParagraphStyle)NSParagraphStyle.Default.MutableCopy();
                defaultParagraphStyle.TextLists =
                    [new NSTextList(MarkerFormat, NSTextListOptions.None)];
                defaultParagraphStyle.HeadIndent = IndentSize;
                defaultParagraphStyle.FirstLineHeadIndent = IndentSize;
                inputAttributes[UIStringAttributeKey.ParagraphStyle] = defaultParagraphStyle;
            }
            else if (existingParagraphStyle == null)
            {
                var defaultParagraphStyle = (NSMutableParagraphStyle)NSParagraphStyle.Default.MutableCopy();
                defaultParagraphStyle.TextLists =
                    [new NSTextList(MarkerFormat, NSTextListOptions.None)];
                defaultParagraphStyle.HeadIndent = IndentSize;
                defaultParagraphStyle.FirstLineHeadIndent = IndentSize;
                inputAttributes[UIStringAttributeKey.ParagraphStyle] = defaultParagraphStyle;
            }
        }
        else if (existingParagraphStyle is { TextLists: { Length: > 0 } textLists }
                 && textLists[0].MarkerFormat == MarkerFormat)
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

        if (selectedRange.Value.Length == 0)
        {
            hasList = RichTextView.CustomTypingAttributes[UIStringAttributeKey.ParagraphStyle] is NSParagraphStyle
                      {
                          TextLists.Length: > 0
                      } paragraphStyle &&
                      paragraphStyle.TextLists[0].MarkerFormat == MarkerFormat;
        }
        else
        {
            RichTextView.AttributedText.EnumerateAttribute(UIStringAttributeKey.ParagraphStyle, selectedRange.Value,
                NSAttributedStringEnumeration.LongestEffectiveRangeNotRequired, Callback);

            void Callback(NSObject value, NSRange range, ref bool stop)
            {
                if (value is NSParagraphStyle paragraphStyle)
                {
                    hasList = paragraphStyle.TextLists.Length > 0 &&
                              paragraphStyle.TextLists[0].MarkerFormat == MarkerFormat;
                    stop = true;
                }
            }
        }

        return hasList;
    }
}
using Foundation;
using LinkedIn.Hakawai;
using SpeakLink.Link;
using SpeakLink.RichText;
using UIKit;

namespace SpeakLink.Controls.MaciOS.SpanStyle;

public class LinkFormattedToolbarSpanStyle : FormattedToolbarSpanStyle
{
    private (NSRange,NSString)? _currentTypingLink;
    

    public LinkFormattedToolbarSpanStyle(SpeakLinkRichTextView richTextView) : base(richTextView)
    {
    }

    public override RichEditorStyle RichEditorStyle => RichEditorStyle.Link;

    public override async void Toggle()
    {
        var text = GetTextForLinkDialog();
        var result = await (RichTextView?.LinkEditorDialogHandler.ShowLinkDialogAsync(text, _currentTypingLink?.Item2?.ToString()) ??
                            Task.FromResult(new LinkDialogResult()
                            {
                                Cancelled = true
                            }));
        if (result.Cancelled || string.IsNullOrWhiteSpace(result.Url))
            return;
        
        if(string.IsNullOrWhiteSpace(result.Text))
            result.Text = result.Url;
        InsertOrReplaceLinkAtRange(Checked ? _currentTypingLink!.Value.Item1 : RichTextView.SelectedRange,
            result.Text, result.Url);
    }
    

    public override void ApplyStyle(NSRange range, HKWTextView textView, bool newState)
    {
    }

    public void InsertOrReplaceLinkAtRange(NSRange range, string text, string link = "google.com")
    {
        RichTextView.TransformTextAtRange(range, (input) =>
        {
            var result = new NSMutableAttributedString(new NSAttributedString(text));
            result.AddAttribute(UIStringAttributeKey.Font, RichTextView.Font, new NSRange(0, result.Length));
            result.AddAttribute(UIStringAttributeKey.Link, new NSString(link), new NSRange(0, result.Length));
            return result;
        });
    }

    private string GetTextForLinkDialog()
    {
        if (_currentTypingLink == null)
            return RichTextView.SelectedRange.Length == 0
                ? string.Empty
                : RichTextView.Text?[(int)RichTextView.SelectedRange.Location ..
                      ((int)RichTextView.SelectedRange.Location + (int)RichTextView.SelectedRange.Length)] ??
                  string.Empty;

        return RichTextView.Text?[(int)_currentTypingLink.Value.Item1.Location ..
                   ((int)_currentTypingLink.Value.Item1.Location +
                    (int)_currentTypingLink.Value.Item1.Length)] ??
               string.Empty;
    }

    public override void UpdateTypingAttributes(NSDictionary? typingAttributes)
    {
        RichTextView.TransformTypingAttributesWithTransformer(inputAttributes =>
        {
            var mutableTypingAttributes = inputAttributes?.MutableCopy() as NSMutableDictionary;
            mutableTypingAttributes!.Remove(UIStringAttributeKey.Link);
            if (Checked && _currentTypingLink?.Item2 != null)
            {
                mutableTypingAttributes.Add(UIStringAttributeKey.Font, RichTextView.Font);
                mutableTypingAttributes.Add(UIStringAttributeKey.Link, _currentTypingLink?.Item2);
            }

            return mutableTypingAttributes;
        });
    }

    public override void OnSelectionChanged()
    {
        var selectedRange = RichTextView.SelectedRange;

        Checked = CheckSpanForRange(selectedRange);
        if (!Checked)
            _currentTypingLink = null;
        
        UpdateTypingAttributes(RichTextView.CustomTypingAttributes);
        RichTextView.CustomTypingAttributes = RichTextView.CustomTypingAttributes;
    }

    public override bool CheckSpanForRange(NSRange? selectedRange)
    {
        if (selectedRange == null || string.IsNullOrEmpty(RichTextView.Text))
            return false;
    
        try
        {
            List<(NSRange, NSString)?> linkRanges = new();
            RichTextView.AttributedText.EnumerateAttribute(UIStringAttributeKey.Link, new NSRange(0, RichTextView.AttributedText.Length), NSAttributedStringEnumeration.None, (NSObject value, NSRange range, ref bool stop) =>
            {
                if (value is NSString link)
                {
                    // Merge continuous ranges with the same link
                    if (linkRanges.Any() && range.Location == linkRanges[^1]!.Value.Item1.Location + linkRanges[^1]!.Value.Item1.Length
                                         && link.Equals(linkRanges[^1]!.Value.Item2))
                    {
                        var lastRange = linkRanges[^1]!.Value.Item1;
                        lastRange.Length += range.Length;
                        linkRanges[^1] = (lastRange, link);
                    }
                    else
                    {
                        linkRanges.Add((range, link));
                    }
                }
            });

            // Check if the selected range is exactly any of the merged link ranges
            NSRange selectedRangeValue = selectedRange.Value;
            (NSRange, NSString)? linkRange = null;
            
            if (selectedRangeValue.Length != 0)
            {
                linkRange = linkRanges.FirstOrDefault(searchRange =>
                    searchRange!.Value.Item1.Location <= selectedRangeValue.Location
                    && searchRange.Value.Item1.Location + searchRange.Value.Item1.Length >=
                    selectedRangeValue.Location + selectedRangeValue.Length);
            }
            else
            {
                linkRange = linkRanges.FirstOrDefault(searchRange =>
                    searchRange!.Value.Item1.Location < selectedRangeValue.Location
                    && searchRange.Value.Item1.Location + searchRange.Value.Item1.Length >
                    selectedRangeValue.Location + selectedRangeValue.Length);
            }

            _currentTypingLink = linkRange;
            return linkRange != null;
        }
        catch (Exception ex)
        {
            _currentTypingLink = null;
            // Ideally log the exception
            return false;
        }
    }
}
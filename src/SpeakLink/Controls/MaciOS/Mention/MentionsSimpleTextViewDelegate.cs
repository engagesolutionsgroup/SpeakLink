using System.Diagnostics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace SpeakLink.Controls.MaciOS;

public class MentionsSimpleTextViewDelegate : UITextViewDelegate
{
    private string? _textBefore;
    private readonly Action<string?, string?>? _textChangedAction;
    private readonly Action? _selectionChanged;
    private bool _paragraphActive;

    public MentionsSimpleTextViewDelegate(Action<string?, string?> textChangedAction,
        Action? selectionChanged)
    {
        _textChangedAction = textChangedAction;
        _selectionChanged = selectionChanged;
    }

    public MentionsSimpleTextViewDelegate()
    {
    }

    protected MentionsSimpleTextViewDelegate(NSObjectFlag t) : base(t)
    {
    }

    protected internal MentionsSimpleTextViewDelegate(NativeHandle handle) : base(handle)
    {
    }

    public override bool ShouldChangeText(UITextView textView, NSRange range, string text)
    {
        // Define if I am inside paragraph
        if (textView is not SpeakLinkRichTextView richTextView
            || richTextView.CustomTypingAttributes[UIStringAttributeKey.ParagraphStyle] is not NSParagraphStyle
            {
                TextLists.Length: > 0
            } paragraphStyle)
        {
            return true;
        }

        var currentText = textView.Text ?? string.Empty;

        if (text == Environment.NewLine && textView.LastTwoCharactersBeforeSelectedRange()?[^1] == '\n')
        {
            var rangeToCheck = new NSRange(range.Location, 1);
            // Check if the range is within the bounds of the text
            if (rangeToCheck.Location < currentText.Length)
            {
                var nextChar = currentText.Substring((int)rangeToCheck.Location, 1);
                if (nextChar == "\n")
                {
                    richTextView.TransformTextAtRange(rangeToCheck, attrStr =>
                    {
                        var mutableString = new NSMutableAttributedString(attrStr);
                        var stringRange = new NSRange(0, mutableString.Length);
                        mutableString.RemoveAttribute(UIStringAttributeKey.ParagraphStyle, stringRange);
                        mutableString.MutableString.SetString(new NSString(""));
                        return mutableString;
                    });

                    var newAttributes = richTextView.CustomTypingAttributes;
                    newAttributes[UIStringAttributeKey.ParagraphStyle] = NSParagraphStyle.Default;
                    richTextView.CustomTypingAttributes = newAttributes;

                    textView.SelectedRange = new NSRange(range.Location, 0);

                    return false;
                }
            }
        }

        if (!textView.IsTextAfterSelectedRangeEmpty())
            return true;

        // Case 1: Inserting new line after a numbered/bullet point in order to always render the bullet/number immediately
        // after new line.
        if (text == Environment.NewLine)
        {
            var nextCharRange = new NSRange(range.Location, 1);
            // Check if the range is within the bounds of the text
            if (nextCharRange.Location < currentText.Length)
            {
                var nextChar = currentText.Substring((int)nextCharRange.Location, (int)nextCharRange.Length);
                if (nextChar != "\n" && nextChar != " ")
                {
                    // Insert a new line after the second point
                    textView.InsertText("\n");
                    // Set the cursor position just after the inserted text
                    textView.SelectedRange = new NSRange(range.Location + 1, 0);
                    return true;
                }
            }
            else
            {
                if (!textView.Text.EndsWith(Environment.NewLine))
                {
                    textView.InsertText("\n");
                }

                textView.SelectedRange = range;
                return true;
            }
        }

        if (text == string.Empty)
        {
            var prevCharRange = new NSRange(range.Location - 1, 1);
            if (prevCharRange.Location >= 0 && prevCharRange.Location < text.Length)
            {
                var prevChar =
                    text[(int)prevCharRange.Location..((int)prevCharRange.Location + (int)prevCharRange.Length)];
                if (prevChar == Environment.NewLine)
                {
                    var rangeToDelete = new NSRange(range.Location, 1);
                    textView.TextStorage.Replace(rangeToDelete, "");
                    textView.SelectedRange = new NSRange(range.Location - 1, 0);
                    return true;
                }
            }
        }

        return true;
    }

    public override void Changed(UITextView textView)
    {
        _textChangedAction?.Invoke(_textBefore, textView.Text);
    }

    public override void SelectionChanged(UITextView textView)
    {
        if (textView is SpeakLinkRichTextView richTextView)
        {
            if (textView.SelectedRange.Length == 0)
            {
                _paragraphActive = richTextView.CustomTypingAttributes[UIStringAttributeKey.ParagraphStyle]
                    is NSParagraphStyle { TextLists.Length: > 0 };
            }
            else
            {
                _paragraphActive = richTextView.AttributedText.GetAttribute(UIStringAttributeKey.ParagraphStyle,
                    textView.SelectedRange.Location, out _) is NSParagraphStyle { TextLists.Length: > 0 };
            }
        }

        _selectionChanged?.Invoke();
    }
}

public static class TextViewExtensions
{
    public static bool IsTextAfterSelectedRangeEmpty(this UITextView textView)
    {
        var text = textView.Text;
        var selectedRange = textView.SelectedRange;
        if (selectedRange.Location >= text.Length)
            return true;

        var textAfterSelectedRange = text.Substring((int)selectedRange.Location);
        return string.IsNullOrWhiteSpace(textAfterSelectedRange);
    }

    public static string? LastTwoCharactersBeforeSelectedRange(this UITextView textView)
    {
        var text = textView.Text;
        if (string.IsNullOrEmpty(text))
            return null;

        var selectedRange = textView.SelectedRange;
        var startIndex = Math.Max(0, (int)selectedRange.Location - 2);
        var length = Math.Min((int)selectedRange.Location, 2);

        // Extract the last two characters before the selected range
        var substringRange = new NSRange(startIndex, length);

        if (startIndex + length > text.Length)
            return null;

        var lastTwoCharacters = text.Substring(startIndex, length);

        return lastTwoCharacters;
    }
}
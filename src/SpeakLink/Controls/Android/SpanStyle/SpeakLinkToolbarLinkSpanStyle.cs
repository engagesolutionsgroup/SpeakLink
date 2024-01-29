using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Android.Text;
using SpeakLink.Controls.Android.Spans;
using SpeakLink.Controls.Android.Toolbar;
using SpeakLink.Link;
using SpeakLink.RichText;

namespace SpeakLink.Controls.Android.SpanStyle;

public partial class SpeakLinkToolbarLinkSpanStyle : IAndroidToolbarSpanStyle
{
    private Command? _toggleCommand;
    public const string Http = "http://";
    public const string Https = "https://";

    public SpeakLinkToolbarLinkSpanStyle(SpeakLinkRichEditText engageRichEditor)
    {
        ParentEditText = engageRichEditor;
    }

    public SpeakLinkRichEditText ParentEditText { get; init; }
    public RichEditorStyle RichEditorStyle => RichEditorStyle.Link;
    public bool Checked { get; set; }
    
    public ICommand ToggleCommand => _toggleCommand ??= new Command(Toggle);
    public async void Toggle()
    {
        var (link, text) = GetLinkTextFromSelection();
        var linkDialogResult = await (ParentEditText?.LinkEditorDialogHandler?.ShowLinkDialogAsync(text, link) ??
                                      Task.FromResult(new LinkDialogResult()));
        if(linkDialogResult.Cancelled)
            return;
        
        InsertLink(linkDialogResult.Url, linkDialogResult.Text);
    }

    public void ApplyStyle(IEditable sequence, int inputStartPosition, int inputEndPosition)
    {
        DetectAndApplyLinkSpans(sequence, inputStartPosition, inputEndPosition);
    }

    private void DetectAndApplyLinkSpans(IEditable sequence, int inputStartPosition, int inputEndPosition)
    {
    }
    
    private bool IsValidUrl(string url) => UrlLinkRegex().IsMatch(url);

    private SpeakLinkLinkSpan? FindExistingSpan(IEditable sequence, int start, int end)
    {
        var spans = sequence.GetSpans<SpeakLinkLinkSpan>(start, end);
        // Additional logic to find the most relevant span, if any
        return spans.FirstOrDefault();
    }

    private void InsertLink(string url, string text) 
    {
        if (string.IsNullOrWhiteSpace(url)) 
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(text))
            text = url;

        if (!url.StartsWith(Http) && !url.StartsWith(Https))
            url = Http + url;
        
        
        if (ParentEditText != null)
        {
            var editable = ParentEditText.EditableText!;
            var start = Math.Min(ParentEditText.SelectionStart, ParentEditText.SelectionEnd);
            var end = Math.Max(ParentEditText.SelectionEnd, ParentEditText.SelectionStart);
            if (start == end)
                editable.Insert(start, text);
            else
                editable.Replace(start, end, text);

            editable.SetSpan(new SpeakLinkLinkSpan(url), start, start + text.Length, SpanTypes.ExclusiveExclusive);
        }
    }

    private (string? link, string? text) GetLinkTextFromSelection()
    {
        var selStart = ParentEditText.SelectionStart;
        var selEnd = ParentEditText.SelectionEnd;
        
        if (!Checked && selStart == selEnd)
            return (string.Empty, string.Empty);
        
        var editable = ParentEditText.EditableText!;
        if(selStart > 0 && selStart == selEnd)
        {
            var span = editable.GetSpans<SpeakLinkLinkSpan>(selStart - 1, selStart).FirstOrDefault();
            if (span != null)
            {
                var link = span.URL;
                var text = Java.Lang.ICharSequenceExtensions.SubSequence(editable, editable.GetSpanStart(span), editable.GetSpanEnd(span));
                ParentEditText.SetSelection(editable.GetSpanStart(span), editable.GetSpanEnd(span));
                return (link, text);
            }

            return (string.Empty, string.Empty);
        }
        else
        {
            var spans = editable.GetSpans<SpeakLinkLinkSpan>(selStart, selEnd);
            var text = Java.Lang.ICharSequenceExtensions.SubSequence(editable, selStart, selEnd);
           

            if (spans.Length > 1)
                return (string.Empty, text);
            
            return (spans.FirstOrDefault()?.URL, text);
        }
    }

    [GeneratedRegex("\\b(?:https?://|www\\.)\\S+\\b")]
    private static partial Regex UrlLinkRegex();

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
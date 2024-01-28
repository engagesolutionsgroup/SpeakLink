using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Foundation;
using LinkedIn.Hakawai;
using SpeakLink.RichText;

namespace SpeakLink.Controls.MaciOS.SpanStyle;

public abstract class FormattedToolbarSpanStyle : IAppleToolbarSpanStyle
{
    private bool _checked;
    private Command? _toggleCommand;
    public SpeakLinkRichTextView RichTextView { get; set; }

    public abstract RichEditorStyle RichEditorStyle { get; }

    public bool Checked
    {
        get => _checked;
        set => SetField(ref _checked, value);
    }
    
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
    
    protected FormattedToolbarSpanStyle(SpeakLinkRichTextView richTextView)
    {
        RichTextView = richTextView;
    }
    
    public virtual void Toggle()
    {
        var newState = !Checked;
        if (RichTextView.SelectedRange.Length == 0)
        {
            Checked = newState;
            UpdateTypingAttributes(RichTextView.CustomTypingAttributes);
        }
        else
            ApplyStyle(RichTextView.SelectedRange, RichTextView, newState);
    }

    public ICommand ToggleCommand => _toggleCommand ??= new Command(Toggle);

    private NSRange? GetRangeForCheck(NSRange selectedRange)
    {
        if (selectedRange.Length == 0)
        {
            if (RichTextView!.Text!.Length > 0 && RichTextView.Text.Length >= selectedRange.Location)
            {
                if (selectedRange.Location == 0)
                {
                    selectedRange.Length = 1;
                }
                else
                {
                    selectedRange.Location -= 1;
                    selectedRange.Length = 1;
                }
            }
            else
                return null;
        }

        return selectedRange;
    }
    
    
    public virtual void OnSelectionChanged()
    {
        Checked = CheckSpanForRange(GetRangeForCheck(RichTextView.SelectedRange));
        UpdateTypingAttributes(RichTextView.CustomTypingAttributes);
    }

    public abstract void ApplyStyle(NSRange range, HKWTextView textView, bool newState);

    public abstract void UpdateTypingAttributes(NSDictionary? typingAttributes);

    public abstract bool CheckSpanForRange(NSRange? selectedRange);

    protected NSAttributedString AddAttributeToString(NSAttributedString inputString,
        NSString key, NSObject value)
    {
        if (inputString.Length  == 0)
            return inputString;

        var mutableInput = new NSMutableAttributedString(inputString);
        mutableInput.RemoveAttribute(key,
            new NSRange(0, inputString.Length));
        mutableInput.AddAttribute(key,
            value,
            new NSRange(0,inputString.Length));
        return mutableInput;
    }
}
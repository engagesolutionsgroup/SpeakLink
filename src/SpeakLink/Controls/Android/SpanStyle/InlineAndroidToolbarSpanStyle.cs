using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Android.Text;
using SpeakLink.Controls.Android.Toolbar;
using SpeakLink.RichText;

namespace SpeakLink.Controls.Android.SpanStyle;

public class InlineToolbarSpanStyle<T> : IAndroidToolbarSpanStyle
    where T : Java.Lang.Object, new()
{
    private bool _checked;
    private ICommand? _toggleCommand;

    public RichEditorStyle RichEditorStyle { get; set; }
    public SpeakLinkRichEditText ParentEditText { get; init; }
    
    public bool Checked
    {
        get => _checked;
        set => SetField(ref _checked, value);
    }

    public ICommand ToggleCommand => _toggleCommand ??= new Command(Toggle);

    public InlineToolbarSpanStyle(SpeakLinkRichEditText editText, RichEditorStyle associatedXplatStyle)
    {
        ParentEditText = editText;
        RichEditorStyle = associatedXplatStyle;
    }
    
    public void Toggle()
    {
        if (ParentEditText?.EditableText != null)
        {
            Checked = !Checked;
            ApplyStyle(ParentEditText.EditableText,
                ParentEditText.SelectionStart,
                ParentEditText.SelectionEnd);
            ParentEditText.RaiseStyleChanged(RichEditorStyle);
        }
    }


    public T NewSpan() => new();
    
    public void ApplyStyle(IEditable? editable, int start, int end)
    {
        if (editable == null) throw new ArgumentNullException(nameof(editable));
        if (Checked)
        {
            if (end > start)
            {
                var spans = editable.GetSpans<T>(start, end);
                var existingTSpan = spans.FirstOrDefault();

                if (existingTSpan == null)
                {
                    CheckAndMergeSpan(editable, start, end);
                }
                else
                {
                    int existingTSpanStart = editable.GetSpanStart(existingTSpan);
                    int existingTSpanEnd = editable.GetSpanEnd(existingTSpan);
                    if (existingTSpanStart <= start && existingTSpanEnd >= end)
                    {
                        ChangeSpanInsideStyle(editable, start, end, existingTSpan);
                    }
                    else
                    {
                        CheckAndMergeSpan(editable, start, end);
                    }
                }
            }
            else
            {
                T[] spans = editable.GetSpans<T>(start, end);
                if (spans.Any())
                {
                    T span = spans[0];
                    int lastSpanStart = editable.GetSpanStart(span);
                    foreach (T e in spans)
                    {
                        int lastSpanStartTmp = editable.GetSpanStart(e);
                        if (lastSpanStartTmp > lastSpanStart)
                        {
                            lastSpanStart = lastSpanStartTmp;
                            span = e;
                        }
                    }

                    int eStart = editable.GetSpanStart(span);
                    int eTnd = editable.GetSpanEnd(span);

                    if (eStart >= eTnd)
                    {
                        editable.RemoveSpan(span);
                        ExtendPreviousSpan(editable, eStart);

                        Checked = false;
                        Extensions.UpdateCheckStatus(this, false);
                    }
                }
            }
        }
        else
        {
            if (end > start)
            {
                T[] spans = editable.GetSpans<T>(start, end);
                if (spans.Length > 0)
                {
                    if (spans.FirstOrDefault() is T span)
                    {
                        int ess = editable.GetSpanStart(span);
                        int ese = editable.GetSpanEnd(span);
                        if (start >= ese)
                        {
                            editable.RemoveSpan(span);
                            editable.SetSpan(span, ess, start - 1, SpanTypes.ExclusiveInclusive);
                        }
                        else if (start == ess && end == ese)
                        {
                            editable.RemoveSpan(span);
                        }
                        else if (start > ess && end < ese)
                        {
                            editable.RemoveSpan(span);
                            T spanLeft = NewSpan();
                            editable.SetSpan(spanLeft, ess, start, SpanTypes.ExclusiveInclusive);
                            T spanRight = NewSpan();
                            editable.SetSpan(spanRight, end, ese, SpanTypes.ExclusiveInclusive);
                        }
                        else if (start == ess && end < ese)
                        {
                            editable.RemoveSpan(span);
                            T newSpan = NewSpan();
                            editable.SetSpan(newSpan, end, ese, SpanTypes.ExclusiveInclusive);
                        }
                        else if (start > ess && end == ese)
                        {
                            editable.RemoveSpan(span);
                            T newSpan = NewSpan();
                            editable.SetSpan(newSpan, ess, start, SpanTypes.ExclusiveInclusive);
                        }
                    }
                }
            }
            else if (end == start)
            {
                // User changes focus position
                // Do nothing for this case
            }
            else
            {
                T[] spans = editable.GetSpans<T>(start, end);
                if (spans.Any())
                {
                    T? span = spans.FirstOrDefault();
                    if (span != null)
                    {
                        int eStart = editable.GetSpanStart(span);
                        int eTnd = editable.GetSpanEnd(span);

                        if (eStart < eTnd)
                        {
                            Checked = true;
                            Extensions.UpdateCheckStatus(this, true);
                        }
                    }
                }
            }
        }
    }

    protected virtual void ChangeSpanInsideStyle(ISpannable editable, int start, int end, T e)
    {
        // Android.Util.Log.T("ART", "in side a span!!");
        // Do nothing by default
    }

    private void CheckAndMergeSpan(IEditable editable, int start, int end)
    {
        T? leftSpan = null;
        T?[] leftSpans = editable.GetSpans<T>(start, start);
        if (leftSpans.Length > 0)
        {
            leftSpan = leftSpans[0];
        }

        T? rightSpan = null;
        T?[] rightSpans = editable.GetSpans<T>(end, end);
        if (rightSpans.Length > 0)
        {
            rightSpan = rightSpans[0];
        }

        int leftSpanStart = editable.GetSpanStart(leftSpan);
        int rightSpanEnd = editable.GetSpanEnd(rightSpan);
        RemoveAllSpans(editable, start, end);
        if (leftSpan != null && rightSpan != null)
        {
            T tSpan = NewSpan();
            editable.SetSpan(tSpan, leftSpanStart, rightSpanEnd, SpanTypes.ExclusiveInclusive);
        }
        else if (leftSpan != null)
        {
            T tSpan = NewSpan();
            editable.SetSpan(tSpan, leftSpanStart, end, SpanTypes.ExclusiveInclusive);
        }
        else if (rightSpan != null)
        {
            T tSpan = NewSpan();
            editable.SetSpan(tSpan, start, rightSpanEnd, SpanTypes.ExclusiveInclusive);
        }
        else
        {
            T tSpan = NewSpan();
            editable.SetSpan(tSpan, start, end, SpanTypes.ExclusiveInclusive);
        }
    }

    private void RemoveAllSpans(ISpannable? editable, int start, int end)
    {
        var allSpans = editable.GetSpans<T>(start, end);
        foreach (T span in allSpans)
        {
            editable.RemoveSpan(span);
        }
    }

    protected virtual void ExtendPreviousSpan(ISpannable editable, int pos)
    {
        // Do nothing by default
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<TK>(ref TK field, TK value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<TK>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

internal static class Extensions
{
    public static T[] GetSpans<T>(this IEditable? editable, int start, int end)
    {
        var spans = editable?.GetSpans(start, end,
            Java.Lang.Class.FromType(typeof(global::Android.Text.Style.CharacterStyle)));
        return
            spans?.OfType<T>().ToArray() ?? Array.Empty<T>();
    }

    public static T[] GetSpans<T>(this ISpannable? editable, int start, int end)
    {
        var spans = editable?.GetSpans(start, end,
            Java.Lang.Class.FromType(typeof(global::Android.Text.Style.CharacterStyle)));
        return
            spans?.OfType<T>().ToArray() ?? Array.Empty<T>();
    }

    public static void UpdateCheckStatus<T>(InlineToolbarSpanStyle<T> spanStyle, bool newCheckedValue)
        where T : Java.Lang.Object, new()
    {
        Debug.WriteLine("UpdateCheckStatus");
    }
}
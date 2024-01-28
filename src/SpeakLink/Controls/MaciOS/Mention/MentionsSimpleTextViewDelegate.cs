using Foundation;
using ObjCRuntime;
using UIKit;

namespace SpeakLink.Controls.MaciOS;

public class MentionsSimpleTextViewDelegate : UITextViewDelegate
{
    private string? _textBefore;
    private readonly Action<string?, string?>? _textChangedAction;
    private readonly Action? _selectionChanged;

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

    public override void Changed(UITextView textView)
    {
        _textChangedAction?.Invoke(_textBefore, textView.Text);
    }

    public override void SelectionChanged(UITextView textView)
    {
        _selectionChanged?.Invoke();
    }
}
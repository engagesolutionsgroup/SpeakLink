using Foundation;
using ObjCRuntime;
using UIKit;

namespace SpeakLink.Controls.MaciOS;

public class MentionsSimpleTextViewDelegate : UITextViewDelegate
{
    private string? _textBefore;
    private readonly Action<string?, string?>? _action;
    
    public MentionsSimpleTextViewDelegate(Action<string?, string?> action)
    {
        _action = action;
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
        _textBefore = textView.Text;
        return true;
    }

    public override void Changed(UITextView textView)
    {
        _action?.Invoke(_textBefore, textView.Text);
    }
}
using Android.Runtime;
using Android.Text;
using Java.Lang;

namespace SpeakLink.Controls.Android;

public class SpeakLinkMentionTextWatcher : Java.Lang.Object, ITextWatcher
{
    private readonly Action<string?, string?>? _invokeTextChangedAction;
    private string? _beforeTextChanged;

    public SpeakLinkMentionTextWatcher(Action<string?, string?> invokeTextChangedAction)
    {
        _invokeTextChangedAction = invokeTextChangedAction;
    }

    public SpeakLinkMentionTextWatcher(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
    {
    }

    public void AfterTextChanged(IEditable? s)
    {
        _invokeTextChangedAction?.Invoke(_beforeTextChanged, s?.ToString());
        _beforeTextChanged = null;
    }

    public void BeforeTextChanged(ICharSequence? s, int start, int count, int after)
    {
        _beforeTextChanged = s.ToString();
    }

    public void OnTextChanged(ICharSequence? s, int start, int before, int count)
    {
       
    }
}
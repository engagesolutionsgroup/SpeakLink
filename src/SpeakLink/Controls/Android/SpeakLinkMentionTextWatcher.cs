using Android.Graphics;
using Android.Runtime;
using Android.Text;
using Java.Lang;
using LinkedIn.Spyglass.Mentions;
using LinkedIn.Spyglass.Ui;

namespace SpeakLink.Controls.Android;

public class SpeakLinkMentionTextWatcher : Java.Lang.Object, ITextWatcher, MentionsEditText.IMentionWatcher
{
    private readonly Action<string?, string?>? _invokeTextChangedAction;
    private readonly Action<IMentionable, string, int, int>? onMentionAddedRef;
    
    private string? _beforeTextChanged;
    
    public SpeakLinkMentionTextWatcher(Action<string?, string?> invokeTextChangedAction,
        Action<IMentionable, string, int, int>? onMentionAdded = null)
    {
        _invokeTextChangedAction = invokeTextChangedAction;
        onMentionAddedRef = onMentionAdded;
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
        _beforeTextChanged = s?.ToString();
    }

    public void OnTextChanged(ICharSequence? s, int start, int before, int count)
    {
    }

    public void OnMentionAdded(IMentionable mentionable, string text, int start, int end)
    {
        onMentionAddedRef?.Invoke(mentionable, text, start, end);
    }

    public void OnMentionDeleted(IMentionable mentionable, string text, int start, int end)
    {
    }

    public void OnMentionPartiallyDeleted(IMentionable mentionable, string text, int start, int end)
    {
    }
}
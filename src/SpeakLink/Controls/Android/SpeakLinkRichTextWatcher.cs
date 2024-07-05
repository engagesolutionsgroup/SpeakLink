using Android.Runtime;
using Android.Text;
using Java.Lang;
using LinkedIn.Spyglass.Mentions;
using LinkedIn.Spyglass.Ui;
using SpeakLink.Controls.Android.Toolbar;

namespace SpeakLink.Controls.Android;

public class SpeakLinkRichTextWatcher : Java.Lang.Object, ITextWatcher, MentionsEditText.IMentionWatcher
{
    private readonly Action<string?, string?>? _textChangedCallback;
    private bool _ignoreNextTextChange = false;
    private int _inputStartPosition = 0;
    private int _inputEndPosition = 0;
    private string? _beforeTextChanged;

    public AndroidRichToolbarState EngageRichToolbarState { get; }
    
    public SpeakLinkRichTextWatcher(AndroidRichToolbarState engageRichToolbarState, Action<string?,string?>? textChangedCallback)
    {
        _textChangedCallback = textChangedCallback;
        EngageRichToolbarState = engageRichToolbarState;
    }


    public SpeakLinkRichTextWatcher(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
    {
    }

    public void AfterTextChanged(IEditable? sequence)
    {
        if (_ignoreNextTextChange) 
            return;

        Log($"AfterTextChanged:| {sequence?.ToString() ?? "NULL" }");


        if (_inputEndPosition <= _inputStartPosition)
        {
            Log($"After Text Change Delete: start {_inputStartPosition} end {_inputEndPosition} ");
        }

        foreach (var selectedStyle in EngageRichToolbarState.Styles)
        {
            selectedStyle.ApplyStyle(sequence, _inputStartPosition, _inputEndPosition);
        }
        
        if(!_ignoreNextTextChange)
            _textChangedCallback?.Invoke(_beforeTextChanged, sequence?.ToString());
    }

    public void BeforeTextChanged(ICharSequence? sequence, int start, int count, int after)
    {
        _beforeTextChanged = sequence?.ToString();
        if (_ignoreNextTextChange)
            return;

        Log($"BeforeTextChanged:| Sequence {sequence}, Start {start}, Count {count}, After {after}");
    }

    public void OnTextChanged(ICharSequence? sequence, int start, int before, int count)
    {
        if (_ignoreNextTextChange)
            return;

        Log($"OnTextChanged:| Sequence {sequence}, Start {start}, Count {count}, Before {before}");
        
        _inputStartPosition = start;
        _inputEndPosition = start + count;
    }
    
    public void IgnoreNextTextChange(bool ignore) 
    {
        _ignoreNextTextChange = ignore;
    }

    private static void Log(string message) 
        => global::Android.Util.Log.Debug(nameof(SpeakLinkRichTextWatcher), message);

    public void OnMentionAdded(IMentionable mentionable, string text, int start, int end)
    {
    }

    public void OnMentionDeleted(IMentionable mentionable, string text, int start, int end)
    {
    }

    public void OnMentionPartiallyDeleted(IMentionable mentionable, string text, int start, int end)
    {
    }
}
using Android.Runtime;
using Android.Text;
using Java.Lang;
using SpeakLink.Controls.Android.Toolbar;

namespace SpeakLink.Controls.Android;

public class SpeakLinkRichTextWatcher : Java.Lang.Object, ITextWatcher
{
    private readonly Action<string?, string?>? _textChangedCallback;
    private bool ignoreNextTextChange = false;
    private int inputStartPosition = 0;
    private int inputEndPosition = 0;
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
        if (ignoreNextTextChange) 
            return;

        Log($"AfterTextChanged:| {sequence?.ToString() ?? "NULL" }");


        if (inputEndPosition <= inputStartPosition)
        {
            Log($"After Text Change Delete: start {inputStartPosition} end {inputEndPosition} ");
        }

        foreach (var selectedStyle in EngageRichToolbarState.Styles)
        {
            selectedStyle.ApplyStyle(sequence, inputStartPosition, inputEndPosition);
        }
        
        if(!ignoreNextTextChange)
            _textChangedCallback?.Invoke(_beforeTextChanged, sequence?.ToString());
    }

    public void BeforeTextChanged(ICharSequence? sequence, int start, int count, int after)
    {
        _beforeTextChanged = sequence?.ToString();
        if (ignoreNextTextChange)
            return;

        Log($"BeforeTextChanged:| Sequence {sequence}, Start {start}, Count {count}, After {after}");
    }

    public void OnTextChanged(ICharSequence? sequence, int start, int before, int count)
    {
        if (ignoreNextTextChange)
            return;

        Log($"OnTextChanged:| Sequence {sequence}, Start {start}, Count {count}, Before {before}");
        
        inputStartPosition = start;
        inputEndPosition = start + count;
    }
    
    public void IgnoreNextTextChange(bool ignore) 
    {
        ignoreNextTextChange = ignore;
    }

    private static void Log(string message) 
        => global::Android.Util.Log.Debug(nameof(SpeakLinkRichTextWatcher), message);
}
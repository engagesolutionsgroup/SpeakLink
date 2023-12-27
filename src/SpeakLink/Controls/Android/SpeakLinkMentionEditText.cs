using System.Windows.Input;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Java.Lang;
using LinkedIn.Spyglass.Mentions;
using LinkedIn.Spyglass.Suggestions;
using LinkedIn.Spyglass.Tokenization;
using LinkedIn.Spyglass.Ui;
using SpeakLink.Mention;
using TextChangedEventArgs = Microsoft.Maui.Controls.TextChangedEventArgs;

namespace SpeakLink.Controls.Android;

// All the code in this file is only included on Android.
public class SpeakLinkMentionEditText : MentionsEditText, IQueryTokenReceiver, ISuggestionsVisibilityManager
{
    private WordTokenizerConfig _wordTokenizer;
    private readonly List<string> _bucket = new();
    private SpeakLinkMentionTextWatcher? _speakLinkMentionTextWatcher;
    private bool _textWatcherAdded = false;
    
    internal new event  EventHandler<TextChangedEventArgs> OnTextChanged;
    public event EventHandler<bool> DisplaySuggestionChanged;
    private string _explicitChars = "@";
    private MentionSpanConfig _mentionSpanConfig;
    private bool _ignoreTextChangeNotification;


    protected SpeakLinkMentionEditText(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer) 
        => Initialize();

    public SpeakLinkMentionEditText(Context context) : base(context)
        => Initialize();

    public SpeakLinkMentionEditText(Context context, IAttributeSet? attrs) : base(context, attrs)
        => Initialize();

    public SpeakLinkMentionEditText(Context context, IAttributeSet? attrs, int defStyle) : base(context, attrs, defStyle)
        => Initialize();

    public void SetText(string text)
    {
        _ignoreTextChangeNotification = true;
        Text = text;
        _ignoreTextChangeNotification = false;
    }
    
    protected virtual void Initialize()
    {
        _speakLinkMentionTextWatcher = new SpeakLinkMentionTextWatcher(InvokeOnTextChanged);
        SetMentionSpanConfig(new MentionSpanConfig.Builder().Build());
    }

    private void InvokeOnTextChanged(string? oldValue, string? newValue)
    {
        if (_ignoreTextChangeNotification)
            return;
        
        OnTextChanged?.Invoke(this, new TextChangedEventArgs(oldValue, newValue));
    }

    public event EventHandler<MentionSearchEventArgs> MentionSearched;

    public bool IsMentionsEnabled { get; set; }

    public virtual void UpdateTokenizer(string explicitChars)
    {
        if(_explicitChars == explicitChars)
            return;
        
        _explicitChars = explicitChars;
        SetupMentions(explicitChars);
    }

    public void SetupMentions(string explicitChars = "@",
        int maxNumKeywords = 2,
        int threshold = 150)
    {
        _explicitChars = explicitChars;
        _wordTokenizer = new WordTokenizerConfig
                .Builder()
            .SetExplicitChars(explicitChars)
            .SetMaxNumKeywords(maxNumKeywords)
            .SetThreshold(threshold)
            .Build();
        
        this.Tokenizer = new WordTokenizer(_wordTokenizer);

        SetQueryTokenReceiver(this);
        SetSuggestionsVisibilityManager(this);

        if (!_textWatcherAdded)
            AddTextChangedListener(_speakLinkMentionTextWatcher);
    }

    public IList<string> OnQueryReceived(QueryToken queryToken)
    {
        if (!queryToken.IsExplicit)
            return _bucket;

        var args = new MentionSearchEventArgs(queryToken.ExplicitChar.ToString(), queryToken.Keywords);
        MentionSearched?.Invoke(this, args);
        DisplaySuggestions(true);

        if (MentionSearchCommand?.CanExecute(args) ?? false)
            MentionSearchCommand?.Execute(args);

        return _bucket;
    }

    public void DisplaySuggestions(bool display)
    {
        DisplaySuggestionChanged?.Invoke(this, display);
        IsDisplayingSuggestions = display;
    }

    public bool IsDisplayingSuggestions { get; set; }

    public ICommand MentionSearchCommand { get; set; }

    public MentionSpanConfig MentionSpanConfig
    {
        get => _mentionSpanConfig;
        set => SetMentionSpanConfig(value);
    }

    public override void SetMentionSpanConfig(MentionSpanConfig config)
    {
        base.SetMentionSpanConfig(config);
        _mentionSpanConfig = config;
    }

    public void SetTextFormattedFromBinding(ICharSequence? convertToSpannableText)
    {
        _ignoreTextChangeNotification = true;
        if (convertToSpannableText != null)
            TextFormatted = new MentionsEditable(convertToSpannableText);
        else
            Text = null;
        _ignoreTextChangeNotification = false;
    }
}


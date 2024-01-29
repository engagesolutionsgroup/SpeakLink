using System.Diagnostics;
using System.Windows.Input;
using CoreGraphics;
using Foundation;
using LinkedIn.Hakawai;
using ObjCRuntime;
using SpeakLink.Mention;
using UIKit;
using TextChangedEventArgs = Microsoft.Maui.Controls.TextChangedEventArgs;

namespace SpeakLink.Controls.MaciOS;

public class SpeakLinkMentionTextView : HKWTextView
{
    public const string HkwMentionAttributeName = "HKWMentionAttributeName";
    public static readonly NSString HkwMentionAttributeNameString = new(HkwMentionAttributeName);
    private HKWMentionsPluginV2? _mentionsPlugin;
    private ICommand? _mentionSearchCommand;
    private MentionsHkwMentionsCustomChooserViewDelegate? _chooserViewDelegate;
    private MentionsHkwMentionsStateChangeDelegate? _stateChangeDelegate;
    private bool _chooserViewVisible;

    private UILabel? _placeholderLabel;
    private string? _mentionExplicitCharacter;
    
    protected bool IgnoreTextChangeNotification;

    public event EventHandler<MentionSearchEventArgs>? MentionSearched;
    public event EventHandler<bool>? DisplaySuggestionChanged;
    public event EventHandler<bool>? FirstResponderStateChanged;


    public NSDictionary? MentionUnselectedAttributes
    {
        get => (NSDictionary?)_mentionsPlugin?.ValueForKey(new NSString("mentionUnhighlightedAttributes"))!;
        set => _mentionsPlugin?.SetValueForKey(value, new NSString("mentionUnhighlightedAttributes"));
    }

    public NSDictionary? MentionSelectedAttributes
    {
        get => (NSDictionary?)_mentionsPlugin?.ValueForKey(new NSString("mentionHighlightedAttributes"));
        set => _mentionsPlugin?.SetValueForKey(value, new NSString("mentionHighlightedAttributes"));
    }

    internal UIColor? MentionTextColor
    {
        get => MentionUnselectedAttributes?.TryGetValue(UIStringAttributeKey.ForegroundColor, out var color) == true
            ? color as UIColor
            : null;
        set
        {
            var mentionUnselectedAttributes = MentionUnselectedAttributes;
            MentionUnselectedAttributes = new NSMutableDictionary(mentionUnselectedAttributes)
            {
                { UIStringAttributeKey.ForegroundColor, value ?? UIColor.Black }
            };
        }
    }

    public UIColor? MentionSelectedTextColor
    {
        get => MentionSelectedAttributes?.TryGetValue(UIStringAttributeKey.ForegroundColor, out var color) == true
            ? color as UIColor
            : UIColor.White;
        set
        {
            var mentionSelectedAttributes = MentionSelectedAttributes;
            if (mentionSelectedAttributes == null)
                return;

            MentionSelectedAttributes = new NSMutableDictionary(mentionSelectedAttributes)
            {
                { UIStringAttributeKey.ForegroundColor, value ?? UIColor.Black }
            };
        }
    }

    public UIColor? MentionSelectedBackgroundColor
    {
        get => MentionSelectedAttributes?.TryGetValue(new NSString("HKWRoundedRectBackgroundAttributeName"),
            out var color) == true
            ? color as UIColor
            : UIColor.Black;
        set
        {
            var mentionSelectedAttributes = MentionSelectedAttributes;
            if (mentionSelectedAttributes == null)
                return;

            MentionSelectedAttributes = new NSMutableDictionary(mentionSelectedAttributes)
            {
                {
                    new NSString("HKWRoundedRectBackgroundAttributeName"),
                    HKWRoundedRectBackgroundAttributeValue.ValueWithBackgroundColor(value ?? UIColor.Black)
                }
            };
        }
    }

    private void SendOnMentionSearch(MentionSearchEventArgs e)
    {
        if (e.ControlCharacter != '\0'.ToString() && _chooserViewVisible == false)
            SendOnDisplaySuggestionsChanged(true);

        MentionSearched?.Invoke(this, e);
    }

    private void SendOnDisplaySuggestionsChanged(bool newState)
    {
        _chooserViewVisible = newState;
        DisplaySuggestionChanged?.Invoke(this, newState);
    }

    public SpeakLinkMentionTextView() => Initialize();

    public SpeakLinkMentionTextView(NSCoder coder) : base(coder)=> Initialize();

    protected SpeakLinkMentionTextView(NSObjectFlag t) : base(t)=> Initialize();

    protected internal SpeakLinkMentionTextView(NativeHandle handle) : base(handle)
    {
    }

    public SpeakLinkMentionTextView(CGRect frame, NSTextContainer? textContainer) 
        : base(frame, textContainer) => Initialize();

    public SpeakLinkMentionTextView(CGRect frame) : base(frame) => Initialize();

    protected virtual void Initialize()
    {
        
    }

    public bool IsMentionsEnabled { get; set; }

    public string? ExplicitCharacters
    {
        get => _mentionExplicitCharacter;
        set => SetupMentions(value);
    }

    public void SetText(string text)
    {
        IgnoreTextChangeNotification = true;
        Text = text;
        IgnoreTextChangeNotification = false;
    }

    public ICommand? MentionSearchCommand
    {
        get => _mentionSearchCommand;
        set
        {
            _mentionSearchCommand = value;
            if (_chooserViewDelegate != null)
                _chooserViewDelegate.MentionSearchCommand = value;
        }
    }

    public bool SuggestionPopupVisible
    {
        get => _chooserViewVisible;
        set => _chooserViewVisible = value;
    }

    public string PlaceholderText
    {
        get => _placeholderLabel?.Text ?? string.Empty;
        set => SetPlaceholderText(value);
    }

    public ICommand? ImageInputCommand { get; set; }

    public void SetupMentions(string? explicitCharacters = "@+＠")
    {
        if (string.IsNullOrWhiteSpace(explicitCharacters))
            return;

        if (_mentionExplicitCharacter == explicitCharacters)
            return;

        _mentionExplicitCharacter = explicitCharacters;
        var characterSet = NSCharacterSet.FromString(explicitCharacters);
        EnableMentionsPluginV2 = true;
        DirectlyUpdateQueryWithCustomDelegate = true;
        if (_mentionsPlugin != null)
        {
            _mentionsPlugin.Dispose();
            _chooserViewDelegate?.Dispose();
            _stateChangeDelegate?.Dispose();
        }

        _mentionsPlugin =
            HKWMentionsPluginV2.MentionsPluginWithChooserMode(
                HKWMentionsChooserPositionMode.CustomLockBottomNoArrow,
                characterSet, 1);
        _mentionsPlugin.NotifyTextViewDelegateOnMentionDeletion = true;
        _mentionsPlugin.NotifyTextViewDelegateOnMentionCreation = true;

        _chooserViewDelegate = new MentionsHkwMentionsCustomChooserViewDelegate
        {
            MentionSearchCommand = MentionSearchCommand,
            OnMentionSearch = SendOnMentionSearch
        };

        _stateChangeDelegate =
            new MentionsHkwMentionsStateChangeDelegate(SendOnDisplaySuggestionsChanged, OnMentionDeleted);

        _mentionsPlugin.CustomChooserViewDelegate = _chooserViewDelegate;
        _mentionsPlugin.StateChangeDelegate = _stateChangeDelegate;

        ControlFlowPlugin = _mentionsPlugin;
        WeakExternalDelegate = new MentionsSimpleTextViewDelegate(OnTextChangedDelegate, OnSelectionChanged);
    }

    private void OnMentionDeleted(HKWMentionsPlugin mentionsPlugin, HKWMentionsEntityProtocol entity)
        => OnTextChangedDelegate(null, Text);

    internal void OnTextChangedDelegate(string? oldValue, string? newValue)
    {
        HidePlaceholderIfTextIsPresent(Text);
        if (IgnoreTextChangeNotification)
            return;

        TextChanged?.Invoke(this, new TextChangedEventArgs(oldValue, newValue));
    }

    public event EventHandler<TextChangedEventArgs>? TextChanged;

    public void MentionSelected(IHKWMentionsEntityProtocol mention)
        => _mentionsPlugin?.HandleSelectionForEntity(mention);

    private void SetPlaceholderText(string value)
    {
        _placeholderLabel ??= CreatePlaceholderLabel();
        HidePlaceholderIfTextIsPresent(Text);
        _placeholderLabel.Text = value;
        AddSubview(_placeholderLabel);
    }

    public override NSAttributedString AttributedText
    {
        get => base.AttributedText;
        set
        {
            base.AttributedText = value;
            HidePlaceholderIfTextIsPresent(Text);
        }
    }

    public override void LayoutSubviews()
    {
        base.LayoutSubviews();
        UpdatePlaceholderLabelFrame();
    }

    private void HidePlaceholderIfTextIsPresent(string? value)
    {
        if (_placeholderLabel != null)
            _placeholderLabel.Hidden = !string.IsNullOrEmpty(value);
    }

    private void UpdatePlaceholderLabelFrame()
    {
        if (Bounds != CGRect.Empty && _placeholderLabel is not null)
        {
            var x = TextContainer.LineFragmentPadding;
            var y = TextContainerInset.Top;
            var width = Bounds.Width - x * 2;
            var height = Frame.Height - (TextContainerInset.Top + TextContainerInset.Bottom);

            _placeholderLabel.Frame = new CGRect(x, y, width, height);
        }
    }

    private UILabel CreatePlaceholderLabel() =>
        new()
        {
            Font = Font ?? UIFont.PreferredCaption1,
            TextColor = UIColor.LightGray
        };

    public void SetPlaceholderFont(UIFont newFont)
    {
        if (_placeholderLabel != null)
            _placeholderLabel.Font = newFont;
    }

    public override bool BecomeFirstResponder()
    {
        var baseResult = base.BecomeFirstResponder();
        if (baseResult)
            FirstResponderStateChanged?.Invoke(this, true);

        return baseResult;
    }

    public override bool ResignFirstResponder()
    {
        var baseResult = base.ResignFirstResponder();
        if (baseResult)
            FirstResponderStateChanged?.Invoke(this, true);
        return baseResult;
    }

    public override bool CanPerform(Selector action, NSObject? withSender)
    {
        const string pasteSelectorName = "paste:";
        if (action?.Name == pasteSelectorName && UIPasteboard.General.Image != null &&
            ImageInputCommand != null)
            return true;

        return base.CanPerform(action, withSender);
    }

    public override async void Paste(NSObject? sender)
    {
        var pasteboard = UIPasteboard.General;
        if (pasteboard.DataForPasteboardType(UIPasteboardExtensions.GifSelector) is { Length: > 0 }
            || (pasteboard.Image != null && (pasteboard.String?.StartsWith("data:image") ?? true)))
        {
            var image = await pasteboard.GetImageFromPasteboardAsync();
            if (ImageInputCommand?.CanExecute(image) ?? false)
            {
                ImageInputCommand.Execute(image);
                return;
            }
        }

        base.Paste(sender);
    }

    protected virtual void OnSelectionChanged()
    {
        
    }
}
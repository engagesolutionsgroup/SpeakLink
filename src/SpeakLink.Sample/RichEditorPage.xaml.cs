using SpeakLink.RichText;

namespace SpeakLink.Sample;

public partial class RichEditorMainPage : ContentPage
{
    public RichEditorMainPage()
    {
        InitializeComponent();

        BindingContext = new MentionsViewModel();

        Loaded += OnLoaded;
        Unloaded += UnLoaded;
    }

    private void UnLoaded(object? sender, EventArgs e)
    {
        Unloaded -= UnLoaded;
        RichEditor.Focused -= UpdateToolbarPosition;
        RichEditor.Unfocused -= UpdateToolbarPosition;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        Loaded -= OnLoaded;
        RichEditor.Focused += UpdateToolbarPosition;
        RichEditor.Unfocused += UpdateToolbarPosition;
    }

    private void UpdateToolbarPosition(object? sender, FocusEventArgs e)
    {
        if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
        {
            var additionalScrollFix = 50;
            TopContent.Margin = new(0, 0, 0, additionalScrollFix);
            BottomInputLayout.TranslateTo(0, e.IsFocused ? -additionalScrollFix : 0);
        }
    }
    
    private void HideKeyboard(object? sender, TappedEventArgs e)
    {
        if (!RichEditor.IsFocused)
            return;

        if (RichEditor.IsSuggestionsPopupVisible
            && e.GetPosition(MentionCollectionView) is { } position
            && position.InsideElement(MentionCollectionView))
        {
            return;
        }

        RichEditor.Unfocus();
    }

    private void UserTappedFromMentionSelector(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is User user)
            RichEditor.InsertMention(user.Id.ToString(), user.FullName);
    }
}
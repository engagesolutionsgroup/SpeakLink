namespace SpeakLink.Sample;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        this.MentionEditor.ImageInputCommand = new Command(ImageInsertFromKeyboard);
    }

    private void ImageInsertFromKeyboard(object obj)
    {
        if (obj is string imagePathFromTemp)
        {
            KeyboardInputImage.Source = imagePathFromTemp;
        }
    }

    private void HideKeyboard(object? sender, TappedEventArgs e)
    {
        if (!MentionEditor.IsFocused)
            return;
        
        if (MentionEditor.IsSuggestionsPopupVisible
            && e.GetPosition(MentionCollectionView) is { } position
            && position.InsideElement(MentionCollectionView))
        {
            return;
        }

        MentionEditor.Unfocus();
    }

    private void UserTappedFromMentionSelector(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is User user)
            MentionEditor.InsertMention(user.Id.ToString(), user.FullName);
    }
}
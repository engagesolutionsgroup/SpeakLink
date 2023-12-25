using Microsoft.Maui.Platform;

namespace SpeakLink.Sample;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void HideKeyboard(object? sender, TappedEventArgs e)
    {
        if (MentionEditor.IsSuggestionsPopupVisible 
            && e.GetPosition(MentionCollectionView) is { } position
            && position.Inside(MentionCollectionView))
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
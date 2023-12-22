namespace SpeakLink.Sample;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void SelectableItemsView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is User user)
            MentionEditor.InsertMention(user.Id.ToString(), user.FullName);
        (sender as CollectionView).SelectedItem = null;

    }
}
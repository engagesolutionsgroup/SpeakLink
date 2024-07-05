using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakLink.Sample;

public partial class MultilinePage : ContentPage
{
    public MultilinePage()
    {
        InitializeComponent();
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
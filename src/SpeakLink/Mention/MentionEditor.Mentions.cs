using System.ComponentModel;
using System.Windows.Input;
using SpeakLink.Mention;

namespace SpeakLink.Mention;

public partial class MentionEditor : IMentionController
{
    public event EventHandler<MentionSearchEventArgs> MentionSearched;
    
    public static readonly BindableProperty IsMentionsEnabledProperty = BindableProperty.Create(
        nameof(IsMentionsEnabled), typeof(bool), typeof(MentionEditor), true);
    
    public static readonly BindableProperty MentionSearchCommandProperty = BindableProperty.Create(
        nameof(MentionSearchCommand), typeof(ICommand), typeof(MentionEditor));
   
    public static readonly BindableProperty IsSuggestionsPopupVisibleProperty = BindableProperty.Create(
        nameof(IsSuggestionsPopupVisible), typeof(bool), typeof(MentionEditor), false, BindingMode.TwoWay);
    
    public static readonly BindableProperty MentionTextColorProperty = BindableProperty.Create(
        nameof(MentionTextColor), typeof(Color), typeof(MentionEditor));
    
    public static readonly BindableProperty MentionSelectedTextColorProperty = BindableProperty.Create(
        nameof(MentionSelectedTextColor), typeof(Color), typeof(MentionEditor));
    
    public static readonly BindableProperty MentionSelectedBackgroundColorProperty = BindableProperty.Create(
        nameof(MentionSelectedBackgroundColor), typeof(Color), typeof(Editor));


    [EditorBrowsable(EditorBrowsableState.Never)]
    void IMentionController.SendMentionSearched(MentionSearchEventArgs e)
    {
        MentionSearched?.Invoke(this, e);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void SendDisplaySuggestionsChanged(bool newState)
    {
        IsSuggestionsPopupVisible = newState;
    }
    
    public bool IsMentionsEnabled
    {
        get => (bool)GetValue(IsMentionsEnabledProperty);
        set => SetValue(IsMentionsEnabledProperty, value);
    }
    
    public ICommand MentionSearchCommand
    {
        get => (ICommand)GetValue(MentionSearchCommandProperty);
        set => SetValue(MentionSearchCommandProperty, value);
    }
    
    public bool IsSuggestionsPopupVisible
    {
        get => (bool)GetValue(IsSuggestionsPopupVisibleProperty);
        set => SetValue(IsSuggestionsPopupVisibleProperty, value);
    }

    public void InsertMention(string mentionId, string mentionText)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(mentionId));
        ArgumentException.ThrowIfNullOrEmpty(nameof(mentionText));

        InsertMention(new MentionEntity { Id = mentionId,Name = mentionText});
    }

    public void InsertMention(MentionEntity mentionEntity)
    {
        Handler?.Invoke(nameof(MentionInsertRequestCommand), mentionEntity);
    }
    
    public Color? MentionTextColor
    {
        get => (Color)GetValue(MentionTextColorProperty);
        set => SetValue(MentionTextColorProperty, value);
    }
    
    public Color? MentionSelectedTextColor
    {
        get => (Color)GetValue(MentionSelectedTextColorProperty);
        set => SetValue(MentionSelectedTextColorProperty, value);
    }
    
    public Color? MentionSelectedBackgroundColor
    {
        get => (Color)GetValue(MentionSelectedBackgroundColorProperty);
        set => SetValue(MentionSelectedBackgroundColorProperty, value);
    }
}
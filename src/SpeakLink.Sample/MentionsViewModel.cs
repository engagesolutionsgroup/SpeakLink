using System.ComponentModel;
using System.Runtime.CompilerServices;
using SpeakLink.Mention;

namespace SpeakLink.Sample;

public class MentionsViewModel : INotifyPropertyChanged
{
    private List<User> _mentionUsers;
    private bool _isDisplayingMentions;
    private FormattedString formattedString = new();

    public MentionService MentionService { get; set; }

    public FormattedString FormattedString 
    {
        get => formattedString;
        set => SetField(ref formattedString, value);
    }

    public MentionsViewModel()
    {
        MentionService = new MentionService();
        MentionSearchCommand = new Command<MentionSearchEventArgs>(OnMentionSearch);
        _mentionUsers = new();
    }

    public List<User> MentionUsers
    {
        get => _mentionUsers;
        set => SetField(ref _mentionUsers, value);
    }

    public bool IsDisplayingMentions
    {
        get => _isDisplayingMentions;
        set =>SetField(ref _isDisplayingMentions, value);
    }

    private void OnMentionSearch(MentionSearchEventArgs mentionSearchEventArgs)
    {
        MentionUsers = MentionService.SearchForUsers(mentionSearchEventArgs.MentionQuery);
        if (MentionUsers.Count == 0)
            IsDisplayingMentions = false;
    }

    public Command<MentionSearchEventArgs> MentionSearchCommand { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
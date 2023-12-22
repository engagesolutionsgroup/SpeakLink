namespace SpeakLink.Mention;

public class MentionSearchEventArgs(string controlCharacter, string mentionQuery) : EventArgs
{
    public string ControlCharacter { get; set; } = controlCharacter;
    public string MentionQuery { get; set; } = mentionQuery;
}
namespace SpeakLink.Mention;

public class MentionSpan : Span
{
    public string MentionId { get; set; }

    public MentionSpan(string id, string name)
    {
        MentionId = id;
        Text = $"{name}";
    }
}
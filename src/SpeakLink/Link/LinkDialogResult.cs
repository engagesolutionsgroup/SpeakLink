namespace SpeakLink.Link;


public class LinkDialogResult
{
    public bool Cancelled { get; set; } = true;
    public string Text { get; set; }
    public string Url { get; set; }
}
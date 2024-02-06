namespace SpeakLink.Link;

public class LinkSpan : Span
{
    public string Url { get; set; }
    public LinkSpan(string url, string text)
    {
        Url = url;
        Text = text;
    }
}
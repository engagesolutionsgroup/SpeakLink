namespace SpeakLink.Link;

public interface ILinkEditorDialogHandler
{
    Task<LinkDialogResult> ShowLinkDialogAsync(string? existingText, string? existingUrl);
}
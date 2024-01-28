namespace SpeakLink.Link;

public interface ILinkEditorDialogInvoker
{
    Task<LinkDialogResult> ShowLinkDialogAsync(string? existingText, string? existingUrl);
}
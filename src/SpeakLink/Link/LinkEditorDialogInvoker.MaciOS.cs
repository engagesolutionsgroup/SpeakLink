using UIKit;

namespace SpeakLink.Link;

public partial class LinkEditorDialogHandler 
{
    public async Task<LinkDialogResult> ShowLinkDialogAsync(string? existingText, string? existingUrl)
    {
        var alertController = UIAlertController.Create("Enter link information",
            !string.IsNullOrWhiteSpace(existingUrl) ? "Please update link" : "Please insert link",
            UIAlertControllerStyle.Alert);
        TaskCompletionSource<LinkDialogResult> completionSource = new();
        alertController.AddTextField(textField =>
        {
            textField.Text = existingText;
            textField.Placeholder = "Text";
        });

        alertController.AddTextField(textField =>
        {
            textField.Text = existingUrl;
            textField.Placeholder = "Url";
        });

        string firstInput, secondInput = string.Empty;
        alertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alert =>
        {
            completionSource.TrySetResult(new LinkDialogResult
            {
                Cancelled = true,
                Text = string.Empty,
                Url = string.Empty,
            });
            alertController.DismissViewController(true,null);
        }));
        
        alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, alert =>
        {
            firstInput = alertController.TextFields[0].Text;
            secondInput = alertController.TextFields[1].Text;
            completionSource.TrySetResult(new LinkDialogResult
            {
                Cancelled = false,
                Text = firstInput,
                Url = secondInput
            });
            alertController.DismissViewController(true,null);
        }));

        var topController = UIApplication.SharedApplication.KeyWindow!.RootViewController;
        // Present the alert
        await topController!.PresentViewControllerAsync(alertController, true);
        
        return await completionSource.Task;
    }
}
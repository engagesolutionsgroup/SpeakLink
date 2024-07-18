using Android.App;
using Android.Content;
using Android.Widget;
using View = Android.Views.View;

namespace SpeakLink.Link;

public partial class LinkEditorDialogHandler 
{
    private AlertDialog? _dialog;
    
    public Task<LinkDialogResult> ShowLinkDialogAsync(string? existingText, string? existingUrl)
    {
        var activity = Platform.CurrentActivity;
        var text = existingText ?? string.Empty;
        var existingLink = existingUrl;

        var dialogResult = new LinkDialogResult();

        TaskCompletionSource<LinkDialogResult> taskCompletionSource = new();
        
        var (dialogView,textEditText,urlEditText) = CreateDialogView(activity, text, existingLink);

        var builder =
            new AlertDialog.Builder(activity)
                .SetTitle("Link")
                .SetMessage("Message")
                .SetView(dialogView)
                .SetPositiveButton("Ok", (_, _) =>
                {
                    dialogResult.Cancelled = false;
                    dialogResult.Text = textEditText!.Text!;
                    dialogResult.Url = urlEditText!.Text!;
                    taskCompletionSource.TrySetResult(dialogResult);
                    _dialog?.Dismiss();
                }).SetNegativeButton("Cancel", (_, _) =>
                {
                    dialogResult.Cancelled = true;
                    taskCompletionSource.TrySetResult(dialogResult);
                    _dialog?.Dismiss();
                });
        
        _dialog = builder!.Create();
        _dialog.DismissEvent += (sender, args) => taskCompletionSource.TrySetResult(dialogResult);
        _dialog!.Show();

        return taskCompletionSource.Task;
    }

    private static (View, EditText textEditText,EditText urlEditText) CreateDialogView(Activity activity, string text, string? existingLink)
    {
        var textEditText = new EditText(activity);
        textEditText.Text = text;
        textEditText.Hint = "Text";
       
        var urlEditText = new EditText(activity);
        urlEditText.Text = existingLink;
        urlEditText.Hint = "Link";

        var linerLayout = new LinearLayout(activity)
        {
            Orientation = Orientation.Vertical
        };
        
        linerLayout.AddView(textEditText);
        linerLayout.AddView(urlEditText);
        
        return (linerLayout, textEditText, urlEditText);
    }
}
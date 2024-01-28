using Android.App;
using Android.Content;
using Android.Widget;
using View = Android.Views.View;

namespace SpeakLink.Link;

public partial class LinkEditorDialogInvoker 
{
    private readonly Context _context;
    private AlertDialog? dialog;
    
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
                    dialog?.Dismiss();
                }).SetNegativeButton("Cancel", (_, _) =>
                {
                    dialogResult.Cancelled = true;
                    taskCompletionSource.TrySetResult(dialogResult);
                    dialog?.Dismiss();
                });
        
        dialog = builder!.Create();
        dialog.DismissEvent += (sender, args) => taskCompletionSource.TrySetResult(dialogResult);
        dialog!.Show();

        return taskCompletionSource.Task;
    }

    private static (View, EditText textEditText,EditText urlEditText) CreateDialogView(Activity activity, string text, string? existingLink)
    {
        // 1. Instantiate an AlertDialog.Builder with its constructor
        //var view = activity.LayoutInflater.Inflate(Resource.Layout.dialog_engage_rich_editor_link, null);
       // view!.FindViewById<EditText>(Resource.Id.engage_rich_editor_dialog_insert_link_edit)!.Text = text;
        //view!.FindViewById<EditText>(Resource.Id.engage_rich_editor_dialog_insert_text_edit)!.Text = existingLink;
        
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
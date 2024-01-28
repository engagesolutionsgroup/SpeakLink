using SpeakLink.Controls.MaciOS;
using SpeakLink.Link;

namespace SpeakLink.Handlers;

public partial class RichEditorHandler
{
    protected override SpeakLinkMentionTextView CreatePlatformView()
    {
        return new SpeakLinkRichTextView();
    }

    protected override void ConnectHandler(SpeakLinkMentionTextView platformView)
    {
        base.ConnectHandler(platformView);

        if (VirtualView != null && platformView is SpeakLinkRichTextView richEditText)
        {
            VirtualView.ToolbarState = richEditText.ToolbarState!;

            if (MauiContext?.Services.GetService<ILinkEditorDialogHandler>() is { } linkEditorDialogInvoker)
                richEditText.LinkEditorDialogHandler = linkEditorDialogInvoker;
        }
    }
}
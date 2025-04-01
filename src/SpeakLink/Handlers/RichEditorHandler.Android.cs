using SpeakLink.Controls.Android;
using SpeakLink.Link;
using SpeakLink.RichText;

namespace SpeakLink.Handlers;

public partial class RichEditorHandler
{
    protected override SpeakLinkMentionEditText CreatePlatformView()
    {
        return new SpeakLinkRichEditText(Context);
    }

    protected override void ConnectHandler(SpeakLinkMentionEditText platformView)
    {
        base.ConnectHandler(platformView);

        if (VirtualView != null && platformView is SpeakLinkRichEditText richEditText)
        {
            VirtualView.ToolbarState = richEditText.ToolbarState!;

            if (MauiContext?.Services.GetService<ILinkEditorDialogHandler>() is { } linkEditorDialogInvoker)
                richEditText.LinkEditorDialogHandler = linkEditorDialogInvoker;

            richEditText.SpanStyleChanged += OnSpanStyleChanged;
        }
    }

    protected override void DisconnectHandler(SpeakLinkMentionEditText platformView)
    {
        base.DisconnectHandler(platformView);
        if(platformView is SpeakLinkRichEditText richEditText)
            richEditText.SpanStyleChanged -= OnSpanStyleChanged;
    }

    private void OnSpanStyleChanged(object? sender, RichEditorStyle e) => RaiseFormattedTextChanged();
}
using SpeakLink.Controls.MaciOS;

namespace SpeakLink.Handlers;

public partial class RichEditorHandler
{
    protected override SpeakLinkMentionTextView CreatePlatformView()
    {
        return new SpeakLinkMentionTextView();
    }

    protected override void ConnectHandler(SpeakLinkMentionTextView platformView)
    {
        base.ConnectHandler(platformView);

        if (VirtualView != null && platformView is SpeakLinkMentionTextView richEditText)
            return;
        //VirtualView.ToolbarState = richEditText.ToolbarState;
    }
}
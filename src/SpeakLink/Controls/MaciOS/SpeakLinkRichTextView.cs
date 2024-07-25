using System.Diagnostics;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using SpeakLink.Controls.MaciOS.Toolbar;
using SpeakLink.Link;
using UIKit;

namespace SpeakLink.Controls.MaciOS;

public class SpeakLinkRichTextView : SpeakLinkMentionTextView
{
    private NSMutableDictionary? _customTypingAttributes;
    public PlatformToolbarState? ToolbarState { get; set; }
    public ILinkEditorDialogHandler LinkEditorDialogHandler { get; set; }

    public SpeakLinkRichTextView()
    {
    }

    public SpeakLinkRichTextView(NSCoder coder) : base(coder)
    {
    }

    protected SpeakLinkRichTextView(NSObjectFlag t) : base(t)
    {
    }

    protected internal SpeakLinkRichTextView(NativeHandle handle) : base(handle)
    {
    }

    public SpeakLinkRichTextView(CGRect frame, NSTextContainer? textContainer) : base(frame, textContainer)
    {
    }

    public SpeakLinkRichTextView(CGRect frame) : base(frame)
    {
    }

    protected override void Initialize()
    {
        ToolbarState = new PlatformToolbarState(this);
    }
    
    protected override void OnSelectionChanged()
    {
        base.OnSelectionChanged();
        if (ToolbarState?.Styles?.Count() > 0)
        {
            foreach (var style in ToolbarState.Styles)
                style.OnSelectionChanged();
        }
    }

    internal void RaiseFormattedTextChanged()
        => OnTextChangedDelegate(null, this.Text);
}
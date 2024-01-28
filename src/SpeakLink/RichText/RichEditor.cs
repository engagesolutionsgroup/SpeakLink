using SpeakLink.Mention;

namespace SpeakLink.RichText;

public class RichEditor : MentionEditor
{
    static readonly BindablePropertyKey ToolbarStatePropertyKey = BindableProperty.CreateReadOnly(
        nameof(ToolbarState), typeof(RichEditorToolbarState), typeof(RichEditor),
        null);
    
    public static readonly BindableProperty ToolbarStateProperty = ToolbarStatePropertyKey.BindableProperty;

    public RichEditorToolbarState ToolbarState
    {
        get => (RichEditorToolbarState)this.GetValue(ToolbarStateProperty);
        internal set => this.SetValue(ToolbarStatePropertyKey, value);
    }
}
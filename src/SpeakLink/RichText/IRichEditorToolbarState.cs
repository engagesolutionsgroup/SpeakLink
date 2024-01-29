namespace SpeakLink.RichText;

public interface IRichEditorToolbarState<out T>
    where T : IToolbarSpanStyle
{
    IEnumerable<T> Styles { get; }
}
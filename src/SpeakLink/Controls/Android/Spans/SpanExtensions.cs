using Android.Text;

namespace SpeakLink.Controls.Android.Spans;

public static class SpanExtensions
{
    public static T[] GetSpans<T>(this IEditable? editable, int start, int end)
    {
        var spans = editable?.GetSpans(start, end,
            Java.Lang.Class.FromType(typeof(global::Android.Text.Style.CharacterStyle)));
        return
            spans?.OfType<T>().ToArray() ?? Array.Empty<T>();
    }

    public static T[] GetSpans<T>(this ISpannable? editable, int start, int end)
    {
        var spans = editable?.GetSpans(start, end,
            Java.Lang.Class.FromType(typeof(global::Android.Text.Style.CharacterStyle)));
        return
            spans?.OfType<T>().ToArray() ?? Array.Empty<T>();
    }
}
using System.Globalization;
using SpeakLink.RichText;

namespace SpeakLink.Sample;

public class RichEditorStyleToGlyph : IValueConverter
{
    private FormattedString GetFormattedString(string text, FontAttributes fontAttributes, TextDecorations textDec) =>
        new() { Spans = { new Span { Text = text, FontAttributes = fontAttributes, TextDecorations = textDec } } };
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (RichEditorStyle) value! switch
        {
            RichEditorStyle.Bold => GetFormattedString("B", FontAttributes.Bold, TextDecorations.None),
            RichEditorStyle.Italic => GetFormattedString("I", FontAttributes.Italic, TextDecorations.None),
            RichEditorStyle.Strikethrough =>
                GetFormattedString("S", FontAttributes.None, TextDecorations.Strikethrough),
            RichEditorStyle.Underline => GetFormattedString("S", FontAttributes.None, TextDecorations.Underline),
            RichEditorStyle.Link => GetFormattedString("L", FontAttributes.None, TextDecorations.None),
            RichEditorStyle.Subscript => GetFormattedString("Sub", FontAttributes.None, TextDecorations.None),
            RichEditorStyle.Superscript => GetFormattedString("Sup", FontAttributes.None, TextDecorations.None),
            RichEditorStyle.BulletList => GetFormattedString("|•", FontAttributes.None, TextDecorations.None),
            RichEditorStyle.NumberList => GetFormattedString("|1", FontAttributes.None, TextDecorations.None),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
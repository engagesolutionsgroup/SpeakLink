using System.Globalization;

namespace SpeakLink.Sample;

public class FormattedStringDescriptionConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var formattedString = value as FormattedString;
        var description = string.Empty;
        foreach (var span in formattedString?.Spans ?? [])
        {
            var spanDescription = $"T: {span.GetType()}" +
                                  $"Text {span.Text}" +
                                  $"Foreground: {span.TextColor}";
            description += spanDescription;
        }

        return description;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
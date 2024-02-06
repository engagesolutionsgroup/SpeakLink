using System.Globalization;
using SpeakLink.Link;
using SpeakLink.Mention;

namespace SpeakLink.Sample;

public class FormattedStringDescriptionConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var formattedString = value as FormattedString;
        var description = string.Empty;
        foreach (var span in formattedString?.Spans ?? [])
        {
            var spanDescription = $"🌀{span.GetType().Name}" +
                                  $"✍️{span.Text}";
            switch (span)
            {
                case LinkSpan linkSpan:
                    spanDescription += $" 🔗{linkSpan.Text}";
                    break;
                case MentionSpan mentionSpan:
                    spanDescription += $" 💬{mentionSpan.MentionId}";
                    break;
            }

            description += spanDescription + Environment.NewLine;
        }

        return description;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
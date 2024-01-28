using SpeakLink.RichText;

namespace SpeakLink.Controls.MaciOS.SpanStyle;

public interface IAppleToolbarSpanStyle : IToolbarSpanStyle
{
    void OnSelectionChanged();
}
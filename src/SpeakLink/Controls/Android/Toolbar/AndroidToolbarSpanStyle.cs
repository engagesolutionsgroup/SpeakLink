using Android.Text;
using SpeakLink.RichText;

namespace SpeakLink.Controls.Android.Toolbar;

public interface IAndroidToolbarSpanStyle : IToolbarSpanStyle
{
    SpeakLinkRichEditText ParentEditText { get; init; }

    void ApplyStyle(IEditable? sequence, int inputStartPosition, int inputEndPosition);
}
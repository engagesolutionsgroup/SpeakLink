using SpeakLink.Controls.MaciOS.SpanStyle;
using SpeakLink.RichText;

namespace SpeakLink.Controls.MaciOS.Toolbar;

public class PlatformToolbarState : RichEditorToolbarState, IRichEditorToolbarState<IAppleToolbarSpanStyle>
{
    private readonly ItalicFormattedToolbarSpanStyle _italicSpanStyle;
    private readonly BoldFormattedToolbarSpanStyle _boldSpanStyle;
    private readonly StrikethroughFormattedToolbarSpanStyle _strikethroughSpanStyle;
    private readonly UnderlineFormattedToolbarSpanStyle _underlineSpanStyle;
    private readonly LinkFormattedToolbarSpanStyle _linkSpanStyle;
    private readonly SubscriptFormattedToolbarSpanStyle _subscriptSpanStyle;
    private readonly SuperscriptFormattedToolbarSpanStyle _superscriptSpanStyle;

    public PlatformToolbarState(SpeakLinkRichTextView owner)
    {
        _italicSpanStyle = new ItalicFormattedToolbarSpanStyle(owner);
        _boldSpanStyle = new BoldFormattedToolbarSpanStyle(owner);
        _strikethroughSpanStyle = new StrikethroughFormattedToolbarSpanStyle(owner);
        _underlineSpanStyle = new UnderlineFormattedToolbarSpanStyle(owner);
        _subscriptSpanStyle = new SubscriptFormattedToolbarSpanStyle(owner);
        _superscriptSpanStyle = new SuperscriptFormattedToolbarSpanStyle(owner);

        _linkSpanStyle = new LinkFormattedToolbarSpanStyle(owner);

        Styles = [
            _boldSpanStyle,
            _italicSpanStyle, 
            _strikethroughSpanStyle,
            _underlineSpanStyle,
            _linkSpanStyle,
            _subscriptSpanStyle,
            _superscriptSpanStyle
        ];
    }

    public override IEnumerable<IAppleToolbarSpanStyle> Styles { get; }
}
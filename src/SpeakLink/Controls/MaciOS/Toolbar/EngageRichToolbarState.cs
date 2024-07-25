using SpeakLink.Controls.MaciOS.SpanStyle;
using SpeakLink.Controls.MaciOS.SpanStyle.Paragraph;
using SpeakLink.RichText;

namespace SpeakLink.Controls.MaciOS.Toolbar;

public class PlatformToolbarState : RichEditorToolbarState, IRichEditorToolbarState<IAppleToolbarSpanStyle>
{
    private readonly ItalicFormattedToolbarSpanStyle _italicSpanStyle;
    private readonly BoldFormattedToolbarSpanStyle _boldSpanStyle;
    private readonly StrikethroughFormattedToolbarSpanStyle _strikethroughSpanStyle;
    private readonly UnderlineFormattedToolbarSpanStyle _underlineSpanStyle;
    private readonly LinkFormattedToolbarSpanStyle _linkSpanStyle;
    private readonly ParagraphListToolbarSpanStyle _bulletListSpanStyle;
    private readonly ParagraphListToolbarSpanStyle _numberListSpanStyle;

    public PlatformToolbarState(SpeakLinkRichTextView owner)
    {
        _italicSpanStyle = new ItalicFormattedToolbarSpanStyle(owner);
        _boldSpanStyle = new BoldFormattedToolbarSpanStyle(owner);
        _strikethroughSpanStyle = new StrikethroughFormattedToolbarSpanStyle(owner);
        _underlineSpanStyle = new UnderlineFormattedToolbarSpanStyle(owner);
        _bulletListSpanStyle = new BulletListToolbarSpanStyle(owner);
        _numberListSpanStyle = new NumberListToolbarSpanStyle(owner);
        //_subscriptSpanStyle = new InlineToolbarSpanStyle<SpeakLinkSubscriptSpan>(owner, RichEditorStyle.Subscript);
        //_superscriptSpanStyle = new InlineToolbarSpanStyle<SpeakLinkSuperscriptSpan>(owner, RichEditorStyle.Superscript);

        _linkSpanStyle = new LinkFormattedToolbarSpanStyle(owner);

        Styles = [
            _boldSpanStyle,
            _italicSpanStyle, 
            _strikethroughSpanStyle,
            _underlineSpanStyle,
            _linkSpanStyle,
            _bulletListSpanStyle,
            _numberListSpanStyle
        ];
    }

    public override IEnumerable<IAppleToolbarSpanStyle> Styles { get; }
}
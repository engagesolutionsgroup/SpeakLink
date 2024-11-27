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
    private  List<IAppleToolbarSpanStyle> platformStyles;

    public PlatformToolbarState(SpeakLinkRichTextView owner)
    {
        _italicSpanStyle = new ItalicFormattedToolbarSpanStyle(owner);
        _boldSpanStyle = new BoldFormattedToolbarSpanStyle(owner);
        _strikethroughSpanStyle = new StrikethroughFormattedToolbarSpanStyle(owner);
        _underlineSpanStyle = new UnderlineFormattedToolbarSpanStyle(owner);
        //_subscriptSpanStyle = new InlineToolbarSpanStyle<SpeakLinkSubscriptSpan>(owner, RichEditorStyle.Subscript);
        //_superscriptSpanStyle = new InlineToolbarSpanStyle<SpeakLinkSuperscriptSpan>(owner, RichEditorStyle.Superscript);

        _linkSpanStyle = new LinkFormattedToolbarSpanStyle(owner);

        platformStyles = [
            _boldSpanStyle,
            _italicSpanStyle, 
            _strikethroughSpanStyle,
            _underlineSpanStyle,
            _linkSpanStyle
        ];
    }

    public override void RemoveStyle(RichEditorStyle style)
    {
        var styleItem = Styles.FirstOrDefault(x => x.RichEditorStyle == style);
        if (styleItem != null)
        {
            platformStyles.Remove(styleItem);
            platformStyles = [..platformStyles];
        }
        
        
        OnPropertyChanged(nameof(Styles));
        
    }

    public override IEnumerable<IAppleToolbarSpanStyle> Styles => platformStyles;
}
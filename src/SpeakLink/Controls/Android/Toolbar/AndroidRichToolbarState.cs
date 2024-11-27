using SpeakLink.Controls.Android.Spans;
using SpeakLink.Controls.Android.SpanStyle;
using SpeakLink.RichText;

namespace SpeakLink.Controls.Android.Toolbar;

public class AndroidRichToolbarState : RichEditorToolbarState, IRichEditorToolbarState<IAndroidToolbarSpanStyle>
{
    private readonly InlineToolbarSpanStyle<SpeakLinkItalicSpan> _italicSpanStyle;
    private readonly InlineToolbarSpanStyle<SpeakLinkBoldSpan> _boldSpanStyle;
    private readonly InlineToolbarSpanStyle<SpeakLinkStrikethroughSpan> _strikethroughSpanStyle;
    private readonly InlineToolbarSpanStyle<SpeakLinkUnderlineSpan> _underlineSpanStyle;
    private readonly InlineToolbarSpanStyle<SpeakLinkSubscriptSpan> _subscriptSpanStyle;
    private readonly InlineToolbarSpanStyle<SpeakLinkSuperscriptSpan> _superscriptSpanStyle;
    private readonly SpeakLinkToolbarLinkSpanStyle _linkSpanStyle;
    private List<IAndroidToolbarSpanStyle> platformStyles;

    public AndroidRichToolbarState(SpeakLinkRichEditText owner)
    {
        _italicSpanStyle = new InlineToolbarSpanStyle<SpeakLinkItalicSpan>(owner, RichEditorStyle.Italic);
        _boldSpanStyle = new InlineToolbarSpanStyle<SpeakLinkBoldSpan>(owner, RichEditorStyle.Bold);
        _strikethroughSpanStyle =
            new InlineToolbarSpanStyle<SpeakLinkStrikethroughSpan>(owner, RichEditorStyle.Strikethrough);
        _underlineSpanStyle = new InlineToolbarSpanStyle<SpeakLinkUnderlineSpan>(owner, RichEditorStyle.Underline);
        //_subscriptSpanStyle = new InlineToolbarSpanStyle<SpeakLinkSubscriptSpan>(owner, RichEditorStyle.Subscript);
        // _superscriptSpanStyle = new InlineToolbarSpanStyle<SpeakLinkSuperscriptSpan>(owner, RichEditorStyle.Superscript);
        _linkSpanStyle = new SpeakLinkToolbarLinkSpanStyle(owner);

         platformStyles =
        [
            _boldSpanStyle,
            _italicSpanStyle,
            _strikethroughSpanStyle,
            _underlineSpanStyle,
            // _subscriptSpanStyle, 
            // _superscriptSpanStyle,
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
        
        this.OnPropertyChanged(nameof(Styles));
    }

    public override IEnumerable<IAndroidToolbarSpanStyle> Styles => platformStyles;
}
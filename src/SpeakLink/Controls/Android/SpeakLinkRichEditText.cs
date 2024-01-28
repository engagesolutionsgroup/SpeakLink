using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Text.Style;
using Android.Util;
using LinkedIn.Spyglass.Mentions;
using SpeakLink.Controls.Android.Spans;
using SpeakLink.Controls.Android.Toolbar;
using SpeakLink.Link;
using SpeakLink.RichText;

namespace SpeakLink.Controls.Android;

public class SpeakLinkRichEditText : SpeakLinkMentionEditText
{
    private SpeakLinkRichTextWatcher _richTextWatcher;
    public AndroidRichToolbarState? ToolbarState { get; set; }
    public ILinkEditorDialogInvoker LinkEditorDialogInvoker { get; set; }

    protected SpeakLinkRichEditText(IntPtr javaReference, JniHandleOwnership transfer) 
        : base(javaReference, transfer)
    {
    }

    public SpeakLinkRichEditText(Context context) : base(context)
    {
    }

    public SpeakLinkRichEditText(Context context, IAttributeSet? attrs) : base(context, attrs)
    {
    }

    public SpeakLinkRichEditText(Context context, IAttributeSet? attrs, int defStyle) : base(context, attrs, defStyle)
    {
    }

    protected override void Initialize()
    {
        SetMentionSpanConfig(new MentionSpanConfig.Builder().Build());
        ToolbarState = new(this);
        _richTextWatcher = new SpeakLinkRichTextWatcher(ToolbarState, InvokeOnTextChanged);
        this.AddTextChangedListener(_richTextWatcher);
    }

    protected override void OnSelectionChanged(int selStart, int selEnd)
    {
        base.OnSelectionChanged(selStart, selEnd);

        if (this.ToolbarState == null)
            return;

        var boldExists = false;
        var italicsExists = false;
        var underlinedExists = false;
        var strikethroughExists = false;
        var subscriptExists = false;
        var superscriptExists = false;
        var linkSpanExists = false;

        var editable = this.EditableText!;
        CharacterStyle[] characterStyleSpans;

        bool SpanWithinWithoutBounds<T>() where T : CharacterStyle =>
            editable.GetSpans<T>(selStart, selEnd).FirstOrDefault() is { } characterStyleSpan &&
            editable.GetSpanStart(characterStyleSpan) < selStart &&
            editable.GetSpanEnd(characterStyleSpan) > selEnd;

        bool SpanWithin<T>() where T : CharacterStyle =>
            editable.GetSpans<T>(selStart, selEnd).FirstOrDefault() is { } characterStyleSpan &&
            editable.GetSpanStart(characterStyleSpan) <= selStart &&
            editable.GetSpanEnd(characterStyleSpan) >= selEnd;

        if (selStart > 0 && selStart == selEnd) //Cursor
        {
            characterStyleSpans = editable.GetSpans<CharacterStyle>(selStart - 1, selStart);

            foreach (var characterStyleSpan in characterStyleSpans)
            {
                switch (characterStyleSpan)
                {
                    case StyleSpan styleSpan:
                        switch (styleSpan.Style)
                        {
                            case TypefaceStyle.Bold:
                                boldExists = true;
                                break;
                            case TypefaceStyle.Italic:
                                italicsExists = true;
                                break;
                        }

                        break;
                    case SpeakLinkUnderlineSpan:
                        underlinedExists = true;
                        break;
                    case SpeakLinkStrikethroughSpan:
                        strikethroughExists = true;
                        break;
                }

                subscriptExists = SpanWithin<SpeakLinkSubscriptSpan>();
                superscriptExists = SpanWithin<SpeakLinkSuperscriptSpan>();
                linkSpanExists = SpanWithinWithoutBounds<SpeakLinkLinkSpan>();
            }
        }
        else //Selection
        {
            characterStyleSpans = editable.GetSpans<CharacterStyle>(selStart, selEnd);

            foreach (var characterStyleSpan in characterStyleSpans)
            {
                var spanStart = editable.GetSpanStart(characterStyleSpan);
                var spanEnd = editable.GetSpanEnd(characterStyleSpan);
                bool spanWithin = spanStart <= selStart && spanEnd >= selEnd;

                if (spanWithin)
                {
                    switch (characterStyleSpan)
                    {
                        case StyleSpan styleSpan:
                            switch (styleSpan.Style)
                            {
                                case TypefaceStyle.Bold:
                                    boldExists = true;
                                    break;
                                case TypefaceStyle.Italic:
                                    italicsExists = true;
                                    break;
                            }

                            break;
                        case SpeakLinkUnderlineSpan:
                            underlinedExists = true;
                            break;
                        case SpeakLinkStrikethroughSpan:
                            strikethroughExists = true;
                            break;
                        case SpeakLinkLinkSpan:
                            linkSpanExists = selStart != selEnd || selStart != 0;
                            break;
                    }

                    subscriptExists = SpanWithin<SpeakLinkSubscriptSpan>();
                    superscriptExists = SpanWithin<SpeakLinkSuperscriptSpan>();
                }
            }
        }

        SetToolbarStyleState(RichEditorStyle.Bold, boldExists);
        SetToolbarStyleState(RichEditorStyle.Italic, italicsExists);
        SetToolbarStyleState(RichEditorStyle.Underline, underlinedExists);
        SetToolbarStyleState(RichEditorStyle.Strikethrough, strikethroughExists);
        SetToolbarStyleState(RichEditorStyle.Subscript, subscriptExists);
        SetToolbarStyleState(RichEditorStyle.Superscript, superscriptExists);
        SetToolbarStyleState(RichEditorStyle.Link, linkSpanExists);
    }

    protected void SetToolbarStyleState(RichEditorStyle richEditorStyle, bool value)
    {
        if (ToolbarState?.Styles.FirstOrDefault(x
                => x.RichEditorStyle == richEditorStyle)
            is { } associatedStyleSpan)
            associatedStyleSpan.Checked = value;
    }
}
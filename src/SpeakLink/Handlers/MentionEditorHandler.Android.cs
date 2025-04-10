using System.Reflection;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.Core.View;
using Java.Lang;
using LinkedIn.Spyglass.Mentions;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using SpeakLink.Controls.Android;
using SpeakLink.Controls.Android.Spans;
using SpeakLink.Link;
using SpeakLink.Mention;
using MentionSpan = LinkedIn.Spyglass.Mentions.MentionSpan;
using Rect = Microsoft.Maui.Graphics.Rect;
using TextChangedEventArgs = Microsoft.Maui.Controls.TextChangedEventArgs;
using View = Microsoft.Maui.Controls.View;

namespace SpeakLink.Handlers;

public partial class MentionEditorHandler : ViewHandler<MentionEditor, SpeakLinkMentionEditText>
{
    private bool _ignoreFormattedTextChanges;

    public static partial void MapText(MentionEditorHandler handler, MentionEditor view)
    {
        if (handler._ignoreFormattedTextChanges)
            return;
        
        handler.PlatformView?.SetText(view.Text);
        if (handler.VirtualView != null)
        {
            handler.VirtualView.ResizeIfNeeded();
            if (string.IsNullOrEmpty(view.Text))
                handler.VirtualView.FormattedText = null;
        }
    }

    static void MapIsEnabled(MentionEditorHandler handler, MentionEditor view)
    {
        handler.PlatformView.Enabled = view.IsEnabled;
    }

    public static partial void MapFont(MentionEditorHandler handler, MentionEditor view)
    {
        var font = view.Font;

        var fontManager = handler.MauiContext!.Services.GetRequiredService<IFontManager>();
        var tf = fontManager.GetTypeface(font);
        handler.PlatformView.Typeface = tf;

        var fontSize = fontManager.GetFontSize(font);
        handler.PlatformView.SetTextSize(fontSize.Unit, fontSize.Value);
    }

    public static partial void MapTextColor(MentionEditorHandler handler, MentionEditor view)
    {
        if (view.TextColor != null)
            handler.PlatformView.SetTextColor(view.TextColor.ToPlatform());
    }

    public static partial void MapIsMentionsEnabled(MentionEditorHandler handler, MentionEditor view)
    {
        if (view.IsMentionsEnabled)
        {
            if (handler.PlatformView.IsDisplayingSuggestions)
                handler.ElementController.SendDisplaySuggestionsChanged(true);
        }
        else
        {
            if (!handler.PlatformView.IsDisplayingSuggestions)
                handler.ElementController.SendDisplaySuggestionsChanged(false);
        }
    }

    public static void MapMentionSearchCommand(MentionEditorHandler handler, MentionEditor view)
    {
        handler.PlatformView.MentionSearchCommand = view.MentionSearchCommand;
    }

    public static partial void MapIsSuggestionsPopupVisible(MentionEditorHandler handler, MentionEditor view)
    {
        handler.PlatformView.IsDisplayingSuggestions = view.IsSuggestionsPopupVisible;
    }

    public static void MapAutoSize(MentionEditorHandler handler, MentionEditor view)
    {
        if (view.AutoSize == EditorAutoSizeOption.TextChanges)
            handler.PlatformView.InputType |= InputTypes.TextFlagMultiLine;
        else
            handler.PlatformView.InputType &= ~InputTypes.TextFlagMultiLine;
    }

    public static partial void MapMentionInsertRequested(MentionEditorHandler handler, MentionEditor view, object? arg)
    {
        if (arg is IMentionable mentionable)
        {
            handler.PlatformView.InsertMention(mentionable);
            handler.RaiseFormattedTextChanged();
        }
    }

    protected override void ConnectHandler(SpeakLinkMentionEditText platformView)
    {
        base.ConnectHandler(platformView);
        platformView.OnTextChanged += OnTextChanged;
        platformView.MentionSearched += MentionSearched;
        platformView.DisplaySuggestionChanged += DisplaySuggestionChanged;
        platformView.CursorSelectionChanged += CursorSelectionChanged;
        platformView.SetupMentions();
    }

    private void CursorSelectionChanged(object? sender, (int selStart, int selEnd) e)
    {
        ElementController?.SendSelectionChanged(e.selStart, e.selEnd);
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        RaiseFormattedTextChanged();
    }

    protected void RaiseFormattedTextChanged()
    {
        _ignoreFormattedTextChanges = true;
        ElementController?.SendFormattedTextChanged(GetFormattedText());
        ElementController?.OnTextChanged(null, this.PlatformView.Text);
        _ignoreFormattedTextChanges = false;
    }

    private void DisplaySuggestionChanged(object? sender, bool e)
    {
        if (VirtualView?.IsMentionsEnabled ?? false)
            ElementController.SendDisplaySuggestionsChanged(e);
    }

    private void MentionSearched(object? sender, MentionSearchEventArgs e)
        => ElementController.SendMentionSearched(e);

    protected override void DisconnectHandler(SpeakLinkMentionEditText platformView)
    {
        platformView.OnTextChanged -= OnTextChanged;
        platformView.MentionSearched -= MentionSearched;
        platformView.DisplaySuggestionChanged -= DisplaySuggestionChanged;
        platformView.CursorSelectionChanged -= CursorSelectionChanged;
        base.DisconnectHandler(platformView);
    }

    protected override SpeakLinkMentionEditText CreatePlatformView()
    {
        return new SpeakLinkMentionEditText(Context);
    }

    public override void PlatformArrange(Rect frame)
    {
        PrepareForTextViewArrange(frame);
        base.PlatformArrange(frame);
    }

    private void PrepareForTextViewArrange(Rect frame)
    {
        if (frame.Width < 0 || frame.Height < 0)
        {
            return;
        }

        var platformView = PlatformView;
        if (platformView == null)
        {
            return;
        }

        var virtualView = VirtualView;
        if (virtualView == null)
        {
            return;
        }

        // Depending on our layout situation, the TextView may need an additional measurement pass at the final size
        // in order to properly handle any TextAlignment properties and some internal bookkeeping
        if (NeedsExactMeasure(virtualView))
        {
            platformView.Measure(MakeMeasureSpecExact(platformView, frame.Width),
                MakeMeasureSpecExact(platformView, frame.Height));
        }
    }

    internal static int MakeMeasureSpecExact(SpeakLinkMentionEditText view, double size)
    {
        // Convert to a native size to create the spec for measuring
        var deviceSize = (int)view.Context.ToPixels(size);
        return MeasureSpecMode.Exactly.MakeMeasureSpec(deviceSize);
    }

    public static partial void MapFormattedText(MentionEditorHandler handler, MentionEditor view)
    {
        if (handler._ignoreFormattedTextChanges)
            return;
        
        if (view?.FormattedText is not null)
            handler.PlatformView.SetTextFormattedFromBinding(ConvertToSpannableText(handler, view));
    }

    internal static bool NeedsExactMeasure(IView virtualView)
    {
        if (virtualView.VerticalLayoutAlignment != Microsoft.Maui.Primitives.LayoutAlignment.Fill
            && virtualView.HorizontalLayoutAlignment != Microsoft.Maui.Primitives.LayoutAlignment.Fill)
        {
            // Layout Alignments of Start, Center, and End will be laying out the TextView at its measured size,
            // so we won't need another pass with MeasureSpecMode.Exactly
            return false;
        }

        if (virtualView is { Width: >= 0, Height: >= 0 })
        {
            // If the Width and Height are both explicit, then we've already done MeasureSpecMode.Exactly in 
            // both dimensions; no need to do it again
            return false;
        }

        // We're going to need a second measurement pass so TextView can properly handle alignments
        return true;
    }

    protected static void MapExplicitCharacter(MentionEditorHandler handler, MentionEditor view)
    {
        handler.PlatformView.UpdateTokenizer(view.ExplicitCharacters);
    }

    protected static ICharSequence ConvertToSpannableText(MentionEditorHandler handler, MentionEditor view)
    {
        var formattedString = view.FormattedText;
        if (formattedString == null)
            return new SpannableString(string.Empty);

        var defaultColor = view.TextColor;

        var builder = new System.Text.StringBuilder();

        foreach (var span in formattedString.Spans)
        {
            builder.Append(span.Text);
        }

        var spannable = new SpannableString(builder.ToString());

        var c = 0;
        for (int i = 0; i < formattedString.Spans.Count; i++)
        {
            var span = formattedString.Spans[i];
            var text = span.Text;
            if (text == null)
                continue;

            int start = c;
            int end = start + text.Length;
            c = end;

            if (span is Mention.MentionSpan mentionSpan)
                spannable.SetSpan(new MentionSpan(
                        new MentionEntity { Id = mentionSpan.MentionId, Name = mentionSpan.Text },
                        handler.PlatformView.MentionSpanConfig),
                    start, end, SpanTypes.ExclusiveExclusive);

            // TextColor
            var textColor = span.TextColor;
            if (textColor is not null)
                spannable.SetSpan(new ForegroundColorSpan(textColor.ToPlatform()), start, end,
                    SpanTypes.ExclusiveExclusive);

            // BackgroundColor
            if (span.BackgroundColor is not null)
                spannable.SetSpan(new BackgroundColorSpan(span.BackgroundColor.ToPlatform()), start, end,
                    SpanTypes.ExclusiveExclusive);

            // LineHeight & CharacterSpacing & Font is ignored for now

            // TextDecorations
            var textDecorations = span.IsSet(Span.TextDecorationsProperty)
                ? span.TextDecorations
                : TextDecorations.None;
            if (textDecorations.HasFlag(TextDecorations.Strikethrough))
                spannable.SetSpan(new StrikethroughSpan(), start, end, SpanTypes.ExclusiveExclusive);
            if (textDecorations.HasFlag(TextDecorations.Underline))
                spannable.SetSpan(new UnderlineSpan(), start, end, SpanTypes.ExclusiveExclusive);
            if (span.FontAttributes.HasFlag(FontAttributes.Bold))
                spannable.SetSpan(new StyleSpan(TypefaceStyle.Bold), start, end, SpanTypes.ExclusiveExclusive);
            if (span.FontAttributes.HasFlag(FontAttributes.Italic))
                spannable.SetSpan(new StyleSpan(TypefaceStyle.Italic), start, end, SpanTypes.ExclusiveExclusive);
        }

        return spannable;
    }


    public static void MapMentionColors(MentionEditorHandler handler, MentionEditor view)
    {
        var mentionTextColor = view.MentionTextColor?.ToPlatform().ToArgb() ??
                               handler.PlatformView.MentionSpanConfig.NormalTextColor;

        var mentionSelectedTextColor = view.MentionSelectedTextColor?.ToPlatform().ToArgb() ??
                                       handler.PlatformView.MentionSpanConfig.SelectedTextColor;

        var mentionSelectedBackgroundColor = view.MentionSelectedBackgroundColor?.ToPlatform().ToArgb() ??
                                             handler.PlatformView.MentionSpanConfig.SelectedTextBackgroundColor;

        if (handler.PlatformView.MentionSpanConfig.NormalTextColor != mentionTextColor
            || handler.PlatformView.MentionSpanConfig.SelectedTextColor != mentionSelectedTextColor
            || handler.PlatformView.MentionSpanConfig.SelectedTextBackgroundColor != mentionSelectedBackgroundColor)
        {
            handler.PlatformView.MentionSpanConfig = new MentionSpanConfig.Builder()
                .SetMentionTextColor(mentionTextColor)
                .SetSelectedMentionTextColor(mentionSelectedTextColor)
                .SetSelectedMentionTextBackgroundColor(mentionSelectedBackgroundColor)
                .Build();

            //Force to update mentions colors
            MapFormattedText(handler, view);
            handler.RaiseFormattedTextChanged();
        }
    }

    protected virtual FormattedString ConvertFromSpannableText(ISpanned? spannable)
    {
        var formattedString = new FormattedString();
        if (spannable == null)
            return formattedString;

        int length = spannable.Length();

        List<(ISpanned spannedSegment, MentionSpan? mention)> spannedSegments =
            ExtractSeparateSpanSegments<MentionSpan>(spannable, length);


        foreach (var (spannedSegment, mention) in spannedSegments)
        {
            if (mention != null)
            {
                formattedString.Spans.Add(ToMauiMentionSpan(mention));
            }
            else
            {
                List<(ISpanned innerSpannedSegment, SpeakLinkLinkSpan? link)> innerSpannedSegments =
                    ExtractSeparateSpanSegments<SpeakLinkLinkSpan>(spannedSegment, spannedSegment.Length());
                foreach (var (innerSpannedSegment, link) in innerSpannedSegments)
                {
                    if (link != null)
                    {
                        var linkText = innerSpannedSegment.SubSequence(innerSpannedSegment.GetSpanStart(link),
                            innerSpannedSegment.GetSpanEnd(link));
                        formattedString.Spans.Add(ToMauiLinkSpan(link, linkText));
                    }
                    else
                    {
                        foreach (var mauiSpan in GetMauiSpansFromSpannable(innerSpannedSegment))
                            formattedString.Spans.Add(mauiSpan);
                    }
                }
            }
        }

        return formattedString;
    }

    private LinkSpan ToMauiLinkSpan(SpeakLinkLinkSpan link, string linkText) => new(link.URL, linkText);

    private static List<(ISpanned spannedSegment, T? span)> ExtractSeparateSpanSegments<T>(ISpanned spannable,
        int length)
        where T : CharacterStyle
    {
        var spannedSegments = new List<(ISpanned spannedSegment, T? mention)>();

        var nativeMentionSpans = spannable
            .GetSpans(0, length, Class.FromType(typeof(T)))
            ?.OfType<T>()
            .OrderBy(spannable.GetSpanStart)
            .Select(x => (mention: x, start: spannable.GetSpanStart(x), end: spannable.GetSpanEnd(x)))
            .ToArray() ?? [];

        if (nativeMentionSpans.Length == 0)
        {
            spannedSegments.Add((spannable, null));
        }
        else
        {
            var startOrigin = 0;
            for (var i = 0; i < nativeMentionSpans.Length; i++)
            {
                var mentionSpanStart = nativeMentionSpans[i].start;
                var mentionSpanEnd = nativeMentionSpans[i].end;

                if (mentionSpanStart > startOrigin)
                    spannedSegments.Add(
                        ((ISpanned)spannable.SubSequenceFormatted(startOrigin, mentionSpanStart),
                            null));

                spannedSegments.Add(((ISpanned)spannable.SubSequenceFormatted(mentionSpanStart, mentionSpanEnd),
                    nativeMentionSpans[i].mention));

                startOrigin = mentionSpanEnd;

                if (nativeMentionSpans.Length - 1 == i && mentionSpanEnd < length)
                    spannedSegments.Add(((ISpanned)spannable.SubSequenceFormatted(mentionSpanEnd, length), null));
            }
        }

        return spannedSegments;
    }

    protected virtual IEnumerable<Span> GetMauiSpansFromSpannable(ISpanned spannedSegment)
    {
        int nextSpanTransition;
        for (int i = 0; i < spannedSegment.Length(); i = nextSpanTransition)
        {
            // find the next span transition
            nextSpanTransition =
                spannedSegment.NextSpanTransition(i, spannedSegment.Length(), Class.FromType(typeof(CharacterStyle)));

            // get all spans in this range
            var spansForSegment = spannedSegment.GetSpans(i, nextSpanTransition, Class.FromType(typeof(CharacterStyle)))
                ?.OfType<CharacterStyle>().ToArray() ?? [];

            var resultMauiSpan = new Span { Text = spannedSegment.SubSequence(i, nextSpanTransition) };
            foreach (var nativeSpan in spansForSegment)
            {
                switch (nativeSpan)
                {
                    case ForegroundColorSpan foregroundColorSpan:
                        resultMauiSpan.TextColor =
                            new Android.Graphics.Color(foregroundColorSpan.ForegroundColor).ToColor();
                        break;
                    case BackgroundColorSpan backgroundColorSpan:
                        resultMauiSpan.BackgroundColor =
                            new Android.Graphics.Color(backgroundColorSpan.BackgroundColor).ToColor();
                        break;
                    case SpeakLinkStrikethroughSpan:
                        resultMauiSpan.TextDecorations |= TextDecorations.Strikethrough;
                        break;
                    case SpeakLinkUnderlineSpan:
                        resultMauiSpan.TextDecorations |= TextDecorations.Underline;
                        break;
                    case StyleSpan styleSpan:
                        switch (styleSpan)
                        {
                            case SpeakLinkBoldSpan:
                                resultMauiSpan.FontAttributes |= FontAttributes.Bold;
                                break;
                            case SpeakLinkItalicSpan:
                                resultMauiSpan.FontAttributes |= FontAttributes.Italic;
                                break;
                            case { Style: TypefaceStyle.BoldItalic }:
                                resultMauiSpan.FontAttributes |= FontAttributes.Bold | FontAttributes.Italic;
                                break;
                        }

                        break;
                }
            }

            yield return resultMauiSpan;
        }
    }

    protected virtual FormattedString GetFormattedText() =>
        ConvertFromSpannableText(PlatformView?.TextFormatted as ISpanned);

    private Mention.MentionSpan ToMauiMentionSpan(MentionSpan mentionSpan)
        => new(mentionSpan.Mention.SuggestibleId.ToString(), mentionSpan.DisplayString)
        {
            TextColor = new Android.Graphics.Color(PlatformView.MentionSpanConfig.NormalTextColor).ToColor()
        };

    private static void MapPlaceholderText(MentionEditorHandler handler, MentionEditor editor)
        => handler.PlatformView.Hint = editor.Placeholder;

    internal static void MapIsFocused(MentionEditorHandler handler, MentionEditor editor)
    {
        SpeakLinkMentionEditText? platformView = handler.PlatformView;
        if (platformView == null)
            return;
        if (editor.IsFocused)
        {
            if (!IsSoftInputShowing(platformView))
                ShowSoftInput(platformView);
        }
        else
        {
            if (IsSoftInputShowing(platformView))
                HideSoftInput(platformView);
        }
        // We can't use the default implementation because it will interfere with selection view click
        //handler.RegisterForHideHideSoftInputOnTappedChangedManager(handler, editor);
    }

    private void RegisterForHideHideSoftInputOnTappedChangedManager(MentionEditorHandler handler,
        MentionEditor mentionEditor)
    {
        if (typeof(View).Assembly.GetType("Microsoft.Maui.Controls.HideSoftInputOnTappedChangedManager") is
            { } inputKeyboardManagerType
            && handler.MauiContext?.Services.GetService(inputKeyboardManagerType) is { } inputKeyboardManager
            && inputKeyboardManagerType.GetMethod("UpdateFocusForView", BindingFlags.NonPublic | BindingFlags.Instance)
                is { } method)
        {
            method.Invoke(inputKeyboardManager, [mentionEditor]);
        }
    }

    internal static bool IsSoftInputShowing(Android.Views.View view)
    {
        var insets = ViewCompat.GetRootWindowInsets(view);
        if (insets is null)
        {
            return false;
        }

        var result = insets.IsVisible(WindowInsetsCompat.Type.Ime());
        return result;
    }

    internal static bool HideSoftInput(Android.Views.View view)
    {
        using var inputMethodManager =
            (InputMethodManager?)view.Context?.GetSystemService(Android.Content.Context.InputMethodService);
        var windowToken = view.WindowToken;

        if (windowToken is not null && inputMethodManager is not null)
        {
            return inputMethodManager.HideSoftInputFromWindow(windowToken, HideSoftInputFlags.None);
        }

        return false;
    }

    internal static bool ShowSoftInput(TextView inputView)
    {
        using var inputMethodManager =
            (InputMethodManager?)inputView.Context?.GetSystemService(Android.Content.Context.InputMethodService);

        // The zero value for the second parameter comes from 
        // https://developer.android.com/reference/android/view/inputmethod/InputMethodManager#showSoftInput(android.view.View,%20int)
        // Apparently there's no named value for zero in this case
        return inputMethodManager?.ShowSoftInput(inputView, 0) is true;
    }

    public static void MapMentionCommand(MentionEditorHandler handler, MentionEditor editor)
    {
        if (handler?.PlatformView != null)
            handler.PlatformView.ImageInputCommand = editor.ImageInputCommand;
    }

    private static void MapSelectionLength(MentionEditorHandler handler, MentionEditor editor)
    {
        if (handler.PlatformView != null &&
            System.Math.Abs(handler.PlatformView.SelectionEnd - handler.PlatformView.SelectionStart) !=
            editor.SelectionLength)
            handler.PlatformView.SetSelectionRange(editor.CursorPosition, editor.SelectionLength);
    }

    private static void MapCursorPosition(MentionEditorHandler handler, MentionEditor editor)
    {
        if (handler.PlatformView != null && handler.PlatformView.SelectionStart != editor.CursorPosition)
            handler.PlatformView.SetSelectionRange(editor.CursorPosition, editor.SelectionLength);
    }

    public static partial void MapMentionFontFamily(MentionEditorHandler handler, MentionEditor editor)
    {
        var fontManager = handler.MauiContext?.Services?.GetService<IFontManager>();
        if (fontManager != null)
        {
            if (editor.MentionFont == null)
                handler.PlatformView.SetMentionFontFamily(null);
            else
            {
                var typeface = fontManager.GetTypeface(editor.MentionFont.Value);
                handler.PlatformView.SetMentionFontFamily(typeface);
            }
        }
    }
}
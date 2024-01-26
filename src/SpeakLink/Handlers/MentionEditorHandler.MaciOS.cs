using System.Diagnostics.CodeAnalysis;
using Foundation;
using LinkedIn.Hakawai;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;
using SpeakLink.Controls.MaciOS;
using SpeakLink.Mention;
using TextChangedEventArgs = Microsoft.Maui.Controls.TextChangedEventArgs;


namespace SpeakLink.Handlers;

public partial class MentionEditorHandler : ViewHandler<MentionEditor, SpeakLinkMentionTextView>
{
    private bool _ignoreFormattedTextChanges;

    static partial void MapText(MentionEditorHandler handler, MentionEditor view)
    {
        handler.PlatformView?.SetText(view.Text);
    }

    static partial void MapFont(MentionEditorHandler handler, MentionEditor view)
    {
        var nativeFont =
            view.Font.ToUIFont(handler.MauiContext!.Services.GetRequiredService<IFontManager>());
        handler.PlatformView.Font = nativeFont;
        handler.PlatformView.TransformTypingAttributesWithTransformer(x =>
        {
            var newTypingAttributes = new NSMutableDictionary(x);
            if (newTypingAttributes.ContainsKey(UIStringAttributeKey.Font))
                newTypingAttributes[UIStringAttributeKey.Font] = nativeFont;

            return newTypingAttributes;
        });

        handler.PlatformView.SetPlaceholderFont(nativeFont);
    }

    private static void MapAutoSize(MentionEditorHandler handler, MentionEditor view)
    {
        if (view.AutoSize != EditorAutoSizeOption.TextChanges) 
            return;
        
        handler.PlatformView.InvalidateIntrinsicContentSize();
    }

    static partial void MapTextColor(MentionEditorHandler handler, MentionEditor view)
    {
        handler.PlatformView.TextColor = view.TextColor?.ToPlatform() ?? GetDefaultLabelColor();
    }

    private static void MapIsEnabled(MentionEditorHandler handler, MentionEditor view)
    {
        handler.PlatformView.Editable = view.IsEnabled;
    }

    static partial void MapIsMentionsEnabled(MentionEditorHandler handler, MentionEditor view)
    {
        handler.PlatformView.IsMentionsEnabled = view.IsMentionsEnabled;
    }

    private static void MapMentionSearchCommand(MentionEditorHandler handler, MentionEditor view)
    {
        handler.PlatformView.MentionSearchCommand = view.MentionSearchCommand;
    }

    protected override void ConnectHandler(SpeakLinkMentionTextView platform)
    {
        base.ConnectHandler(platform);
        platform.MentionSearched += MentionSearched;
        platform.TextChanged += OnTextChanged;
        platform.DisplaySuggestionChanged += DisplaySuggestionChanged;
        platform.FirstResponderStateChanged += FirstResponderStateChanged;
        platform.SetupMentions();
        platform.ScrollEnabled = true;
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        _ignoreFormattedTextChanges = true;
        ElementController?.OnTextChanged(e.OldTextValue, e.NewTextValue);
        ElementController?.SendFormattedTextChanged(GetFormattedText());
        _ignoreFormattedTextChanges = false;
    }

    private void DisplaySuggestionChanged(object? sender, bool e)
    {
        ElementController?.SendDisplaySuggestionsChanged(e);
    }

    private void MentionSearched(object? sender, MentionSearchEventArgs e)
    {
        ElementController?.SendMentionSearched(e);
    }

    protected override void DisconnectHandler(SpeakLinkMentionTextView platform)
    {
        platform.MentionSearched -= MentionSearched;
        platform.DisplaySuggestionChanged -= DisplaySuggestionChanged;
        platform.TextChanged -= OnTextChanged;
        platform.FirstResponderStateChanged -= FirstResponderStateChanged;
        base.DisconnectHandler(platform);
    }

    private void FirstResponderStateChanged(object? sender, bool e)
    {
        if(VirtualView is IView view && view.IsFocused != e)
            view.IsFocused = e;
    }

    static partial void MapIsSuggestionsPopupVisible(MentionEditorHandler handler, MentionEditor view)
    {
        handler.PlatformView.SuggestionPopupVisible = view.IsSuggestionsPopupVisible;
    }

    protected override SpeakLinkMentionTextView CreatePlatformView()
    {
        return new SpeakLinkMentionTextView();
    }

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    private static UIColor GetDefaultLabelColor()
    {
        if (OperatingSystem.IsIOSVersionAtLeast(13)
            || OperatingSystem.IsTvOSVersionAtLeast(13)
            || OperatingSystem.IsMacCatalystVersionAtLeast(13))
            return UIColor.Label;

        return UIColor.Black;
    }

    private static void MapExplicitCharacter(MentionEditorHandler handler, MentionEditor editor)
    {
        handler.PlatformView.ExplicitCharacters = editor.ExplicitCharacters;
    }

    static partial void MapMentionInsertRequested(MentionEditorHandler handler, MentionEditor view, object? arg)
    {
        if (arg is IHKWMentionsEntityProtocol mentionsEntityProtocol)
            handler.PlatformView.MentionSelected(mentionsEntityProtocol);
    }

    static partial void MapFormattedText(MentionEditorHandler handler, MentionEditor view)
    {
        if (handler._ignoreFormattedTextChanges)
            return;
        handler.PlatformView.AttributedText = handler.ConvertFormattedTextToAttributedText(view);
    }

    protected virtual NSAttributedString ConvertFormattedTextToAttributedText(
        MentionEditor editor)
    {
        var formattedString = editor.FormattedText;
        if (formattedString == null)
            return new NSAttributedString(string.Empty);

        var attributed = new NSMutableAttributedString();
        foreach (var span in formattedString.Spans)
        {
            if (span.Text == null)
                continue;

            span.FontFamily ??= editor.FontFamily;

            var attributedStringSpan = span.ToNSAttributedString(
                MauiContext!.Services.GetRequiredService<IFontManager>(),
                0d,
                defaultFont: editor.ToFont(),
                defaultColor: editor.TextColor,
                defaultTextTransform: TextTransform.None);

            if (span is MentionSpan mentionSpan)
            {
                var mutableAttributedStringSpan = new NSMutableAttributedString(attributedStringSpan);

                mutableAttributedStringSpan.AddAttribute(SpeakLinkMentionTextView.HkwMentionAttributeNameString,
                    HKWMentionsAttribute.MentionWithText(mentionSpan.Text, mentionSpan.MentionId),
                    new NSRange(0, span.Text.Length));
                mutableAttributedStringSpan.AddAttribute(UIStringAttributeKey.ForegroundColor,
                    editor.MentionTextColor?.ToPlatform() ?? PlatformView.TextColor ?? UIColor.Black,
                    new NSRange(0, span.Text.Length));
                attributedStringSpan = mutableAttributedStringSpan;
            }

            attributed.Append(attributedStringSpan);
        }

        return attributed;
    }

    protected virtual FormattedString GetFormattedText()
    {
        var linkMentionsEditText = PlatformView;
        var attributedString = PlatformView.AttributedText;
        var formattedString = new FormattedString();

        nint length = attributedString.Length;
        nint i = 0;
        while (i < length)
        {
            var attributes = attributedString.GetAttributes(i, out var effectiveRange);

            var text = attributedString.Substring(effectiveRange.Location, effectiveRange.Length).Value;

            Span span;
            if (attributes?.ContainsKey(SpeakLinkMentionTextView.HkwMentionAttributeNameString) ?? false)
            {
                // Extract mention attributes
                var mentionAttribute =
                    (HKWMentionsAttribute)attributes[SpeakLinkMentionTextView.HkwMentionAttributeNameString];
                span = new MentionSpan(mentionAttribute.EntityIdentifier, mentionAttribute.MentionText);
            }
            else
            {
                span = new Span { Text = text };
            }

            // Apply common attributes to the span (e.g., font, color)
            if (attributes?.ContainsKey(UIStringAttributeKey.Font) ?? false)
            {
                var font = (UIFont)(attributes[UIStringAttributeKey.Font]);
                if (font.FamilyName != linkMentionsEditText.Font?.FamilyName)
                    span.FontFamily = font.FamilyName;
                if (font.FontDescriptor.SymbolicTraits.HasFlag(UIFontDescriptorSymbolicTraits.Bold))
                    span.FontAttributes |= FontAttributes.Bold;
                if (font.FontDescriptor.SymbolicTraits.HasFlag(UIFontDescriptorSymbolicTraits.Italic))
                    span.FontAttributes |= FontAttributes.Italic;

                if (font.PointSize != linkMentionsEditText.Font?.PointSize)
                    span.FontSize = font.PointSize;
            }

            if ((attributes?.ContainsKey(UIStringAttributeKey.ForegroundColor) ?? false)
                && attributes[UIStringAttributeKey.ForegroundColor] is UIColor foregroundColor)
            {
                span.TextColor = foregroundColor.ToColor();
            }

            if (attributes?.ContainsKey(UIStringAttributeKey.UnderlineStyle) ?? false)
            {
                span.TextDecorations |= TextDecorations.Underline;
            }

            if (attributes?.ContainsKey(UIStringAttributeKey.StrikethroughStyle) ?? false)
            {
                span.TextDecorations |= TextDecorations.Strikethrough;
            }

            if (attributes?.ContainsKey(UIStringAttributeKey.BackgroundColor) ?? false)
            {
                span.BackgroundColor = ((UIColor)attributes[UIStringAttributeKey.BackgroundColor]).ToColor();
            }

            formattedString.Spans.Add(span);
            i = effectiveRange.Location + effectiveRange.Length;
        }

        return formattedString;
    }

    public static void MapMentionColors(MentionEditorHandler handler, MentionEditor view)
    {
        var mentionTextColor = view.MentionTextColor?.ToPlatform();
        var mentionSelectedTextColor = view.MentionSelectedTextColor?.ToPlatform();
        var mentionSelectedBackgroundColor = view.MentionSelectedBackgroundColor?.ToPlatform();

        if (!ArgbEquivalent(handler.PlatformView.MentionTextColor, mentionTextColor))
            handler.PlatformView.MentionTextColor = mentionTextColor;
        if (!ArgbEquivalent(handler.PlatformView.MentionSelectedTextColor, mentionSelectedTextColor))
            handler.PlatformView.MentionSelectedTextColor = mentionSelectedTextColor;
        if (!ArgbEquivalent(handler.PlatformView.MentionSelectedBackgroundColor, mentionSelectedBackgroundColor))
            handler.PlatformView.MentionSelectedBackgroundColor = mentionSelectedBackgroundColor;
    }

    public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
    {
        if (double.IsInfinity(widthConstraint) || double.IsInfinity(heightConstraint))
        {
            // If we drop an infinite value into base.GetDesiredSize for the Editor, we'll
            // get an exception; it doesn't know what do to with it. So instead we'll size
            // it to fit its current contents and use those values to replace infinite constraints

            PlatformView.SizeToFit();

            if (double.IsInfinity(widthConstraint)) widthConstraint = PlatformView.Frame.Size.Width;

            if (double.IsInfinity(heightConstraint)) heightConstraint = PlatformView.Frame.Size.Height;
        }

        return base.GetDesiredSize(widthConstraint, heightConstraint);
    }

    protected static void MapPlaceholderText(MentionEditorHandler handler, MentionEditor editor)
    {
        handler.PlatformView.PlaceholderText = editor.Placeholder;
    }

    public static bool ArgbEquivalent(UIColor? color1, UIColor? color2, double? tolerance = null)
    {
        const double minTolerance = 0.000001d;

        bool Equal(nfloat v1, nfloat v2, double tolerance)
        {
            return Math.Abs(v1 - v2) <= tolerance;
        }

        tolerance ??= minTolerance;

        if (color1 == null && color2 == null)
            return true;
        if (color1 == null || color2 == null)
            return false;

        color1.GetRGBA(out nfloat red1, out nfloat green1, out nfloat blue1, out nfloat alpha1);
        color2.GetRGBA(out nfloat red2, out nfloat green2, out nfloat blue2, out nfloat alpha2);

        return Equal(red1, red2, tolerance.Value)
            && Equal(green1, green2, tolerance.Value)
            && Equal(blue1, blue2, tolerance.Value)
            && Equal(alpha1, alpha2, tolerance.Value);
    }

    internal static void MapIsFocused(MentionEditorHandler handler, MentionEditor editor)
    {
        var platformView = handler.PlatformView;
        if (editor.IsFocused)
        {
            if (!platformView.IsFirstResponder)
                platformView.BecomeFirstResponder();
        }
        else
        {   
            if (platformView.IsFirstResponder)
                handler.PlatformView.ResignFirstResponder();
        }
#if !MACCATALYST
        //_resignFirstResponderActionDisposable = ResignFirstResponderTouchGestureRecognizer.Update(handler.PlatformView);
#endif
    }
    
    public static void MapMentionCommand(MentionEditorHandler handler, MentionEditor editor)
    {
        if (handler?.PlatformView != null)
            handler.PlatformView.ImageInputCommand = editor.ImageInputCommand;
    }
}
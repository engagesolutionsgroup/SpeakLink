using System.Globalization;
using SpeakLink.RichText;

namespace SpeakLink.Sample;

public partial class RichEditorMainPage : ContentPage
{
    public RichEditorMainPage()
    {
        InitializeComponent();

        this.BindingContext = new MentionsViewModel();
    }
    

    private void HideKeyboard(object? sender, TappedEventArgs e)
    {
        if (!MentionEditor.IsFocused)
            return;
        
        if (MentionEditor.IsSuggestionsPopupVisible
            && e.GetPosition(MentionCollectionView) is { } position
            && position.InsideElement(MentionCollectionView))
        {
            return;
        }

        MentionEditor.Unfocus();
    }

    private void UserTappedFromMentionSelector(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is User user)
            MentionEditor.InsertMention(user.Id.ToString(), user.FullName);
    }

    private void StyleTapped(object? sender, TappedEventArgs e)
    {
        if(sender is VisualElement { BindingContext: IToolbarSpanStyle spanStyle })
            spanStyle.Toggle();
    }
}

public class RichEditorStyleToGlyph : IValueConverter
{
    private FormattedString GetFormattedString(string text, FontAttributes fontAttributes, TextDecorations textDec) =>
        new() { Spans = { new Span { Text = text, FontAttributes = fontAttributes, TextDecorations = textDec } } };


    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (RichEditorStyle) value! switch
        {
            RichEditorStyle.Bold => GetFormattedString("B", FontAttributes.Bold, TextDecorations.None),
            RichEditorStyle.Italic => GetFormattedString("I", FontAttributes.Italic, TextDecorations.None),
            RichEditorStyle.Strikethrough =>
                GetFormattedString("S", FontAttributes.None, TextDecorations.Strikethrough),
            RichEditorStyle.Underline => GetFormattedString("S", FontAttributes.None, TextDecorations.Underline),
            RichEditorStyle.Link => GetFormattedString("L", FontAttributes.None, TextDecorations.None),
            RichEditorStyle.Subscript => GetFormattedString("Sub", FontAttributes.None, TextDecorations.None),
            RichEditorStyle.Superscript => GetFormattedString("Sup", FontAttributes.None, TextDecorations.None),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
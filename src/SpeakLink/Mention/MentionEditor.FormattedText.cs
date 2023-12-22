namespace SpeakLink.Mention;

public partial class MentionEditor
{
    public static readonly BindableProperty FormattedTextProperty = BindableProperty.Create(
        nameof(FormattedText), typeof(FormattedString), typeof(MentionEditor),
        new FormattedString(), BindingMode.TwoWay, propertyChanged: OnFormattedTextChanged);

    internal static void OnFormattedTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MentionEditor editor)
            editor.ResizeIfNeeded();
    }

    public FormattedString? FormattedText
    {
        get => (FormattedString)GetValue(FormattedTextProperty);
        set => SetValue(FormattedTextProperty, value);
    }

    public void SendFormattedTextChanged(FormattedString formattedString)
    {
        FormattedText = formattedString;
    }
}
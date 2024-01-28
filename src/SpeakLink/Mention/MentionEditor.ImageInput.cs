using System.Windows.Input;

namespace SpeakLink.Mention;

public partial class MentionEditor
{
    public static readonly BindableProperty ImageInputCommandProperty = BindableProperty.Create(
        nameof(ImageInputCommand), typeof(ICommand), typeof(MentionEditor), default);

    public ICommand ImageInputCommand
    {
        get => (ICommand)GetValue(ImageInputCommandProperty);
        set => SetValue(ImageInputCommandProperty, value);
    }
}
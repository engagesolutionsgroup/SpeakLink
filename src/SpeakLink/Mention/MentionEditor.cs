using System.ComponentModel;

namespace SpeakLink.Mention;

public partial class MentionEditor : View, IEditorController, IPaddingElement
{    
    private double _previousWidthConstraint;
    private double _previousHeightConstraint;
    private Rect _previousBounds;
    
    internal string MentionInsertRequestCommand = nameof(MentionInsertRequestCommand);

    /// <summary>Bindable property for <see cref="AutoSize"/>.</summary>
    public static readonly BindableProperty AutoSizeProperty = BindableProperty.Create(nameof(AutoSize),
        typeof(EditorAutoSizeOption), typeof(MentionEditor), EditorAutoSizeOption.Disabled,
        propertyChanged: (bindable, oldValue, newValue)
            => ((MentionEditor)bindable)?.UpdateAutoSizeOption());

    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text),
        typeof(string), typeof(MentionEditor), default(string));


    public static readonly BindableProperty ExplicitCharactersProperty = BindableProperty.Create(
        nameof(ExplicitCharacters), typeof(string), typeof(MentionEditor), "@");

    public string ExplicitCharacters
    {
        get => (string)GetValue(ExplicitCharactersProperty);
        set => SetValue(ExplicitCharactersProperty, value);
    }

    public event EventHandler Completed;

    public EditorAutoSizeOption AutoSize
    {
        get => (EditorAutoSizeOption)GetValue(AutoSizeProperty);
        set => SetValue(AutoSizeProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    private void UpdateAutoSizeOption() => ResizeIfNeeded();

    [EditorBrowsable(EditorBrowsableState.Never)]
    void IEditorController.SendCompleted()
    {
        Completed?.Invoke(this, EventArgs.Empty);
    }

    public virtual void OnTextChanged(string? oldValue, string? newValue) 
        => ResizeIfNeeded();

    protected virtual void ResizeIfNeeded()
    {
        if (AutoSize == EditorAutoSizeOption.TextChanges)
            InvalidateMeasure();
    }

    public void OnCompleted()
    {
        (this as IEditorController).SendCompleted();
    }

    protected override Size ArrangeOverride(Rect bounds)
    {
        
        _previousBounds = bounds;
        return base.ArrangeOverride(bounds);
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        bool TheSame(double width, double otherWidth)
        {
            return Math.Abs(width - otherWidth) < double.Epsilon;
        }

        if (AutoSize == EditorAutoSizeOption.Disabled &&
            Width > 0 &&
            Height > 0)
        {
            if (TheSame(_previousHeightConstraint, heightConstraint) &&
                TheSame(_previousWidthConstraint, widthConstraint))
                return new Size(Width, Height);

            if (TheSame(_previousHeightConstraint, _previousBounds.Height) &&
                TheSame(_previousWidthConstraint, _previousBounds.Width))
                return new Size(Width, Height);
        }

        _previousWidthConstraint = widthConstraint;
        _previousHeightConstraint = heightConstraint;
        return base.MeasureOverride(widthConstraint, heightConstraint);
    }

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder), typeof(string), typeof(Editor), default);

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }
}
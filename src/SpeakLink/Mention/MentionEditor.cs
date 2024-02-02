using System.ComponentModel;

namespace SpeakLink.Mention;

public partial class MentionEditor : View, IEditorController
{    
    private double _previousWidthConstraint;
    private double _previousHeightConstraint;
    private Rect _previousBounds;
    
    internal string MentionInsertRequestCommand = nameof(MentionInsertRequestCommand);
    
    public event EventHandler<TextChangedEventArgs>? TextChanged;
    public event EventHandler<SelectionChangedEventArgs> SelectionChangedEventArgs; 

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

    void IMentionController.OnTextChanged(string? oldValue, string? newValue)
    {
        ResizeIfNeeded();
        TextChanged?.Invoke(this, new TextChangedEventArgs(oldValue, newValue));
    }

    void IMentionController.SendSelectionChanged(int selStart, int selEnd)
    {
        if(CursorPosition != selStart)
            CursorPosition = selStart;
        
        var selectionLength = selEnd - selStart;
        if (SelectionLength != selectionLength)
            SelectionLength = selectionLength;
        
        SelectionChangedEventArgs?.Invoke(this, new(selStart, selectionLength));
    }

    protected virtual void ResizeIfNeeded()
    {
        if (AutoSize == EditorAutoSizeOption.TextChanges &&
            MeasureOverride(_previousWidthConstraint, _previousHeightConstraint)
            == new Size(_previousBounds.Width, _previousBounds.Height))
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
#if IOS
        if(MaximumHeightRequest > 0 && heightConstraint > MaximumHeightRequest)
            heightConstraint = MaximumHeightRequest;
#endif
        
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


    public static readonly BindableProperty CursorPositionProperty = BindableProperty.Create(
        nameof(CursorPosition), typeof(int), typeof(MentionEditor), 0);

    public int CursorPosition
    {
        get => (int)GetValue(CursorPositionProperty);
        set => SetValue(CursorPositionProperty, value);
    }

    public static readonly BindableProperty SelectionLengthProperty = BindableProperty.Create(
        nameof(SelectionLength), typeof(int), typeof(MentionEditor), 0);

    public int SelectionLength
    {
        get => (int)GetValue(SelectionLengthProperty);
        set => SetValue(SelectionLengthProperty, value);
    }
}
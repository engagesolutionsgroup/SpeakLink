using Microsoft.Maui.Controls.Internals;
using Font = Microsoft.Maui.Font;

namespace SpeakLink.Mention;

public partial class MentionEditor : ITextStyle, IFontElement
{
    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(MentionEditor), null,
            propertyChanged: OnTextColorPropertyChanged);

    public static readonly BindableProperty FontAttributesProperty =
        BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(MentionEditor),
            FontAttributes.None,
            propertyChanged: OnFontAttributesChanged);

    /// <summary>
    /// The backing store for the <see cref="ITextElement.CharacterSpacing" /> bindable property.
    /// </summary>
    public static readonly BindableProperty CharacterSpacingProperty =
        BindableProperty.Create(nameof(CharacterSpacing), typeof(double), typeof(MentionEditor), 0.0d,
            propertyChanged: OnCharacterSpacingPropertyChanged);
    
    /// <summary>
    /// The backing store for the <see cref="IFontElement.FontFamily" /> bindable property.
    /// </summary>
    public static readonly BindableProperty FontFamilyProperty =
        BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(IFontElement), default(string),
            propertyChanged: OnFontFamilyChanged);

    /// <summary>
    /// The backing store for the <see cref="IFontElement.FontSize" /> bindable property.
    /// </summary>
    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(IFontElement), 0d,
            propertyChanged: OnFontSizeChanged,
            defaultValueCreator: FontSizeDefaultValueCreator);

    public static readonly BindableProperty MentionFontFamilyProperty = BindableProperty.Create(
        nameof(MentionFontFamily), typeof(string), typeof(MentionEditor), default, 
        propertyChanged:OnMentionFontFamilyChanged);

    public string MentionFontFamily
    {
        get { return (string)GetValue(MentionFontFamilyProperty); }
        set { SetValue(MentionFontFamilyProperty, value); }
    }


    /// <summary>
    /// The backing store for the <see cref="IFontElement.FontAutoScalingEnabled" /> bindable property.
    /// </summary>
    public static readonly BindableProperty FontAutoScalingEnabledProperty =
        BindableProperty.Create(nameof(FontAutoScalingEnabled), typeof(bool), typeof(IFontElement), true,
            propertyChanged: OnFontAutoScalingEnabledChanged);

    private static void OnTextColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((MentionEditor)bindable).OnTextColorPropertyChanged((Color)oldValue, (Color)newValue);
    }

    private static void OnCharacterSpacingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((MentionEditor)bindable).OnCharacterSpacingPropertyChanged((double)oldValue, (double)newValue);
    }

    protected virtual void OnTextColorPropertyChanged(Color oldValue, Color newValue)
    {
    }

    protected virtual void OnCharacterSpacingPropertyChanged(double oldValue, double newValue)
    {
        InvalidateMeasure();
    }
    
    public double FontSizeDefaultValueCreator()
    {
        return Handler?.MauiContext?.Services?.GetService<IFontManager>()?.DefaultFontSize ?? 14d;
    }

    public void OnFontAttributesChanged(FontAttributes oldValue, FontAttributes newValue)
    {
        //
    }

    private static void OnFontAttributesChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        //throw new NotImplementedException();
    }

    public virtual void OnFontFamilyChanged(string oldValue, string newValue)
    {
        HandleFontChanged();
    }

    public virtual void OnFontSizeChanged(double oldValue, double newValue)
    {
        HandleFontChanged();
    }

    public virtual void OnFontAutoScalingEnabledChanged(bool oldValue, bool newValue)
    {
        HandleFontChanged();
    }

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public double CharacterSpacing
    {
        get => (double)GetValue(CharacterSpacingProperty);
        set => SetValue(CharacterSpacingProperty, value);
    }

    public Font Font
    {
        get
        {
            var size = FontSize;
            return Font.OfSize(FontFamily, size, enableScaling: FontAutoScalingEnabled);
        }
    }
    
    public Font? MentionFont
    {
        get
        {
            var size = FontSize;
            return Font.OfSize(MentionFontFamily, size, enableScaling:FontAutoScalingEnabled);
        }
    }

    protected void HandleFontChanged()
    {
        Handler?.UpdateValue(nameof(ITextStyle.Font));
        InvalidateMeasure();
    }

    private static void OnFontFamilyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((IFontElement)bindable).OnFontFamilyChanged((string)oldValue, (string)newValue);
    }
    
    private static void OnMentionFontFamilyChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is not VisualElement visualElement) 
            return;
        
        visualElement.Handler?.UpdateValue(nameof(MentionFont));
        visualElement.InvalidateMeasure();
    }

    private static void OnFontSizeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((IFontElement)bindable).OnFontSizeChanged((double)oldValue, (double)newValue);
    }

    private static object FontSizeDefaultValueCreator(BindableObject bindable)
    {
        return ((IFontElement)bindable).FontSizeDefaultValueCreator();
    }

    private static void OnFontAutoScalingEnabledChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((IFontElement)bindable).OnFontAutoScalingEnabledChanged((bool)oldValue, (bool)newValue);
    }

    /// <summary>
    /// Gets or sets the font family for the text of this entry. This is a bindable property.
    /// </summary>
    public string FontFamily
    {
        get => (string)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    /// <summary>
    /// Gets or sets the size of the font for the text of this entry. This is a bindable property.
    /// </summary>
    [System.ComponentModel.TypeConverter(typeof(FontSizeConverter))]
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// Determines whether or not the font of this entry should scale automatically according to the operating system settings. Default value is <see langword="true"/>.
    /// This is a bindable property.
    /// </summary>
    /// <remarks>Typically this should always be enabled for accessibility reasons.</remarks>
    public bool FontAutoScalingEnabled
    {
        get => (bool)GetValue(FontAutoScalingEnabledProperty);
        set => SetValue(FontAutoScalingEnabledProperty, value);
    }
    
    public FontAttributes FontAttributes
    {
        get => (FontAttributes)GetValue(FontAttributesProperty);
        set => SetValue(FontAttributesProperty, value);
    }
}
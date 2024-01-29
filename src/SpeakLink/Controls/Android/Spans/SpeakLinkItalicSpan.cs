using System.Runtime.Versioning;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text.Style;

namespace SpeakLink.Controls.Android.Spans;

public class SpeakLinkItalicSpan : StyleSpan
{
    private const TypefaceStyle TypefaceStyle = global::Android.Graphics.TypefaceStyle.Italic;
    
    protected SpeakLinkItalicSpan(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }

    public SpeakLinkItalicSpan(Parcel src) : base(src)
    {
    }

    public SpeakLinkItalicSpan() : base(TypefaceStyle)
    {
    }
    
    [SupportedOSPlatform("android33.0")]
    public SpeakLinkItalicSpan(int fontWeightAdjustment) 
        : base(TypefaceStyle, fontWeightAdjustment)
    {
    }
}
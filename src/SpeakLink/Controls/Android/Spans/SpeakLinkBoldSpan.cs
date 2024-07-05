using System.Runtime.Versioning;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text.Style;

namespace SpeakLink.Controls.Android.Spans;

public class SpeakLinkBoldSpan : StyleSpan
{
    private const TypefaceStyle TypefaceStyle = global::Android.Graphics.TypefaceStyle.Bold;

    protected SpeakLinkBoldSpan(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }

    public SpeakLinkBoldSpan(Parcel src) : base(src)
    {
    }

    public SpeakLinkBoldSpan() : base(TypefaceStyle)
    {
    }

    [SupportedOSPlatform("android33.0")]
    public SpeakLinkBoldSpan(int fontWeightAdjustment)
        : base(TypefaceStyle, fontWeightAdjustment)
    {
    }
}
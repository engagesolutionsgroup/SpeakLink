using Android.OS;
using Android.Runtime;
using Android.Text.Style;

namespace SpeakLink.Controls.Android.Spans;

public class SpeakLinkStrikethroughSpan : StrikethroughSpan
{
    protected SpeakLinkStrikethroughSpan(IntPtr javaReference, JniHandleOwnership transfer) 
        : base(javaReference, transfer)
    {
    }

    public SpeakLinkStrikethroughSpan()
    {
    }

    public SpeakLinkStrikethroughSpan(Parcel src) : base(src)
    {
    }
}
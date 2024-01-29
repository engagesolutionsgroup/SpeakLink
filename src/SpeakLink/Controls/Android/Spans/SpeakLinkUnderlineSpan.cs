using Android.OS;
using Android.Runtime;
using Android.Text.Style;

namespace SpeakLink.Controls.Android.Spans;

public class SpeakLinkUnderlineSpan : UnderlineSpan
{
    protected SpeakLinkUnderlineSpan(IntPtr javaReference, JniHandleOwnership transfer) 
        : base(javaReference, transfer)
    {
    }

    public SpeakLinkUnderlineSpan()
    {
    }

    public SpeakLinkUnderlineSpan(Parcel src) : base(src)
    {
    }
}
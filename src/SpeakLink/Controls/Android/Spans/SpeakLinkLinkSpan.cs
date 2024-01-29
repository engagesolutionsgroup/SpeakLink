using Android.OS;
using Android.Runtime;
using Android.Text.Style;

namespace SpeakLink.Controls.Android.Spans;

public class SpeakLinkLinkSpan : URLSpan
{
    protected SpeakLinkLinkSpan(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }

    public SpeakLinkLinkSpan(Parcel src) : base(src)
    {
    }

    public SpeakLinkLinkSpan(string? url) : base(url)
    {
    }
}
using Android.OS;
using Android.Runtime;
using Android.Text.Style;

namespace SpeakLink.Controls.Android.Spans;

public class SpeakLinkSubscriptSpan : SubscriptSpan
{
    protected SpeakLinkSubscriptSpan(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }

    public SpeakLinkSubscriptSpan()
    {
    }

    public SpeakLinkSubscriptSpan(Parcel src) : base(src)
    {
    }
}
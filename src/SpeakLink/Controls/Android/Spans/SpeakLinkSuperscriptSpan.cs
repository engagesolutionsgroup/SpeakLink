using Android.OS;
using Android.Runtime;
using Android.Text.Style;

namespace SpeakLink.Controls.Android.Spans;

public class SpeakLinkSuperscriptSpan : SuperscriptSpan
{
    protected SpeakLinkSuperscriptSpan(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }

    public SpeakLinkSuperscriptSpan()
    {
    }

    public SpeakLinkSuperscriptSpan(Parcel src) : base(src)
    {
    }
}
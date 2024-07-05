using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using LinkedIn.Spyglass.Mentions;

namespace SpeakLink.Controls.Android.Spans;

public class SpeakLinkMentionSpan : MentionSpan
{
    private Typeface? _typeface;
    
    protected SpeakLinkMentionSpan(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }

    public SpeakLinkMentionSpan(Parcel src) : base(src)
    {
    }

    public SpeakLinkMentionSpan(IMentionable mentionable) : base(mentionable) 
    { 
    }
    public SpeakLinkMentionSpan(IMentionable mentionable, MentionSpanConfig mentionSpanConfig) : base(mentionable, mentionSpanConfig) 
    { 
    }

    public SpeakLinkMentionSpan(IMentionable mentionable, MentionSpanConfig mentionSpanConfig, Typeface? typeface) : base(mentionable, mentionSpanConfig) 
    { 
        _typeface = typeface;
    }

    public override void UpdateDrawState(TextPaint ds)
    {
        
        if(_typeface != null)
        {
            ds.SetTypeface(_typeface);
        }

        base.UpdateDrawState(ds);
    }
}
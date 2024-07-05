using Android.Graphics;
using LinkedIn.Spyglass.Mentions;
using LinkedIn.Spyglass.Ui;
using SpeakLink.Controls.Android.Spans;

namespace SpeakLink.Controls.Android;

public class SpeakLinkMentionSpanFactory : MentionsEditText.MentionSpanFactory
{
    public Typeface? MentionTypeface { get; set; }
    public override MentionSpan CreateMentionSpan(IMentionable mention, MentionSpanConfig? config)
        => (config != null) ? new SpeakLinkMentionSpan(mention, config, MentionTypeface)  : new MentionSpan(mention);
}
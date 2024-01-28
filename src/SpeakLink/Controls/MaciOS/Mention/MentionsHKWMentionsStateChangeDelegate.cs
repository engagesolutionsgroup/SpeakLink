using Foundation;
using LinkedIn.Hakawai;
using ObjCRuntime;

namespace SpeakLink.Controls.MaciOS;

public class MentionsHkwMentionsStateChangeDelegate : HKWMentionsStateChangeDelegate
{
    public MentionsHkwMentionsStateChangeDelegate(Action<bool>? sendOnDisplaySuggestionsChanged,
        Action<HKWMentionsPlugin, HKWMentionsEntityProtocol>? mentionDeleted)
    {
        SendOnDisplaySuggestionsChanged = sendOnDisplaySuggestionsChanged;
        MentionDeleted = mentionDeleted;
    }

    protected MentionsHkwMentionsStateChangeDelegate(NSObjectFlag t) : base(t)
    {
    }

    protected internal MentionsHkwMentionsStateChangeDelegate(NativeHandle handle) : base(handle)
    {
    }

    [Export("mentionsPlugin:stateChangedTo:from:")]
    public override void StateChangedTo(HKWMentionsPlugin plugin, HKWMentionsPluginState newState,
        HKWMentionsPluginState oldState)
    {
        IsDisplaying = newState == HKWMentionsPluginState.CreatingMention;
        SendOnDisplaySuggestionsChanged?.Invoke(newState == HKWMentionsPluginState.CreatingMention);
    }

    public override void Selected(HKWMentionsEntityProtocol entity, NSIndexPath indexPath)
    {
        SendOnDisplaySuggestionsChanged?.Invoke(false);
        IsDisplaying = false;
    }

    public override void DeletedMention(HKWMentionsPlugin plugin, HKWMentionsEntityProtocol entity, UIntPtr location)
    {
        base.DeletedMention(plugin, entity, location);
        SendOnDisplaySuggestionsChanged?.Invoke(false);
        IsDisplaying = false;
        
        MentionDeleted?.Invoke(plugin, entity);
    }

    public override void  MentionsPluginWillActivateChooserView(HKWMentionsPlugin plugin)
    {
        SendOnDisplaySuggestionsChanged?.Invoke(true);
        IsDisplaying = true;
    }

    public override void MentionsPluginActivatedChooserView(HKWMentionsPlugin plugin)
    {
        SendOnDisplaySuggestionsChanged?.Invoke(true);
        IsDisplaying = true;
    }

    public override void MentionsPluginDeactivatedChooserView(HKWMentionsPlugin plugin)
    {
        SendOnDisplaySuggestionsChanged?.Invoke(false);
        IsDisplaying = false;
    }

    public Action<bool>? SendOnDisplaySuggestionsChanged { get; set; }
    public Action<HKWMentionsPlugin, HKWMentionsEntityProtocol>? MentionDeleted { get; set; }
    public bool IsDisplaying { get; set; }
}
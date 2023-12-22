using System.Windows.Input;
using Foundation;
using LinkedIn.Hakawai;
using ObjCRuntime;
using SpeakLink.Mention;

namespace SpeakLink.Controls.MaciOS;

public class MentionsHkwMentionsCustomChooserViewDelegate : HKWMentionsCustomChooserViewDelegate
{
    public Action<MentionSearchEventArgs>? OnMentionSearch;

    public ICommand? MentionSearchCommand { get; set; }

    public override void DidUpdateKeyString(string keyString, ushort character)
    {
        var eventArgs = new MentionSearchEventArgs(((char)character).ToString(), keyString!);

        OnMentionSearch?.Invoke(eventArgs);

        if (MentionSearchCommand?.CanExecute(eventArgs) ?? false)
            MentionSearchCommand!.Execute(eventArgs);
    }

    public override bool EntityCanBeTrimmed(HKWMentionsEntityProtocol entity)
    {
        return false;
    }
}
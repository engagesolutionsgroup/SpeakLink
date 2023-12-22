
#if ANDROID
using Object = Java.Lang.Object;
using Android.OS;
using Android.Runtime;
using LinkedIn.Spyglass.Mentions;

#elif IOS || MACCATALYST
using ObjCRuntime;
using Foundation;
using LinkedIn.Hakawai;
#endif

namespace SpeakLink.Mention;

public class MentionEntity
#if MACCATALYST || IOS
    : NSObject, IHKWMentionsEntityProtocol
#endif
#if ANDROID
    : Object, IMentionable
#endif
{
    public MentionEntity(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; init; }
    public string Name { get; init; }


#if IOS || MACCATALYST
    public MentionEntity(NSObjectFlag x) : base(x)
    {
    }

    protected internal MentionEntity(NativeHandle handle) : base(handle)
    {
    }

    protected MentionEntity(NativeHandle handle, bool alloced) : base(handle, alloced)
    {
    }

    NSString IHKWMentionsEntityProtocol.EntityId()
    {
        return new NSString(Id);
    }

    NSString IHKWMentionsEntityProtocol.EntityName()
    {
        return new NSString(Name);
    }

    NSDictionary IHKWMentionsEntityProtocol.EntityMetadata => new();
#endif
#if ANDROID
    protected MentionEntity()
    {
    }

    public MentionEntity(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
    {
    }
    
    public int DescribeContents() => 0;

    public void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
    {
        dest.WriteString(Name);
        dest.WriteString(Id);
    }

    public int SuggestibleId => Int32.TryParse(Id, out var id) ? id : Id.GetHashCode();

    public string SuggestiblePrimaryText => Name;

    public string GetTextForDisplayMode(IMentionable.MentionDisplayMode p0) => Name;

    public IMentionable.MentionDeleteStyle DeleteStyle => IMentionable.MentionDeleteStyle.FullDelete!;
    
    public static MentionEntity? FromParcel(Parcel? source)
    {
        if (source == null)
            return null;
        
        var name = source.ReadString()!;
        var id = source.ReadString()!;
        return new MentionEntity(id, name);
    }

    public class PersonLoader : Object, IParcelableCreator
    {
        public Object? CreateFromParcel(Parcel? source)
        {
            return FromParcel(source);
        }

        public Object[]? NewArray(int size) => new MentionEntity[size];
    }
#endif
}
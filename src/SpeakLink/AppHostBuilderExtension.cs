using SpeakLink.Handlers;
using SpeakLink.Mention;

namespace SpeakLink;

public static class AppHostBuilderExtension
{
    public static MauiAppBuilder UseSpeakLink(this MauiAppBuilder builder)
    {
        return builder.ConfigureMauiHandlers(handlerCollection =>
        {
            handlerCollection.AddHandler<MentionEditor, MentionEditorHandler>();
        });
    }
}
using SpeakLink.Handlers;
using SpeakLink.Link;
using SpeakLink.Mention;
using SpeakLink.RichText;

namespace SpeakLink;

public static class AppHostBuilderExtension
{
    public static MauiAppBuilder UseSpeakLink(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<ILinkEditorDialogInvoker, LinkEditorDialogInvoker>();
        return builder.ConfigureMauiHandlers(handlerCollection =>
        {
            handlerCollection.AddHandler<MentionEditor, MentionEditorHandler>();
            handlerCollection.AddHandler<RichEditor, RichEditorHandler>();
        });
    }
}
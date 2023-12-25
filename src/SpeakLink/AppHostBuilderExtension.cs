using Microsoft.Maui.Hosting;
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

public static class ViewExtensions
{
    public static IEnumerable<VisualElement> Ancestors(this VisualElement? element)
    {
        while(element != null)
        {
            yield return element;
            element = element.Parent as VisualElement;
        }
    }
    
    public static Point GetAbsolutePosition(this VisualElement visualElement)
    {
        if (visualElement == null) 
            throw new ArgumentNullException(nameof(visualElement));
        
        var ancestors = visualElement.Ancestors().ToArray();
        var x = ancestors.Sum(ancestor => ancestor.X);
        var y = ancestors.Sum(ancestor => ancestor.Y);

        return new Point(x, y);
    }
    
    public static Point GetAbsolutePositionRelativeToScreen(this VisualElement element)
    {
        var result = new Point(element.X, element.Y);
        var parent = element.Parent as VisualElement;
        while (parent != null)
        {
            result.X += parent.X;
            result.Y += parent.Y;
            parent = parent.Parent as VisualElement;
        }

        return result;
    }
    
    public static bool Inside(this Point point, VisualElement element)
    {
        var absolutePosition = element.GetAbsolutePosition();
        return point.X >= absolutePosition.X
               && point.Y >= absolutePosition.Y
               && point.X <= absolutePosition.X + element.Width
               && point.Y <= absolutePosition.Y + element.Height;
    }
}
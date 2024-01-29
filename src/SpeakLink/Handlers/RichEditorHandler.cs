using SpeakLink.RichText;

namespace SpeakLink.Handlers;

public partial class RichEditorHandler : MentionEditorHandler
{
    public new RichEditor? VirtualView => base.VirtualView as RichEditor;
    
    public new static readonly IPropertyMapper<RichEditor, RichEditorHandler> PropertyMapper =
        new PropertyMapper<RichEditor, RichEditorHandler>(MentionEditorHandler.PropertyMapper)
        {
            
        };

    public new static readonly CommandMapper<RichEditor, RichEditorHandler>? CommandMapper =
        new(MentionEditorHandler.CommandMapper)
        {

        };
    
    public RichEditorHandler()
        : this(PropertyMapper, CommandMapper)
    {
    }

    public RichEditorHandler(IPropertyMapper mapper) : base(mapper)
    {
    }

    public RichEditorHandler(IPropertyMapper mapper, CommandMapper? commandMapper) 
        : base(mapper, commandMapper)
    {
    }
}
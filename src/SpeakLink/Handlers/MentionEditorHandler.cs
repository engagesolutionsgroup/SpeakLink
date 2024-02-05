using SpeakLink.Mention;

namespace SpeakLink.Handlers;

public partial class MentionEditorHandler
{
    public static readonly IPropertyMapper<MentionEditor, MentionEditorHandler> PropertyMapper =
        new PropertyMapper<MentionEditor, MentionEditorHandler>(ViewMapper)
        {
            [nameof(MentionEditor.IsEnabled)] = MapIsEnabled,
            [nameof(MentionEditor.IsFocused)] = MapIsFocused,
            //Default font appearance
            [nameof(MentionEditor.Text)] = MapText,
            [nameof(ITextStyle.Font)] = MapFont,
            [nameof(ITextStyle.TextColor)] = MapTextColor,
            [nameof(MentionEditor.AutoSize)] = MapAutoSize,
            [nameof(MentionEditor.Placeholder)] = MapPlaceholderText,
            //FormattedText
            [nameof(MentionEditor.FormattedText)] = MapFormattedText,
            //Mentions
            [nameof(MentionEditor.IsMentionsEnabled)] = MapIsMentionsEnabled,
            [nameof(MentionEditor.ExplicitCharacters)] = MapExplicitCharacter,
            [nameof(MentionEditor.IsSuggestionsPopupVisible)] = MapIsSuggestionsPopupVisible,
            [nameof(MentionEditor.MentionTextColor)] = MapMentionColors,
            [nameof(MentionEditor.MentionSelectedTextColor)] = MapMentionColors,
            [nameof(MentionEditor.MentionSelectedBackgroundColor)] = MapMentionColors,
            //Mentions Commands
            [nameof(MentionEditor.MentionSearchCommand)] = MapMentionSearchCommand,
            [nameof(MentionEditor.ImageInputCommandProperty)] = MapMentionCommand,
            //Cursor
            [nameof(MentionEditor.CursorPosition)] = MapCursorPosition,
            [nameof(MentionEditor.SelectionLength)] = MapSelectionLength,
        };

    public static readonly CommandMapper<MentionEditor, MentionEditorHandler> CommandMapper =
        new(ViewCommandMapper)
        {
            [nameof(MentionEditor.MentionInsertRequestCommand)] = MapMentionInsertRequested,
            [nameof(View.Focus)] = MapFocus,
            [nameof(View.Unfocus)] = MapUnfocus,
        };

    static partial void MapFormattedText(MentionEditorHandler handler, MentionEditor view);
    static partial void MapIsSuggestionsPopupVisible(MentionEditorHandler handler, MentionEditor view);
    static partial void MapMentionInsertRequested(MentionEditorHandler handler, MentionEditor view, object? arg);
    static partial void MapIsMentionsEnabled(MentionEditorHandler handler, MentionEditor view);
    static partial void MapTextColor(MentionEditorHandler handler, MentionEditor view);
    static partial void MapFont(MentionEditorHandler handler, MentionEditor view);
    static partial void MapText(MentionEditorHandler handler, MentionEditor view);

    internal IMentionController ElementController => VirtualView;

    public MentionEditorHandler() : this(PropertyMapper, CommandMapper)
    {
    }

    public MentionEditorHandler(IPropertyMapper mapper) : base(mapper)
    {
    }

    public MentionEditorHandler(IPropertyMapper mapper, CommandMapper? commandMapper) : base(mapper, commandMapper)
    {
        
    }
}
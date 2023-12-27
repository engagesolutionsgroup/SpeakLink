namespace SpeakLink.Mention;

internal interface IMentionController
{
    internal void SendMentionSearched(MentionSearchEventArgs e);
    internal void SendDisplaySuggestionsChanged(bool newState);
    internal void OnTextChanged(string? oldValue, string? newValue);
    internal void SendFormattedTextChanged(FormattedString getFormattedText);
}
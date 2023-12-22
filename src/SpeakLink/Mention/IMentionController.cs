namespace SpeakLink.Mention;

internal interface IMentionController
{
    void SendMentionSearched(MentionSearchEventArgs e);
    void SendDisplaySuggestionsChanged(bool newState);
    void OnTextChanged(string? oldValue, string? newValue);
    void SendFormattedTextChanged(FormattedString getFormattedText);
}
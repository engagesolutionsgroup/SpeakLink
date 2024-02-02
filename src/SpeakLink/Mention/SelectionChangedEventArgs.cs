namespace SpeakLink.Mention;

public class SelectionChangedEventArgs(int start, int length) : EventArgs
{
    public int Start { get; } = start;
    public int Length { get; } = length;
    public int End => Start + Length;
}
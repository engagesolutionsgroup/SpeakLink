using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpeakLink.RichText;

public class RichEditorToolbarState : IRichEditorToolbarState<IToolbarSpanStyle>, INotifyPropertyChanged
{
    public virtual IEnumerable<IToolbarSpanStyle> Styles { get; } = [];
    
    public virtual void RemoveStyle(RichEditorStyle style)
    {
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
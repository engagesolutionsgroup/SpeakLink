using System.ComponentModel;
using System.Windows.Input;

namespace SpeakLink.RichText;

public interface IToolbarSpanStyle : INotifyPropertyChanged
{
    RichEditorStyle RichEditorStyle { get;}
    bool Checked { get; internal set; }
    void Toggle();
    ICommand ToggleCommand { get; }
}
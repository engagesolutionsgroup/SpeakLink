using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeakLink.RichText;

namespace SpeakLink.Sample;

public partial class RootPage : ContentPage
{
    public RootPage()
    {
        InitializeComponent();
    }

    private async void NavigateToMainPage(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }
    
    private async void NavigateToRichEditorPage(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new RichEditorMainPage());
    }
}
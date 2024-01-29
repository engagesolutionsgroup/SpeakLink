namespace SpeakLink.Sample;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new NavigationPage(new RootPage());
    }
}
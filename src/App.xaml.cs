using ArtHex.Services;

namespace ArtHex;

public partial class App : Application
{
    public App(AppService appService)
    {
        appService.InitializeApp();
        InitializeComponent();

        MainPage = new AppShell();
    }
}

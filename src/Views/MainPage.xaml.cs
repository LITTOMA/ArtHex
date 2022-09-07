using SixLabors.ImageSharp;

namespace ArtHex;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}



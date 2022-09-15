using ArtHex.Services;
using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;

namespace ArtHex;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
        builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("CascadiaCode.ttf", "CascadiaCode");
			});

		builder.Services.AddSingleton<AppService>();
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<MainViewModel>();

		return builder.Build();
	}
}

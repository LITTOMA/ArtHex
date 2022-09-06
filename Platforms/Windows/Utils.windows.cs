using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage;

namespace ArtHex;

public static partial class Utils
{
    public static async partial void SaveTextFile(string text)
    {
        FileSavePicker savePicker = new FileSavePicker();

        // Retrieve the window handle (HWND) of the current WinUI 3 window.
        var window = ArtHex.WinUI.App.Current.Application.Windows[0].Handler.PlatformView;
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        // Initialize the folder picker with the window handle (HWND).
        WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);

        savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        savePicker.FileTypeChoices.Add("Intel Hex File", new List<string>() { ".hex" });
        savePicker.SuggestedFileName = "Joystick";

        StorageFile file = await savePicker.PickSaveFileAsync();
        if (file != null)
        {
            CachedFileManager.DeferUpdates(file);
            await FileIO.WriteTextAsync(file, text);
            FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
            if (status == FileUpdateStatus.Complete)
            {
                CommunityToolkit.Maui.Alerts.Toast.Make($"Hex file saved to {file.Path}").Show();
            }
        }
    }
}

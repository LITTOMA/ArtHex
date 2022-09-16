using ArtHex.Models;
using System.Text.Json;

namespace ArtHex.Services
{
    public class AppService
    {
        private readonly DataService dataService;
        public bool IsAppInitialized => Preferences.Get(nameof(IsAppInitialized), false);

        public AppService(DataService dataService)
        {
            this.dataService = dataService;
        }

        public async void InitializeApp()
        {
            if (!IsAppInitialized)
            {
                // Import presets flash data to db.
                using var stream = await FileSystem.OpenAppPackageFileAsync("presets.json");
                var reader = new StreamReader(stream);
                var data = await reader.ReadToEndAsync();
                List<FlashData> flashDatas = JsonSerializer.Deserialize<List<FlashData>>(data);
                dataService.AddFlashDatas(flashDatas);

                // Update the flag.
                Preferences.Set(nameof(IsAppInitialized), true);
            }
        }
    }
}

using ArtHex.Models;
using System.Text.Json;

namespace ArtHex.Services
{
    public class AppService
    {
        public AppService()
        {
            appDatabase = new SQLite.SQLiteConnection(Path.Combine(FileSystem.AppDataDirectory, "app.db"));
            appDatabase.CreateTable<FlashData>();
        }

        SQLite.SQLiteConnection appDatabase;
        public bool IsAppInitialized => Preferences.Get(nameof(IsAppInitialized), false);

        public async void InitializeApp()
        {
            if (!IsAppInitialized)
            {
                // Import presets flash data to db.
                using var stream = await FileSystem.OpenAppPackageFileAsync("presets.json");
                var reader = new StreamReader(stream);
                var data = await reader.ReadToEndAsync();
                List<FlashData> flashDatas = JsonSerializer.Deserialize<List<FlashData>>(data);
                appDatabase.InsertAll(flashDatas);

                // Update the flag.
                Preferences.Set(nameof(IsAppInitialized), true);
            }
        }
    }
}

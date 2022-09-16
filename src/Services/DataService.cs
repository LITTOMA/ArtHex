using ArtHex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtHex.Services
{
    public class DataService
    {
        public DataService()
        {
            appDatabase = new SQLite.SQLiteConnection(Path.Combine(FileSystem.AppDataDirectory, "app.db"));
            appDatabase.CreateTable<FlashData>();
        }

        SQLite.SQLiteConnection appDatabase;

        public void AddFlashDatas(List<FlashData> flashDatas)
        {
            appDatabase.InsertOrReplace(flashDatas);
        }

        public FlashData GetFlashData(string game, string board)
        {
            return appDatabase.Table<FlashData>().FirstOrDefault(data => data.GameName == game && data.BoardName == board);
        }

        public IEnumerable<FlashData> GetAllFlashDatas()
        {
            return appDatabase.Table<FlashData>().Select(data => data);
        }

        public IEnumerable<FlashDataInfo> GetAllFlashDataInfos()
        {
            return appDatabase.Table<FlashData>().Select(data => new FlashDataInfo() { Id = data.Id, BoardName = data.BoardName, GameName = data.GameName });
        }
    }
}

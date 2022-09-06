using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtHex
{
    public static partial class Utils
    {
        public static async partial void SaveTextFile(string text)
        {
            var fileName = $"Joystick-{DateTime.Now:yyyy-MM-dd}.hex";
            var savePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            var saveDir = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);

            File.WriteAllText(savePath, text);
            await Share.RequestAsync(new ShareFileRequest()
            {
                Title = fileName,
                File = new ShareFile(savePath)
            });
        }
    }
}

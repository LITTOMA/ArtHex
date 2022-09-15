using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ArtHex.Models
{
    public class FlashData
    {
        [SQLite.PrimaryKey]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("game")]
        public string GameName { get; set; }

        [JsonPropertyName("board")]
        public string BoardName { get; set; }

        [JsonPropertyName("imageDataOffset")]
        public int ImageDataOffset { get; set; }

        [JsonPropertyName("imageDataSize")]
        public int ImageDataSize { get; set; }

        [JsonPropertyName("bin")]
        [JsonConverter(typeof(Base64BytesConverter))]
        public byte[] Binary { get; set; }
    }
}

using ArtHex.Models;
using HexIO;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace ArtHex;

public static partial class Utils
{
    //static Dictionary<string, (int Offset, int Size)> ImageDataOffsetSize = new Dictionary<string, (int Offset, int Size)>
    //{
    //    { "at90usb1286", new(){ Offset=0x185, Size=4800 } },
    //    { "atmega16u2",  new(){ Offset=0x185, Size=4800 } },
    //    { "atmega32u4",  new(){ Offset=0x185, Size=4800 } },
    //};
    //static Dictionary<string, string> BoardArchitechture = new Dictionary<string, string>
    //{
    //    { "Teensy 2.0++",   "at90usb1286" },
    //    { "Arduino UNO R3", "atmega16u2"  },
    //    { "Arduino Micro",  "atmega32u4"  },
    //};

    public static void ProcessImage(SixLabors.ImageSharp.Image image, float ditherScale)
    {
        if (image == null)
            return;

        PaletteQuantizer paletteQuantizer = new(
                new(new[] {
                    SixLabors.ImageSharp.Color.White,
                    SixLabors.ImageSharp.Color.Black,
                }),
                new()
                {
                    DitherScale = ditherScale,
                    MaxColors = 2
                });
        image.Mutate(operation =>
        {
            operation.Quantize(paletteQuantizer);
        });
    }

    public static async Task<string> MakeHexAsync(SixLabors.ImageSharp.Image image, FlashData flashData)
    {
        if (image.Width != 320 || image.Height != 120)
        {
            throw new Exception($"Invalid image width({image.Width}) or height({image.Height})");
        }

        var a8 = image.CloneAs<Rgb24>();
        Memory<byte> imageData = new Memory<byte>(new byte[a8.Width * a8.Height / 8]);
        int pos = 0;
        byte b = 0;
        byte mask = 1;
        a8.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                Span<Rgb24> pixelRow = accessor.GetRowSpan(y);
                for (int x = 0; x < accessor.Width; x++)
                {
                    if (pixelRow[x].R == 0 && pixelRow[x].G == 0 && pixelRow[x].B == 0)
                    {
                        b |= mask;
                    }

                    mask <<= 1;
                    if (mask == 0)
                    {
                        imageData.Span[pos] = b;
                        pos++;
                        mask = 1;
                        b = 0;
                    }
                }
            }
        });
        return await MakeHexAsync(imageData, flashData);
    }

    public static async Task<string> MakeHexAsync(ReadOnlyMemory<byte> imagePixelData, FlashData flashData)
    {
        try
        {
            var templateData = flashData.Binary;

            using var templateStream = new MemoryStream(templateData);
            templateStream.Seek(flashData.ImageDataOffset, SeekOrigin.Begin);
            templateStream.Write(imagePixelData.Span);
            templateStream.Seek(0, SeekOrigin.Begin);

            using var templateReader = new BinaryReader(templateStream);
            using var hexStream = new MemoryStream();
            IntelHexStreamWriter hexStreamWriter = new IntelHexStreamWriter(hexStream);
            while (true)
            {
                var position = (ushort)templateStream.Position;
                var line = templateReader.ReadBytes(16);
                if (line.Length == 0)
                    break;
                hexStreamWriter.WriteDataRecord(position, line);
            }

            hexStreamWriter.Close();
            hexStream.Flush();
            using var resultReader = new StreamReader(new MemoryStream(hexStream.ToArray()));
            return resultReader.ReadToEnd();
        }
        catch (FileNotFoundException ex)
        {
            throw new NotSupportedException($"Pre-built assembly {ex.FileName} not found.");
        }
    }

    public static partial void SaveTextFile(string text);
}

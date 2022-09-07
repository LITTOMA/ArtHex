using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SixLabors.ImageSharp;

namespace ArtHex
{
    [INotifyPropertyChanged]
    public partial class MainViewModel
    {
        public MainViewModel()
        {
            LoadConfigs();
            PropertyChanged += MainViewModel_PropertyChanged;
        }

        private void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            StoreConfigs();
        }

        private void StoreConfigs()
        {
            Preferences.Set(nameof(SaveConfigs), SaveConfigs);
            if (SaveConfigs)
            {
                Preferences.Set(nameof(Dither), Dither);
                Preferences.Set(nameof(GameVersion), GameVersion);
                Preferences.Set(nameof(TargetBoard), TargetBoard);
            }
            else
            {
                Preferences.Remove(nameof(Dither));
                Preferences.Remove(nameof(GameVersion));
                Preferences.Remove(nameof(TargetBoard));
            }
        }

        private void LoadConfigs()
        {
            SaveConfigs = Preferences.Get(nameof(SaveConfigs), false);
            if (SaveConfigs)
            {
                Dither = Preferences.Get(nameof(Dither), 0.6f);
                GameVersion = Preferences.Get(nameof(GameVersion), "Splatoon 2");
                TargetBoard = Preferences.Get(nameof(TargetBoard), "Teensy 2.0++");
            }
            else
            {
                Dither = 0.6f;
                GameVersion = "Splatoon 2";
                TargetBoard = "Teensy 2.0++";
            }
        }

        [ObservableProperty]
        private ImageSource originImage;

        [ObservableProperty]
        private ImageSource monoImage;

        [ObservableProperty]
        private float dither;

        [ObservableProperty]
        private string hex;

        [ObservableProperty]
        private string gameVersion;

        [ObservableProperty]
        private string targetBoard;

        [ObservableProperty]
        private bool saveConfigs;

        [ObservableProperty]
        private bool imageOptionsIsVisible;

        private byte[] originImageData;
        private SixLabors.ImageSharp.Image currentImage;

        partial void OnDitherChanged(float value)
        {
            MonoImage = new StreamImageSource()
            {
                Stream = async (ct) =>
                {
                    if (originImageData == null)
                        return null;

                    var image = await SixLabors.ImageSharp.Image.LoadAsync(new MemoryStream(originImageData), ct);
                    Utils.ProcessImage(image, value);
                    var imageStream = new MemoryStream();
                    await image.SaveAsBmpAsync(imageStream);
                    currentImage = image;
                    imageStream.Position = 0;
                    return imageStream;
                }
            };
        }

        [RelayCommand]
        private async void ChooseImage()
        {
            var file = await MediaPicker.PickPhotoAsync();
            if (file == null) return;

            var image = SixLabors.ImageSharp.Image.Load(await file.OpenReadAsync());

            // save a copy of the image.
            using var originStream = new MemoryStream();
            image.SaveAsPng(originStream);
            originImageData = originStream.ToArray();
            OriginImage = new StreamImageSource()
            {
                Stream = async (ct) => new MemoryStream(originImageData)
            };

            OnDitherChanged(Dither);
        }

        [RelayCommand]
        private void ImageOptions()
        {
            ImageOptionsIsVisible = true;
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                ImageOptionsIsVisible = false;
            });
        }

        [RelayCommand]
        private async Task MakeHex()
        {
            if (currentImage != null)
            {
                Hex = await Utils.MakeHexAsync(currentImage, TargetBoard, GameVersion.ToLower().Replace(" ", ""));
                Utils.SaveTextFile(Hex);
            }
        }
    }
}

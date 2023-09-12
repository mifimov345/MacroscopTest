using MacroscopTest.Models;
using MacroscopTest.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using MacroscopTest.Utilities;
using System.Windows.Documents;
using System.Net.Http;

namespace MacroscopTest.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ICameraService _cameraService;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<VideoViewModel> VideoStreams { get; } = new ObservableCollection<VideoViewModel>();
        public ObservableCollection<CameraModel> Cameras { get; } = new ObservableCollection<CameraModel>();

        public MainViewModel(ICameraService cameraService)
        {
            _cameraService = cameraService;
            Initialize();
        }

        private async void Initialize()
        {
            try
            {
                var cameraList = await _cameraService.GetCamerasAsync();
                foreach (var camera in cameraList)
                {
                    Cameras.Add(camera);
                    var mjpegUrl = $"http://demo.macroscop.com:8080/mobile?login=root&channelid={camera.Id}&resolutionX=100&resolutionY=160&fps=25";
                    VideoViewModel videoViewModel = new VideoViewModel(mjpegUrl, camera.Id);
                    VideoStreams.Add(videoViewModel);
                    Console.WriteLine($"Added camera '{camera.Name}' with MJPEG URL: {mjpegUrl}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        public MainViewModel() : this(new CameraService())
        {
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MacroscopTest.Utilities;

namespace MacroscopTest.ViewModels
{
    public class VideoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _mjpegUrl;
        public string MjpegUrl
        {
            get { return _mjpegUrl; }
            set
            {
                _mjpegUrl = value;
                OnPropertyChanged(nameof(MjpegUrl));
            }
        }

        private BitmapImage _videoFrame;
        public BitmapImage VideoFrame
        {
            get { return _videoFrame; }
            set
            {
                _videoFrame = value;
                OnPropertyChanged(nameof(VideoFrame));
            }
        }

        private string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public VideoViewModel(string mjpegUrl, string id)
        {
            MjpegUrl = mjpegUrl;
            Id = id;
            StartFetchingFrames();
        }

        private async void StartFetchingFrames()
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    try
                    {
                        using (var stream = await httpClient.GetStreamAsync(MjpegUrl))
                        {
                            MjpegStreamReader parser = new MjpegStreamReader(stream);

                            await Task.Run(() =>
                            {
                                while (true)
                                {
                                    byte[] frameBytes = parser.GetNextFrame();
                                    if (frameBytes == null)
                                    {
                                        break;
                                    }
                                    if (Application.Current != null)
                                    {
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            BitmapImage bitmap = ConvertBytesToBitmap(frameBytes);
                                            if (bitmap != null)
                                            {
                                                VideoFrame = bitmap;
                                            }
                                        });
                                    }
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error fetching MJPEG stream: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching frames: {ex.Message}");
            }
        }

        private BitmapImage ConvertBytesToBitmap(byte[] frameBytes)
        {
            try
            {
                using (MemoryStream frameStream = new MemoryStream(frameBytes))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    frameStream.Position = 0;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = frameStream;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting frame bytes to bitmap: {ex.Message}");
                return null;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}

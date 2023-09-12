using MacroscopTest.Models;
using MacroscopTest.ViewModels;
using System.Globalization;
using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace MacroscopTest
{
    public partial class MainWindow : Window
    {
        double currentWidth;

        double currentHeight;

        bool isFullScreen = false;

        string currentId;

        double maxHeight;

        double maxWidth;

        public MainWindow()
        {
            InitializeComponent();
            var viewModel = DataContext as MainViewModel;
            var cameras = viewModel?.Cameras;
            ResizeMode = ResizeMode.NoResize;
            Loaded += Window_Loaded;
            MainViewModel MainviewModel_ = new MainViewModel();
            DataContext = MainviewModel_;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image current = (Image)sender;
            if (isFullScreen && (current.Width == maxWidth) && (current.Height == maxHeight))
            {
                current.Stretch = Stretch.UniformToFill;
                current.Width = currentWidth;
                current.Height = currentHeight;
                current.Margin = new Thickness(10);
                isFullScreen = false;
                Archive.Visibility = Visibility.Hidden;
                currentId = null;
            }
            else if (!isFullScreen)
            {
                currentWidth = current.ActualWidth;
                currentHeight = current.ActualHeight;
                current.Stretch = Stretch.Uniform;
                double aspectRatio = currentWidth / currentHeight;
                double newX, newY, newWidth, newHeight;
                if (aspectRatio > Width / Height)
                {
                    newWidth = Width * 0.8;
                    newHeight = newWidth / aspectRatio;
                }
                else
                {
                    newHeight = Height * 0.8;
                    newWidth = newHeight * aspectRatio;
                }
                newX = (Width - newWidth) / 2.0;
                newY = (Height - newHeight) / 2.0;
                current.Width = newWidth;
                current.Height = newHeight;
                current.Margin = new Thickness(newX, newY, 0, 0);
                isFullScreen = true;
                maxHeight = current.Height;
                maxWidth = current.Width;
                string CamId = current.Tag as string;
                currentId = CamId;
                CreateArchive(CamId);
            }
        }

        public async void CreateArchive(string id)
        {
            var testing = $"http://demo.macroscop.com:8080/archivefragments?channelid={id}&fromtime=12.09.2023%2004:08:05&totime=12.09.2023%2007:08:05";
            HttpClient _HttpClicent = new HttpClient();
            _HttpClicent.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "ZGVtb2xvZ2luOjc1NTgwNjU2YTM5NDI5MjQ2MGViYjRiMDM2ZWJlYWYx");
            HttpResponseMessage response = await _HttpClicent.GetAsync(testing);
            if (response.IsSuccessStatusCode)
            {
                string xmlContent = await response.Content.ReadAsStringAsync();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent);

                XmlNamespaceManager nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsMgr.AddNamespace("ns", "http://www.macroscop.com");

                XmlNodeList archiveFragments = xmlDoc.SelectNodes("//ns:ArchiveFragment", nsMgr);
                foreach (XmlNode archiveFragment in archiveFragments)
                {
                    string startTime = archiveFragment.SelectSingleNode("ns:FromTime", nsMgr)?.InnerText;
                    string endTime = archiveFragment.SelectSingleNode("ns:ToTime", nsMgr)?.InnerText;
                    if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
                    {
                        string result = $"{startTime} - {endTime}";
                        ListBoxItem lbi = new ListBoxItem();
                        lbi.Selected += Lbi_Selected;
                        lbi.Content = result;
                        Archive.Items.Add(lbi);
                    }
                }
                Archive.Visibility = Visibility.Visible;
            }
        }

        private void Lbi_Selected(object sender, RoutedEventArgs e)
        {
            ListBoxItem lbi = (ListBoxItem)sender;
            string Collected = (string)lbi.Content;
            string[] Parts = Collected.Split('-');
            string result = "";
            for (int i = 0; i < Parts.Length / 2; i++)
            {
                result += Parts[i] + ".";
            }
            string test = "";
            for (int i = 0; i < result.Length - 3; i++)
            {
                test += result[i];
            }
            DateTime resultdate = DateTime.ParseExact(test, "dd.mm.yyyy'T'hh:mm:ss", CultureInfo.InvariantCulture);
            string Url = $"http://demo.macroscop.com:8080/mobile?login=root&channelid={currentId}&resolutionX=640&resolutionY=480&fps=25&mode=archive&starttime={resultdate}";
        }
    }
}

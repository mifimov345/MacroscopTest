using MacroscopTest.Models;
using MacroscopTest.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace MacroscopTest
{
    public partial class MainWindow : Window
    {
        double currentWidth;

        double currentHeight;

        bool isFullScreen = false;

        double maxHeight;

        double maxWidth;

        public MainWindow()
        {
            InitializeComponent();
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
            }
        }


    }
}

using LogSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Labyrint
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();

            // Set one solid background colour
            SplashScreen1.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 110, 155, 178));

            // Get the image here
            string assemblyName = Assembly.GetEntryAssembly().GetName().Name;
            BitmapImage backgroundImage = new BitmapImage(new Uri("pack://application:,,,/" + assemblyName + ";component/Assets/Sprites/Achtergrond.png", UriKind.Absolute));

            // Set the backgroundImage in the background rectangle
            background.Fill = new ImageBrush()
            {
                ImageSource = backgroundImage
            };
        }
    }
}

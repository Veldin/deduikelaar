using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LogSystem;

namespace Labyrint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Bind the KeyUp and KeyDown methods.
            Window.GetWindow(this).KeyUp += KeyUp;
            Window.GetWindow(this).KeyDown += KeyDown;
        }

        /* KeyDown */
        /* 
        * Add the given key in the pressedKeys collection.
        * The argument is the given key represented as a string.
        */
        public void KeyDown(object sender, KeyEventArgs args)
        {
            Debug.WriteLine(args.Key.ToString());
        }

        /* KeyDown */
        /* 
         * Remove the given key in the pressedKeys collection.
         * The argument is the given key represented as a string.
         */
        public void KeyUp(object sender, KeyEventArgs args)
        {
            Debug.WriteLine(args.Key.ToString());
        }
    }
}

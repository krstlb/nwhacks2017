using System;
using System.Collections.Generic;
using System.IO;
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
using Tobii.EyeX.Framework;

namespace nwHacks2017
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var gazedDataStream = EyeXValues.s_Wpf.CreateGazePointDataStream(GazePointDataMode.LightlyFiltered);

            TextBlock textBlock = new TextBlock();
            textBlock.Text = "Hello World ajshdkajhsdkjahsdkjahsdkjahdkjhaksjdhkajsdhkashdkajsdhkasjdhaksjdhaksjdhkajshdkajshdkajshdkjashdkhaskdjhakhsd";
            textBlock.Foreground = new SolidColorBrush(Colors.Black);
            Canvas.SetLeft(textBlock, 500);
            Canvas.SetTop(textBlock, 500);
            this.Test_Canvas.Children.Add(textBlock);

            EyeXValues.s_Wpf.Start();
            gazedDataStream.Next += (s, e) =>
            {
                Point screenPoint = this.Test_Canvas.PointToScreen(new Point(e.X, e.Y));
                Console.WriteLine("Gaze point at ({0:0.0}, {1:0.0}) @{2:0}, {0:0.0}", screenPoint.X, screenPoint.Y, e.Timestamp);
                Ellipse ellipse = new Ellipse();
                ellipse.Width = 4;
                ellipse.Height = 4;
                ellipse.Fill = new SolidColorBrush(Colors.Red);
                Canvas.SetLeft(ellipse, screenPoint.X);
                Canvas.SetTop(ellipse, screenPoint.Y);
                this.Test_Canvas.Children.Add(ellipse);
            };

            Console.WriteLine("Listening for gaze data, press any key to exit...");
            Console.In.Read();
        }

        private async void openFileBtn_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            //dlg.DefaultExt = ".png";
            //dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result.HasValue && result.Value)
            {
                // Open document 
                string filename = dlg.FileName;

                try
                {
                    using (StreamReader sr = new StreamReader(filename))
                    {
                        String line = await sr.ReadToEndAsync();
                        textField.Text = line;
                    }
                }
                catch (Exception ex)
                {
                    Console.Write("Could not read the file");
                }
            }
        }

        private void finishBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

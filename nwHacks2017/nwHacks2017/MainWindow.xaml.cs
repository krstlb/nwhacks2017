using System;
using System.Collections.Generic;
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
                Point screenPoint = this.Test_Canvas.PointToScreen(new Point(e.X, e.Y - 23));
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
    }
}

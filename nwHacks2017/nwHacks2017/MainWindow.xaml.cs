using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    public partial class MainWindow : Window
    {
        Stopwatch durationTimer = new Stopwatch();
        Queue<Ellipse> previousPoints = new Queue<Ellipse>();
        int secondsSinceEpoch = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        int previousTime = 0;
        const int TIME_OUT = 500;
        public MainWindow()
        {
            InitializeComponent();
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
            EyeXValues.s_Wpf.Dispose();
            durationTimer.Stop();
            elapsedTime.Text = "Time: " + durationTimer.Elapsed;
            durationTimer.Reset();
            Console.WriteLine("The Elapsed event was raised at {0}", durationTimer.Elapsed);
        }

        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            clearPointsFromCanvas();
        }

        private void clearPointsFromCanvas()
        {
            TextBlock t = textField;
            this.Test_Canvas.Children.Clear();

            Test_Canvas.Children.Add(t);
        }

        private Ellipse createFixationEllipse(Point p)
        {

            Point screenPoint = this.Test_Canvas.PointFromScreen(p);
            Ellipse ellipse = new Ellipse();
            ellipse.Width = 7;
            ellipse.Height = 7;
            ellipse.Fill = new SolidColorBrush(Colors.Red);
            Canvas.SetLeft(ellipse, screenPoint.X);
            Canvas.SetTop(ellipse, screenPoint.Y);

            return ellipse;
        }
        
        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            elapsedTime.Text = "Time: Running...";
            var gazedDataStream = EyeXValues.s_Wpf.CreateGazePointDataStream(GazePointDataMode.LightlyFiltered);
            durationTimer.Start();

            EyeXValues.s_Wpf.Start();
            gazedDataStream.Next += (s, eyeLocation) =>
            {
                TimeSpan start = DateTime.Now.TimeOfDay;
                int currentTime = (int)start.TotalMilliseconds;
                if (currentTime - previousTime < TIME_OUT)
                {

                    Point eyePoint = new Point(eyeLocation.X, eyeLocation.Y);
                    // Console.WriteLine("Gaze point at ({0:0.0}, {1:0.0}) @{2:0}, {0:0.0}", screenPoint.X, screenPoint.Y, eyeLocation.Timestamp);
                    Ellipse ellipse = createFixationEllipse(eyePoint);
                    this.Test_Canvas.Children.Add(ellipse);
                    previousPoints.Enqueue(ellipse);

                    if (previousPoints.Count == 3)
                    {
                        Test_Canvas.Children.Remove(previousPoints.Dequeue());
                    }
                }

                previousTime = currentTime;
            };
        }
    }
}

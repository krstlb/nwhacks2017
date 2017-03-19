using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
        Queue<Ellipse> previousEllipses = new Queue<Ellipse>();
        Queue<Point> previousPoints = new Queue<Point>();
        Queue<Line> previousLines = new Queue<Line>();
        int secondsSinceEpoch = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        int previousTime = 0;
        const int TIME_OUT = 750;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void getLineHeightCoordinates()
        {
            // get position of text block
            // count number of new lines in text block
            // return array of pairs of y coordinates, each pair
        }

        private Size GetScreenSize(string text, FontFamily fontFamily, double fontSize, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch)
        {
            fontFamily = fontFamily ?? new TextBlock().FontFamily;
            fontSize = fontSize > 0 ? fontSize : new TextBlock().FontSize;
            var typeface = new Typeface(fontFamily, fontStyle, fontWeight, fontStretch);
            var ft = new FormattedText(text ?? string.Empty, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, fontSize, Brushes.Black);
            return new Size(ft.Width, ft.Height);
        }

        private void testBtn_Click(object sender, RoutedEventArgs e)
        {
            Rect textBoundingBox = new Rect(textField.RenderSize);

            double fontHeight = textField.FontSize * textField.FontFamily.LineSpacing;
            //double charWidth = GetScreenSize("X", textField.FontFamily, textField.FontSize, textField.FontStyle, textField.FontStretch).Width;

            Point p = textBoundingBox.TopLeft;
            p.X += textField.Padding.Left;
            p.Y += textField.Padding.Top;

            string[] lines = textField.Text.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                //lines[i].Length * charWidth
            }

            

            Ellipse ellipse = createFixationEllipse(p);
            this.Test_Canvas.Children.Add(ellipse);

            for (int i = 0; i < lines.Length; i++)
            {
                p.Y += fontHeight;
                ellipse = createFixationEllipse(p);
                this.Test_Canvas.Children.Add(ellipse);
            }
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
            //resetBtn_Click(sender, e);
            startBtn.IsEnabled = true;
            EyeXValues.s_Wpf.Dispose();
            previousTime = 0;
            previousEllipses.Clear();
            previousLines.Clear();
            previousPoints.Clear();
            clearPointsFromCanvas();
            durationTimer.Stop();
            elapsedTime.Text = "Time: " + durationTimer.Elapsed;
            durationTimer.Reset();
            Console.WriteLine("The Elapsed event was raised at {0}", durationTimer.Elapsed);
        }

        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            startBtn.IsEnabled = true;
            EyeXValues.s_Wpf.Dispose();
            previousTime = 0;
            previousEllipses.Clear();
            previousLines.Clear();
            previousPoints.Clear();
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
            Ellipse ellipse = new Ellipse();
            ellipse.Width = 7;
            ellipse.Height = 7;
            ellipse.Fill = new SolidColorBrush(Colors.Red);
            Canvas.SetLeft(ellipse, p.X);
            Canvas.SetTop(ellipse, p.Y);

            return ellipse;
        }
        
        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            startBtn.IsEnabled = false;
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
                    Point screenPoint = this.Test_Canvas.PointFromScreen(eyePoint);
                    previousPoints.Enqueue(screenPoint);
                    // Console.WriteLine("Gaze point at ({0:0.0}, {1:0.0}) @{2:0}, {0:0.0}", screenPoint.X, screenPoint.Y, eyeLocation.Timestamp);
                    Ellipse ellipse = createFixationEllipse(screenPoint);
                    this.Test_Canvas.Children.Add(ellipse);
                    previousEllipses.Enqueue(ellipse);

                    if (previousEllipses.Count == 3)
                    {
                        Test_Canvas.Children.Remove(previousEllipses.Dequeue());
                        previousPoints.Dequeue();
                        if (previousLines.Count != 0)
                        {
                            Test_Canvas.Children.Remove(previousLines.Dequeue());
                        }
                    }

                    Point previous = previousPoints.Peek();
                    foreach (Point p in previousPoints)
                    {
                      
                        if(p != previous)
                        {
                            Line line = new Line();
                            line.Stroke = Brushes.DarkRed;
                            line.X1 = previous.X;
                            line.Y1 = previous.Y;
                            line.X2 = p.X;
                            line.Y2 = p.Y;
                            line.StrokeThickness = 3;

                            previousLines.Enqueue(line);
                            Test_Canvas.Children.Add(line);
                        }

                        previous = p;
                    }
                }

                previousTime = currentTime;
            };
        }


    }
}

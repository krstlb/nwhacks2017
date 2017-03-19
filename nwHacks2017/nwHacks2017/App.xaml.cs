using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace nwHacks2017
{

    using System.Windows;
    using EyeXFramework.Wpf;
    using Tobii.EyeX.Framework;
    using System.Diagnostics;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private WpfEyeXHost _eyeXHost;

        public App()
        {
            _eyeXHost = new WpfEyeXHost();

            var gazedDataStream = _eyeXHost.CreateGazePointDataStream(GazePointDataMode.LightlyFiltered);
            _eyeXHost.Start();

            gazedDataStream.Next += (s, e) => Console.WriteLine("Gaze point at ({0:0.0}, {1:0.0}) @{2:0}", e.X, e.Y, e.Timestamp);

            Console.WriteLine("Listening for gaze data, press any key to exit...");
            Console.In.Read();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _eyeXHost.Dispose(); // always dispose on exit
        }
    }
}

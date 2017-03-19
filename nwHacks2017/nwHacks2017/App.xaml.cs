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
    using System.Windows.Media;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
           
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            EyeXValues.s_Wpf.Dispose(); // always dispose on exit
        }
    }

    public static class EyeXValues
    {
        public static WpfEyeXHost s_Wpf = new WpfEyeXHost();
    }

}

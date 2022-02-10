using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Threading;

namespace RCM
{
    class Program
    {
        private static Splash mSplash;
        internal static Application app;

        [System.STAThreadAttribute()]
        static void Main(string[] args)
        {
            RunProgram();
        }
        static void RunProgram()
        {
            app = new Application();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            mSplash = new Splash();
            mSplash.Closed += new EventHandler(mSplash_Closed);

            System.Threading.Thread t = new System.Threading.Thread(Init);
            t.Start();

            app.Run(mSplash);
        }

        static void mSplash_Closed(object sender, EventArgs e)
        {
            app.MainWindow = new MainWindow();
            app.MainWindow.Closed += new EventHandler(MainWindow_Closed);
            app.MainWindow.Show();
        }

        static void MainWindow_Closed(object sender, EventArgs e)
        {
            Settings.Settings.SaveSettings();
            app.Shutdown();
        }

        static void Init()
        {
            Metrics.MetricManager.Instance.LoadingMetrics += new EventHandler(LoadingMetrics);
            ExcelExtractor.Instance.ReadingExcelData += new EventHandler(Instance_ReadingExcelData);
            ExcelExtractor.Instance.ExcelDataReceived += new EventHandler(Instance_ExcelDataReceived);
            //Cursor prev = Mouse.OverrideCursor;
            //Mouse.OverrideCursor = Cursors.Wait;
            mSplash.UpdateStatus("Reading Settings...", 20);
            Settings.Settings.Init();
            mSplash.UpdateStatus("Reading Settings done", 20);
            mSplash.Dispatcher.Invoke(new Action(mSplash.Close));
           // Mouse.OverrideCursor = prev;
        }

        static void Instance_ExcelDataReceived(object sender, EventArgs e)
        {
            mSplash.UpdateStatus("Populating data grid...", 20);
        }

        static void Instance_ReadingExcelData(object sender, EventArgs e)
        {
            mSplash.UpdateStatus("Reading requirements...", 20);
        }
        static void LoadingMetrics(object sender, EventArgs e)
        {
            mSplash.UpdateStatus("Loading Metrics...", 20);
        }
    }
}

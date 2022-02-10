using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.IO;
using System.Web.UI;
using System.Data;
using System.Text.RegularExpressions;
using RCM.Metrics;
namespace RCM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties

        private static DataSet allData;
        public static List<Metric> UsedMetrics = new List<Metric>();
        public static DataSet AllData
        {
            get { return allData; }
            set
            {
                allData = value;
                //Invoke(ExcelDataReceived);
            }
        }

        private ExcelExtractor reader;
        public static event EventHandler ExcelDataReceived;

        private SettingsWindow settings;
        private Chart chart;

        #endregion

        public MainWindow()
        {
            InitializeComponent();                      
           ExcelDataReceived += new EventHandler(MainWindow_ExcelDataReceived);
           if (Requirement.AllRequirements != null) Invoke(ExcelDataReceived);
           
        }

        void MainWindow_ExcelDataReceived(object sender, EventArgs e)
        {
            Cursor prev = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;
           // if(Requirement.AllRequirements.Count>0)
            Requirements.ItemsSource = Requirement.AllRequirements;
            //Reqs.ItemsSource = Requirement.AllRequirements;
            Mouse.OverrideCursor = prev;
        }

        //public void FindListViewItem(DependencyObject obj)
        //{

        //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        //    {
        //        ItemsControl dc = obj as ItemsControl;

        //        if (dc != null)
        //        {
        //            HighlightText(dc);
        //        }
        //        FindListViewItem(VisualTreeHelper.GetChild(obj as DependencyObject, i));
        //    }
        //}

        //private void HighlightText(Object itx)
        //{
        //    if (itx != null)
        //    {
        //        if (itx is TextBox)
        //        {

        //            TextBlock tb = itx as TextBlock;
        //            string[] keywords;
        //            ConjunctionsMetric m = (ConjunctionsMetric)MainWindow.UsedMetrics.Find(f => f is ConjunctionsMetric);
        //            if (m != null)
        //                keywords = m.Keywords;
        //            else keywords = ConjunctionsMetric.DefaultKeywords;
        //            string pattern = @"[^\W\d](\w|[-']{1,2}(?=\w))*";
        //            MatchCollection matches = Regex.Matches(tb.Text, pattern);

        //            tb.Inlines.Clear();
        //            foreach (var item in matches)
        //            {
        //                if (keywords.Contains(item as string))
        //                {
        //                    Run runx = new Run(item as string);
        //                    runx.Background = Brushes.Blue;
        //                    tb.Inlines.Add(runx);
        //                }
        //                else
        //                {
        //                    tb.Inlines.Add(item as string);
        //                }
        //            }
        //            return;
        //        }
        //        else
        //        {
        //            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(itx as DependencyObject); i++)
        //            {
        //                HighlightText(VisualTreeHelper.GetChild(itx as DependencyObject, i));
        //            }
        //        }
        //    }
        //}

        private void SettingsMenu_Click(object sender, RoutedEventArgs e)
        {
            settings = new SettingsWindow();
            settings.Show();
        }
        protected void Invoke(Delegate d)
        {
            if (d != null)
                d.DynamicInvoke(this, new EventArgs());
        }

        private void ScatterChart_Click(object sender, RoutedEventArgs e)
        {
            if (Requirement.AllRequirements != null)
            {
                Thread thread = new Thread(() =>
                    {
                        Chart w = new Chart();
                        w.Show();
                        w.Closed += (sender2, e2) =>
                            w.Dispatcher.InvokeShutdown();

                        System.Windows.Threading.Dispatcher.Run();
                    });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
              
            }
            else
            {
                MessageBox.Show("No requirements found!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OpenMenu_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            dialog.DefaultExt = "xls";
            bool? result = dialog.ShowDialog();

            Cursor prev = Mouse.OverrideCursor;
            if (result == true && dialog.FileName != null && dialog.FileName.Trim() != "")
            {
                Mouse.OverrideCursor = Cursors.Wait;
                ExcelExtractor.Instance.Read(dialog.FileName);
                Invoke(ExcelDataReceived);
            }
            Settings.Settings.SaveSettings();
            Mouse.OverrideCursor = prev;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveMenu_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog op = new SaveFileDialog();
            op.Filter = "Excel Files|*.csv;*.xls;*.xlsx;*.xlsm";
            op.DefaultExt = "csv";
            bool? result = op.ShowDialog();
            Cursor prev = Mouse.OverrideCursor;
            if (result == true && op.FileName != null && op.FileName.Trim() != "")
            {
                Mouse.OverrideCursor = Cursors.Wait;
                Requirements.SelectAllCells();
                Requirements.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                ApplicationCommands.Copy.Execute(null, Requirements);
                System.IO.StreamWriter file1 = new System.IO.StreamWriter(op.FileName);
                file1.Write(Clipboard.GetData(DataFormats.Text));
                file1.Close();
                Requirements.UnselectAll();
               
                //ExcelExtractor.Instance.Save(op.FileName);
            }
            Settings.Settings.SaveSettings();
            Mouse.OverrideCursor = prev;
        }

        private void Run_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            IEnumerable<TextRange> wordRanges = GetAllWordRanges(((Run)sender));
            List<SolidColorBrush> brushes = new List<SolidColorBrush> { Brushes.Red, Brushes.Aqua, Brushes.Beige, Brushes.Bisque, Brushes.Blue, Brushes.Brown, Brushes.BlueViolet, Brushes.Crimson };

            foreach (TextRange tr in wordRanges)
            {
                foreach (Metric<IMetric> m in MetricManager.Instance.AllMetrics)
                {
                    if (m.Instance.DefaultKeywords.Contains(tr.Text.ToLower()))
                    {
                        tr.ApplyPropertyValue(TextElement.ForegroundProperty, brushes[MetricManager.Instance.AllMetrics.IndexOf(m)]);
                        tr.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                    }
                }
            }
        }

        public DataGrid GetRequirements()
        {
            return Requirements;
        }
        public static IEnumerable<TextRange> GetAllWordRanges(Run document)
        {
            List<string> keywords=new List<string>();
            foreach (Metric m in MetricManager.Instance.AllMetrics)
                keywords.AddRange(m.Instance.Keywords);
            TextPointer pointer = document.ContentStart;
            while (pointer != null)
            {
                if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    foreach (string pattern in keywords)
                    {
                        string textRun = pointer.GetTextInRun(LogicalDirection.Forward);
                        MatchCollection matches = Regex.Matches(textRun, @"\b"+pattern+@"\b", RegexOptions.IgnoreCase);
                        foreach (Match match in matches)
                        {
                            int startIndex = match.Index;
                            int length = match.Length;
                            TextPointer start = pointer.GetPositionAtOffset(startIndex);
                            TextPointer end = start.GetPositionAtOffset(length);
                            yield return new TextRange(start, end);
                        }
                    }
                }

                pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
            }
        }

        private void Run_TextInput(object sender, TextCompositionEventArgs e)
        {
            IEnumerable<TextRange> wordRanges = GetAllWordRanges(((Run)sender));
            foreach (TextRange wordRange in wordRanges)
            {
                foreach (string s in ConjunctionsMetric.DefaultKeywords)
                {
                    if (Regex.IsMatch(wordRange.Text, s, RegexOptions.IgnoreCase))
                        wordRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
                }
            }
        }

        private void Run_Loaded(object sender, RoutedEventArgs e)
        {         
            IEnumerable<TextRange> wordRanges = GetAllWordRanges(((Run)sender));
            List<SolidColorBrush> brushes = new List<SolidColorBrush> {  Brushes.Red, Brushes.Aqua, Brushes.Beige, Brushes.Bisque, Brushes.Blue, Brushes.Brown, Brushes.BlueViolet, Brushes.Crimson, Brushes.DarkGreen, Brushes.DarkSalmon};
           
            foreach (TextRange tr in  wordRanges)
                {
                    foreach (Metric<IMetric> m in MetricManager.Instance.AllMetrics)
                    {
                        if (m.Instance.DefaultKeywords.Contains(tr.Text.ToLower()))
                        {
                            tr.ApplyPropertyValue(TextElement.ForegroundProperty, m.Instance.brush );
                            tr.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);                               
                        }
                    }
                }
            }      
        
    }
}

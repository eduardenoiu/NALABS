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
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using RCM.Metrics;

namespace RCM
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
       public SettingsWindow()
        {
            InitializeComponent();          
            Closing += new System.ComponentModel.CancelEventHandler(SettingsWindow_Closing);
            foreach (Metric m in MetricManager.Instance.AllMetrics)
            {
                MetricView view = new MetricView();
                view.Metric = m;                
                Metrics.Children.Add(view);
            }
            ID.Text = ExcelExtractor.Instance.IdCol.ToString();
            Text.Text = ExcelExtractor.Instance.TextCol.ToString();
        }

        void SettingsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ExcelExtractor.Instance.IdCol = Convert.ToInt32(ID.Text);
            ExcelExtractor.Instance.TextCol = Convert.ToInt32(Text.Text);
            Settings.Settings.SaveSettings();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //if (MainWindow.UsedMetrics.Find(s => s is ConjunctionsMetric) == null)
            //{
            //    //MainWindow.UsedMetrics.Add(new ConjunctionsMetric(conjuctionKeywords.Text.Split(';')));
            //}
            //else
            //{
            //    //MainWindow.UsedMetrics.Find(s => s is ConjunctionsMetric).Keywords = conjuctionKeywords.Text.Split(';');
            //}
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //MainWindow.UsedMetrics.Remove((MainWindow.UsedMetrics.Find(s => s is ConjunctionsMetric)));
        }

        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            foreach (MetricView m in Metrics.Children)
            {
                m.Metric.Instance.Keywords = m.Metric.Instance.DefaultKeywords;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (MetricView m in Metrics.Children)
            {
                // MetricManager.Instance.AllMetrics.Find(p => p.Info.Name == m.Metric.Info.Name).Instance.Keywords = 
                   IEnumerable<string> res= m.Keywords.Text.Split(';').Select(p => p.Trim(new char[] { ' ', '"' })).Where(p=>p!="");
                   foreach (string s in res)
                   {
                       s.Trim(new char[] { ' ', '"' });
                   }
                   MetricManager.Instance.AllMetrics.Find(p => p.Info.Name == m.Metric.Info.Name).Instance.Keywords = res.ToArray();
            }
               Requirement.Update();
               this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Parallel.ForEach(MetricManager.Instance.AllMetrics, m =>
            {
                m.Instance.Keywords = m.Instance.DefaultKeywords;
               
            });
            Settings.Settings.SaveSettings();
            Update();
        }

        private void Update()
        {
            
            foreach (MetricView m in Metrics.Children)
            {
                m.Keywords.Clear();
                m.Metric = m.Metric;
            }
        }
           

        private void Label_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}

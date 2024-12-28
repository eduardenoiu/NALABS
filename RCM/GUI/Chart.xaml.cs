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
using System.Windows.Shapes;
using System.Windows.Controls.DataVisualization.Charting;
using System.Threading;

namespace RCM
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    public partial class Chart : Window
    {
        public Chart()
        {
            InitializeComponent();
            if (Requirement.AllRequirements != null)
            {
                Cursor prev = Mouse.OverrideCursor;
                Mouse.OverrideCursor = Cursors.Wait;
                ((ScatterSeries)lineChart.Series[0]).ItemsSource = Requirement.AllRequirements.Select(s => new KeyValuePair<int, int>(Requirement.AllRequirements.IndexOf(s), s.WordNumber));
                
                //((ScatterSeries)lineChart.Series[1]).ItemsSource = Requirement.AllRequirements.Select(s => new KeyValuePair<int, int>(Requirement.AllRequirements.IndexOf(s), s.Conjunctions));
                //((BarSeries)lineChart.Series[2]).ItemsSource = Requirement.AllRequirements.Select(s => new KeyValuePair<int, int>(Requirement.AllRequirements.IndexOf(s), s.VaguePhrases));
                //((BarSeries)lineChart.Series[3]).ItemsSource = Requirement.AllRequirements.Select(s => new KeyValuePair<int, int>(Requirement.AllRequirements.IndexOf(s), s.Optionality));
                //((BarSeries)lineChart.Series[4]).ItemsSource = Requirement.AllRequirements.Select(s => new KeyValuePair<int, int>(Requirement.AllRequirements.IndexOf(s), s.Subjectivity));
                //((BarSeries)lineChart.Series[5]).ItemsSource = Requirement.AllRequirements.Select(s => new KeyValuePair<int, int>(Requirement.AllRequirements.IndexOf(s), s.References)); 
                //((BarSeries)lineChart.Series[6]).ItemsSource = Requirement.AllRequirements.Select(s => new KeyValuePair<int, int>(Requirement.AllRequirements.IndexOf(s), s.Weakness));
                //((BarSeries)lineChart.Series[7]).ItemsSource = Requirement.AllRequirements.Select(s => new KeyValuePair<int, int>(Requirement.AllRequirements.IndexOf(s), s.Imperatives));


                Mouse.OverrideCursor = prev;
            }
        }
    }
}

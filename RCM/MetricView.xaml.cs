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
using RCM.Metrics;
namespace RCM
{
    /// <summary>
    /// Interaction logic for MetricView.xaml
    /// </summary>
    public partial class MetricView : UserControl
    {
        private Metrics.Metric metric;

        public Metrics.Metric Metric
        {
            get { return metric; }
            set { metric = value;
            foreach (string s in (value.Instance as IMetric).Keywords)
                Keywords.Text += "\""+s+"\"; ";
            mName.Content = value.Info.Name;
            }           
        }
        public MetricView()
        {
            InitializeComponent();
          
        }
    }
}

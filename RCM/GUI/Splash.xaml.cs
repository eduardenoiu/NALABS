using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RCM
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : Window
    {
        public Splash()
        {
            InitializeComponent();
        }
        public void UpdateStatus(string Status, double ValueIncrease)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action<string, double>(sUpdate),
                Status, ValueIncrease);
        }
        private void sUpdate(string status, double ValueIncrease)
        {
            Status.Text = status;
            Progress.Value += ValueIncrease;
        }
    }
}

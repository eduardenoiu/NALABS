using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using RCM.Settings;
using System.Windows.Media;
namespace RCM.Metrics
{
    public interface IMetric
    {
        string[] Keywords { get; set; }
        string[] DefaultKeywords { get; }
        //ISettingsHandler settings { get; }
        //public bool Checked;
        double Apply(Requirement req);
        Brush brush {get; } 
    }    
}

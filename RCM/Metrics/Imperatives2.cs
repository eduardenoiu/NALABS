using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace RCM.Metrics
{
    [Export(typeof(IMetric))]
    [MetricInfoAttribute(Name = "Imperatives2", Type = typeof(Imperatives2))]
    //GenLinguisticModelForRQA
    class Imperatives2:IMetric
    {
        public static string[] DefaultKeywords = { "must", "should", "could", "would", "can", "will", "may", "should" };
        public override string ToString()
        {
            return "Number of imperatives2";
        }

        public double Apply(Requirement req)
        {
            return req.Words.Count(f => Keywords.Contains(f.ToLower()));
        }

        string[] Keywords
        {
            get;
            set;
        }


        string[] IMetric.Keywords
        {
            get
            {
                if (Keywords == null) return DefaultKeywords;
                return Keywords;
            }
            set
            {
                this.Keywords = new string[value.Count()];
                int i = 0;
                foreach (string s in value)
                {
                    if (s != "")
                    {
                        this.Keywords[i] = s.Trim(new char[] { '"', ' ' });
                        i++;
                    }
                }
            }

        }

        string[] IMetric.DefaultKeywords
        {
            get { return DefaultKeywords; }
        }

        double IMetric.Apply(Requirement req)
        {
            return req.Words.Count(p => Keywords.Contains(p.ToLower()));
        }
        Brush IMetric.brush
        {
            get { return Brushes.DarkSeaGreen; }
        }
    }
}

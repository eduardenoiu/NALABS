using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace RCM.Metrics
{
    [Export (typeof(IMetric))]
    [MetricInfoAttribute(Name="Number of Vague Phrases", Type=typeof(NVMetric))]
  public class NVMetric : IMetric
    {
        public static string[] DefaultKeywords = { "may", "could", "has to", "have to", "might", "will", "should have", "must have", "all the other", "all other", "based on", "some", "appropriate", "as a", "as an", "a minimum", "up to", "adequate", "as applicable", "be able to", "be capable", "but not limited to", "capability of", "capability to", "effective", "normal" };
        public string[] Keywords
        {
            get;
            set;
        }

        string[] IMetric.Keywords
        {
            get
            {
                if (this.Keywords == null) return DefaultKeywords;
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
        public NVMetric(string[] Keys)
        {
            this.Keywords = Keys;
        }

        public NVMetric()
        {
        }

        public override string ToString()
        {
            //string res = "";
            //if (Keywords != null)
            //{
            //    for (int i = 0; i < Keywords.Length; i++)
            //        res += Keywords[i] + ";";
            //    return res;
            //}
            //else
            //{
            //    for (int i = 0; i < DefaultKeywords.Length; i++)
            //        res += DefaultKeywords[i] + ";";
            //    return res;
            //}
            return "Number of Vague Phrases";
        }

        public double Apply(Requirement req)
        {
            return req.Words.Count(f => Keywords.Contains(f.ToLower()));
        }


        string[] IMetric.DefaultKeywords
        {
            get {return DefaultKeywords; }
        }

        Brush IMetric.brush
        {
            get { return Brushes.Blue; }
        }
    }
}

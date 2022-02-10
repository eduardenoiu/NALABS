using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace RCM.Metrics
{
    [Export(typeof(IMetric))]    
    [MetricInfoAttribute(Name = "Subjectivity metric", Type = typeof(SubjectivityMetric))]
    class SubjectivityMetric: IMetric
    {
         public static string[] DefaultKeywords = {"similar", "better", "similarly", "worse", "having in mind", "take into account", "take into consideration", "as [adjective] as possible" };
       
        //public string[] Keywords
        //{
        //    get;
        //    set;
        //}
        public SubjectivityMetric(string[] Keys)
        {
           this.Keywords = Keys;
        }

        public SubjectivityMetric()
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
            return "Number of conjuctions";
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
            get { if (Keywords == null) return DefaultKeywords;
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
            throw new NotImplementedException();
        }

        Brush IMetric.brush
        {
            get { return Brushes.DarkSalmon; }
        }
    }
}

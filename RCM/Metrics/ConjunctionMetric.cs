using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RCM.Metrics
{
    [Export(typeof(IMetric))]
    [MetricInfoAttribute(Name = "Number of Conjunctions", Type = typeof(ConjunctionsMetric))]
    public class ConjunctionsMetric : IMetric
    {
        public static string[] DefaultKeywords = { "and", "after", "although", "as long as", "before", "but", "else", "if", "in order", "in case", "nor", "or", "otherwise", "once", "since", "then", "though", "till", "unless", "until", "when", "whenever", "where", "whereas", "wherever", "while", "yet" };
        //public string[] Keywords
        //{
        //    get;
        //    set;
        //}
        public ConjunctionsMetric(string[] Keys)
        {
            this.Keywords = Keys;
        }

        public ConjunctionsMetric()
        {
        }

        public override string ToString()
        {
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
                        this.Keywords[i] = s.Trim(new char[] {'"', ' '});
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
          return  req.Words.Count(p => Keywords.Contains(p.ToLower()));
        }

        Brush IMetric.brush
        {
            get { return Brushes.BlueViolet; }
        }

    }
}

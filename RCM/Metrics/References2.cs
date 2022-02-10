using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace RCM.Metrics
{
    [Export(typeof(IMetric))]
    [MetricInfoAttribute(Name = "Number of References2", Type = typeof(References2))]
    // Rendex
    class References2:IMetric
    {
        
        public static string[] DefaultKeywords = {  "defined in reference", "defined in the reference", "specified in reference", "specified in the reference", "specified by reference", "specified by the reference", "see reference", "see the reference", "refer to reference", "refer to the reference", "further reference", "follow reference", "follow the reference", "see" };
      public override string ToString()
      {
          return "Number of References2";
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
          get { return Brushes.Beige; }
      }
    }
}

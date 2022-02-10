using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCM.Metrics
{
   public interface ISplitMetric:IMetric
    {
        char[] splitBy { get; }
    }
}

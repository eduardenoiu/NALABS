using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCM.Metrics
{
    public interface IMetricInfo
    {
        string Name { get; }
        Type[] Dependencies { get; }
        Type Type
        {
            get;
        }

        string Project { get; }
    }
}

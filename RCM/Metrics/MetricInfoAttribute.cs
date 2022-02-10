using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
namespace RCM.Metrics
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    [MetadataAttribute]
    class MetricInfoAttribute:ExportAttribute, IMetricInfo
    {
        
        public Type[] Dependencies {get; set;}
        public string Name{ get; set;}
        public string Project {get; set;}
        public Type Type {get; set;}

        public override string ToString()
        {
            return Name;
        }
    }
}

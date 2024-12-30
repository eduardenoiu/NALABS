using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel.Composition.Hosting;
using System.Threading.Tasks;

namespace RCM.Metrics
{
    public sealed class MetricManager
    {
        private MetricManager(){
           
        }
        private static MetricManager mInstance=new MetricManager();
        private CompositionContainer container;
        public static MetricManager Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new MetricManager();
                return mInstance;
            }
        }
        public string MetricsFolder
        {
            get
            {
                return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Plugins\\"; 
            }   
        }

        public List<Metric<IMetric>> AllMetrics;

        public static void SetMetricKeywords()
        {
            Parallel.ForEach(Instance.AllMetrics, m =>
            {
                m.Instance.Keywords = m.Instance.DefaultKeywords;
            });
        }

        internal void UpdateMetrics()
        {
            invoke(LoadingMetrics);
            if (container != null)
            {
                container.Dispose();
            }
            AggregateCatalog aggCatalog=new AggregateCatalog();
            aggCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Program).Assembly));
            if (!Directory.Exists(Path.GetDirectoryName(MetricsFolder)))
                Directory.CreateDirectory(Path.GetDirectoryName(MetricsFolder));
            string[] tmpDirectories = null;
            try
            {
                tmpDirectories = Directory.GetDirectories(MetricsFolder, "*", SearchOption.AllDirectories);

                List<string> directories = new List<string>(tmpDirectories);
                directories.Add(MetricsFolder);

                foreach (string s in directories)
                {
                    aggCatalog.Catalogs.Add(new DirectoryCatalog(s));
                }

                container = new CompositionContainer(aggCatalog);
            }
            catch
            {

            }
            List<Metric<IMetric>> metrics = new List<Metric<IMetric>>();
            try
            {
                metrics=container.GetExports<IMetric, IMetricInfo>().ToList().ConvertAll<Metric<IMetric>>(n=>new Metric<IMetric>(n));
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message, "MetricManager", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);    
            }

            AllMetrics = RemoveDuplets(metrics);
        }

       private List<Metric<T>> RemoveDuplets<T>(List<Metric<T>> Metrics)
        {
            List<Metric<T>> uniqueMetrics = new List<Metric<T>>();
            foreach (Metric<T> metric in Metrics)
            {
                
                uniqueMetrics.Add(metric);
            }
            return uniqueMetrics;
        }

        public EventHandler LoadingMetrics;
        private void invoke(Delegate d)
        {
            if (d != null)
                d.DynamicInvoke(this, new EventArgs());
        }
    }

}

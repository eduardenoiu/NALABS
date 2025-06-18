using RCM.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RCM.Metrics
{
    public interface Metric
    {
        Type MetricType { get; }
        Type InstanceType { get; }
        IMetricInfo Info { get; }
        IMetric Instance { get; }
        bool IsInitialized { get; }

    }
   public class Metric<T>:Metric
    {
        public Metric(Lazy<T, IMetricInfo> Metric)
        {
            mMetric = Metric;

        }

        private Lazy<T, IMetricInfo> mMetric;
        internal Lazy<T, IMetricInfo> LazyMetric { get { return mMetric; } }
        private T mInstance;
       
        bool HasFailedInitializing;
    
        public T Instance
        {
            get{
                if (mInstance == null && !HasFailedInitializing)
                {
                    try
                    {
                        mInstance = mMetric.Value;
                        IsInitialized = true;
                    }
                    catch (Exception ex)
                    {
                        HasFailedInitializing = true;
                        mInstance = default(T);
                        var errorMessage = "Failed to load metric " + Info.Name + "\n" + ex.Message;
                        Logger.LogError(ex, errorMessage);
                        MessageHelper.ShowWarning(errorMessage, "MetricManager");
                    }
                }
                return mInstance;
            }
        }

        IMetric Metric.Instance
        {
            get
            {
                if (this.Instance is IMetric)
                    return this.Instance as IMetric;
                else return null;
            }
        }
        public IMetricInfo Info
        {
            get { return mMetric.Metadata; }
        }

       public Type MetricType
        {
            get { return typeof(T); }
        }

        public Type InstanceType
        {
            get {return Info.Type;}
        }

        public override string ToString()
        {
            return Info.Name;
        }

        public bool IsInitialized
        {
            get;
            set;
        }
    }
}

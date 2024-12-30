using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using RCM.Metrics;
using System.IO;
namespace RCM.Settings
{
    class Settings
    {
        private static List<ISettingsHandler> mSettingsHandlers = new List<ISettingsHandler>();

        public static List<ISettingsHandler> SettingsHandlers
        {
            get { return Settings.mSettingsHandlers; }
            set { Settings.mSettingsHandlers = value; }
        }

        public static void Init()
        {
            MetricManager.Instance.UpdateMetrics();
            if (File.Exists(SettingsDatabase.Instance.FilePath))
            {
                try
                {
                    foreach (Metric m in MetricManager.Instance.AllMetrics)
                    {
                        DataTable t = SettingsDatabase.Instance.GetTable(m.Info.Name);
                        if (t != null)
                        {
                            m.Instance.Keywords = t.Rows[0]["Keywords"].ToString().Trim(new char[] {'"',' ',';'}).Split(';');
                        }
                    }
                }
                catch { }
                try
                {
                    DataTable ex = SettingsDatabase.Instance.GetTable("ExcelExtractor");
                    if (ex != null)
                    {
                        ExcelExtractor.Instance.IdCol = Convert.ToInt32(ex.Rows[0]["ID"].ToString());
                        ExcelExtractor.Instance.TextCol = Convert.ToInt32(ex.Rows[0]["Text"].ToString());
                        ExcelExtractor.Instance.FilePath = ex.Rows[0]["Path"].ToString();
                    }
                    ExcelExtractor.Instance.Read(ExcelExtractor.Instance.FilePath);
                }
                catch { }                
            }         
        }

        public static void SaveSettings()
        {
            Data.AllData = new DataSet("Settings");
            foreach (Metric m in Metrics.MetricManager.Instance.AllMetrics)
            {
                DataTable t = new DataTable(m.Info.Name);
                t.TableName = m.Info.Name;
                t.Columns.Add("Keywords");
                DataRow row = t.NewRow();
                string words = "";
                foreach (string s in m.Instance.Keywords)
                {
                    if(s!="")
                    words += s + ";";
                }
                row["Keywords"] = words;
                t.Rows.Add(row);
                Data.AllData.Tables.Add(t);
            }
            DataTable ex = new DataTable("ExcelExtractor");
            ex.Columns.Add("ID");
            ex.Columns.Add("Text");
            ex.Columns.Add("Path");
            DataRow r = ex.NewRow();
            r["ID"] = ExcelExtractor.Instance.IdCol;
            r["Text"] = ExcelExtractor.Instance.TextCol;
            r["Path"] = ExcelExtractor.Instance.FilePath ?? ExcelExtractor.Instance.DefaultFilePath;
            ex.Rows.Add(r);
            Data.AllData.Tables.Add(ex);
            Data.Save();
        }

        public static SettingsDatabase Data
        {
            get { return SettingsDatabase.Instance; }
        }        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text;
using System.IO;
using Microsoft;


namespace RCM.Settings
{
   internal class SettingsDatabase
    {
        private DataSet mAllData;

        public DataSet AllData
        {
            get { return mAllData; }
            set { mAllData = value; }
        }
        public XMLReader DataSource;
        private SettingsDatabase()
        {
            DataSource = new XMLReader();
            if (AllData == null)
            {
                if (File.Exists(FilePath))
                    AllData = DataSource.Read(FilePath);
                else
                    AllData = new DataSet("Settings");
            }
        }

        public string FilePath
        {
            get { return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Settings\\Settings.xml"; }
            set { }
        }

        private static SettingsDatabase mInstance = new SettingsDatabase();

        public static SettingsDatabase Instance
        {
            get { return SettingsDatabase.mInstance; }
        }

        public bool Save()
        {
            DataSource.Save(AllData, FilePath);
            return true;
        }

        public void Save(string FilePath)
        {
            DataSource.Save(AllData, FilePath);
        }

        public DataTable AddTable(string name)
        {
            DataTable t = new DataTable(name);
            AllData.Tables.Add(t);
            return t;
        }

        public void AddTable(DataTable t)
        {
            if (t.DataSet != null && t.DataSet != AllData)
            {
                t = t.Copy();
            }
            AllData.Tables.Add(t);
        }

        public void RemoveTable(string name)
        {
            AllData.Tables.Remove(name);
        }
        public void RemoveTable(DataTable t)
        {
            AllData.Tables.Remove(t);
        }

        public void SetTable(string name, DataTable val)
        {
            if (HasTable(name))
                RemoveTable(name);
            if (val != null)
            {
                if (val.TableName != name)
                    val.TableName = name;
                AddTable(val);
            }
        }

        public DataTable GetTable(string name)
        {
            return AllData.Tables[name];
        }

        public static bool TableEquals(DataTable t1, DataTable t2)
        {
            if (t1 == t2) return true;
            if((t1==null && t2!=null) || (t1!=null && t2==null)) return false;
            if (t1.Rows.Count != t2.Rows.Count || t1.Columns.Count != t2.Columns.Count) return false;
            for (int j = 0; j < t1.Columns.Count; j++)
            {
                if (t1.Columns[j].ToString() != t2.Columns[j].ToString())
                    return false;
                for (int i = 0; i < t1.Rows.Count; i++)
                    if (t1.Rows[i][j] != t2.Rows[i][j])
                        return false;
            }
            return true;
        }

        public bool HasTable(string name)
        {
            return AllData.Tables.Contains(name);
        }
        public event EventHandler DataSourceChanged;

    }
        internal class XMLReader
        {
            public void Dispose() { }
            public XMLReader() { }
            public System.Data.DataSet Read(string FilePath)
            {
                if (File.Exists(FilePath) && IsValid(FilePath))
                {
                    try
                    {
                        DataSet s = new DataSet();
                        s.ReadXml(FilePath);
                        return s;
                    }
                    catch
                    {
                        System.Windows.MessageBox.Show("Failed to read XML file " + FilePath);
                    }
                }
                return null;
            }

            public string SelectSource()
            {
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.Filter = "XML Files (*.xml)|*.xml";
                dialog.ShowDialog();
                return dialog.FileName;
            }
            public bool IsValid(string FilePath)
            {
                return (Path.GetExtension(FilePath) == ".xml");
            }
            public void Save(System.Data.DataSet Data, string FilePath)
            {
                if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                Data.WriteXml(FilePath);
            }
        }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RCM.Settings;

namespace RCM.Metrics
{
   public class MetricSettingsImporter: ISettingsHandler
    {
       string name;
       public MetricSettingsImporter(string name)
       {
           this.name = name;
           this.mSettingsTable = new System.Data.DataTable();
         
       }

       private System.Data.DataTable mSettingsTable;
        public System.Data.DataTable SettingsTable
        {
            get
            {
                return mSettingsTable;
            }
            set
            {
                mSettingsTable = value;
            }
        }

        private void SetRow(string Variable, string Value)
        {
            if (SettingsTable != null)
            {

            }

        }
        bool ISettingsHandler.SaveChanges()
        {
            throw new NotImplementedException();
        }

        string ISettingsHandler.TableName
        {
            get { throw new NotImplementedException(); }
        }

        void ISettingsHandler.UpdateSettingsControl()
        {
            throw new NotImplementedException();
        }

        event EventHandler ISettingsHandler.RequestSave
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }


    }
}

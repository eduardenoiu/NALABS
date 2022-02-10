using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace RCM.Settings
{
    public interface ISettingsHandler
    {
        DataTable SettingsTable { get; set; }
        bool SaveChanges();
        string TableName { get; }
        void UpdateSettingsControl();
        event EventHandler RequestSave;
    }
}

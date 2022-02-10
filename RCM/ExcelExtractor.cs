using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Data;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using RCM.Settings;

namespace RCM
{
    class ExcelExtractor
    {
        private string iD = "ID";

        int idCol = 1;

        public int IdCol
        {
            get { return idCol; }
            set { idCol = value; }
        }
        int textCol = 6;

        public int TextCol
        {
            get { return textCol; }
            set { textCol = value; }
        }
                
        private string filePath;


        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }
       
        private Workbook wb;
        private Microsoft.Office.Interop.Excel.Application excel;
        public event EventHandler LoadedData;
        public ExcelExtractor()
        {
            if (FilePath != null)
                Read(FilePath);
        }

        private static ExcelExtractor mInstance;
        public static ExcelExtractor Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new ExcelExtractor();
                return mInstance;
            }
        }
        protected void Invoke(Delegate d)
        {
            if (d != null)
                d.DynamicInvoke(this, new EventArgs());
        }
        public void Read(string path)
        {
            Invoke(ReadingExcelData);
            Instance.FilePath = path;
            //excel = new Microsoft.Office.Interop.Excel.Application();
            //excel.DisplayAlerts = false;
            //Microsoft.Office.Interop.Excel.Workbook wb = excel.Workbooks.Open(FilePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            //Microsoft.Office.Interop.Excel.Worksheet sheet = wb.Worksheets.get_Item(1);
            //Microsoft.Office.Interop.Excel.Range idColumn = sheet.get_Range(idCol, null);
            //Microsoft.Office.Interop.Excel.Range textColumn = sheet.get_Range(textCol, null);
            //DataSet allData = new DataSet();


            if (excel == null)
            {
                bool opened = Open(path);
                if (!opened)
                    return;
            }
            DataSet allData = new DataSet(wb.Name);
            Worksheet sheet = wb.Worksheets[1];
            System.Data.DataTable table = new System.Data.DataTable(sheet.Name);

            Range range = sheet.UsedRange;

            object[,] values = (object[,])range.Value;
            if (values != null)
            {
                int cols = values.GetLength(1);
                int rows = values.GetLength(0);

                table.Columns.Add(new DataColumn(Convert.ToString(values[1, idCol])));
                table.Columns.Add(new DataColumn(Convert.ToString(values[1, textCol])));

                for (int i = 2; i <= rows; i++)
                {
                    DataRow row = table.NewRow();
                    row[0] = values[i, idCol];
                    row[1] = values[i, textCol];
                    table.Rows.Add(row);
                }
                allData.Tables.Add(table);
            }
            Invoke(ExcelDataReceived);
            Close();

            int j = 0;
            ConcurrentBag<Requirement> requirements = new ConcurrentBag<Requirement>();
            System.Data.DataTable reqTable = allData.Tables[0];
            if (reqTable != null && reqTable.Rows.Count != 0)
            {
                foreach(DataRow req in reqTable.Rows)
                {
                    string id = null;
                    string text = null;
                    try
                    {
                        id = Convert.ToString(req[0]).Trim(new char[] { ' ', '\n' });
                        text = Convert.ToString(req[1]);
                        //Requirements.Items.Add(new Requirement(){Id = id, Text = text });                     
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    if (id != null)
                    {
                        Requirement newReq = new Requirement(id, text);
                        requirements.Add(newReq);
                    }
                };
            }
            Requirement.AllRequirements = new ObservableCollection<Requirement>(requirements.ToList());
        }

        public event EventHandler ReadingExcelData;

        public event EventHandler ExcelDataReceived;

        public void Close()
        {
            if (wb != null)
                wb.Close(false, Type.Missing, Type.Missing);
            if (excel != null)
            {
                Marshal.ReleaseComObject(wb);

                excel.Quit();
                Marshal.ReleaseComObject(excel);

                wb = null;
                excel = null;

                GC.GetTotalMemory(false);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.GetTotalMemory(true);
            }
        }

        public bool Open(string path)
        {
            try
            {
                if (excel == null)
                    excel = new Microsoft.Office.Interop.Excel.Application();
                if (excel == null)
                    return false;
                excel.Visible = false;
                excel.DisplayAlerts = false;
                if (File.Exists(path))
                    wb = excel.Workbooks.Open(path, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                else wb = excel.Workbooks.Add();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Save(string path)
        {
            bool opened = Open(path);
            if (!opened)
                return;
            
            Worksheet sheet = wb.Worksheets[1];
            
            sheet.Name = "Requirements measures";

            sheet.Cells[1, 1] = "ID";
            sheet.Cells[1, 2] = "Requirement text";
            int i = 1;
            foreach (Requirement req in Requirement.AllRequirements)
            {
                i++;
                sheet.Cells[i,1] = req.Id;
                sheet.Cells[i,2] = req.Text;
            }

            wb.SaveAs(path, XlFileFormat.xlCSV);
            Close();
        }
    }
}

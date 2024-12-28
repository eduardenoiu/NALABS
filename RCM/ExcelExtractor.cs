using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using RCM.Settings;
using System.Reflection;
using RCM.Extensions;
using ClosedXML.Excel;

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
            if (!File.Exists(FilePath))
            {
                MessageBox.Show("File not found!");
                return;
            }

            try
            {
                System.Data.DataTable dataTable;

                using (var excelWorkbook = new XLWorkbook(filePath))
                {
                    var worksheet = excelWorkbook.Worksheet(1);
                    if (worksheet == null)
                    {
                        MessageBox.Show("No worksheet found in the Excel file.");
                        return;
                    }

                    dataTable = new System.Data.DataTable(worksheet.Name);

                    bool FirstRow = true;
                   
                    // Range for reading the cells based on the last cell used.
                    string readRange = "1:1";
                    foreach (IXLRow row in worksheet.RowsUsed())
                    {
                        // If Reading the First Row (used) then add them as column name
                        if (FirstRow)
                        {
                            // Checking the Last cell used for column generation in datatable
                            readRange = string.Format("{0}:{1}", 1, row.LastCellUsed().Address.ColumnNumber);
                            foreach (IXLCell cell in row.Cells(readRange))
                            {
                                dataTable.Columns.Add(cell.Value.ToString());
                            }
                            FirstRow = false;
                        }
                        else
                        {
                            // Adding a Row in datatable
                            dataTable.Rows.Add();

                            int cellIndex = 0;
                           
                            // Updating the values of datatable
                            foreach (IXLCell cell in row.Cells(readRange))
                            {
                                dataTable.Rows[dataTable.Rows.Count - 1][cellIndex] = cell.Value.ToString();
                                cellIndex++;
                            }
                        }
                    }
                }

                Invoke(ExcelDataReceived);

                int j = 0;
                ConcurrentBag<Requirement> requirements = new ConcurrentBag<Requirement>();
                System.Data.DataTable reqTable = dataTable;
                if (reqTable != null && reqTable.Rows.Count != 0)
                {
                    foreach (DataRow req in reqTable.Rows)
                    {
                        string id = null;
                        string text = null;
                        try
                        {
                            id = Convert.ToString(req[0]).Trim(new char[] { ' ', '\n' });
                            text = Convert.ToString(req[1]);
                            if (id != null)
                            {
                                Requirement newReq = new Requirement(id, text);
                                requirements.Add(newReq);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    };
                }

                var requirementsList = requirements.OrderBy(r => r.Id).ToList();
                Requirement.AllRequirements = new ObservableCollection<Requirement>(requirementsList);

                System.Data.DataTable dt = requirementsList.ToDataTable();
                if (dt != null && dt.Rows.Count > 0)
                {
                    var filePath = Directory.GetCurrentDirectory();
                    dt.ToJson(Path.Combine(filePath, "rcm_output.json"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Excel file: {ex.Message}");
            }
        }

        public event EventHandler ReadingExcelData;

        public event EventHandler ExcelDataReceived;
    }
}

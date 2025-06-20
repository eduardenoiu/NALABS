﻿using ClosedXML.Excel;
using RCM.Extensions;
using RCM.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

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
        int textCol = 2;

        public int TextCol
        {
            get { return textCol; }
            set { textCol = value; }
        }
                
        private string filePath;

        public string DefaultFilePath
        {
            get
            {
                if (EnvironmentContext.IsCI)
                {
                    return System.Configuration.ConfigurationManager.AppSettings["CIFunctionalReqFilePath"];
                }
                else
                {
                    var defaultPath = System.Configuration.ConfigurationManager.AppSettings["DefaultFunctionalReqFilePath"];
                    return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + defaultPath;
                }
            }
        }

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public event EventHandler LoadedData;
        public ExcelExtractor()
        {
            if (FilePath != null)
            {
                Read(FilePath);
            }
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
            if (!File.Exists(path))
            {
                var errorMessage = "The requirements file 'FunctionalReq.xlsx' was not found. Please ensure it exists in the expected directory.";

                Logger.LogWarning(errorMessage, "ExcelExtractor");
                MessageHelper.ShowWarning(errorMessage, "File Not Found");

                return;
            }

            try
            {
                System.Data.DataTable dataTable;

                using (var excelWorkbook = new XLWorkbook(path))
                {
                    var worksheet = excelWorkbook.Worksheet(1);
                    if (worksheet == null)
                    {
                        var errorMessage = "No worksheet found in the requirements excel file.";
                        Logger.LogWarning(errorMessage, "ExcelExtractor");
                        MessageHelper.Show(errorMessage);

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
                            // Add new row to the datatable
                            var newRow = dataTable.NewRow();
                            int cellIndex = 0;

                            foreach (IXLCell cell in row.Cells(readRange))
                            {
                                string cellContent;

                                if (cell.HasFormula)
                                {
                                    // Try to get the cached value
                                    cellContent = cell.CachedValue?.ToString() ?? string.Empty;
                                }
                                else
                                {
                                    cellContent = cell.GetValue<string>();
                                }

                                newRow[cellIndex] = cellContent;
                                cellIndex++;
                            }

                            dataTable.Rows.Add(newRow);
                        }
                    }
                }

                if (dataTable.Rows.Count == 0)
                {
                    var message = "The requirements document is empty!";
                    Logger.LogWarning(message);
                    MessageHelper.ShowWarning(message, "Warning");
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
                            var message = "Faild to parse the requirements document";
                            Logger.LogError(ex, message);
                            MessageHelper.ShowWarning(message, "Warning");

                            return;
                        }
                    };
                }

                var requirementsList = requirements.OrderBy(r => r.Id).ToList();
                Requirement.AllRequirements = new ObservableCollection<Requirement>(requirementsList);

                System.Data.DataTable dt = requirementsList.ToDataTable();
                if (dt != null && dt.Rows.Count > 0)
                {
                    var filePath = Directory.GetCurrentDirectory();
                    dt.ToJson(Path.Combine(filePath, "nalabs_output.json"));
                }
            }
            catch (Exception ex)
            {
                var message = "Faild to load the Excel file.";
                Logger.LogError(ex, message);
                MessageHelper.ShowWarning(message, "ExcelExtractor");
            }
        }

        public event EventHandler ReadingExcelData;

        public event EventHandler ExcelDataReceived;
    }
}

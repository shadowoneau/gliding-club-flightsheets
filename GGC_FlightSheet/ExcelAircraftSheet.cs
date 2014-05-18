using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;

using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;


// Produce a read-only Excel version of the summary sheet for printing

namespace au.org.GGC {
    class ExcelAircraftSheet {
        private List<Summary> Summaries;
        ISheet Sheet;
        HSSFWorkbook Workbook;
        Dictionary<String, ICell> Tags = new Dictionary<string, ICell>();
        static public String Summarysheet_template = @"programdata\aircraft_sheet_excel_template.xls";
        String Airfield, SheetDate;

        public ExcelAircraftSheet(BindingList<Summary> summaries, String airfield, String date) {
            Summaries = CloneAndFinalizeSheet(summaries);
            Airfield = airfield;
            SheetDate = date;
        }

        List<Summary> CloneAndFinalizeSheet(BindingList<Summary> summaries) {
            Summaries = new List<Summary>();
            foreach (Summary summary in summaries) {
                Summaries.Add(summary);
            }
            return Summaries;
        }

        void CreateCell(IRow row, String column, dynamic value) {
            int index = Col(column);
            if (index != -1)
                row.CreateCell(index).SetCellValue(value);
        }

        void SetCell(string cell, dynamic value) {            
            if (Tags.ContainsKey(cell))
                Tags[cell].SetCellValue(value);
        }

        void SetHeaders() {
            SetCell("date", SheetDate);
            SetCell("airfield", Airfield);
        }

        public void Generate() {
            OpenSheet();
            SetHeaders();
            int r = Sheet.LastRowNum-1;
            int c = 0;
            foreach (Summary summary in Summaries) {
                IRow row = Sheet.CreateRow(r++);
                CreateCell(row, "aircraft", summary.Aircraft);
                CreateCell(row, "type", summary.Type);
                CreateCell(row, "flights", summary.Flights);
                CreateCell(row, "time", summary.Time);
                CreateCell(row, "hours", summary.Hours);
                c = Math.Max(c, row.LastCellNum);
            }
            for (int i = 0; i < c; i++)
                Sheet.AutoSizeColumn(i, true);
        }

        public void OpenSheet() {
            InitializeWorkbook();
        }

        public string SaveSheet() {
            String filename = System.IO.Path.GetTempFileName().Replace(".tmp", ".xls");
            FileStream file = new FileStream(filename, FileMode.Create);
            Workbook.Write(file);
            file.Close();
            return filename;
        }

        void InitializeWorkbook() {

            FileStream file = new FileStream(Summarysheet_template, FileMode.Open, FileAccess.Read);

            try {
                Workbook = new HSSFWorkbook(file);
            } catch {
                Workbook = new HSSFWorkbook();
                Workbook.CreateSheet("Sheet1");
            }
            Sheet = Workbook.GetSheet("Sheet1");

            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "Geelong Gliding Club";
            Workbook.DocumentSummaryInformation = dsi;

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "Daily Aircraft Flight Summaries";
            Workbook.SummaryInformation = si;

            LoadTags();
        }

        void LoadTags() {
            for (int i = 0; i <= Sheet.LastRowNum; i++) {
                foreach (ICell cell in Sheet.GetRow(i).Cells) {
                    string cvalue = cell.StringCellValue;
                    if (cvalue.StartsWith("<") && cvalue.EndsWith(">")) {
                        cvalue = cvalue.Substring(1, cvalue.Length-2);
                        Tags[cvalue] = cell;
                    }
                }
            }
        }

        int Row(string tag) {
            if (!Tags.ContainsKey(tag))
                return -1;
            return Tags[tag].RowIndex;
        }

        int Col(string tag) {
            if (!Tags.ContainsKey(tag))
                return -1;
            return Tags[tag].ColumnIndex;
        }
    }
}

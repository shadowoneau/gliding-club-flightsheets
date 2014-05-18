using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;

using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;


// Produce a read-only Excel version of the flight sheet for printing

namespace au.org.GGC {
    class ExcelFlightSheet {
        private List<Flight> Flights;
        ISheet Sheet;
        HSSFWorkbook Workbook;
        Dictionary<String, ICell> Tags = new Dictionary<string, ICell>();
        static public String Flightsheet_template = @"programdata\flightsheet_excel_template.xls";
        String Airfield, Sheetno;

        public ExcelFlightSheet(SortableBindingList<Flight> flights, String airfield, String sheetno) {
            Flights = CloneAndFinalizeSheet(flights);
            Airfield = airfield;
            Sheetno = sheetno;
        }

        List<Flight> CloneAndFinalizeSheet(SortableBindingList<Flight> flights) {
            Flights = new List<Flight>();
            foreach (Flight flight in flights) {
                if (flight.FlightNo == null)
                    continue;
                // Cloning allows us to manipulate our copy of a flight without affecting 
                // the flights on the main sheet (though we don't currently manipulate the printed flights).
                Flight f = flight.Clone();
                Flights.Add(f);
            }
            return Flights;
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
            DateTime? sheetDate = null;
            foreach (Flight flight in Flights) {
                if (flight.TakeOff != null) {
                    sheetDate = flight.TakeOff;
                    break;
                }
            }
            if (sheetDate == null)
                sheetDate = DateTime.Now;

            foreach (Flight flight in Flights) {
                SetCell("date", sheetDate);
                SetCell("airfield", Airfield);
                SetCell("flightsheet#", Sheetno);
                break;
            }
            // Use the first clerk of the day
            foreach (Flight flight in Flights)
               if (flight.Clerk.Length != 0) {
                    SetCell("clerk", flight.Clerk);
                    break;
               }
        }

        public void Generate() {
            OpenSheet();
            SetHeaders();
            int r = Sheet.LastRowNum;
            int flightno = 1;
            int c = 0;
            foreach (Flight flight in Flights) {
                IRow row = Sheet.CreateRow(r++);
                CreateCell(row, "flight#", flightno++);
                CreateCell(row, "pilot1", flight.Pilot1);
                CreateCell(row, "pilot2", flight.Pilot2);
                CreateCell(row, "tug", flight.Tug);
                CreateCell(row, "glider", flight.Glider);
                if (flight.TakeOff != null)
                    CreateCell(row, "takeoff", (DateTime)flight.TakeOff);
                if (flight.TugDown != null)
                    CreateCell(row, "tugdown", (DateTime)flight.TugDown);
                if (flight.GliderDown != null)
                    CreateCell(row, "gliderdown", (DateTime)flight.GliderDown);
                if (flight.TugDown != null && flight.TakeOff != null)
                    CreateCell(row, "towtime", ((DateTime)flight.TugDown - (DateTime)flight.TakeOff).TotalDays);
                if (flight.GliderDown != null && flight.TakeOff != null)
                    CreateCell(row, "flighttime", ((DateTime)flight.GliderDown - (DateTime)flight.TakeOff).TotalDays);
                CreateCell(row, "aef", flight.AEFType);
                CreateCell(row, "annual", flight.AnnualCheck);
                CreateCell(row, "mutual", flight.Mutual);
                CreateCell(row, "notes", flight.Notes);
                CreateCell(row, "chargeto", flight.ChargeTo);
                string notations = flight.AEFType + " " + flight.AnnualCheck + " " + flight.Mutual + " " + flight.Notes;
                if (!string.IsNullOrWhiteSpace(flight.ChargeTo))
                    notations += " Charge to: " + flight.ChargeTo;
                CreateCell(row, "notations", notations);
                c = Math.Max(c, row.LastCellNum);
            }
            for (int i = 0; i < c; i++)
                Sheet.AutoSizeColumn(i, true);

            // Tweak the time columns to add one more character (256 units) of width.
            foreach (String colname in new String [] {"takeoff", "tugdown", "gliderdown", "towtime", "flighttime"}) {
                int col = Col(colname);
                int width = Sheet.GetColumnWidth(col);
                Sheet.SetColumnWidth(col, width + 256);
            } 
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

            FileStream file = new FileStream(Flightsheet_template, FileMode.Open, FileAccess.Read);

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
            si.Subject = "Daily Gliding Flight Log";
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

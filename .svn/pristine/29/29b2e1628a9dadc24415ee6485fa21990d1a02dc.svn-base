using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace au.org.GGC {
    public partial class AircraftSummaries : Form {
        BindingList<Summary> Summaries = new BindingList<Summary>();
        String Airfield;
        String Date;

        public AircraftSummaries(SortableBindingList<Flight> flights, float fontSize, string airfield, string date) {
            InitializeComponent();
            InitColumns();
            CalculateSummaries(flights);
            SummarySheet.DataSource = Summaries;
            SetFontSize(fontSize);
            labelAirfield.Text = "Airfield: " + airfield;
            labelDate.Text = "Date: " + date;
            Airfield = airfield;
            Date = date;
        }

        private void CalculateSummaries(SortableBindingList<Flight> Flights) {
            Dictionary<string, Summary> sums = new Dictionary<string, Summary>();
            foreach (Flight flight in Flights) {
                add(sums, flight.Glider, "Glider", flight.FlightMinutes);
                add(sums, flight.Tug, "Tug", flight.TowMinutes);              
            }
            List<Summary> summaries = sums.Values.ToList();
            summaries.Sort(compareSummary);
            Summaries = new BindingList<Summary>(summaries);
        }

        int compareSummary(Summary i1, Summary i2) {
            return i1.Type.CompareTo(i2.Type) == 0 ?
                    i1.Aircraft.CompareTo(i2.Aircraft) :
                    i1.Type.CompareTo(i2.Type);
        }

        void add(Dictionary<string, Summary> sums, string aircraft, string type, int minutes) {
            if (String.IsNullOrEmpty(aircraft) || String.IsNullOrEmpty(type))
                return;
            if (!sums.ContainsKey(aircraft))
                sums[aircraft] = new Summary(aircraft, type);
            sums[aircraft].Minutes += minutes;
            sums[aircraft].Flights++;
        }


        void InitColumns() {
            SummarySheet.AutoGenerateColumns = false;
           
            string[] columns = { "Aircraft", "Type", "Flights", "Time", "Hours" };
            DataGridViewContentAlignment[] alignments = {
                                DataGridViewContentAlignment.MiddleLeft,
                                DataGridViewContentAlignment.MiddleLeft,
                                DataGridViewContentAlignment .MiddleRight,
                                DataGridViewContentAlignment .MiddleRight,
                                DataGridViewContentAlignment .MiddleRight};
            DataGridViewAutoSizeColumnMode[] autosizemodes = {
                                DataGridViewAutoSizeColumnMode.Fill,
                                DataGridViewAutoSizeColumnMode.AllCells,
                                DataGridViewAutoSizeColumnMode.AllCells,
                                DataGridViewAutoSizeColumnMode.AllCells,
                                DataGridViewAutoSizeColumnMode.AllCells
                                                             };

            for (int i = 0; i < columns.Length; i++ ) {
                var column = columns[i];
                DataGridViewColumn tbcol = new DataGridViewTextBoxColumn();

                // Column names with their spaces removed become the Flight class property to display
                tbcol.DataPropertyName = column.Replace(" ", "").Replace("\n", "");
                tbcol.Name = column;
                tbcol.MinimumWidth = 40;
                tbcol.HeaderCell.Style.Alignment = alignments[i];
                tbcol.CellTemplate.Style.Alignment = alignments[i];
                tbcol.AutoSizeMode = autosizemodes[i];
                SummarySheet.Columns.Add(tbcol);
            }
        }

        private void SetFontSize(float fontsize) {
            SummarySheet.DefaultCellStyle.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, fontsize);
            SummarySheet.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, fontsize);

            SummarySheet.RowTemplate.Height = SummarySheet.RowTemplate.MinimumHeight 
                = (Int32)(22.0 * (fontsize / 8.25));

            foreach (DataGridViewRow row in SummarySheet.Rows) 
                row.Height = row.MinimumHeight = (Int32)(22.0 * (fontsize / 8.25));

            foreach (DataGridViewColumn column in SummarySheet.Columns)
                column.MinimumWidth = (Int32)(40.0 * (fontsize / 8.25));
            SummarySheet.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
            SummarySheet.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e) {
            ExcelAircraftSheet excel = new ExcelAircraftSheet(Summaries, Airfield, Date);
            excel.Generate();
            string fileName = excel.SaveSheet();
            System.Diagnostics.Process.Start(fileName);
        }
    }

    public class Summary {
        public Summary(string aircraft, string type) {
            Minutes = 0;
            Aircraft = aircraft;
            Type = type;
        }
        public String Aircraft { get; set; }
        public String Type { get; set; }
        public int Flights { get; set; }
        public String Time {
            get {
                return new TimeSpan(0, Minutes, 0).ToString(@"h\:mm");
            }
        }
        public String Hours {
            get {
                return (Minutes / 60.0).ToString("F1");
            }
        }
        public int Minutes;
    }
}

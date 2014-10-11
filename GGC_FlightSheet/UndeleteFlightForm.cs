using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace au.org.GGC {
    public partial class UndeleteFlightForm : Form {

        List<Flight> Flights;

        public UndeleteFlightForm(List<Flight> flights, int fontSize) {
            InitializeComponent();
            this.Flights = flights;
            InitSheet();
            SetFontSize(fontSize);
        }

        void InitSheet() {
            InitColumns();
            FlightSheet.DataSource = Flights;
        }

        int[] Timecolumns = { 6, 7, 8 };
        int[] CenteredColumns = { 1, 6, 7, 8, 9, 10 };
        int[] LeftAlignedColumnHeaders = { 12 };
        int[] RightAlignedColumns = { 11 };
        int[] FillColumns = { 12 };

        void InitColumns() {
            FlightSheet.AutoGenerateColumns = false;

            // First column is the Edit/New button
            int iCol = 0;
            var btn = new DataGridViewCheckBoxColumn();
            btn.Name = "Restore";
            btn.DataPropertyName = "Undelete";
            FlightSheet.Columns.Add(btn);

            string[] columns = { "Flight No", "Pilot 1", "Pilot 2", "Tug", 
                                   "Glider", "Take Off", "Tug Down", "Glider Down", 
                                   "Tow Time", "Flight Time", "Est$", "Notations" };


            foreach (var column in columns) {
                ++iCol;
                DataGridViewTextBoxColumn tbcol = new DataGridViewTextBoxColumn();
                // Column names with their spaces removed become the Flight class property to display
                tbcol.DataPropertyName = column.Replace(" ", "").Replace("\n", "").Replace("$", "");
                tbcol.Name = column;
                tbcol.MinimumWidth = 40;
                FlightSheet.Columns.Add(tbcol);
            }

            foreach (var i in Timecolumns)
                FlightSheet.Columns[i].DataPropertyName += "_asString";
            foreach (var i in CenteredColumns)
                FlightSheet.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            foreach (var i in RightAlignedColumns)
                FlightSheet.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            foreach (var i in LeftAlignedColumnHeaders)
                FlightSheet.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            foreach (var i in FillColumns) {
                FlightSheet.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        float[] FontSizes = new float[] { 8.25f, 10f, 12f, 15f, 19f };

        void SetFontSize(int fontindex) {
            if (fontindex >= FontSizes.Length)
                fontindex = 0;

            float fontsize = FontSizes[fontindex];
            FlightSheet.DefaultCellStyle.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, fontsize);
            FlightSheet.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, fontsize);

            FlightSheet.RowTemplate.Height = FlightSheet.RowTemplate.MinimumHeight =
                (Int32)(22.0 * (fontsize / 8.25));
            foreach (DataGridViewRow row in FlightSheet.Rows)
                row.Height = row.MinimumHeight = (Int32)(22.0 * (fontsize / 8.25));

            foreach (DataGridViewColumn column in FlightSheet.Columns)
                column.MinimumWidth = (Int32)(40.0 * (fontsize / 8.25));
            FlightSheet.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
            FlightSheet.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
        }

        public List<Flight> UndeletedFlights = new List<Flight>();

        private void buttonOK_Click(object sender, EventArgs e) {
            foreach (Flight flight in Flights)
                if (flight.Undelete)
                    UndeletedFlights.Add(flight);

            foreach (Flight flight in UndeletedFlights) {
                flight.Undelete = false;
                Flights.Remove(flight);
            }
        }
    }
}

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace au.org.GGC {

    public partial class ChangeFieldAndDateDialog : Form {
        public ChangeFieldAndDateDialog(string airfield, string date, bool is_flightsheet_empty) {
            InitializeComponent();
            Airfield = airfield;
            DateString = date;
            IsFlightsheetEmpty = is_flightsheet_empty;
            InitFields();
            EnableButtons();
        }

        public string Airfield;
        public string DateString;
        public DateTime Date;
        bool IsFlightsheetEmpty;

        public bool MoveFlights {
            get {
                return !radioButtonKeep.Checked;
            }
        }

        void InitFields() {
            comboBoxAirfield.DataSource = Csv.AirfieldsList;
            comboBoxAirfield.DisplayMember = "DisplayName";
            comboBoxAirfield.SelectedIndex = -1;
            comboBoxAirfield.Text = Airfield;
            groupBoxExistingFlights.Enabled = !IsFlightsheetEmpty;
            dateTimePicker_flightsheet.Value = DateTime.ParseExact(DateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        private void comboBoxAirfield_TextChanged(object sender, System.EventArgs e) {
            EnableButtons();
        }

        void EnableButtons() {
            buttonCreateNew.Enabled = RealAirfield().Length != 0;
        }

        string RealAirfield() {
            if (comboBoxAirfield.SelectedItem == null)
                return comboBoxAirfield.Text.Trim();
            else
                return ((Displayable)comboBoxAirfield.SelectedItem).RealName;
        }

        private void buttonCreateNew_Click(object sender, System.EventArgs e) {
            Airfield = RealAirfield();
            Date = dateTimePicker_flightsheet.Value;
            DateString = Date.ToString("yyyyMMdd");
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void documentationToolStripMenuItem_Click(object sender, EventArgs e) {
            new HelpSheet().ShowDialog();
        }

    }
}

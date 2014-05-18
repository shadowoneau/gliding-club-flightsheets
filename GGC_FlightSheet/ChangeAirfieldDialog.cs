using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace au.org.GGC {

    public partial class ChangeFieldAndDateDialog : Form {
        public ChangeFieldAndDateDialog(string airfield, string date) {
            InitializeComponent();
            Airfield = airfield;
            Date = date;
            InitFields();
            EnableButtons();
        }

        public string Airfield;
        public string Date;

        void InitFields() {
            comboBoxAirfield.DataSource = Csv.AirfieldsList;
            comboBoxAirfield.DisplayMember = "DisplayName";
            comboBoxAirfield.SelectedIndex = -1;
            comboBoxAirfield.Text = Airfield;
            dateTimePicker_flightsheet.Value = DateTime.ParseExact(Date, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
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
            Date = dateTimePicker_flightsheet.Value.ToString("yyyyMMdd");
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void documentationToolStripMenuItem_Click(object sender, EventArgs e) {
            new HelpSheet().ShowDialog();
        }

    }
}

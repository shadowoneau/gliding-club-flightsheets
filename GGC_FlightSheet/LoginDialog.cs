using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace au.org.GGC {

    public partial class LoginDialog : Form {
        public LoginDialog() {
            InitializeComponent();
            InitFields();
            EnableButtons();
            ShowVersion();
        }

        public string Airfield;
        public string DateString;
        public DateTime Date;
        public string Clerk;

        void ShowVersion() {
            var text = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            labelVersion.Text = Regex.Replace(text, ".[^.]*$", "");
        }

        void InitFields() {
            SetupAirfieldBox();
            SetupDateBox();
            SetupClerkBox();
        }

        void SetupAirfieldBox() {
            comboBoxAirfield.DataSource = Csv.AirfieldsList;
            comboBoxAirfield.DisplayMember = "DisplayName";
            comboBoxAirfield.SelectedIndex = -1;
        }

        void SetupDateBox() {
            dateTimePicker_flightsheet.Value = DateTime.Now;
        }

        void SetupClerkBox() {
            comboBoxClerk.DataSource = Csv.Instance.LoadPilotsList(isMember: true);
            comboBoxClerk.DisplayMember = "DisplayName";
            comboBoxClerk.SelectedIndex = 0;
            comboBoxClerk.Focus();
        }

        private void comboBoxAirfield_TextChanged(object sender, System.EventArgs e) {
            EnableButtons();
        }

        void EnableButtons() {
            buttonLogin.Enabled = RealAirfield().Length != 0;
        }

        string RealAirfield() {
            if (comboBoxAirfield.SelectedItem == null)
                return comboBoxAirfield.Text.Trim();
            else
                return ((Displayable)comboBoxAirfield.SelectedItem).RealName;
        }

        private void buttonLogin_Click(object sender, System.EventArgs e) {
            Airfield = RealAirfield();
            Date = dateTimePicker_flightsheet.Value;
            DateString = Date.ToString("yyyyMMdd");
            Clerk = comboBoxClerk.Text;
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void comboBoxClerk_Leave(object sender, EventArgs e) {
            MainForm.FixCombo((ComboBox)sender, Csv.Instance.LoadPilotsList(isMember: true));
        }

        private void buttonHelp_Click(object sender, EventArgs e) {
            new HelpSheet().ShowDialog();
        }
    }
}

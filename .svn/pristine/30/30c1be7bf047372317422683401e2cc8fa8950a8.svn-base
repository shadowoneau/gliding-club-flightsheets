using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace au.org.GGC {
    public partial class FlightEditor : Form {

        public FlightEditor(DateTime sheetTime, Flight flight, string[] tugButtons, string[] gliderButtons) {
            InitializeComponent();
            TugButtons = tugButtons;
            GliderButtons = gliderButtons;
            SheetTime = sheetTime;
            Flight = flight.Clone();
            InitFields();
            LoadFormFields();
        }

        public Flight Flight;
        public DateTime SheetTime;
        string[] TugButtons, GliderButtons;

        void LoadFormFields() {
            Pilot1Name = Flight.Pilot1;
            Pilot2Name = Flight.Pilot2;
            Tug = Flight.Tug;
            Glider = Flight.Glider;
            TakeOff = Flight.TakeOff;
            TugDown = Flight.TugDown;
            GliderDown = Flight.GliderDown;
            AnnualCheck = Flight.AnnualCheck;
            Mutual = Flight.Mutual;
            AEFType = Flight.AEFType;
            ChargeTo = Flight.ChargeTo;
            Notes = Flight.Notes;
            FixBlankSelectors();
            SetFlightTitle();
        }

        void SetFlightTitle() {
            buttonDelete.Visible = !Flight.IsEmpty;
            if (Flight.IsEmpty)
                labelTitle.Text = "New Flight Information";
            else
                labelTitle.Text = String.Format("Flight #{0} Information", Flight.FlightNo);
        }

        void FixBlankSelectors() {
            SetBlankSelectorToHelpText(comboBoxPilot1);
            SetBlankSelectorToHelpText(comboBoxPilot2);
            SetBlankSelectorToHelpText(comboBoxTug);
            SetBlankSelectorToHelpText(comboBoxGlider);
            SetBlankSelectorToHelpText(comboBoxChargeTo);
        }

        void SetBlankSelectorToHelpText(ComboBox c) {
            if (c.Text == "") c.SelectedIndex = 0;
        }

        void StoreFormFields() {
            Flight.Pilot1 = Pilot1Name;
            Flight.Pilot1ID = Pilot1ID;
            Flight.Pilot2 = Flight.Pilot2ID = "";
            if (IsGliderDualSeater(Glider)) {
                Flight.Pilot2 = Pilot2Name;
                Flight.Pilot2ID = Pilot2ID;
            }
            Flight.Tug = Tug;
            Flight.Glider = Glider;
            Flight.TakeOff = TakeOff;
            Flight.TugDown = TugDown;
            Flight.GliderDown = GliderDown;
            Flight.AnnualCheck = AnnualCheck;
            Flight.Mutual = Mutual;
            Flight.AEFType = AEFType;
            Flight.ChargeTo = ChargeTo;
            Flight.ChargeToID = ChargeToID;
            Flight.Notes = Notes;
        }

        public string Pilot1Name {
            get { return RealName(comboBoxPilot1); }
            set { comboBoxPilot1.SelectedIndex = -1; comboBoxPilot1.Text = value; }
        }
        public string Pilot1ID { get { return AuxData(comboBoxPilot1); } }
        public string Pilot2Name {
            get { return RealName(comboBoxPilot2); }
            set { comboBoxPilot2.SelectedIndex = -1; comboBoxPilot2.Text = value; }
        }
        public string Pilot2ID { get { return AuxData(comboBoxPilot2); } }
        public string ChargeTo {
            get { return RealName(comboBoxChargeTo); }
            set { comboBoxChargeTo.SelectedIndex = -1; comboBoxChargeTo.Text = value; }
        }
        public string ChargeToID { get { return AuxData(comboBoxChargeTo); } }
        public string Tug {
            get { return RealName(comboBoxTug); }
            set { comboBoxTug.SelectedIndex = -1; comboBoxTug.Text = value; }
        }
        public string Glider {
            get { return RealName(comboBoxGlider); }
            set { comboBoxGlider.SelectedIndex = -1; comboBoxGlider.Text = value; }
        }
        public DateTime? TakeOff {
            get { return ParseTime(textBoxTakeoff.Text); }
            set { textBoxTakeoff.Text = DisplayTime(value); }
        }
        public DateTime? TugDown {
            get { return ParseTime(textBoxTugDown.Text); }
            set { textBoxTugDown.Text = DisplayTime(value); }
        }
        public DateTime? GliderDown {
            get { return ParseTime(textBoxGliderDown.Text); }
            set { textBoxGliderDown.Text = DisplayTime(value); }
        }
        public string AnnualCheck {
            get { return checkBoxAnnual.Checked ? "Annual" : ""; }
            set { checkBoxAnnual.Checked = (value == "Annual"); }
        }
        public string Mutual {
            get { return checkBoxMutual.Checked ? "Mutual" : ""; }
            set { checkBoxMutual.Checked = (value == "Mutual"); }
        }
        public string AEFType {
            get { return comboBoxAEF.Text; }
            set { comboBoxAEF.Text = value; }
        }
        public string Notes {
            get { return textBoxNotes.Text; }
            set { textBoxNotes.Text = value; }
        }

        String DisplayTime(DateTime? datetime) {
            if (datetime == null)
                return "";
            else
                return ((DateTime)datetime).ToString("HH:mm:ss");
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == (Keys.Control | Keys.Return)) {
                SaveFormAndExit();
                return true;
            } else {
                if (keyData == (Keys.Control | Keys.A))
                    return true;
                else
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        DateTime? ParseTime(string text) {
            DateTime parsedTime;
            text = text.Trim();
            if (text.Length != 0)
                foreach (String format in new String[] { "H:mm:ss", "H:mm", "H.mm", "H mm", "Hmm" })
                    if (DateTime.TryParseExact(text, format, null, System.Globalization.DateTimeStyles.AllowInnerWhite, out parsedTime))
                        return new DateTime(SheetTime.Year, SheetTime.Month, SheetTime.Day, parsedTime.Hour, parsedTime.Minute, parsedTime.Second);
            return null;
        }

        string RealName(ComboBox c) {
            if (c.SelectedItem == null)
                return c.Text;
            else
                return ((Displayable)c.SelectedItem).RealName;
        }

        string AuxData(ComboBox c) {
            if (c.SelectedItem == null)
                return "";
            else
                return ((Displayable)c.SelectedItem).AuxData;
        }

        void InitFields() {
            comboBoxPilot1.DataSource = Csv.Instance.GetPilotsList();
            comboBoxPilot2.DataSource = Csv.Instance.GetPilotsList();
            comboBoxChargeTo.DataSource = Csv.Instance.GetPilotsList();
            comboBoxTug.DataSource = Csv.Instance.GetTugsList();
            comboBoxGlider.DataSource = Csv.Instance.GetGlidersList();
            comboBoxAEF.DataSource = Csv.Instance.GetAefTypesList();

            comboBoxPilot1.DisplayMember = "DisplayName";
            comboBoxPilot2.DisplayMember = "DisplayName";
            comboBoxChargeTo.DisplayMember = "DisplayName";
            comboBoxTug.DisplayMember = "DisplayName";
            comboBoxGlider.DisplayMember = "DisplayName";
            comboBoxAEF.DisplayMember = "DisplayName";

            FixButtons(GliderButtons, new Button[] { buttonG0, buttonG1, buttonG2, buttonG3, buttonG4 });
            FixButtons(TugButtons, new Button[] { buttonT0, buttonT1, buttonT2, buttonT3, buttonT4 });
        }

        void FixButtons(string[] labels, Button[] controls) {
            for (int i = 0; i < controls.Length; i++) {
                if (i >= labels.Length) {
                    controls[i].Visible = false;
                } else {
                    controls[i].Visible = true;
                    controls[i].Text = labels[i];
                }
            }
        }

        private void comboBoxPilot_Leave(object sender, EventArgs e) {
            FixComboExactMatch((ComboBox)sender, Csv.Instance.GetPilotsList());
        }

        private void comboBoxTug_Leave(object sender, EventArgs e) {
            FixComboUsingInitial(comboBoxTug, Csv.Instance.GetTugsList(), comboBoxTug.Text);
        }

        private void comboBoxGlider_Leave(object sender, EventArgs e) {
            FixComboUsingInitial(comboBoxGlider, Csv.Instance.GetGlidersList(), comboBoxGlider.Text);
        }

        private bool IsGliderDualSeater(String glider) {
            string prefix = glider.ToLower();
            Displayable matching = null;
            if (prefix.Length > 1) {
                foreach (Displayable s in Csv.Instance.GetGlidersList()) {
                    if (s.DisplayName.ToLower().StartsWith(prefix)) {
                        matching = s;
                        break;
                    }
                }
            }
            return matching == null || matching.AuxData != "1";
        }

        private void EnableDisableP2Entry() {
            bool enable = IsGliderDualSeater(comboBoxGlider.Text);
            comboBoxPilot2.Visible = enable;
            labelPilot2.Visible = enable;
            SwapPilotsButton.Visible = enable;
        }

        void FixComboExactMatch(ComboBox c, List<Displayable> list) {
            string text = c.Text.ToLower();
            foreach (Displayable s in list) {
                if (s.DisplayName.ToLower() == text) {
                    c.Text = s.RealName;
                    break;
                }
            }
        }

        void FixComboUsingInitial(ComboBox c, List<Displayable> list, string text) {
            string prefix = text.ToLower();
            bool found = false;
            foreach (Displayable s in list) {
                if (s.DisplayName.ToLower().StartsWith(prefix)) {
                    c.Text = s.RealName;
                    found = true;
                    break;
                }
            }
            if (!found) {
                c.SelectedIndex = -1;
                c.Text = text;
            }
        }

        private void buttonTug_Click(object sender, EventArgs e) {
            FixComboUsingInitial(comboBoxTug, Csv.Instance.GetTugsList(), ((Button)sender).Text);
        }

        private void buttonGlider_Click(object sender, EventArgs e) {
            FixComboUsingInitial(comboBoxGlider, Csv.Instance.GetGlidersList(), ((Button)sender).Text);
            EnableDisableP2Entry();
        }

        private void buttonTakeOff_Click(object sender, EventArgs e) {
            textBoxTakeoff.Text = DisplayTime(DateTime.Now);
        }

        private void buttonTugDown_Click(object sender, EventArgs e) {
            textBoxTugDown.Text = DisplayTime(DateTime.Now);
        }

        private void buttonGliderDown_Click(object sender, EventArgs e) {
            textBoxGliderDown.Text = DisplayTime(DateTime.Now);
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            SaveFormAndExit();
        }

        void SaveFormAndExit() {
            StoreFormFields();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void buttonDelete_Click(object sender, EventArgs e) {
            var result = MessageBox.Show("Are you sure you want to delete this?", "Confirm Delete", MessageBoxButtons.OKCancel);
            if (result == System.Windows.Forms.DialogResult.OK)
                // Passes back Ignore to invoke the Delete function in the main form.
                this.DialogResult = System.Windows.Forms.DialogResult.Ignore;
        }

        private void buttonClearTO_Click(object sender, EventArgs e) {
            textBoxTakeoff.Text = "";
            textBoxTakeoff.Focus();
        }

        private void buttonClearTD_Click(object sender, EventArgs e) {
            textBoxTugDown.Text = "";
            textBoxTugDown.Focus();
        }

        private void buttonClearGD_Click(object sender, EventArgs e) {
            textBoxGliderDown.Text = "";
            textBoxGliderDown.Focus();
        }

        private void subtract_1_minute(TextBox timebox) {
            DateTime? existing = ParseTime(timebox.Text);
            if (existing != null) {
                existing = ((DateTime)existing).AddMinutes(-1);
                timebox.Text = DisplayTime(existing);
            }
        }

        private void button_minus_1_tow_Click(object sender, EventArgs e) {
            subtract_1_minute(textBoxTugDown);
        }

        private void button_minus_1_flight_Click(object sender, EventArgs e) {
            subtract_1_minute(textBoxGliderDown);
        }

        private void textBoxTime_TextChanged(object sender, EventArgs e) {
            var textBox = (TextBox)sender;
            if (textBox.Text.Trim().Length == 0 || ParseTime(textBox.Text) != null)
                textBox.BackColor = System.Drawing.Color.White;
            else
                textBox.BackColor = System.Drawing.Color.Yellow;
        }

        private void SwapPilotsButton_Click(object sender, EventArgs e) {
            string p1name = this.Pilot1Name;
            this.Pilot1Name = this.Pilot2Name;
            this.Pilot2Name = p1name;
        }

        internal void SetInitialFocus(int colindex) {
            Control[] ControlRows = new Control[] { 
                null, null,
                comboBoxPilot1,
                comboBoxPilot2,
                comboBoxTug,
                comboBoxGlider,
                textBoxTakeoff,
                textBoxTugDown,
                textBoxGliderDown,
                null,
                null,
                checkBoxAnnual,
                checkBoxMutual,
                comboBoxAEF,
                comboBoxChargeTo,
                textBoxNotes
            };
            if (colindex >= 0 && ControlRows[colindex] != null) {
                ControlRows[colindex].Focus();
                ControlRows[colindex].Select();
            }
        }

        private void comboBoxGlider_TextChanged(object sender, EventArgs e) {
            EnableDisableP2Entry();
        }

        private void comboBox_DropDown(object sender, EventArgs e) {
            ComboBox combobox = (ComboBox)sender;
            combobox.AutoCompleteMode = AutoCompleteMode.None;
        }

        private void comboBox_DropDownClosed(object sender, EventArgs e) {
            ComboBox combobox = (ComboBox)sender;
            combobox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        }

        private void comboBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Control && e.KeyCode == Keys.A) {
                ((ComboBox)sender).SelectAll();
                e.Handled = true;
            } else if (e.KeyCode == Keys.Enter) {
                ((ComboBox)sender).DroppedDown = false;
            } else {
                e.Handled = false;
            }
        }
    }
}

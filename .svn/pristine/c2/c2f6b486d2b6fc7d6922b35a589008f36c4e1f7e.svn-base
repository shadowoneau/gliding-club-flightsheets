using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;

using FileHelpers;
using System.Threading;
using System.IO;

namespace au.org.GGC {
    public partial class MainForm : Form {
        SortableBindingList<Flight> Flights = new SortableBindingList<Flight>();
        string FlightSheetRef {
            get { return textBoxFlightSheetRef.Text; }
            set { textBoxFlightSheetRef.Text = value; }
        }
        string _ActiveFile;
        string _FlightSheetDate;

        public MainForm() {
            InitializeComponent();
            CheckFlightSheetsFolder();
            CopyClubDataIntoPlace();
            InitSheet();
            HighlightInvalidFields();
        }

        // Copies the club data installed with the program into the 
        // FlightSheets folder. This done so that cloud-syncing the Flightsheets Folder 
        // (via Dropbox, Google Drive) can update the local club data as well.

        void CopyClubDataIntoPlace() {
            foreach (string filename in new [] {"aeftypes", "aircraft", "airfields", "pilots"}) {
                string fn = "/" + filename + ".csv";
                string target = PersisitedFlightSheetsFolder + fn;
                if (!File.Exists(target))
                    File.Copy("programdata" + fn, target, false);
            }
        }

        void InitSheet() {
            SetLogo();
            InitColumns();
            FlightSheet.DataSource = Flights;
            SetFontSize(PersistedGridFontSize);
            SetupClerkBox();
            StartWallClock();
            LoadFromCsv(GetTodaysAirfieldFilename());
            // Windows XP doesn't have the Unicode <- character.
            if (Environment.OSVersion.Version.Major < 6) {
                labelClerkAlert.Text = "<< Your Name Please?";
                buttonChangeAirfield.Text = "<< Change >>";
            }

        }

        int[] Timecolumns = new int[] { 6, 7, 8 };
        int[] TimeDownColumns = new int[] { 7, 8 };
        int TakeOffTimeColumn = 6, TugDownTimeColumn = 7, GliderDownTimeColumn = 8;
        int[] CenteredColumns = new int[] { 1, 6, 7, 8, 9, 10}; // , 11, 12 };
        int[] LeftAlignedColumnHeaders = new int[] { 11 };

        void InitColumns() {
            FlightSheet.AutoGenerateColumns = false;

            // First column is the Edit/New button
            int iCol = 0;
            var btn = new DataGridViewButtonColumn();
            btn.Name = "Edit";
            btn.DataPropertyName = "Edit";
            FlightSheet.Columns.Add(btn);

            string[] columns = { "Flight No", "Pilot 1", "Pilot 2", "Tug", 
                                   "Glider", "Take Off", "Tug Down", "Glider Down", 
                                   "Tow Time", "Flight Time", "Notations" };
                                   

            foreach (var column in columns) {
                ++iCol;
                DataGridViewColumn tbcol;
                if (Timecolumns.Contains(iCol)) {
                    var bcol = new DataGridViewButtonColumn();
                    bcol.Text = column;
                    bcol.SortMode = DataGridViewColumnSortMode.Automatic;
                    tbcol = bcol;
                } else {
                    tbcol = new DataGridViewTextBoxColumn();
                }
                // Column names with their spaces removed become the Flight class property to display
                tbcol.DataPropertyName = column.Replace(" ", "").Replace("\n", "");
                tbcol.Name = column;
                tbcol.MinimumWidth = 40;
                FlightSheet.Columns.Add(tbcol);
            }

            foreach (var i in Timecolumns)
                FlightSheet.Columns[i].DataPropertyName += "_asString";
            foreach (var i in CenteredColumns)
                FlightSheet.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            foreach (var i in LeftAlignedColumnHeaders)
                FlightSheet.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }

        void ChangeSettings() {
            var browser = new SettingsDialog();
            browser.StoragePath = PersisitedFlightSheetsFolder;
            browser.BackupPath = PersistedBackupsFolder;
            browser.TowAlarmThreshold = PersistedTowAlarmThreshold;
            browser.GliderButtons = PersistedGliderButtons;
            browser.TugButtons = PersistedTugButtons;
            var result = browser.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) {
                if (PersisitedFlightSheetsFolder != browser.StoragePath) {
                    PersisitedFlightSheetsFolder = browser.StoragePath;
                    LoadFromCsv(GetTodaysAirfieldFilename());
                    CopyClubDataIntoPlace();
                }
                if (PersistedTowAlarmThreshold != browser.TowAlarmThreshold) {
                    PersistedTowAlarmThreshold = browser.TowAlarmThreshold;
                    ColorGridRows();
                }
                PersistedTugButtons = browser.TugButtons;
                PersistedGliderButtons = browser.GliderButtons;
                PersistedBackupsFolder = browser.BackupPath;
            }
        }

        void InitializeNewFlightFields(Flight flight) {
            if (flight.IsEmpty) {
                flight.Clerk = comboBoxClerk.Text;
                flight.FlightNo = GetNextFlightNumber();
                flight.Logged = SheetTime(DateTime.Now);
            }
        }

        DateTime SheetTime(DateTime timepart) {
            if (IsSheetEmpty())
                return timepart;
            DateTime sheetdate = (DateTime)Flights[0].Logged;
            return new DateTime(sheetdate.Year, sheetdate.Month, sheetdate.Day, timepart.Hour, timepart.Minute, timepart.Second, DateTimeKind.Local);
        }

        int GetNextFlightNumber() {
            if (Flights.Count == 0)
                return 1;
            else
                return Flights.Max(f => f.IsEmpty ? 0 : (int)f.FlightNo) + 1;
        }

        void SetupClerkBox() {
            comboBoxClerk.DataSource = Csv.Instance.LoadPilotsList(isMember: true);
            comboBoxClerk.DisplayMember = "DisplayName";
            comboBoxClerk.Focus();
        }

        // If an empty time cell is double-clicked the flight is opened for editing.
        void FlightSheet_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex < 0 || e.RowIndex >= Flights.Count)
                return;
            EditFlight(e.RowIndex, e.ColumnIndex);
        }

        void NewFlight() {
            // Silently assert that the last row (which we're overwriting) is empty.
            if (Flights[Flights.Count - 1].IsEmpty)
                EditFlight(Flights.Count - 1);
        }

        bool ClerkAlertVisible {
            set {
                labelTime.Visible = !value;
                labelClerkAlert.Visible = value;
            }
        }

        void RequestClerkLogin() {
            if (ClerkCheck() == false) {
                ClerkAlertVisible = false;
                System.Threading.Thread.Sleep(100);
                ClerkAlertVisible = true;
                System.Media.SystemSounds.Asterisk.Play();
            }
        }

        string [] GetNonMembers() {
            Dictionary<string, bool> pilots = new Dictionary<string,bool>();
            foreach (Flight f in Flights) {
                if (!String.IsNullOrWhiteSpace(f.Pilot1) && String.IsNullOrWhiteSpace(f.Pilot1ID))
                    pilots[f.Pilot1] = true;
                if (!String.IsNullOrWhiteSpace(f.Pilot2) && String.IsNullOrWhiteSpace(f.Pilot2ID))
                    pilots[f.Pilot2] = true;
                if (!String.IsNullOrWhiteSpace(f.ChargeTo) && String.IsNullOrWhiteSpace(f.ChargeToID))
                    pilots[f.ChargeTo] = true;
            }
            return pilots.Keys.ToArray();
        }

        void EditFlight(int rowindex) {
            EditFlight(rowindex, -1);
        }

        void EditFlight(int rowindex, int colindex) {
            RequestClerkLogin();
            var flight = Flights[rowindex];
            var entry = new FlightEditor(SheetTime(DateTime.Now), flight, PersistedTugButtons, PersistedGliderButtons);
            entry.SetInitialFocus(colindex);
            var result = entry.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) {
                InitializeNewFlightFields(entry.Flight);
                Flights[rowindex] = entry.Flight;
                Save();
            }
            // Result.Ignore is returned to indicate a Delete request.
            if (result == System.Windows.Forms.DialogResult.Ignore) {
                RemoveFlight(rowindex);
            }
        }

        void RemoveFlight(int index) {
            Flights.RemoveAt(index);
            Save();
        }

        string AirfieldLabel {
            set { labelAirfield.Text = "Airfield: " + value; }
        }

        // FlightSheetDate stores the date and also sets the date label.
        // The format is YYYYMMDD (an 8 character string)

        String FlightSheetDate {
            set {
                _FlightSheetDate = value;
                labelDate.Text = String.Format("Date: {0}-{1}-{2}",
                       value.Substring(0, 4),
                       value.Substring(4, 2),
                       value.Substring(6, 2));
            }
            get { return _FlightSheetDate; }
        }

        private void comboBoxClerk_Leave(object sender, EventArgs e) {
            FixCombo((ComboBox)sender, Csv.Instance.LoadPilotsList(isMember: true));
        }

        void FixCombo(ComboBox c, List<Displayable> list) {
            FixCombo(c, list, c.Text);
        }

        void FixCombo(ComboBox c, List<Displayable> list, string text) {
            string prefix = text.ToLower();
            foreach (Displayable s in list) {
                if (s.DisplayName.ToLower().StartsWith(prefix)) {
                    c.Text = s.RealName;
                    break;
                }
            }
        }

        String[] Images = new String[] { "programdata/piecart.jpg", "programdata/GGCLogo.png" };
        int Imagei = 0;

        void SetLogo() {
            Imagei = (Imagei + 1) % Images.Length;
            Logo.ImageLocation = Images[Imagei];
        }
        private void Logo_Click(object sender, EventArgs e) {
            SetLogo();
        }

        private void changeFlightSheetFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            ChangeSettings();
        }

        void SetSheetDateAndFieldFromFilename(string filename) {
            // Filename is e.g.: FlightSheet_20130413_Bacchus_Marsh.csv
            String[] parts = Path.GetFileNameWithoutExtension(filename).Split("_".ToCharArray(), 3);
            FlightSheetDate = parts[1];
            AirfieldLabel = parts[2].Replace("_", " ");
        }

        bool IsSheetEmpty() {
            return Flights.Count == 0 || Flights.Count(f => f.IsEmpty) == Flights.Count;
        }

        void EnsureEmptyRowPresent() {
            if (Flights.Count == 0 || Flights.Count(f => f.IsEmpty) == 0) {
                Flights.AddNew();
                FlightSheet.FirstDisplayedScrollingRowIndex = Math.Max(0, FlightSheet.RowCount - 1);
            }
            ColorGridRows();
        }

        void Save() {
            SaveToCsv();
            EnsureEmptyRowPresent();
            ButtonizeSheet();
        }

        void SaveToCsv() {
            List<FlightEntry> flightEntries = new List<FlightEntry>();
            foreach (var flight in Flights) {
                if (flight.IsEmpty)
                    continue;
                var entry = new FlightEntry();
                entry.FltNo = flight.FlightNo.ToString();
                entry.Place = PersistedAirfield;
                entry.FltSheetRef = FlightSheetRef;
                entry.FltDate = flight.Logged;
                entry.P1Name = flight.Pilot1;
                entry.P1MemID = flight.Pilot1ID;
                entry.P2Name = flight.Pilot2;
                entry.P2MemID = flight.Pilot2ID;
                entry.AltPayAll = flight.ChargeTo;
                entry.AltPayAllMemID = flight.ChargeToID;
                entry.Tug = flight.Tug;
                entry.Glider = flight.Glider;
                entry.TakeOff = flight.TakeOff;
                entry.TugDown = flight.TugDown;
                entry.GliderDown = flight.GliderDown;
                entry.TugDuration = flight.GetTowMinutes().ToString();
                entry.GliderDuration = flight.GetFlightMinutes().ToString();
                entry.Mutual = flight.Mutual;
                entry.TIFType = Displayable.DisplayToReal(Csv.Instance.GetAefTypesList(), flight.AEFType);
                entry.AnnualChkOK = flight.AnnualCheck;
                entry.Notes = flight.Notes;
                entry.Clerk = flight.Clerk;
                flightEntries.Add(entry);
            }

            SaveToDisk(_ActiveFile, flightEntries);
            if (String.IsNullOrWhiteSpace(PersistedBackupsFolder))
                MessageBox.Show("No backups folder has been set. Please set one in Options->Settings", "No Backup Folder Set", MessageBoxButtons.OK);
            else {
                string backupFile = Path.Combine(PersistedBackupsFolder, Path.GetFileName(_ActiveFile));
                SaveToDisk(backupFile, flightEntries);
            }
        }

        // Save to disk by creating a new file and moving it on top of the old one.
        // This avoids the case of wiping out the file during a failed write.

        void SaveToDisk(string filename, List<FlightEntry> flightEntries) {
            var engine = new FileHelperEngine<FlightEntry> { HeaderText = FlightEntry.Header };
            string tempfile = Path.Combine(Path.GetDirectoryName(filename), "flights.temp");
            while (true) {
                try {
                    engine.WriteFile(tempfile, flightEntries);
                    File.Delete(filename);
                    File.Move(tempfile, filename);
                } catch (Exception e) {
                    var result = MessageBox.Show(String.Format("Error writing {0}, reason: {1}\nIs your backup device fitted?", filename, e.Message), 
                        "Error Updating File", MessageBoxButtons.RetryCancel);
                    if (result == System.Windows.Forms.DialogResult.Retry)
                        continue;
                }
                break;
            }
        }

        void LoadFromCsv(string filename) {
            _ActiveFile = filename;
            Flights.Clear();
            FlightSheetRef = "";
            SetSheetDateAndFieldFromFilename(filename);
            foreach (var entry in Csv.Instance.LoadFlightEntries(filename)) {
                FlightSheetRef = entry.FltSheetRef;
                var flight = new Flight();
                flight.FlightNo = ParseInt(entry.FltNo);
                flight.Logged = entry.FltDate;
                flight.Pilot1 = entry.P1Name;
                flight.Pilot1ID = entry.P1MemID;
                flight.Pilot2 = entry.P2Name;
                flight.Pilot2ID = entry.P2MemID;
                flight.ChargeTo = entry.AltPayAll;
                flight.ChargeToID = entry.AltPayAllMemID;
                flight.Tug = entry.Tug;
                flight.Glider = entry.Glider;
                flight.TakeOff = entry.TakeOff;
                flight.TugDown = entry.TugDown;
                flight.GliderDown = entry.GliderDown;
                flight.Mutual = entry.Mutual;
                flight.AEFType = Displayable.RealToDisplay(Csv.Instance.GetAefTypesList(), entry.TIFType);
                flight.AnnualCheck = entry.AnnualChkOK;
                flight.Notes = entry.Notes;
                flight.Clerk = entry.Clerk;
                Flights.Add(flight);
            }
            EnsureEmptyRowPresent();
            ButtonizeSheet();
        }

        int ParseInt(string val) {
            int v = 0;
            if (Int32.TryParse(val, out v))
                return v;
            return 0;
        }

        Thread WallClockThread;

        void StartWallClock() {
            WallClockThread = new Thread(new ThreadStart(WallClock));
            WallClockThread.Start();
        }

        void WallClock() {
            bool toggle = false;
            bool towAlarm = false;
            int everyMinute = 0;
            while (true) {
                everyMinute = (everyMinute + 1) % 60;
                toggle = !toggle;
                labelOverTow.SafeInvoke(d => d.Visible = toggle & towAlarm);
                labelClerkAlert.SafeInvoke(d => d.Visible = toggle & !ClerkReady);
                labelTime.SafeInvoke(d => d.Text = DateTime.Now.ToString("HH:mm:ss"));
                CalculateFlightTimes();
                if (!towAlarm)
                    everyMinute = -1;
                // Play an alert once a minute while there's an over-tow condition
                if (everyMinute == 0)
                    System.Media.SystemSounds.Exclamation.Play();
                towAlarm = CheckForTowAlarm();
                Thread.Sleep(1000);
            }
        }

        System.Drawing.Color PreLaunchColor = System.Drawing.Color.LightGreen;
        System.Drawing.Color TowColor = System.Drawing.Color.Yellow;
        System.Drawing.Color FlightColor = System.Drawing.Color.LightBlue;
        System.Drawing.Color TowAlarmColor = System.Drawing.Color.Red;

        // Colors tow alarm rows red
        bool CheckForTowAlarm() {
            bool towAlarm = false;
            for (int i = 0; i < Flights.Count-1; i++) {
                var flight = Flights[i];
                if (flight.IsInTow && flight.GetTowMinutes() > PersistedTowAlarmThreshold) {
                    FlightSheet.Rows[i].DefaultCellStyle.BackColor = TowAlarmColor;
                    towAlarm = true;
                }
            }
            return towAlarm;
        }

        // Colors grid rows according to flight status
        void ColorGridRows() {
            for (int i = 0; i < Flights.Count - 1; i++) {
                var flight = Flights[i];
                var color = System.Drawing.Color.White;
                if (flight.TakeOff == null)
                    color = PreLaunchColor;
                else if (flight.IsInTow)
                    color = TowColor;
                else if (flight.GliderDown == null)
                    color = FlightColor;
                FlightSheet.Rows[i].DefaultCellStyle.BackColor = color;
            }
        }

        void CalculateFlightTimes() {
            bool changed = false;
            foreach (var flight in Flights) {
                changed |= FixTimes(flight);
                changed |= CalcActive(flight);
            }
            if (changed)
                FlightSheet.SafeInvoke(d => d.Refresh());
        }

        // If no takeoff skip
        // no tugdown -> tugdown = now and plus suffix
        // no gliderdown -> gliderdown = now and plus suffix
        // 
        bool FixTimes(Flight flight) {
            bool changed = false;
            if (flight.TakeOff != null) {
                string towtime = flight.GetTowMinutes().ToString();
                string flighttime = flight.GetFlightMinutes().ToString();
                if (flight.IsInTow) towtime += '+';
                if (flight.GliderDown == null) flighttime += '+';
                changed = (flight.TowTime != towtime) || (flight.FlightTime != flighttime);
                if (changed) {
                    flight.TowTime = towtime;
                    flight.FlightTime = flighttime;
                }
            }
            return changed;
        }

        bool CalcActive(Flight flight) {
            bool changed = false;
            bool active = flight.TakeOff == null || flight.IsInTow || flight.GliderDown == null;
            if (flight.GliderDown != null) {
                int age = Convert.ToInt32((SheetTime(DateTime.Now) - (DateTime)flight.GliderDown).TotalMinutes);
                if (age < 10)
                    active = true;
            }
            string display = active ? "Y" : "";
            if (flight.New != display) {
                flight.New = display;
                changed = true;
            }
            return changed;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            WallClockThread.Abort();
        }

        private void fontSizeToolStripMenuItem_Click(object sender, EventArgs e) {
            SetAndSaveFontSize(Convert.ToInt32(((ToolStripMenuItem)sender).Tag));
        }
  
        void PutCheckMarkOnFontMenu(int index) {
            int i = 0;
            foreach (ToolStripMenuItem m in fontSizeToolStripMenuItem.DropDownItems)
                m.Checked = index == i++;
        }

        void SetAndSaveFontSize(int fontindex) {
            PersistedGridFontSize = fontindex;
            SetFontSize(fontindex);
        }

        float[] FontSizes = new float[] { 8.25f, 10f, 12f, 15f, 19f };

        void SetFontSize(int fontindex) {
            if (fontindex >= FontSizes.Length)
                fontindex = 0;
            PutCheckMarkOnFontMenu(fontindex);
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
            contextMenuStripFlights.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, fontsize);
            menuStripMain.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, fontsize);
        }

        private void comboBoxClerk_TextChanged(object sender, EventArgs e) {
            ClerkCheck();
        }

        void HighlightInvalidFields() {
            RefNoCheck();
            ClerkCheck();
        }

        void RefNoCheck() {
            if (textBoxFlightSheetRef.Text.Trim().Length == 0)
                textBoxFlightSheetRef.BackColor = System.Drawing.Color.Yellow;
            else
                textBoxFlightSheetRef.BackColor = System.Drawing.Color.White;
        }

        bool ClerkReady {
            get {
                var text = comboBoxClerk.Text.Trim();
                var item = ((Displayable)comboBoxClerk.SelectedItem);
                bool clerkReady = item == null ? text.Length != 0 : item.RealName.Length != 0;
                return clerkReady;
            }
        }

        bool ClerkCheck() {
            ClerkAlertVisible = !ClerkReady;
            return ClerkReady;
        }

        // Clones an existing flight into a new one with the time fields cleared out
        void CloneFlight(int index) {
            var flight = Flights[index].Clone();
            flight.TakeOff = flight.TugDown = flight.GliderDown = null;
            flight.TowTime = flight.FlightTime = "";
            flight.FlightNo = null;
            flight.AnnualCheck = flight.Notes = flight.AEFType = "";
            InitializeNewFlightFields(flight);
            Flights[Flights.Count - 1] = flight;
            Save();
        }

        private void addANewFlightToolStripMenuItem_Click(object sender, EventArgs e) {
            NewFlight();
        }

        private void exitFlightSheetsToolStripMenuItem_Click(object sender, EventArgs e) {
            Environment.Exit(0);
        }

        private void FlightSheet_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            ColorGridRows();
            ButtonizeSheet();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            ColorGridRows();
            ButtonizeSheet();
        }

        private void aboutGGCFlightSheetsToolStripMenuItem_Click(object sender, EventArgs e) {
            new AboutBoxFlightSheets().ShowDialog();
        }

        private void documentationToolStripMenuItem_Click(object sender, EventArgs e) {
            new HelpSheet().Show();
        }

        void CheckFlightSheetsFolder() {
            while (true) {
                try {
                    if (!Directory.Exists(PersisitedFlightSheetsFolder))
                        Directory.CreateDirectory(PersisitedFlightSheetsFolder);
                    break;
                } catch (Exception e) {
                    MessageBox.Show("Cannot proceed until a valid flight sheets folder is set -- failing to use: " 
                        + PersisitedFlightSheetsFolder + ", reason: "+e.Message, "Flight Sheets Folder Must Be Set", MessageBoxButtons.OK);
                    ChangeSettings();
                }
            }
        }

        string GetTodaysAirfieldFilename() {
            return GetAirfieldFilename(PersistedAirfield, DateTime.Now);
        }

        string GetAirfieldFilename(String airfield, DateTime date) {
            return GetAirfieldFilename(airfield, date.ToString("yyyyMMdd"));
        }

        string GetAirfieldFilename(String airfield, String date) {
            CheckFlightSheetsFolder();
            return string.Format("{0}/FlightSheet_{1}_{2}.csv", PersisitedFlightSheetsFolder, date, airfield.Replace(" ", "_"));
        }

        private void textBoxFlightSheetRef_TextChanged(object sender, EventArgs e) {
            RefNoCheck();
        }

        private void buttonChangeAirfield_Click(object sender, EventArgs e) {
            var changeForm = new ChangeFieldAndDateDialog(PersistedAirfield, FlightSheetDate);
            var result = changeForm.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) {
                if (PersistedAirfield != changeForm.Airfield || FlightSheetDate != changeForm.Date) {
                    PersistedAirfield = changeForm.Airfield;
                    LoadFromCsv(GetAirfieldFilename(PersistedAirfield, changeForm.Date));
                }
            }
        }

        // Selection-related grid operations

        int SelectedRow, SavedColumnSelection, SavedRowSelection;

        // For functions that operate on a row selection, this function
        // 1) determines whether this is a valid selection
        // 2) turns a single cell selection into a row selection and 
        // 3) returns whether there was a valid selection.

        bool FormRowSelection() {
            return FlightSheet.SelectedCells.Count != 0 
                    && FormRowSelection(
                        FlightSheet.SelectedCells[0].RowIndex, 
                        FlightSheet.SelectedCells[0].ColumnIndex);
        }

        bool FormRowSelection(DataGridView.HitTestInfo hit) {
            return hit.Type == DataGridViewHitTestType.Cell
                    && FormRowSelection(hit.RowIndex, hit.ColumnIndex);
        }

        bool FormRowSelection(int row, int column) {
            if (FlightSheet.SelectionMode == DataGridViewSelectionMode.FullRowSelect)
                return true;

            SelectedRow = -1;
            bool validSelection =
                  column >= 0
                  && row >= 0
                  && row < FlightSheet.RowCount - 1;
            if (validSelection) {
                FlightSheet.ClearSelection();
                FlightSheet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                FlightSheet.Rows[row].Selected = true;
                SelectedRow = row;
                SavedRowSelection = row;
                SavedColumnSelection = column;
                FlightSheet.FirstDisplayedScrollingRowIndex = row;
            }
            return validSelection;
        }

        void RestoreCellSelection() {
            FlightSheet.SelectionMode = DataGridViewSelectionMode.CellSelect;
            FlightSheet.Rows[SavedRowSelection].Cells[SavedColumnSelection].Selected = true;
        }

        // Presents the grid's context menu when a flight is right-clicked.
        private void FlightSheet_MouseDown(object sender, MouseEventArgs e) {
            var hit = FlightSheet.HitTest(e.X, e.Y);
            if (e.Button == MouseButtons.Right) {
                if (FormRowSelection(hit)) {
                    RequestClerkLogin();
                    contextMenuStripFlights.Show(MousePosition);
                }
            } else {
                FlightSheet.SelectionMode = DataGridViewSelectionMode.CellSelect;
                SavedRowSelection = hit.RowIndex;
                SavedColumnSelection = hit.ColumnIndex;
            }
        }

        private void contextMenuStripFlights_Closing(object sender, ToolStripDropDownClosingEventArgs e) {
            RestoreCellSelection();
        }

        // Implements the Edit button by handling a single click in the edit column.
        private void FlightSheet_CellClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex < 0) // Ignores any double-clicks in a column header
                return;
            if (e.ColumnIndex == 0)
                EditFlight(e.RowIndex);
            else {
                var cell = FlightSheet.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (cell is DataGridViewButtonCell) {
                    int timei = e.ColumnIndex - Timecolumns[0];
                    Flights[e.RowIndex][timei] = DateTime.Now;
                    FlightSheet.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
                    Save();
                }
            }
        }

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            bool validSelection = FormRowSelection();

            foreach (ToolStripItem item in editToolStripMenuItem.DropDownItems)
                if (new string[] { "delete", "edit", "duplicate" }.Contains(item.Tag))
                    item.Enabled = validSelection;
        }

        private void editToolStripMenuItem_DropDownClosed(object sender, EventArgs e) {
            RestoreCellSelection();
        }  

        private void deleteSelectedFileToolStripMenuItem_Click(object sender, EventArgs e) {
            if (FormRowSelection()) {
                DeleteSelectedFlight();
                RestoreCellSelection();
            }
        }

        private void deleteThisEntryToolStripMenuItem_Click(object sender, EventArgs e) {
            DeleteSelectedFlight();
        }

        void DeleteSelectedFlight() {
            int row = SelectedRow;
            var result = MessageBox.Show(
                String.Format("Are you sure you want to delete Flight #{0}?", Flights[row].FlightNo),
                "Confirm Delete", MessageBoxButtons.OKCancel);
            if (result == System.Windows.Forms.DialogResult.OK)
                RemoveFlight(FlightSheet.SelectedCells[0].RowIndex);
            else
                RestoreCellSelection();
        }

        private void cloneSelectedFlightToolStripMenuItem_Click(object sender, EventArgs e) {
            if (FormRowSelection()) {
                CloneFlight(SelectedRow);
                RestoreCellSelection();
            }
        }
         
        private void cloneIntoNewEntryToolStripMenuItem_Click(object sender, EventArgs e) {
            CloneFlight(SelectedRow);
            RestoreCellSelection();
        }

        private void editSToolStripMenuItem_Click(object sender, EventArgs e) {
            if (FormRowSelection()) {
                EditFlight(SelectedRow);
                RestoreCellSelection();
            }
        }

        private void editThisEntryToolStripMenuItem_Click(object sender, EventArgs e) {
            EditFlight(SelectedRow);
            RestoreCellSelection();
        }

        void ButtonizeSheet() {
            for (int i = 0; i < FlightSheet.Rows.Count; i++)
                SetRowTimeButtons(i);
        }

        void SetRowTimeButtons(int row) {
            bool buttonIn = false;
            bool newRow = row == FlightSheet.Rows.Count - 1;
            for (var i = 0; i < 3; i++) {
                // Skips the TugDown button for winches & motorgliders 
                bool noButton = i == 1 && Flights[row].IsTugless;
                if (!noButton && Flights[row][i] == null && !buttonIn && !newRow) {
                    var button = new DataGridViewButtonCell();
                    FlightSheet.Rows[row].Cells[Timecolumns[i]] = button;

                    if (Timecolumns[i] == TakeOffTimeColumn) {
                        string prefix = "Take";
                        if (!String.IsNullOrEmpty(Flights[row].Glider))
                            prefix = Flights[row].Glider.Split()[0];
                        FlightSheet.Rows[row].Cells[Timecolumns[i]].Value = prefix + " Off";
                    } else if (Timecolumns[i] == TugDownTimeColumn) {
                        string prefix = "Tug";
                        if (!String.IsNullOrEmpty(Flights[row].Tug))
                            prefix = Flights[row].Tug.Split()[0];
                        FlightSheet.Rows[row].Cells[Timecolumns[i]].Value = prefix + " Down";
                    } else if (Timecolumns[i] == GliderDownTimeColumn) {
                        string prefix = "Glider";
                        if (!String.IsNullOrEmpty(Flights[row].Glider))
                            prefix = Flights[row].Glider.Split()[0];
                        FlightSheet.Rows[row].Cells[Timecolumns[i]].Value = prefix + " Down";
                    } else {
                        button.UseColumnTextForButtonValue = true;
                    }
                    buttonIn = true;
                } else {
                    FlightSheet.Rows[row].Cells[Timecolumns[i]] = new DataGridViewTextBoxCell();
                    if (Timecolumns[i] == TakeOffTimeColumn)
                        Flights[row].TakeOff_asString = "";
                    if (Timecolumns[i] == TugDownTimeColumn)
                        Flights[row].TugDown_asString = "";
                    if (Timecolumns[i] == GliderDownTimeColumn)
                        Flights[row].GliderDown_asString = "";
                }

            }
        }

        static public void Fatal(string message) {
            MessageBox.Show("A fatal error has occurred: " + message + "\n\nProgram will exit", "Fatal Error", MessageBoxButtons.OK);
            Environment.Exit(1);
        }

        #region Persisted Settings
        int PersistedGridFontSize {
            get {
                try {
                    return Convert.ToInt32(CustomProperties<FlightSheetSettings>.Settings.Default.GridFontSize);
                } catch {
                    return 0;
                }
            }
            set {
                CustomProperties<FlightSheetSettings>.Settings.Default.GridFontSize = value.ToString();
                CustomProperties<FlightSheetSettings>.Settings.Save();
            }
        }
        int PersistedTowAlarmThreshold {
            get {
                try {
                    return Convert.ToInt32(CustomProperties<FlightSheetSettings>.Settings.Default.TowAlarmThreshold);
                } catch {
                    return 0;
                }
            }
            set {
                CustomProperties<FlightSheetSettings>.Settings.Default.TowAlarmThreshold = value.ToString();
                CustomProperties<FlightSheetSettings>.Settings.Save();
            }
        }
        string PersisitedFlightSheetsFolder {
            get {
                return CustomProperties<FlightSheetSettings>.Settings.Default.FlightSheetsFolder;
            }
            set {
                CustomProperties<FlightSheetSettings>.Settings.Default.FlightSheetsFolder = value;
                CustomProperties<FlightSheetSettings>.Settings.Save();
            }
        }
        string PersistedBackupsFolder {
            get {
                return CustomProperties<FlightSheetSettings>.Settings.Default.BackupsFolder;
            }
            set {
                CustomProperties<FlightSheetSettings>.Settings.Default.BackupsFolder = value;
                CustomProperties<FlightSheetSettings>.Settings.Save();
            }
        }
        string PersistedAirfield {
            get {
                return CustomProperties<FlightSheetSettings>.Settings.Default.Airfield;
            }
            set {
                CustomProperties<FlightSheetSettings>.Settings.Default.Airfield = value;
                CustomProperties<FlightSheetSettings>.Settings.Save();
            }
        }
        string[] PersistedTugButtons {
            get {
                return CustomProperties<FlightSheetSettings>.Settings.Default.TugButtons.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }
            set {
                CustomProperties<FlightSheetSettings>.Settings.Default.TugButtons = String.Join(",", value);
                CustomProperties<FlightSheetSettings>.Settings.Save();
            }
        }
        string[] PersistedGliderButtons {
            get {
                return CustomProperties<FlightSheetSettings>.Settings.Default.GliderButtons.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }
            set {
                CustomProperties<FlightSheetSettings>.Settings.Default.GliderButtons = String.Join(",", value);
                CustomProperties<FlightSheetSettings>.Settings.Save();
            }
        }



        #endregion

        private void printWithExcelToolStripMenuItem_Click(object sender, EventArgs e) {
            ExcelFlightSheet excel = new ExcelFlightSheet(Flights, PersistedAirfield, FlightSheetRef);
            excel.Generate();
            string fileName = excel.SaveSheet();
            System.Diagnostics.Process.Start(fileName);
        }

        private void openArchivedFlightSheetToolStripMenuItem1_Click(object sender, EventArgs e) {
            var browser = new ArchivedSheetSelector();
            DialogResult result = browser.ShowDialog();
            switch (result) {
                case System.Windows.Forms.DialogResult.OK:
                    PersistedAirfield = browser.Airfield;
                    LoadFromCsv(browser.Filename);
                    break;
            }
        }

        private void openTodaysFlightSheetToolStripMenuItem_Click(object sender, EventArgs e) {
            LoadFromCsv(GetTodaysAirfieldFilename());
        }
    }
}

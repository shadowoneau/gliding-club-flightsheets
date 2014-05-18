using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace au.org.GGC {

    public partial class ArchivedSheetSelector : Form {
        public ArchivedSheetSelector() {
            InitializeComponent();
            InitFields();
            EnableButtons();
        }

        public string Filename, Airfield;

        void InitFields() {
            LoadExistingList();
        }

        void LoadExistingList() {
            if (Directory.Exists(FlightsDirectory)) {
                // Filename is e.g.: FlightSheet_20130413_Bacchus_Marsh.csv
                // Sort in reverse date order (i.e. most recent first) but alphabetically
                // by airfield within each day.
                var dataSource =
                    (from fn in Directory.GetFiles(FlightsDirectory, "FlightSheet_*.csv")
                     orderby  Path.GetFileName(fn).Split("_".ToCharArray())[1] descending,
                     Path.GetFileName(fn).Split("_".ToCharArray())[2] ascending
                     select new Flightfile(fn)).ToList();
                listBoxFileList.DataSource = dataSource;
                listBoxFileList.DisplayMember = "Formatted_name";
            }
            
            EnableButtons();
        }

        private void textBoxReference_TextChanged(object sender, System.EventArgs e) {
            EnableButtons();
        }

        private void comboBoxAirfield_TextChanged(object sender, System.EventArgs e) {
            EnableButtons();
        }

        void EnableButtons() {
            buttonOpenExisting.Enabled = listBoxFileList.Text.Trim().Length != 0;
        }

        private void buttonOpenExisting_Click(object sender, EventArgs e) {
            ReturnSelectedFile();
        }

        void ReturnSelectedFile() {
            Filename = ((Flightfile)listBoxFileList.SelectedItem).Filepath;
            // Filename is e.g.: FlightSheet_20130413_Bacchus_Marsh.csv
            var fileparts = Path.GetFileNameWithoutExtension(Filename).Split("_".ToCharArray(),3);
            Airfield = fileparts[2].Replace("_", " ");
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        string FlightsDirectory {
            get {
                return CustomProperties<FlightSheetSettings>.Settings.Default.FlightSheetsFolder;
            }
        }

        private void listBoxFileList_SelectedIndexChanged(object sender, EventArgs e) {
            
        }

        private void buttonOpenTodaySheet_Click(object sender, EventArgs e) {
            this.DialogResult = System.Windows.Forms.DialogResult.Ignore;
        }

        private void listBoxFileList_MouseDoubleClick(object sender, MouseEventArgs e) {
            ReturnSelectedFile();
        }
    }

    public class Flightfile {
        public string Filepath { get; set; }
        public string Filename { get; set; }
        public string Formatted_name { get; set; }
        public Flightfile(string fn) {
            Filepath = fn;
            Filename = Path.GetFileName(fn);
            int len = File.ReadAllLines(fn).Length - 1;
            String[] parts = fn.Split("_".ToCharArray(), 3);
            String ymd = parts[1];
            String airfield = parts[2].Replace("_", " ").Replace(".csv", "");
            String date = ymd.Substring(0, 4) + "-" + ymd.Substring(4, 2) + "-" + ymd.Substring(6, 2);
            Formatted_name = String.Format("{0,4:G} {1} {2}", len, date, airfield);
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace au.org.GGC {
    public partial class SettingsDialog : Form {
        public SettingsDialog() {
            InitializeComponent();
            CheckPaths();
        }

        public string ClubInitials {
            get { return textBoxClubInitials.Text; }
            set { textBoxClubInitials.Text = value; }
        }

        public string StoragePath {
            get { return textBoxFolderName.Text; }
            set { textBoxFolderName.Text = value; }
        }

        public string BackupPath {
            get { return textBoxBackup.Text; }
            set { textBoxBackup.Text = value; }
        }

        public int TowAlarmThreshold {
            get { return Convert.ToInt32(textBoxTowAlarmThreshold.Text); }
            set { textBoxTowAlarmThreshold.Text = value.ToString(); }
        }

        private void buttonBrowse_Click(object sender, EventArgs e) {
            folderBrowserDialogStorage.SelectedPath = StoragePath;
            var result = folderBrowserDialogStorage.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
                StoragePath = folderBrowserDialogStorage.SelectedPath;
        }

        private void buttonBrowseBackup_Click(object sender, EventArgs e) {
            folderBrowserDialogBackup.SelectedPath = BackupPath;
            var result = folderBrowserDialogBackup.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
                BackupPath = folderBrowserDialogBackup.SelectedPath;
        }

        public string [] TugButtons {
            get { return textBoxTugButtons.Text.Trim().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries); }
            set { textBoxTugButtons.Text = String.Join("\r\n", value); }
        }

        public string[] GliderButtons {
            get { return textBoxGliderButtons.Text.Trim().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries); }
            set { textBoxGliderButtons.Text = String.Join("\r\n", value); }
        }

        private void textBoxTowAlarmThreshold_TextChanged(object sender, EventArgs e) {
            int val;
            buttonOK.Enabled = Int32.TryParse(textBoxTowAlarmThreshold.Text, out val);
        }

        void CheckStorageFolder(string folderName, Label warning) {
            bool okay = false;
            string message = "";
            string tempFile = Path.Combine(folderName, "folder.test.file");
            try {
                File.Create(tempFile).Close();
                File.Delete(tempFile);
                okay = true;
            } catch (Exception e) {
                message = e.Message;
            }
            warning.Text = "Folder Not Valid: " + message;
            warning.Visible = !okay;
        }

        private void textBoxFolderName_TextChanged(object sender, EventArgs e) {
            CheckStorageFolder(StoragePath, labelStorageFolderWarning);
        }

        private void textBoxBackup_TextChanged(object sender, EventArgs e) {
            CheckStorageFolder(BackupPath, labelBackupFolderWarning);
        }

        void CheckPaths() {
            CheckStorageFolder(StoragePath, labelStorageFolderWarning);
            CheckStorageFolder(BackupPath, labelBackupFolderWarning);
        }

        private void buttonEditPrintSheetTemplate_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start(ExcelFlightSheet.Flightsheet_template);
        }

        private void OpenFlightSheetFolderButton_Click(object sender, EventArgs e) {
            Process.Start("explorer.exe", textBoxFolderName.Text);
        }

        private void OpenBackupFolderButton_Click(object sender, EventArgs e) {
            Process.Start("explorer.exe", textBoxBackup.Text);
        }

     
    }
}

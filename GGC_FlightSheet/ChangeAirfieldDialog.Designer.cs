namespace au.org.GGC {
    partial class ChangeFieldAndDateDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonCreateNew = new System.Windows.Forms.Button();
            this.comboBoxAirfield = new System.Windows.Forms.ComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.documentationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.dateTimePicker_flightsheet = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBoxExistingFlights = new System.Windows.Forms.GroupBox();
            this.radioButtonKeep = new System.Windows.Forms.RadioButton();
            this.radioButtonMove = new System.Windows.Forms.RadioButton();
            this.menuStrip1.SuspendLayout();
            this.groupBoxExistingFlights.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCreateNew
            // 
            this.buttonCreateNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCreateNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCreateNew.Location = new System.Drawing.Point(263, 337);
            this.buttonCreateNew.Name = "buttonCreateNew";
            this.buttonCreateNew.Size = new System.Drawing.Size(67, 34);
            this.buttonCreateNew.TabIndex = 2;
            this.buttonCreateNew.Text = "OK";
            this.buttonCreateNew.UseVisualStyleBackColor = true;
            this.buttonCreateNew.Click += new System.EventHandler(this.buttonCreateNew_Click);
            // 
            // comboBoxAirfield
            // 
            this.comboBoxAirfield.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxAirfield.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxAirfield.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxAirfield.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxAirfield.FormattingEnabled = true;
            this.comboBoxAirfield.Location = new System.Drawing.Point(22, 113);
            this.comboBoxAirfield.Name = "comboBoxAirfield";
            this.comboBoxAirfield.Size = new System.Drawing.Size(305, 26);
            this.comboBoxAirfield.TabIndex = 0;
            this.comboBoxAirfield.TextChanged += new System.EventHandler(this.comboBoxAirfield_TextChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(356, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.documentationToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // documentationToolStripMenuItem
            // 
            this.documentationToolStripMenuItem.Name = "documentationToolStripMenuItem";
            this.documentationToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.documentationToolStripMenuItem.Text = "Documentation";
            this.documentationToolStripMenuItem.Click += new System.EventHandler(this.documentationToolStripMenuItem_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(22, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 18);
            this.label2.TabIndex = 7;
            this.label2.Text = "Set Flightsheet Airfield";
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Browse to the folder where flight sheets are stored";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(20, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(316, 24);
            this.label1.TabIndex = 8;
            this.label1.Text = "Change Flightsheet Airfield and Date";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCancel.Location = new System.Drawing.Point(25, 337);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(67, 34);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // dateTimePicker_flightsheet
            // 
            this.dateTimePicker_flightsheet.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimePicker_flightsheet.Location = new System.Drawing.Point(22, 191);
            this.dateTimePicker_flightsheet.Name = "dateTimePicker_flightsheet";
            this.dateTimePicker_flightsheet.Size = new System.Drawing.Size(305, 24);
            this.dateTimePicker_flightsheet.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(22, 170);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 18);
            this.label3.TabIndex = 11;
            this.label3.Text = "Set Flightsheet Date";
            // 
            // groupBoxExistingFlights
            // 
            this.groupBoxExistingFlights.Controls.Add(this.radioButtonKeep);
            this.groupBoxExistingFlights.Controls.Add(this.radioButtonMove);
            this.groupBoxExistingFlights.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxExistingFlights.Location = new System.Drawing.Point(22, 242);
            this.groupBoxExistingFlights.Name = "groupBoxExistingFlights";
            this.groupBoxExistingFlights.Size = new System.Drawing.Size(305, 74);
            this.groupBoxExistingFlights.TabIndex = 12;
            this.groupBoxExistingFlights.TabStop = false;
            this.groupBoxExistingFlights.Text = "Existing flights?";
            // 
            // radioButtonKeep
            // 
            this.radioButtonKeep.AutoSize = true;
            this.radioButtonKeep.Checked = true;
            this.radioButtonKeep.Location = new System.Drawing.Point(34, 22);
            this.radioButtonKeep.Name = "radioButtonKeep";
            this.radioButtonKeep.Size = new System.Drawing.Size(145, 22);
            this.radioButtonKeep.TabIndex = 1;
            this.radioButtonKeep.TabStop = true;
            this.radioButtonKeep.Text = "Keep on old sheet";
            this.radioButtonKeep.UseVisualStyleBackColor = true;
            // 
            // radioButtonMove
            // 
            this.radioButtonMove.AutoSize = true;
            this.radioButtonMove.Location = new System.Drawing.Point(34, 45);
            this.radioButtonMove.Name = "radioButtonMove";
            this.radioButtonMove.Size = new System.Drawing.Size(151, 22);
            this.radioButtonMove.TabIndex = 0;
            this.radioButtonMove.Text = "Move to new sheet";
            this.radioButtonMove.UseVisualStyleBackColor = true;
            // 
            // ChangeFieldAndDateDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 392);
            this.Controls.Add(this.groupBoxExistingFlights);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dateTimePicker_flightsheet);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxAirfield);
            this.Controls.Add(this.buttonCreateNew);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangeFieldAndDateDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Change Flightsheet Airfield and Date";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBoxExistingFlights.ResumeLayout(false);
            this.groupBoxExistingFlights.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCreateNew;
        private System.Windows.Forms.ComboBox comboBoxAirfield;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolStripMenuItem documentationToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonCancel;
        public System.Windows.Forms.DateTimePicker dateTimePicker_flightsheet;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBoxExistingFlights;
        private System.Windows.Forms.RadioButton radioButtonKeep;
        private System.Windows.Forms.RadioButton radioButtonMove;
    }
}
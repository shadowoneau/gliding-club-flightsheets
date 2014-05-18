using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace au.org.GGC {
    public partial class HelpSheet : Form {
        public HelpSheet() {
            InitializeComponent();
            richTextBoxHelpSheet.LoadFile(@"programdata\GGC_Flight_Sheet_Documentation.rtf", RichTextBoxStreamType.RichText);
        }
    }
}

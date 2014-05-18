using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

// A Gliding Club FlightSheet logging program.
// E. Woudenberg for GGC, Autumn 2013, Melbourne

// Todo:
//   Add treatments for motorized gliders
//   Offer airfields from recent past in selection list (based on archive).

namespace au.org.GGC {
    static class GGC_FlightSheet {
        [STAThread]
        static void Main() {
            if (ApplicationRunningHelper.AlreadyRunning()) {
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}

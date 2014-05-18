using System;

namespace au.org.GGC {
    // These are the objects held in the FlightSheet DataGridView.
    // They are loaded from and written to the FlightEntry objects stored in the .csv file.
    // They are also loaded into and read from the EntryForm.
    
    public class Flight {
        public Flight Clone() {
            return (Flight)this.MemberwiseClone();
        }
        // This allows access to the three timestamps though a small array.
        public DateTime? this[int i] {
            get {
                if (i == 0) return TakeOff;
                if (i == 1) return TugDown;
                if (i == 2) return GliderDown;
                return null;
            }
            set {
                if (i == 0) TakeOff = value;
                if (i == 1) TugDown = value;
                if (i == 2) GliderDown = value;
            }
        }
        public bool IsEmpty { get { return FlightNo == null; } }
        public String Edit { get { return IsEmpty ? "New" : "Edit"; } }
        public int? FlightNo { get; set; }
        public DateTime? Logged { get; set; }
        public String Pilot1 { get; set; }
        public String Pilot1ID { get; set; }
        public String Pilot2 { get; set; }
        public String Pilot2ID { get; set; }
        public String Tug { get; set; }
        public String Glider { get; set; }
        public DateTime? TakeOff { get; set; }
        public DateTime? TugDown { get; set; }
        public DateTime? GliderDown { get; set; }
        // The _asString variants are to support e.g. "SSO Down" buttons.
        String takeoff_string;
        public String TakeOff_asString {
            get {
                if (TakeOff == null) return takeoff_string;
                return ((DateTime)TakeOff).ToString("HH:mm");
            }
            set {
                takeoff_string = value;
            }
        }
        String tugdown_string;
        public String TugDown_asString {
            get {
                if (IsTugless) return null;
                if (TugDown == null) return tugdown_string;
                return ((DateTime)TugDown).ToString("HH:mm");
            }
            set {
                tugdown_string = value;
            }
        }
        String gliderdown_string;
        public String GliderDown_asString {
            get {
                if (GliderDown == null) return gliderdown_string;
                return ((DateTime)GliderDown).ToString("HH:mm");
            }
            set {
                gliderdown_string = value;
            }
        }

        public String AnnualCheck { get; set; }
        public String Mutual { get; set; }
        public String AEFType { get; set; }
        public String ChargeTo { get; set; }
        public String ChargeToID { get; set; }
        public String Notes { get; set; }
        public String Clerk { get; set; }
        public String TowTime { get; set; }
        public String FlightTime { get; set; }
        public String Notations {
            get {
                String notes = "";
                notes += addifpresent(", ", AnnualCheck);
                notes += addifpresent(", ", Mutual);
                notes += addifpresent(", ", AEFType);
                notes += addifpresent(", ", Notes);
                notes += addifpresent(", Charge to: ", ChargeTo);
                notes = notes.Trim(", ".ToCharArray()).Replace("\n", " ");
                return notes;
            }
        }

        private String addifpresent(String prefix, String body) {
            String result = "";
            if (!String.IsNullOrWhiteSpace(body))
                result = prefix + body;
            return result;
        }

        public string New { get; set; }

        public bool IsInTow {
            get {
                return (this.TugDown == null && !IsTugless);
            }
        }

        public bool IsTugless {
            get { return IsWinchLaunch || IsMotorGlider; }
        }

        bool IsWinchLaunch {
            get { return Csv.Instance.IsWinch(Tug); }
        }

        bool IsMotorGlider {
            get { 
                return Csv.Instance.IsMotorGlider(Tug) || 
                      (Csv.Instance.IsMotorGlider(Glider) && String.IsNullOrWhiteSpace(Tug)); 
            }
        }

        public TimeSpan TowTimeSpan {
            get {
                if (this.TakeOff != null && !IsWinchLaunch && !IsMotorGlider) {
                    DateTime takeoff = (DateTime)this.TakeOff;
                    DateTime tugdown = DateTime.Now;
                    if (this.TugDown != null) tugdown = (DateTime)this.TugDown;
                    return tugdown - takeoff;
                }
                return TimeSpan.FromMinutes(0);
            }
        }

        public TimeSpan FlightTimeSpan {
            get {
                if (this.TakeOff != null) {
                    DateTime takeoff = (DateTime)this.TakeOff;
                    DateTime gliderdown = DateTime.Now;
                    if (this.GliderDown != null) gliderdown = (DateTime)this.GliderDown;
                    return gliderdown - takeoff;
                }
                return TimeSpan.FromMinutes(0);
            }
        }

        public int GetTowMinutes() {
            return Convert.ToInt32(TowTimeSpan.TotalMinutes);
        }

        public int GetFlightMinutes() {
            return Convert.ToInt32(FlightTimeSpan.TotalMinutes);
        }
    }
}

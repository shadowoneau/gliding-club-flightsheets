using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FileHelpers;
using System.IO;

namespace au.org.GGC {
    public class Csv {

        static public Csv Instance = new Csv();

        static List<Displayable> PilotsList = null;
        static List<Displayable> TugsList = null;
        static List<Displayable> GlidersList = null;
        static List<Displayable> AirfieldsList = null;
        static List<Displayable> AefTypesList = null;
        static Dictionary<string, string> AircraftType = null;

        public List<Displayable> GetPilotsList() {
            if (PilotsList == null)
                PilotsList = LoadPilotsList(isMember: false);
            return new List<Displayable>(PilotsList);
        }
        public List<Displayable> GetPilotsList(string [] nonMembers) {
            if (PilotsList == null)
                PilotsList = LoadPilotsList(isMember: false);
            var first = new List<Displayable>(PilotsList);
            var second = SynthPilotsFromNonmembers(nonMembers);
            first.AddRange(second);
            return first;
        }
        public List<Displayable> GetTugsList() {
            if (TugsList == null)
                TugsList = LoadAircraftList(tug: true);
            return TugsList;
        }
        public List<Displayable> GetGlidersList() {
            if (GlidersList == null)
                GlidersList = LoadAircraftList(tug: false);
            return GlidersList;
        }
        public List<Displayable> GetAirfieldsList() {
            if (AirfieldsList == null)
                AirfieldsList = LoadAirfieldList();
            return AirfieldsList;
        }

        public List<Displayable> GetAefTypesList() {
            if (AefTypesList == null)
                AefTypesList = LoadAefTypes();
            return AefTypesList;
        }

        public string GetAircraftType(string reg) {
            if (AircraftType == null)
                AircraftType = LoadAircraftTypes();
            reg = reg.ToLower();
            return AircraftType.ContainsKey(reg) ? AircraftType[reg] : "";
        }

        public bool IsWinch(string regPlusName) {
            return regPlusName != null &&
                GetAircraftType(regPlusName.Split(" ".ToCharArray())[0]) == "w";
        }

        public bool IsMotorGlider(string regPlusName) {
            return regPlusName != null && 
                GetAircraftType(regPlusName.Split(" ".ToCharArray())[0]) == "m";
        }

        const string DropdownHelp = "-- Select from list or Type in --";

        public List<Displayable> LoadPilotsList(bool isMember=false) {
            List<Displayable> final = new List<Displayable>();
            final.Add(new Displayable() { DisplayName = DropdownHelp, RealName = "" });
            FileHelperEngine<Pilot> engine = new FileHelperEngine<Pilot>();
            engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;
            string fn = FlightSheetsFolder + "/pilots.csv";
            if (!File.Exists(fn))
                MainForm.Fatal("Could not find file: " + fn);
            var res = engine.ReadFile(fn);
            if (engine.ErrorManager.ErrorCount > 0)
                engine.ErrorManager.SaveErrors("Errors.txt");
            foreach (Pilot pilot in res) {
                if (isMember && pilot.Club.ToLower() != "ggc")
                    continue;
                string name = pilot.LastName;
                if (pilot.FirstName != "")
                    name += ", " + pilot.FirstName;
                name += ClubSuffix(pilot.Club);
                Displayable d = new Displayable();
                d.DisplayName = name;
                d.RealName = name;
                d.AuxData = pilot.ID;
                final.Add(d);
                if (pilot.FirstName != "") {
                    d = new Displayable();
                    d.DisplayName = pilot.FirstName + " " + pilot.LastName + ClubSuffix(pilot.Club);
                    d.RealName = name;
                    final.Add(d);
                }
            }
            final.Sort(CompareDisplays);
            return final;
        }

        public List<Displayable> SynthPilotsFromNonmembers(string[] values) {
            List<Displayable> r = new List<Displayable>();
            foreach (string vv in values) {
                string v = vv.Replace(" [NEW]", "");
                string[] parts = v.Split(" ,".ToCharArray(), 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1) {
                    string firstname = parts[0];
                    string lastname = parts[1];
                    if (v.Contains(",")) {
                        lastname = parts[0];
                        firstname = parts[1];
                    }
                    string norm = firstname + " " + lastname + " [NEW]";
                    string rev = lastname + ", " + firstname + " [NEW]";
                    r.Add(new Displayable(norm, rev));
                    r.Add(new Displayable(rev, rev));
                } else {
                    r.Add(new Displayable(v + " [NEW]", v + " [NEW]"));
                }
            }
            return r;            
        }

        List<Displayable> LoadAircraftList(bool tug) {
            List<Displayable> final = new List<Displayable>();
            final.Add(new Displayable() { DisplayName = DropdownHelp, RealName = "" });
            FileHelperEngine<Aircraft> engine = new FileHelperEngine<Aircraft>();
            engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;
            string fn = FlightSheetsFolder + "/aircraft.csv";
            if (!File.Exists(fn))
                MainForm.Fatal("Could not find file: " + fn);
            var res = engine.ReadFile(fn);
            if (engine.ErrorManager.ErrorCount > 0)
                engine.ErrorManager.SaveErrors("Errors.txt");
            foreach (Aircraft aircraft in res) {
                string actype = aircraft.Type.ToLower();
                bool isTug = (actype == "t" || actype == "m" || actype == "w");
                bool isGlider = (actype == "g" || actype == "m");
                if (tug && isTug || !tug && isGlider) {
                    string name = aircraft.Reg + " " + aircraft.Name + ClubSuffix(aircraft.Club);
                    Displayable d = new Displayable();
                    d.DisplayName = d.RealName = name;
                    d.AuxData = aircraft.Seats;
                    final.Add(d);
                    if (aircraft.Name != "") {
                        d = new Displayable();
                        d.DisplayName = aircraft.Name + " " + aircraft.Reg + ClubSuffix(aircraft.Club);
                        d.RealName = name;
                        d.AuxData = aircraft.Seats;
                        final.Add(d);
                    }
                }
            }
            final.Sort(CompareDisplays);
            return final;
        }

        Dictionary<string, string> LoadAircraftTypes() {
            FileHelperEngine<Aircraft> engine = new FileHelperEngine<Aircraft>();
            engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;
            string fn = FlightSheetsFolder + "/aircraft.csv";
            if (!File.Exists(fn))
                MainForm.Fatal("Could not find file: " + fn);
            var res = engine.ReadFile(fn);
            if (engine.ErrorManager.ErrorCount > 0)
                engine.ErrorManager.SaveErrors("Errors.txt");
            Dictionary<string, string> types = new Dictionary<string, string>();
            foreach (Aircraft aircraft in res)
                types[aircraft.Reg.ToLower()] = aircraft.Type.ToLower();
            return types;
        }

        List<Displayable> LoadAirfieldList() {
            List<Displayable> final = new List<Displayable>();
            // Commented out because I prefer to have Bacchus Marsh be the default
            //final.Add(new Displayable() { DisplayName = DropdownHelp, RealName = "" });
            FileHelperEngine<Airfield> engine = new FileHelperEngine<Airfield>();
            engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;
            string fn = FlightSheetsFolder + "/airfields.csv";
            if (!File.Exists(fn))
                MainForm.Fatal("Could not find file: " + fn);
            var res = engine.ReadFile(fn);
            if (engine.ErrorManager.ErrorCount > 0)
                engine.ErrorManager.SaveErrors("Errors.txt");
            foreach (Airfield airfield in res) {
                Displayable d = new Displayable();
                d.DisplayName = d.RealName = airfield.Name;
                final.Add(d);
            }
            return final;
        }

        public List<FlightEntry> LoadFlightEntries(string filename) {
            FileHelperEngine<FlightEntry> engine = new FileHelperEngine<FlightEntry>();
            engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;
            FlightEntry[] res = new FlightEntry[0];
            if (File.Exists(filename)) {
                res = engine.ReadFile(filename);
                if (engine.ErrorManager.ErrorCount > 0)
                    engine.ErrorManager.SaveErrors("Errors.txt");
            }
            return new List<FlightEntry>(res);
        }

        public List<Displayable> LoadAefTypes() {
            FileHelperEngine<AefType> engine = new FileHelperEngine<AefType>();
            engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;
            string fn = FlightSheetsFolder + "/aeftypes.csv";
            if (!File.Exists(fn))
                MainForm.Fatal("Could not find file: " + fn);

            var res = engine.ReadFile(fn);
            if (engine.ErrorManager.ErrorCount > 0)
                engine.ErrorManager.SaveErrors("Errors.txt");

            List<Displayable> final = new List<Displayable>();
            foreach (AefType aefType in res)
                final.Add(new Displayable(
                    aefType.Name, 
                    aefType.Code + " " + aefType.Name));
            return final;
        }

        // Sort alphabetically but put GGC entries first
        int CompareDisplays(Displayable a, Displayable b) {
            bool aclub = a.DisplayName.Contains("[");
            bool bclub = b.DisplayName.Contains("[");
            if (aclub ^ bclub)
                return (aclub ? 1 : -1);
            else
                return string.Compare(a.DisplayName, b.DisplayName);
        }

        string ClubSuffix(string club) {
            string name = "";
            if (club.ToLower() != "ggc" && club != "")
                name = " [" + club + "]";
            return name;
        }

        string FlightSheetsFolder {
            get {
                return CustomProperties<FlightSheetSettings>.Settings.Default.FlightSheetsFolder;
            }
            set {
                CustomProperties<FlightSheetSettings>.Settings.Default.FlightSheetsFolder = value;
                CustomProperties<FlightSheetSettings>.Settings.Save();
            }
        }

    }

    public class Displayable {
        public Displayable() { this.AuxData = ""; }
        public Displayable(string DisplayName, string RealName) {
            this.DisplayName = DisplayName;
            this.RealName = RealName;
            this.AuxData = "";
        }
        public String DisplayName { get; set; }
        public String RealName { get; set; }
        public String AuxData { get; set; }
        public static string DisplayToReal(List<Displayable> dlist, string display) {
            foreach (var d in dlist)
                if (d.DisplayName == display)
                    return d.RealName;
            return "";
        }
        public static string RealToDisplay(List<Displayable> dlist, string real) {
            foreach (var d in dlist)
                if (d.RealName == real)
                    return d.DisplayName;
            return "";
        }
    }

    [DelimitedRecord(",")]
    public sealed class AefType {
        [FieldQuoted('"', QuoteMode.OptionalForRead),FieldTrim(TrimMode.Both)] public String Code;
        [FieldQuoted('"', QuoteMode.OptionalForRead),FieldTrim(TrimMode.Both)] public String Name;
    }

    [DelimitedRecord(",")]
    public sealed class Pilot {
        [FieldQuoted('"', QuoteMode.OptionalForRead),FieldTrim(TrimMode.Both)] public String LastName;
        [FieldQuoted('"', QuoteMode.OptionalForRead),FieldTrim(TrimMode.Both)] public String FirstName;
        [FieldQuoted('"', QuoteMode.OptionalForRead),FieldTrim(TrimMode.Both)] public String ID;
        [FieldQuoted('"', QuoteMode.OptionalForRead),FieldTrim(TrimMode.Both)] public String Club;
    }

    [DelimitedRecord(",")]
    public sealed class Aircraft {
       [FieldQuoted('"', QuoteMode.OptionalForRead),FieldTrim(TrimMode.Both)] public String Reg;
       [FieldQuoted('"', QuoteMode.OptionalForRead),FieldTrim(TrimMode.Both)] public String Type;
       [FieldQuoted('"', QuoteMode.OptionalForRead),FieldTrim(TrimMode.Both)] public String Club;
       [FieldQuoted('"', QuoteMode.OptionalForRead),FieldTrim(TrimMode.Both)] public String Seats;
       [FieldQuoted('"', QuoteMode.OptionalForRead),FieldTrim(TrimMode.Both)] public String Name;
    }

    [DelimitedRecord(",")]
    public sealed class Airfield {
       [FieldQuoted('"', QuoteMode.OptionalForRead),FieldTrim(TrimMode.Both)] public String Name;
    }

    [DelimitedRecord(",")]
    [IgnoreFirst] 
    public sealed class FlightEntry {
        public static string Header="FltDate,FltSheetRef,FltNo,Place,P1Name,P1MemID,P2Name,P2MemID,Tug,Glider,TakeOff,TugDown,GliderDown,GliderDuration,TugDuration,FltShtType,Mutual,TIFType,AnnualChk1,AnnualChk2,AnnualChkOK,Solo,Kms,AltPay1,AltPay1MemID,AltPay2,AltPay2MemID,AltPayAll,AltPayAllMemID,Notes,ImpExpFlag,Clerk";
        [FieldQuoted('"', QuoteMode.OptionalForRead)] 
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm:ss")]  public DateTime? FltDate; 
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String FltSheetRef;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String FltNo;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String Place;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String P1Name;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String P1MemID;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String P2Name;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String P2MemID;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String Tug;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String Glider;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] 
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm:ss")]  public DateTime? TakeOff;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] 
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm:ss")]  public DateTime? TugDown;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] 
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm:ss")]  public DateTime? GliderDown;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String GliderDuration;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String TugDuration;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String FltShtType;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String Mutual;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String TIFType;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String AnnualChk1;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String AnnualChk2;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String AnnualChkOK;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String Solo;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String Kms;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String AltPay1;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String AltPay1MemID;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String AltPay2;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String AltPay2MemID;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String AltPayAll;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String AltPayAllMemID;
        [FieldQuoted('"', QuoteMode.OptionalForRead, MultilineMode.AllowForBoth)] public String Notes;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String ImpExpFlag;
        [FieldQuoted('"', QuoteMode.OptionalForRead)] public String Clerk;
    }
}


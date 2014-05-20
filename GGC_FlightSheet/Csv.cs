using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FileHelpers;
using System.IO;

namespace au.org.GGC {
    public class Csv {
        public Csv() {
            LoadAircraftDict();
            LoadAefTypesDict();
            LoadPilotDict();
            PilotsList = LoadPilotsList(isMember: false);
            TugsList = LoadAircraftList(tug: true);
            GlidersList = LoadAircraftList(tug: false);
            AirfieldsList = LoadAirfieldList();
            AefTypesList = LoadAefTypes();
        }

        static public Csv Instance = new Csv();

        public static List<Displayable> PilotsList;
        public static List<Displayable> TugsList;
        public static List<Displayable> GlidersList;
        public static List<Displayable> AirfieldsList;
        public static List<Displayable> AefTypesList;
        public static Dictionary<String, Aircraft> AircraftDict; // Indexed by Aircraft Rego
        public static Dictionary<String, Pilot> PilotDict; // Indexed by Pilot ID
        public static Dictionary<String, AefType> AefTypeDict; // Indexed by type code

        public string GetAircraftType(string reg) {
            return AircraftDict.ContainsKey(reg) ? AircraftDict[reg].Type.ToLower() : "";
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

        public void LoadPilotDict() {
            PilotDict = new Dictionary<string, Pilot>();
            FileHelperEngine<Pilot> engine = new FileHelperEngine<Pilot>();
            engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;
            string fn = FlightSheetsFolder + "/pilots.csv";
            if (!File.Exists(fn))
                MainForm.Fatal("Could not find file: " + fn);
            var res = engine.ReadFile(fn);
            if (engine.ErrorManager.ErrorCount > 0)
                engine.ErrorManager.SaveErrors("Errors.txt");
            foreach (Pilot pilot in res) {
                PilotDict[pilot.ID] = pilot;
            }
        }

        public List<Displayable> LoadPilotsList(bool isMember=false) {
            List<Displayable> final = new List<Displayable>();
            final.Add(new Displayable() { DisplayName = DropdownHelp, RealName = "" });
            foreach (Pilot pilot in PilotDict.Values) {
                if (isMember && pilot.Club.ToLower() != CLubInitials.ToLower())
                    continue;
                string name = pilot.LastName;
                if (pilot.FirstName != "")
                    name += ", " + pilot.FirstName;
                name += ClubSuffix(pilot.Club);
                Displayable d = new Displayable();
                d.DisplayName = name;
                d.RealName = name;
                d.Key = pilot.ID;
                final.Add(d);
                if (pilot.FirstName != "") {
                    d = new Displayable();
                    d.DisplayName = pilot.FirstName + " " + pilot.LastName + ClubSuffix(pilot.Club);
                    d.RealName = name;
                    d.Key = pilot.ID;
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

        public void LoadAircraftDict() {
            AircraftDict = new Dictionary<string, GGC.Aircraft>();
            FileHelperEngine<Aircraft> engine = new FileHelperEngine<Aircraft>();
            engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;
            string fn = FlightSheetsFolder + "/aircraft.csv";
            if (!File.Exists(fn))
                MainForm.Fatal("Could not find file: " + fn);
            var res = engine.ReadFile(fn);
            if (engine.ErrorManager.ErrorCount > 0)
                engine.ErrorManager.SaveErrors("Errors.txt");
            foreach (Aircraft aircraft in res)
                AircraftDict[aircraft.Reg] = aircraft;
        }

        List<Displayable> LoadAircraftList(bool tug) {
            List<Displayable> final = new List<Displayable>();
            final.Add(new Displayable() { DisplayName = DropdownHelp, RealName = "" });
            foreach (Aircraft aircraft in AircraftDict.Values) {
                string actype = aircraft.Type.ToLower();
                bool isTug = (actype == "t" || actype == "m" || actype == "w");
                bool isGlider = (actype == "g" || actype == "m");
                if (tug && isTug || !tug && isGlider) {
                    string name = aircraft.Reg + " " + aircraft.Name + ClubSuffix(aircraft.Club);
                    Displayable d = new Displayable();
                    d.DisplayName = d.RealName = name;
                    d.Key = aircraft.Reg;
                    final.Add(d);
                    if (aircraft.Name != "") {
                        d = new Displayable();
                        d.DisplayName = aircraft.Name + " " + aircraft.Reg + ClubSuffix(aircraft.Club);
                        d.RealName = name;
                        d.Key = aircraft.Reg;
                        final.Add(d);
                    }
                }
            }
            final.Sort(CompareDisplays);
            return final;
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

        public void LoadAefTypesDict() {
            AefTypeDict = new Dictionary<string, AefType>();
            FileHelperEngine<AefType> engine = new FileHelperEngine<AefType>();
            engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;
            string fn = FlightSheetsFolder + "/aeftypes.csv";
            if (!File.Exists(fn))
                MainForm.Fatal("Could not find file: " + fn);

            var res = engine.ReadFile(fn);
            if (engine.ErrorManager.ErrorCount > 0)
                engine.ErrorManager.SaveErrors("Errors.txt");
            foreach (AefType aefType in res)
                AefTypeDict[aefType.Code] = aefType;
        }

        public List<Displayable> LoadAefTypes() {
            List<Displayable> final = new List<Displayable>();
            foreach (AefType aefType in AefTypeDict.Values) {
                var d = new Displayable();
                d.DisplayName = aefType.Name;
                d.RealName = aefType.Code + " " + aefType.Name;
                d.Key = aefType.Code;
                final.Add(d);
            }
            return final;
        }

        // Sort alphabetically but put the club's own entries first
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
            if (club.ToLower() != CLubInitials.ToLower() && club != "")
                name = " [" + club + "]";
            return name;
        }

        string CLubInitials {
            get { return CustomProperties<FlightSheetSettings>.Settings.Default.ClubInitials; }
        }

        string FlightSheetsFolder {
            get {
                return CustomProperties<FlightSheetSettings>.Settings.Default.FlightSheetsFolder;
            }
        }

        internal bool IsClubMember(string PilotID) {
            return PilotID != null && PilotDict.ContainsKey(PilotID) && PilotDict[PilotID].Club == CLubInitials;
        }
    }

    public class Displayable {
        public Displayable() { this.Key = ""; }
        public Displayable(string DisplayName, string RealName) {
            this.DisplayName = DisplayName;
            this.RealName = RealName;
            this.Key = "";
        }
        public String DisplayName { get; set; }
        public String RealName { get; set; }
        public String Key { get; set; }
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
        public static string DisplayToKey(List<Displayable> dlist, string display) {
            foreach (var d in dlist)
                if (d.DisplayName == display)
                    return d.Key;
            return "";
        }
    }

    [DelimitedRecord(",")]
    public sealed class AefType {
        [FieldQuoted('"', QuoteMode.OptionalForRead),FieldTrim(TrimMode.Both)] public String Code;
        [FieldQuoted('"', QuoteMode.OptionalForRead),FieldTrim(TrimMode.Both)] public String Name;
        [FieldQuoted('"', QuoteMode.OptionalForRead),FieldTrim(TrimMode.Both),FieldNullValue(typeof(Int32), "0")] public Int32 Rate;
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
       [FieldQuoted('"', QuoteMode.OptionalForRead),FieldTrim(TrimMode.Both),FieldNullValue(typeof(Int32), "1")] public Int32 Seats;
       [FieldQuoted('"', QuoteMode.OptionalForRead),FieldTrim(TrimMode.Both)] public String Name;
       [FieldQuoted('"', QuoteMode.OptionalForRead),FieldTrim(TrimMode.Both),FieldNullValue(typeof(Int32), "0")] public Int32 Rate;
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


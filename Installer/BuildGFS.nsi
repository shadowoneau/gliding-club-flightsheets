# Installer script for GGC FlightSheets
# 
# MUI Symbol Definitions
!include Sections.nsh
!include MUI2.nsh
!include nsDialogs.nsh
!include XML.nsh
!insertmacro MUI_LANGUAGE English

!define APPNAME "GGC_FlightSheets"
!define COMPANYNAME "Geelong_Gliding_Club"
!define DESCRIPTION "A simple program for maintaining glider flight sheets"
!define MUI_PRODUCT "GGC_FlightSheets"
!define MUI_FILE "GGC_FlightSheet"

# These three must be integers
!define VERSIONMAJOR 1
!define VERSIONMINOR 0
!define VERSIONBUILD 0

# define installer name
outFile "GGC_FlightSheetsInstaller.exe"
 
# set desktop as install directory
InstallDir "$PROGRAMFILES\${COMPANYNAME}\${APPNAME}"
Name "${COMPANYNAME} - ${APPNAME}" 

# installer properties
XPStyle on
 
!include "FolderDialog.nsdinc"

!insertmacro MUI_PAGE_DIRECTORY

# Invoke page to set the Flightsheets folder
Page custom fnc_GFSinstaller_Show fnc_GFSinstaller_Leave

# Invoke our install section
!insertmacro MUI_PAGE_INSTFILES

Var DestDir

Section "install"
    StrCpy $DestDir $R0

    ClearErrors
    ReadRegDWORD $0 HKLM "Software\Microsoft\Net Framework Setup\NDP\v4\Full" "Install"
    IfErrors dotNet40NotFound
    IntCmp $0 1 dotNet40Found
    dotNet40NotFound: 
        Banner::show /set 76 "Installing .NET Framework 4.0" "Please wait"  
        SetOutPath $TEMP
        File "c:\Users\eric\Desktop\Gliding\GGC\GGC_FlightSheet\Installer\tools\dotNetFx40_Full_setup.exe"
        ; if you don't have $TEMP already, add here:
        ExecWait "$TEMP\dotNetFx40_Full_setup.exe /passive /norestart"
        Delete /REBOOTOK "$TEMP\dotNetFx40_Full_setup.exe"
        Banner::destroy
    dotNet40Found:

    # Pack binaries
    setOutPath $INSTDIR
    File /r c:\Users\eric\Desktop\Gliding\GGC\GGC_FlightSheet\GGC_FlightSheet\bin\Debug\*.exe
    File /r c:\Users\eric\Desktop\Gliding\GGC\GGC_FlightSheet\GGC_FlightSheet\bin\Debug\*.dll
    File /r c:\Users\eric\Desktop\Gliding\GGC\GGC_FlightSheet\GGC_FlightSheet\bin\Debug\*.pdb
    File /r c:\Users\eric\Desktop\Gliding\GGC\GGC_FlightSheet\GGC_FlightSheet\bin\Debug\*.config
    File /r c:\Users\eric\Desktop\Gliding\GGC\GGC_FlightSheet\GGC_FlightSheet\bin\Debug\programdata
    File /oname=SettingsPrototype.xml c:\Users\eric\Desktop\Gliding\GGC\GGC_FlightSheet\GGC_FlightSheet\bin\Debug\FlightSheetSettings.xml

    # define uninstaller name
    writeUninstaller $INSTDIR\FlightSheetsUninstaller.exe

    # If config file not already present, create one from the prototype
    IfFileExists "$INSTDIR/FlightSheetSettings.xml" Config_file_present
	${xml::LoadFile} "$INSTDIR/SettingsPrototype.xml" $0

        # Point at Installed location of FlightSheets folder 
        ${xml::CreateText} "$DestDir" $R0
        ${xml::RootElement} $R1 $0
        ${xml::FirstChildElement} "FlightSheetsFolder" $0 $1
        ${xml::FirstChild} "" $0 $1
        ${xml::ReplaceNode} $R0 $0

        # And set the BackupsFolder to blank
        ${xml::CreateText} "" $R0
        ${xml::RootElement} $R1 $0
        ${xml::FirstChildElement} "BackupsFolder" $0 $1
        ${xml::FirstChild} "" $0 $1
        ${xml::ReplaceNode} $R0 $0

    ${xml::SaveFile} "$INSTDIR/FlightSheetSettings.xml" $0

Config_file_present:

    # Give the settings file read write access for all users
    AccessControl::GrantOnFile \
    "$INSTDIR\FlightSheetSettings.xml" "(BU)" "GenericRead + GenericWrite"
 
    # create desktop shortcut
    CreateShortCut "$DESKTOP\${MUI_PRODUCT}.lnk" "$INSTDIR\${MUI_FILE}.exe" ""
 
	# create start-menu items
    CreateDirectory "$SMPROGRAMS\${MUI_PRODUCT}"
    CreateShortCut "$SMPROGRAMS\${MUI_PRODUCT}\Uninstall.lnk" \
    	"$INSTDIR\FlightSheetsUninstaller.exe" "" "$INSTDIR\Uninstall.exe" 0
    CreateShortCut "$SMPROGRAMS\${MUI_PRODUCT}\${MUI_PRODUCT}.lnk" \
    	"$INSTDIR\${MUI_FILE}.exe" "" "$INSTDIR\${MUI_FILE}.exe" 0
 
SectionEnd

# create a section to define what the FlightSheetsUninstaller does.
# the section will always be named "Uninstall"

section "Uninstall"

    # Always delete FlightSheetsUninstaller first
    delete "$INSTDIR\FlightSheetsUninstaller.exe"

    # now delete installed payload
	RMDir /r $INSTDIR\programdata
	delete $INSTDIR\Errors.txt
	delete $INSTDIR\FileHelpers.dll
	delete $INSTDIR\FileHelpers.pdb
	delete $INSTDIR\FileHelpers.xml
	delete $INSTDIR\FlightSheetSettings.xml
	delete $INSTDIR\GGC_FlightSheet.exe
	delete $INSTDIR\GGC_FlightSheet.exe.config
	delete $INSTDIR\GGC_FlightSheet.pdb
	delete $INSTDIR\GGC_FlightSheet.vshost.exe
	delete $INSTDIR\GGC_FlightSheet.vshost.exe.config
	delete $INSTDIR\WindowsFormsApplication1.vshost.exe.config

    # Delete Start Menu Shortcuts
    Delete "$DESKTOP\${MUI_PRODUCT}.lnk"
    Delete "$SMPROGRAMS\${MUI_PRODUCT}\*.*"
    RmDir  "$SMPROGRAMS\${MUI_PRODUCT}"
 
sectionEnd


if [ $# -lt 1 ]
then
  echo 'usage: make.sh <version> (e.g. 1.0.5)'
  exit
fi
version="$1"
config="../GGC_FlightSheet/bin/Debug/FlightSheetSettings.xml"

echo Current config file: $config
cat $config
echo
echo -n cr to proceed:
read a
makensis BuildGFS.nsi
mv GGC_FlightSheetsInstaller.exe "GGC_FlightSheetsInstaller_$version.exe"
cp "GGC_FlightSheetsInstaller_$version.exe" "/Users/eric/Desktop/GoogleDrive/Google Drive/Software"
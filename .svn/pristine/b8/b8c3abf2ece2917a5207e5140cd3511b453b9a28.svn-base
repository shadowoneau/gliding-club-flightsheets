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

# Copy in the latest aircraft and pilots from the shared drive
for i in aircraft.csv airfields.csv pilots.csv
do
   cp "/Users/eric/Desktop/GoogleDrive/Google Drive/FlightSheets/$i" \
	/Users/eric/Desktop/Gliding/GGC/GGC_FlightSheet/GGC_FlightSheet/bin/Debug/programdata/
done

# build it
makensis /V2 BuildGFS.nsi

# Copy out the finished installer to the shared drive

set -x
mv GGC_FlightSheetsInstaller.exe "GGC_FlightSheetsInstaller_$version.exe"
cp "GGC_FlightSheetsInstaller_$version.exe" "/Users/eric/Desktop/GoogleDrive/Google Drive/Software"
echo Done
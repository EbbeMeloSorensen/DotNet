@echo off
echo Executing my application 2 times
REM cmd /k "" ".\DMI.SMS.UI.Console.exe" createStationInformation -i 7913 -n Kylling
cmd /k "" ".\DMI.SMS.UI.Console.exe" listStationInformations
echo Done! Press any key to continue
pause >nul
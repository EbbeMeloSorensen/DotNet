@echo off
echo Executing my application 2 times
call ".\DMI.SMS.UI.Console.exe" createStationInformation -i 7916 -n Bamse
call ".\DMI.SMS.UI.Console.exe" createStationInformation -i 7916 -n Kylling
call ".\DMI.SMS.UI.Console.exe" createStationInformation -i 7916 -n Luna
call ".\DMI.SMS.UI.Console.exe" createStationInformation -i 7916 -n Aske
call ".\DMI.SMS.UI.Console.exe" createStationInformation -i 7916 -n Arthur
call ".\DMI.SMS.UI.Console.exe" listStationInformations
echo Done! Press any key to continue
pause >nul
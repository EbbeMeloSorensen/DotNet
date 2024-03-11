launch.json:

{
    // Use IntelliSense to learn about possible attributes.
    // Hover to view descriptions of existing attributes.
    // For more information, visit: https://go.microsoft.com/fwlink/?linkid=830387
    "version": "0.2.0",
    "configurations": [
        {
            "name": ".NET Core Launch (console)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/DMI.DAL.ObsDB.UI.Console/bin/Debug/net6.0/DMI.DAL.ObsDB.UI.Console.dll",
            "args": ["fetch", "-h", "nanoq-ro.dmi.dk", "-d", "obsdb", "-u", "ebs", "-p", "Vm6PAkPh", "-s", "06041", "-q", "temp_dry" ],
            "cwd": "${workspaceFolder}/DMI.DAL.ObsDB.UI.Console",
            "console": "internalConsole",
            "stopAtEntry": false
        },
    ]
}
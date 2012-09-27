#define MyAppName "File Replicator"
#define MyAppPublisher "TRock"
#define MyAppExeName "FileReplicator.exe"

#define SetupBaseName "filereplicator_setup_"
#define AppVersion GetFileVersion("src\TRock.FileReplicator\bin\Release\FileReplicator.exe")
#define AVF1 Copy(AppVersion, 1, Pos(".", AppVersion) - 1) + "_" + Copy(AppVersion, Pos(".", AppVersion) + 1)
#define AVF2 Copy(AVF1, 1, Pos(".", AVF1 ) - 1) + "_" + Copy(AVF1 , Pos(".", AVF1 ) + 1)
#define AppVersionFile Copy(AVF2, 1, Pos(".", AVF2 ) - 1) + "_" + Copy(AVF2 , Pos(".", AVF2 ) + 1)

[Setup]
AppId={{40DB756E-2B56-4D90-B17E-23DF745E1F66}
AppName={#MyAppName}
AppVersion={#AppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\{#MyAppPublisher}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=setup
OutputBaseFilename={#SetupBaseName + AppVersionFile}
Compression=lzma
SolidCompression=yes
SetupIconFile=src\TRock.FileReplicator\Resources\app.ico

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1
Name: "startup"; Description: "Start automatically at startup"; GroupDescription: "Windows startup"; Flags: unchecked;

[Files]
Source: "src\TRock.FileReplicator\bin\Release\*.*"; Excludes: "*.vshost.exe*, *.pdb, *.xml"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent


[Registry]
Root: "HKCU"; Subkey: "SOFTWARE\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "{#MyAppName}"; ValueData: "{app}\{#MyAppExeName}"; Flags: uninsdeletevalue; Tasks: startup

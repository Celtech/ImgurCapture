; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{CDD855A0-5BA8-4115-AED7-45AB6B7E0894}
AppName=Imgur Hub v1.112
AppVersion=1.112
;AppVerName=Imgur Hub v1.102 1.102
AppPublisher=Celtech Development
AppPublisherURL=https://github.com/Celtech/Snapr
AppSupportURL=https://github.com/Celtech/Snapr
AppUpdatesURL=https://github.com/Celtech/Snapr
DefaultDirName={pf}\Imgur Hub
DefaultGroupName=Imgur Hub
AllowNoIcons=yes
LicenseFile=D:\Projects\Programming\C#\Imgur Hub\license.rtf
OutputBaseFilename=setup
SetupIconFile=D:\Projects\Programming\C#\Imgur Capture\favicon.ico
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "D:\Projects\Programming\C#\Imgur Capture\Imgur Capture\bin\Release\Imgur Capture.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Projects\Programming\C#\Imgur Capture\Imgur Capture\bin\Release\pop.wav"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Projects\Programming\C#\Imgur Capture\Imgur Capture\bin\Release\ping.wav"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\Imgur Hub v1.112"; Filename: "{app}\Imgur Capture.exe"
Name: "{commondesktop}\Imgur Hub v1.112"; Filename: "{app}\Imgur Capture.exe"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\Imgur Hub v1.112"; Filename: "{app}\Imgur Capture.exe"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\Imgur Capture.exe"; Description: "{cm:LaunchProgram,Imgur Hub v1.112}"; Flags: nowait postinstall skipifsilent runascurrentuser


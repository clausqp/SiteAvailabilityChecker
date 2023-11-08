#define AppName "SysTrayApp"
#define SolutionFolder "SysTrayApp"
#define AppVersion "1.0.0.0"
#define ProductVersion "Release 1.0.0"
#define BuildType "Debug"
;#define BuildType "Release"
#define Publisher "CQP"
#define AppURL "http://www.cqp.com/"
;#define Instance "{param:instance|}"
#define Instance "1"
#define InstallDir "\CQP\SysTrayApp" + Instance
#define ServiceName AppName + Instance
#define ServiceDisplayName "CQP SysTrayApp " + Instance
#define LogFileBase "C:\logs\"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{6B0B4553-1B78-47D6-BA69-8D59933133C8}
AppName=ct{#AppName}
AppVersion={#AppVersion}
AppVerName={#AppVersion}
AppPublisher={#Publisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
DefaultDirName={pf}\{#InstallDir}
UninstallFilesDir={pf}\{#InstallDir}
DisableDirPage=yes
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
PrivilegesRequired=admin
OutputDir=..\..\Build\Package\Apps
OutputBaseFilename={#AppName}
Compression=lzma
SolidCompression=yes
CreateUninstallRegKey=no
VersionInfoVersion={#AppVersion}
VersionInfoProductTextVersion={#ProductVersion}
SetupLogging=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
; All files below had to be added in all Application projects
Source: "bin\{#BuildType}\netcoreapp3.1\{#AppName}.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\netcoreapp3.1\{#AppName}.dll.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\netcoreapp3.1\*.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\netcoreapp3.1\*.dll"; DestDir: "{app}"; Flags: ignoreversion
; Source: "bin\{#BuildType}\netcoreapp3.1\logging.config"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files


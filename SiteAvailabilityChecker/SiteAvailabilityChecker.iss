#define AppName "SiteAvailabilityChecker"
#define SolutionFolder "SiteAvailabilityChecker"
#define AppVersion "1.0.0.0"
#define ProductVersion "Release 1.0.0"
#define BuildType "Debug"
;#define BuildType "Release"
#define Publisher "CQP"
#define AppURL "http://www.cqp.com/"
;#define Instance "{param:instance|}"
#define Instance "1"
#define InstallDir "\CQP\SiteAvailabilityChecker" + Instance
#define ServiceName AppName + Instance
#define ServiceDisplayName "CQP SiteAvailabilityChecker " + Instance
#define LogFileBase "C:\logs\"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{7DB2732E-1975-46AC-96E8-73296699F5CE5}
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
Source: "bin\{#BuildType}\{#AppName}.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\{#AppName}.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\{#BuildType}\logging.config"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Run]
Filename: {sys}\sc.exe; Parameters: "create {#ServiceName} DisplayName= ""{#ServiceDisplayName}"" start= delayed-auto binPath= ""{app}\{#AppName}.exe"""; Flags: runhidden; 
Filename: {sys}\sc.exe; Parameters: "failure {#ServiceName} reset=0 actions= restart/60000/restart/60000/restart/60000"; Flags: runhidden; 

[UnInstallRun]
Filename: {sys}\sc.exe; Parameters: "delete {#ServiceName}" ; Flags: runhidden

[UninstallDelete]
;Remove installation log file
Type: files; Name: "{app}\*.txt"

[Code]
procedure CurStepChanged(CurStep: TSetupStep);
var
  logfilepathname, logfilename, newfilepathname, Instance: string;
begin
  //Copy log file to install folder
  if CurStep = ssDone then
  begin
    Instance := ExpandConstant('{param:instance|}')
    Log('*** {#AppName}' + Instance + ' - Copying log file ***');
    logfilepathname := ExpandConstant('{log}');
    logfilename := ExtractFileName(logfilepathname);
    newfilepathname := ExpandConstant('{app}\') + logfilename;
    FileCopy(logfilepathname, newfilepathname, false);
  end;

end;
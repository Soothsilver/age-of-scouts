#define MyAppName "Age of Scouts"
#define MyAppPublisher "Petr Hudeèek"
#define MyAppURL "https://github.com/Soothsilver/age-of-scouts"
#define MyAppExeName "Spouštìè Age of Scouts.exe"
#define MyAppVersion GetFileVersion('..\ReadyToRelease\Spouštìè Age of Scouts.exe')


[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{6589967F-8A68-4F4D-967D-36B3E30216B0}
AppName={#MyAppName}
AppVersion={#MyAppVersion};
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputBaseFilename=Instalátor Age of Scouts
WizardImageFile=Banner.bmp
SetupIconFile=Icon.ico
Compression=lzma
SolidCompression=yes
WizardSmallImageFile=Banner.bmp
AppCopyright=Petr Hudeèek 2018
UninstallDisplayName={#MyAppName} {#MyAppVersion}
OutputDir=..\Releases

[Languages]                                            
Name: "czech"; MessagesFile: "compiler:Languages\Czech.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "..\ReadyToRelease\*"; DestDir: "{app}"; Flags: ignoreversion createallsubdirs recursesubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon
Name: "{group}\{cm:UninstallProgram, {#MyAppName}}"; Filename: "{uninstallexe}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Messages]          
czech.InstallingLabel=Instaluji [name] na váš poèítaè. Mùže to trvat nìkolik minut - prosím nepøerušujte instalaci.
 
[Code]
//english.InstallingLabel=Setup installs [name] on your computer. This may take a few minutes - please do not interrupt the installation.

function IsDotNetDetected(version: string; service: cardinal): boolean;
// Indicates whether the specified version and service pack of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1.4322'     .NET Framework 1.1
//    'v2.0.50727'    .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var
    key: string;
    install, serviceCount: cardinal;
    success: boolean;
begin
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + version;
    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;
    // .NET 4.0 uses value Servicing instead of SP
    if Pos('v4', version) = 1 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;
    result := success and (install = 1) and (serviceCount >= service);
end;
procedure CurStepChanged(CurStep: TSetupStep);
var 
ResultCode: Integer;
begin
  if CurStep = ssPostInstall then begin
  //  if IsDotNetDetected('v4\Client', 0) then begin
  //  end else begin
 //     Exec(ExpandConstant('{app}\helpers\dotNetFx40_Full_setup.exe'), '/passive /norestart', '', SW_SHOW, ewWaitUntilTerminated, ResultCode);
  //  end;
    if RegKeyExists(HKEY_LOCAL_MACHINE, 'Software\Microsoft\XNA\Framework\v4.0') then begin
    end else begin
      ShellExec('', ExpandConstant('{app}\helpers\xnafx40_redist.msi'), '/passive /norestart', '', SW_SHOW, ewWaitUntilTerminated, ResultCode); // 
    end;
    end;

end;


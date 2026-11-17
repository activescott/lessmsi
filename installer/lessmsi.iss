; Inno Setup script for LessMSI
; Build:  iscc lessmsi.iss
; Produces lessmsi-<version>-setup.exe, which includes the uninstaller (unins000.exe).

#define AppName        "LessMSI"
#define AppVersion     "2.12.11"
#define AppPublisher   "Scott Willeke"
#define AppUrl         "https://lessmsi.activescott.com"
#define GuiExe         "lessmsi-gui.exe"
#define CliExe         "lessmsi.exe"
#define StagingDir     "..\dist\staging"

[Setup]
; A new AppId would be a different product to Windows, so never change this GUID.
AppId={{8D9E2A64-5B1F-4C77-9E3A-1F4C0B7A2D15}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppUrl}
AppSupportURL={#AppUrl}
AppUpdatesURL={#AppUrl}
VersionInfoVersion={#AppVersion}
VersionInfoDescription={#AppName} Setup
VersionInfoProductName={#AppName}
VersionInfoProductVersion={#AppVersion}
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
LicenseFile=..\LICENSE
OutputDir=..\dist
OutputBaseFilename=lessmsi-{#AppVersion}-setup
SetupIconFile=..\src\LessMsi.Gui\Resources\LessmsiIcon.ico
UninstallDisplayIcon={app}\{#GuiExe}
UninstallDisplayName={#AppName} {#AppVersion}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesAllowed=x86compatible x64compatible arm64
MinVersion=10.0
AllowNoIcons=yes
; lessmsi needs the .NET Framework 4.8 runtime; checked in [Code] below.
AppMutex=LessMSI.Setup.Mutex

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "explorercontextmenu"; Description: "Add ""Extract with LessMSI"" to the right-click menu of .msi files"; GroupDescription: "Windows Explorer integration:"

[Files]
; Binaries, config files and the 21 localized resource folders (cs, da, de, ... zh).
Source: "{#StagingDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#GuiExe}"
Name: "{group}\{#AppName} (command line)"; Filename: "{app}\{#CliExe}"
Name: "{group}\{cm:UninstallProgram,{#AppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#GuiExe}"; Tasks: desktopicon

[Registry]
; Right-click entries for .msi files. Flags uninsdeletekey so the uninstaller removes them again.
Root: HKCR; Subkey: "Msi.Package\shell\LessMSIExtract"; ValueType: string; ValueName: ""; \
    ValueData: "Extract with &LessMSI"; Flags: uninsdeletekey; Tasks: explorercontextmenu
Root: HKCR; Subkey: "Msi.Package\shell\LessMSIExtract"; ValueType: string; ValueName: "Icon"; \
    ValueData: "{app}\{#GuiExe},0"; Tasks: explorercontextmenu
; The output directory argument must end with a backslash, otherwise lessmsi treats it as a file name.
Root: HKCR; Subkey: "Msi.Package\shell\LessMSIExtract\command"; ValueType: string; \
    ValueName: ""; ValueData: """{app}\{#CliExe}"" x ""%1"" ""%1_extracted\\"""; \
    Flags: uninsdeletekey; Tasks: explorercontextmenu
Root: HKCR; Subkey: "Msi.Package\shell\LessMSIOpen"; ValueType: string; ValueName: ""; \
    ValueData: "Open with LessMSI &GUI"; Flags: uninsdeletekey; Tasks: explorercontextmenu
Root: HKCR; Subkey: "Msi.Package\shell\LessMSIOpen"; ValueType: string; ValueName: "Icon"; \
    ValueData: "{app}\{#GuiExe},0"; Tasks: explorercontextmenu
Root: HKCR; Subkey: "Msi.Package\shell\LessMSIOpen\command"; ValueType: string; \
    ValueName: ""; ValueData: """{app}\{#GuiExe}"" ""%1"""; Flags: uninsdeletekey; \
    Tasks: explorercontextmenu

[Run]
Filename: "{app}\{#GuiExe}"; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"; \
    Flags: nowait postinstall skipifsilent

[Code]
const
  { .NET Framework 4.8 is release 528040 or higher. }
  DotNet48Release = 528040;

function HasDotNet48: Boolean;
var
  Release: Cardinal;
begin
  Result := RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full',
    'Release', Release) and (Release >= DotNet48Release);
end;

function InitializeSetup: Boolean;
var
  Message: String;
begin
  Result := True;
  if not HasDotNet48 then
  begin
    Message := 'LessMSI requires the .NET Framework 4.8, which does not appear to be installed.' + #13#10#13#10 + 'Continue with the installation anyway?';
    if MsgBox(Message, mbConfirmation, MB_YESNO) = IDNO then
      Result := False;
  end;
end;

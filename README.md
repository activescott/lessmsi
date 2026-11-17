# LessMSI 2.12.12

A Windows utility with a graphical user interface and a command line interface that can be used to
view and extract the contents of an MSI file, a patch (.msp) or a merge module (.msm).

This is a build of the [upstream lessmsi project](https://github.com/activescott/lessmsi) with the
changes listed under [What's new](#whats-new-in-21212) below, packaged as an installable setup.

---

## What's new in 2.12.12

### Merge module (.msm) support

Merge modules are Windows Installer databases too, but they have no `Media` table: the specification
requires their cabinet to be stored in the `_Streams` table under the name `MergeModule.CABinet`.
lessmsi assumed a `Media` table existed and failed on every merge module with
`Failed to create view with query: SELECT * FROM 'Media'`.

A missing (or empty) `Media` table now falls back to the package streams, so merge modules extract
like any other package:

```
lessmsi x Microsoft_VC143_CRT_x64.msm out\
    1/20    vcruntime140.dll_amd64...
    ...
    20/20   vcruntime140_threads.dll_amd64...
```

* `.msm` files can be opened in the GUI, dropped onto it, or picked in the file dialog.
* `lessmsi v file.msm` prints the module version from `ModuleSignature` (merge modules have no
  `ProductVersion` property).
* The optional Windows Explorer integration now adds **Extract with LessMSI** and
  **Open with LessMSI GUI** to `.msm` files as well as `.msi` files.
* Files whose cab entry name collides are still disambiguated with a `.duplicate1` suffix, which is
  what happens in the Microsoft VC++ runtime modules (they carry several architectures).

## What's new in 2.12.11

### 1. Extract stub / wrapper MSI files (2.12.10)
Some MSIs contain no rows in the `File` table at all and instead carry the whole payload in the
`Binary` table, which they launch with a custom action. Firefox and Thunderbird ship this way, and so
do many "wrapped" installers. Previously `lessmsi x` extracted nothing from those files and only
printed `…setup.cab: no folders in cabinet`.

When a full extraction produces no files, lessmsi now falls back to writing the `Binary` table
streams to `<output dir>\_Binary\<stream name>`, so the payload is never lost:

```
lessmsi x "Thunderbird Setup 152.0b3.msi" "out\"
    1/1       WrappedExe
```

Cabinets that declare zero folders are also skipped before they reach the native cab reader, which
removes the misleading error text. Normal MSI files extract exactly as before.

### 2. Bigger, resizable user interface (2.12.11)

* Default window is 1100 x 760 instead of 464 x 512, minimum size 860 x 560, and everything scales
  with the window because all panes are docked or anchored.
* Base font is now Segoe UI 10 pt (was 9 pt), grid rows are 26 px, and the tab strip, the file name
  row, the button bars and the summary pane all got more breathing room.
* Buttons have real padding, so they are easier to hit.
* The legacy `StatusBar` was replaced with a `StatusStrip` that grows with the font.

### 3. Dark mode (2.12.11)

`Edit > Dark mode` (Ctrl+D) switches the whole application, including dialogs, between the light and
dark schemes, and the choice is saved per user:

* menu bar and drop downs (custom `ProfessionalColorTable`), tab headers (owner drawn), grids
  including column headers and alternating rows, text boxes, combo boxes, list boxes, buttons,
  group boxes and the status strip,
* the window title bar too, via `DwmSetWindowAttribute(DWMWA_USE_IMMERSIVE_DARK_MODE)`,
* the error dialog, preferences, language picker, about box and the extraction progress dialog.

Scrollbars are drawn by Windows itself and stay in the system colour, which is the usual look for
WinForms applications.

---

## Usage

Extract from the command line (the output directory must end with a backslash, otherwise the argument
is treated as a file filter). The input may be an `.msi`, an `.msp` or an `.msm`:

```
lessmsi x <msiFileName> [<outputDir>\]
lessmsi xo  ...   # overwrite existing files
lessmsi xfo ...   # extract flat into one folder, overwriting
lessmsi xfr ...   # extract flat into one folder, renaming duplicates
lessmsi l -t Component <msiFileName>   # list a table as CSV
lessmsi v <msiFileName>                # print the MSI ProductVersion
lessmsi o <msiFileName>                # open in the GUI
```

Run `lessmsi h` for the full help. The GUI accepts one optional file name argument and can also open
files dropped onto it.

## Installation

Run `lessmsi-2.12.12-setup.exe`. The installer:

* installs to `%ProgramFiles(x86)%\LessMSI` (all users, needs elevation),
* creates Start Menu entries, an optional desktop icon and the uninstaller,
* optionally adds **Extract with LessMSI** and **Open with LessMSI GUI** to the right-click menu of
  `.msi` and `.msm` files (removed again on uninstall),
* requires the .NET Framework 4.8 and warns at startup if it is missing.

## Building

Requirements: Windows with the .NET Framework 4.8 developer tools, plus the following, which the
build script expects at these default locations (override with the script parameters):

| Tool | Path |
| --- | --- |
| MSBuild (.NET Framework 4.8) | `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe` |
| Roslyn compiler (`Microsoft.Net.Compilers.Toolset` 4.8, `tasks\net472`) | `build-tools\roslyn\tasks\net472` |
| .NET Framework 4.8 SDK tools (`AL.exe`) | `%ProgramFiles(x86)%\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8.1 Tools` |

The projects are old style (non SDK) .NET Framework 4.8 projects and the sources use C# 9 syntax, so
the compiler bundled with MSBuild 4.8 (C# 5) is not enough; that is why the Roslyn toolset is used:

```powershell
# restore NuGet packages (packages.config based)
build-tools\nuget.exe restore src\LessMsi.sln -OutputDirectory src\packages

# build Release/x86
.\build-release.ps1

# run the unit tests
.\src\packages\xunit.runner.console.2.1.0\tools\xunit.console.x86.exe src\LessMsi.Tests\bin\Release\LessMsi.Tests.dll

# build the installer (Inno Setup 6 must be installed)
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" installer\lessmsi.iss
```

Outputs: binaries in `src\LessMsi.Cli\bin\Release` and `src\LessMsi.Gui\bin\Release`, the staged
release layout in `dist\staging`, and the installer in `dist\lessmsi-2.12.12-setup.exe`
(`stage-and-package.ps1` does the staging and the ISCC run in one step).

## Release artifacts

| File | Description |
| --- | --- |
| `dist\lessmsi-2.12.12-setup.exe` | Inno Setup installer (2.5 MB, includes the uninstaller) |
| `dist\lessmsi-2.12.12-portable.zip` | Portable copy of the same binaries |
| `dist\screenshots` | Light and dark mode screenshots used to check the UI |

## Credits

### Author and maintainers

* **Scott Willeke** ([@activescott](https://github.com/activescott)) — author and maintainer of
  [lessmsi](https://github.com/activescott/lessmsi), and also the author of the `LessIO` and
  `libmspack4n` libraries that this tool is built on. Copyright (c) 2004 Scott Willeke.
* **[@mega5800](https://github.com/mega5800)** — co-maintainer of the upstream project.

### Localization

The 21 UI translations (cs, da, de, es, fi, fr, hu, it, ja, ko, nl, no, pl, pt, ru, sk, sq, sr-Cyrl,
sr-Latn, sv, zh) are contributed by the lessmsi community through upstream pull requests. Thank you.

### Third party components

| Component | Author / license | Used for |
| --- | --- | --- |
| [lessmsi](https://github.com/activescott/lessmsi) | Scott Willeke, MIT | The application itself |
| `LessIO` | Scott Willeke | Compound file (OLE storage) reader |
| `libmspack4n` / `mspack.dll` | .NET wrapper by Scott Willeke; [libmspack](https://www.cabextract.org.uk/libmspack/) by Stuart Caie, LGPL | Cabinet (CAB) decompression |
| WiX Toolset v2 interop (`wix.dll`, `wixcab.dll`) | Microsoft Corporation, [Windows Installer XML](https://wixtoolset.org/) | MSI database access |
| NDesk.Options / [Mono.Options](https://github.com/xamarin/Xamarin.Forms) | Jonathan Pryor, MIT | Command line parsing |
| [xUnit.net](https://xunit.net/) | Brad Wilson and contributors, Apache 2.0 | Unit tests |
| [Inno Setup](https://jrsoftware.org/isinfo.php) | Jordan Russell and Martijn Laan | Installer and uninstaller |
| [Roslyn](https://github.com/dotnet/roslyn) compiler platform | .NET Foundation, MIT | Build time compiler |
| .NET Framework 4.8 | Microsoft Corporation | Runtime |

### License

lessmsi is released under the MIT license; see [LICENSE](LICENSE).

## Links

* Upstream project and issue tracker: <https://github.com/activescott/lessmsi>
* Documentation wiki (including the command line reference): <https://github.com/activescott/lessmsi/wiki>
* Home page: <https://lessmsi.activescott.com>

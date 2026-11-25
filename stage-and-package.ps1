# Stages the Release/x86 build into dist\staging and compiles the Inno Setup installer.
param(
    [string]$Iscc = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
)

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot
$staging = Join-Path $root "dist\staging"
$gui = Join-Path $root "src\LessMsi.Gui\bin\Release"
$cli = Join-Path $root "src\LessMsi.Cli\bin\Release"

if (Test-Path $staging) { Remove-Item $staging -Recurse -Force }
New-Item -ItemType Directory -Force -Path $staging | Out-Null

# CLI and GUI share the same dependency set, so take everything from the CLI output and then
# overlay the GUI executable and its culture resources.
Copy-Item "$cli\*" $staging -Recurse -Force
Copy-Item "$gui\lessmsi-gui.exe" $staging -Force
Copy-Item "$gui\lessmsi-gui.exe.config" $staging -Force
if (Test-Path "$gui\*.dll") { Copy-Item "$gui\*.dll" $staging -Force }
if (Test-Path "$gui\Strings.*.resources.dll") { Copy-Item "$gui\Strings.*.resources.dll" $staging -Force }
foreach ($dir in Get-ChildItem $gui -Directory) {
    $target = Join-Path $staging $dir.Name
    if (Test-Path $target) {
        Copy-Item "$($dir.FullName)\*" $target -Recurse -Force
    } else {
        Copy-Item $dir.FullName $staging -Recurse -Force
    }
}

# wixcab.dll ships next to the sources but is not copied by the build.
Copy-Item (Join-Path $root "lib\wixcab.dll") $staging -Force

Remove-Item "$staging\*.pdb" -Force -ErrorAction SilentlyContinue
Remove-Item "$staging\*.vshost.exe*" -Force -ErrorAction SilentlyContinue

& $Iscc (Join-Path $root "installer\lessmsi.iss")
if ($LASTEXITCODE -ne 0) { throw "ISCC failed (exit $LASTEXITCODE)" }

Write-Host "Staged files:" -ForegroundColor Cyan
Get-ChildItem $staging -File | Measure-Object | ForEach-Object { "  $($_.Count) files in $staging" }
Get-ChildItem (Join-Path $root "dist") -Filter "lessmsi-*-setup.exe" | ForEach-Object { "  $($_.Name)  $($_.Length) bytes" }

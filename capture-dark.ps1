param(
    [string]$Msi = "C:\Users\Administrator\Downloads\Compressed\lessmsi-src\src\Lessmsi.Tests\TestFiles\MsiInput\python-2.7.3.msi",
    [string]$OutDir = "C:\Users\Administrator\Downloads\Compressed\lessmsi-src\dist\screenshots",
    [string]$ShotName = "dark-maximized.png"
)

$ErrorActionPreference = "Stop"
New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes

$signature = @'
using System;
using System.Runtime.InteropServices;
public class Shot2 {
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT { public int Left, Top, Right, Bottom; }
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    [DllImport("user32.dll")] public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, uint nFlags);
    [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}
'@
Add-Type -TypeDefinition $signature

function Save-WindowShot([IntPtr]$handle, [string]$path) {
    $rect = New-Object Shot2+RECT
    [void][Shot2]::GetWindowRect($handle, [ref]$rect)
    $bmp = New-Object System.Drawing.Bitmap(($rect.Right - $rect.Left), ($rect.Bottom - $rect.Top))
    $gfx = [System.Drawing.Graphics]::FromImage($bmp)
    $hdc = $gfx.GetHdc()
    $ok = [Shot2]::PrintWindow($handle, $hdc, 2)
    $gfx.ReleaseHdc($hdc); $gfx.Dispose()
    $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png); $bmp.Dispose()
    Write-Host ("saved {0} printWindow={1}" -f $path, $ok)
}

$exe = "C:\Users\Administrator\Downloads\Compressed\lessmsi-src\src\LessMsi.Gui\bin\Release\lessmsi-gui.exe"
$proc = Start-Process -FilePath $exe -ArgumentList "`"$Msi`"" -PassThru
Start-Sleep -Seconds 7
$proc.Refresh()
$h = $proc.MainWindowHandle

# Enumerate the Edit menu through UI Automation to prove the menu entry and its shortcut exist.
$root = [System.Windows.Automation.AutomationElement]::FromHandle($h)
$menuItems = $root.FindAll([System.Windows.Automation.TreeScope]::Descendants,
    (New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
        [System.Windows.Automation.ControlType]::MenuItem)))
Write-Host "menu items found: $($menuItems.Count)"
foreach ($item in $menuItems) {
    $name = $item.Current.Name
    $keys = ''
    try { $keys = $item.Current.AcceleratorKey } catch { }
    if ($name) { Write-Host ("  - {0}  [{1}]" -f $name, $keys) }
}

[void][Shot2]::ShowWindow($h, 3)
Start-Sleep -Seconds 2
Save-WindowShot $h (Join-Path $OutDir $ShotName)

$proc.CloseMainWindow() | Out-Null
Start-Sleep -Seconds 2
if (-not $proc.HasExited) { $proc.Kill() }

# Captures every tab of the GUI by posting mouse clicks straight to the native tab control
# (all other automation routes are unavailable for WinForms tabs).
$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Drawing

$sig = @'
using System;
using System.Text;
using System.Runtime.InteropServices;
public class TabClick {
    [StructLayout(LayoutKind.Sequential)] public struct RECT { public int Left, Top, Right, Bottom; }
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr h, out RECT r);
    [DllImport("user32.dll")] public static extern bool PrintWindow(IntPtr h, IntPtr d, uint f);
    [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr h, int c);
    [DllImport("user32.dll")] public static extern bool EnumChildWindows(IntPtr p, EnumProc cb, IntPtr l);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern int GetClassNameW(IntPtr h, StringBuilder sb, int max);
    [DllImport("user32.dll")] public static extern int SendMessage(IntPtr h, int msg, IntPtr w, IntPtr l);
    public delegate bool EnumProc(IntPtr h, IntPtr l);

    public static IntPtr FindTab(IntPtr parent) {
        IntPtr found = IntPtr.Zero;
        EnumChildWindows(parent, delegate(IntPtr h, IntPtr l) {
            var sb = new StringBuilder(256); GetClassNameW(h, sb, 256);
            if (sb.ToString().IndexOf("SysTabControl32", StringComparison.OrdinalIgnoreCase) >= 0) { found = h; return false; }
            return true; }, IntPtr.Zero);
        return found;
    }

    public static void ClickAt(IntPtr hwnd, int x, int y) {
        IntPtr lParam = (IntPtr)((y << 16) | (x & 0xFFFF));
        SendMessage(hwnd, 0x0201, (IntPtr)1, lParam);   // WM_LBUTTONDOWN
        SendMessage(hwnd, 0x0202, IntPtr.Zero, lParam); // WM_LBUTTONUP
    }
}
'@
Add-Type -TypeDefinition $sig

function Save-Shot([IntPtr]$handle, [string]$path) {
    $r = New-Object TabClick+RECT
    [void][TabClick]::GetWindowRect($handle, [ref]$r)
    $bmp = New-Object System.Drawing.Bitmap(($r.Right - $r.Left), ($r.Bottom - $r.Top))
    $gfx = [System.Drawing.Graphics]::FromImage($bmp)
    $hdc = $gfx.GetHdc()
    [void][TabClick]::PrintWindow($handle, $hdc, 2)
    $gfx.ReleaseHdc($hdc); $gfx.Dispose()
    $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png); $bmp.Dispose()
}

$exe = "C:\Users\Administrator\Downloads\Compressed\lessmsi-src\src\LessMsi.Gui\bin\Release\lessmsi-gui.exe"
$msi = "C:\Users\Administrator\Downloads\Compressed\lessmsi-src\src\Lessmsi.Tests\TestFiles\MsiInput\python-2.7.3.msi"
$out = "C:\Users\Administrator\Downloads\Compressed\lessmsi-src\dist\screenshots"

$proc = Start-Process -FilePath $exe -ArgumentList "`"$msi`"" -PassThru
Start-Sleep -Seconds 8
$proc.Refresh()
$main = $proc.MainWindowHandle
[void][TabClick]::ShowWindow($main, 3)
Start-Sleep -Seconds 2

$tab = [TabClick]::FindTab($main)
if ($tab -eq [IntPtr]::Zero) { throw "tab control not found" }

$positions = @(250, 380, 430, 480)
$labels = @("ExtractFiles", "TableView", "Summary", "Streams")
for ($i = 0; $i -lt $positions.Count; $i++) {
    [TabClick]::ClickAt($tab, $positions[$i], 12)
    Start-Sleep -Milliseconds 1500
    $path = Join-Path $out ("tab-" + $labels[$i] + ".png")
    Save-Shot $main $path
    Write-Host ("saved {0} hash={1}" -f (Split-Path $path -Leaf), (Get-FileHash $path -Algorithm MD5).Hash)
}

$proc.CloseMainWindow() | Out-Null
Start-Sleep -Seconds 2
if (-not $proc.HasExited) { $proc.Kill() }


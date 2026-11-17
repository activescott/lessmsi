# Verifies the dark mode menu entry: expands Edit, invokes "Dark mode", then checks the saved setting.
$ErrorActionPreference = "Stop"
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes

$exe = "${env:ProgramFiles(x86)}\LessMSI\lessmsi-gui.exe"
$msi = "C:\Users\Administrator\Downloads\Compressed\lessmsi-src\src\Lessmsi.Tests\TestFiles\MsiInput\python-2.7.3.msi"

$proc = Start-Process -FilePath $exe -ArgumentList "`"$msi`"" -PassThru
Start-Sleep -Seconds 7
$proc.Refresh()
$h = $proc.MainWindowHandle

$cond = { param($name) New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, $name) }
$typeCond = New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::MenuItem)

$form = [System.Windows.Automation.AutomationElement]::FromHandle($h)
$edit = $form.FindFirst([System.Windows.Automation.TreeScope]::Descendants, (& $cond "Edit"))
if (-not $edit) { throw "Edit menu not found" }

$expand = $edit.GetCurrentPattern([System.Windows.Automation.ExpandCollapsePattern]::Pattern)
$expand.Expand()
Start-Sleep -Milliseconds 900

$dark = [System.Windows.Automation.AutomationElement]::RootElement.FindFirst(
    [System.Windows.Automation.TreeScope]::Descendants,
    (New-Object System.Windows.Automation.AndCondition($typeCond, (& $cond "Dark mode"))))
if (-not $dark) {
    # Print every visible menu item so a naming mismatch is obvious.
    $all = [System.Windows.Automation.AutomationElement]::RootElement.FindAll([System.Windows.Automation.TreeScope]::Descendants, $typeCond)
    Write-Host "visible menu items:"
    foreach ($i in $all) { if ($i.Current.Name) { Write-Host ("  " + $i.Current.Name) } }
    throw "'Dark mode' menu item not found"
}
Write-Host ("found menu item: '{0}' accelerator='{1}'" -f $dark.Current.Name, $dark.Current.AcceleratorKey)

$invoke = $dark.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)
$invoke.Invoke()
Start-Sleep -Seconds 2

$proc.CloseMainWindow() | Out-Null
Start-Sleep -Seconds 3
if (-not $proc.HasExited) { $proc.Kill() }
Write-Host "gui closed"

$cfg = Get-ChildItem "$env:LOCALAPPDATA\LessMsi" -Recurse -Filter user.config | Where-Object { $_.FullName -like '*2.12.11*' } | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if (-not $cfg) { throw "no 2.12.11 user.config written" }
Write-Host ("settings: " + $cfg.FullName)
$line = Select-String -Path $cfg.FullName -Pattern 'DarkMode' -Context 0,1
if ($line) { Write-Host ("DarkMode persisted = " + $line.Context.PostContext[0].Trim()) } else { Write-Host "DarkMode NOT persisted" }

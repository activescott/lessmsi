
try {
	
	$theFile = Join-Path $(Split-Path -parent $MyInvocation.MyCommand.Definition) "AddWindowsExplorerShortcut.exe.ignore"
	#Write-Host "Creating " $theFile
	New-Item $theFile -type file

	$zipFile = Join-Path $(Split-Path -parent $MyInvocation.MyCommand.Definition) "__ZIP_FILE__"

	$toolsDir = (Split-Path -parent $MyInvocation.MyCommand.Definition)
	Get-ChocolateyUnzip -FileFullPath "$zipFile" -Destination $toolsDir

} catch {
	throw $_.Exception
}

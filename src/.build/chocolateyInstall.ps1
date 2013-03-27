
try {
	
	$theFile = Join-Path $(Split-Path -parent $MyInvocation.MyCommand.Definition) "AddWindowsExplorerShortcut.exe.ignore"
	Write-Host "Creating " $theFile
	New-Item $theFile -type file

	Install-ChocolateyZipPackage 'LessMsi' '__URL__' "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

	Write-ChocolateySuccess 'lessmsi'

} catch {
	Write-ChocolateyFailure 'lessmsi' $($_.Exception.Message)
	throw 
}

@SET THISPATH=%~dp0
@SET THISFILENAME=%~n0

@REM: Builds a simple setup from the WIX script (.wsx). See following references for more info:
@REM: http://www.codeproject.com/Tips/105638/A-quick-introduction-Create-an-MSI-installer-with
@REM: http://wix.tramontana.co.hu/tutorial/getting-started/the-software-package
@REM: http://wixtoolset.org/documentation/manual/v3/
@REM: http://wixtoolset.org/documentation/manual/v3/overview/light.html


@SET OUTPUT_MSI=msi_with_external_cab.msi
@SET OUTPUT_CAB=msi_with_external_cab.cab

c:\tools\wix\candle.exe .\create_msi_with_external_cab.wxs

c:\tools\wix\light.exe -out msi_with_external_cab.msi create_msi_with_external_cab.wixobj

move %OUTPUT_MSI% .\MsiInput\
move %OUTPUT_CAB% .\MsiInput\

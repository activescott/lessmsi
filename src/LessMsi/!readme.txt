**************************************************
This project is hosted at http://code.google.com/p/lessmsi/ - You should report bugs there and get the original code from there.
If you're using this I'd love to know about it. Drop me a line via http://code.google.com/p/lessmsi/
All files in this project are licensed uder the MIT License unless otherwise noted.
Special thanks to Ben Motmans for his QuickSort routine in the Anculus project http://code.google.com/p/libanculus-sharp/ 
Why the hell isn't there a _public_ QuickSort implementation in the BCL? Surely there is one, just not one I can find to use.
-scott willeke
**************************************************
TODO:
	- fix problem with Sliksvn: The problem is that it's extracting Windows directories and other directories under the main install dir. I can't figure out how to detect other roots other than the main install directory. I think MSI must replace some of those Directory entry names as "system defined properties" before the MSI actually begins its install. I can't find anything documented about it though...
		- example: 
			File: msvcr90.dll, SourceDir\SlikSvn\bin\Windows\winsxs\amd64_microsoft.vc90.crt_1fc8b3b9a1e18e3b_9.0.30729.4148_none_08e3747fa83e48bc, 624448, 9.0.30729.4148
			DirectryTable Entry: WinSxsDirectory.30729.4148.Microsoft_VC90_CRT_x64.QFE.DD7E30AD_4555_3131_8F48_1849E9DBC229, WindowsFolder.30729.4148.Microsoft_VC90_CRT_x64.QFE.DD7E30AD_4555_3131_8F48_1849E9DBC229, winsxs
		- Is it save to look for Entries begining with WinSxsDirectory or DefaultDir with winsxs?
		- K, this seems to be explained at: http://msdn.microsoft.com/en-us/library/aa369292%28VS.85%29.aspx & http://msdn.microsoft.com/en-us/library/aa369532%28VS.85%29.aspx
			Note the MsiAssemblyName table (which is not automatically shown by lessmsi!
	- More Unit tests with common/interesting MSI files.
	- MSI Patch Support
	- Make it work on MAC/Linux.
		- Remove dependency on Wix (big need here is a managed CAB library)
		- Remove dependency on Win32. 
	- Figure out why Directory nesting is screwed up. For example, in TortoisSVN MSIs the Windows directory and Common Files directory conents get nested under the main install dir. Why??
		- I believe this is realted to hte problem with Sliksvn as noted above.


Done:
	- Vista Support (just stopped the crash when running under LUA)
	- Fixed problem with Extract Button not working on current .NET Frameworks.
	- Proper Vista Support
		- Create a separate exe to do the registry tricks.
		- Embed a proper manifest (use the following in post build steps): mt.exe -manifest "$(ProjectDir)$(TargetName).exe.manifest" 
-updateresource:"$(TargetDir)$(TargetName).exe;#1"
	- Allow copy/paste of rows in the table view & file view tabs of GUI.
	- Make columns of File List sortable.
	- Make columns of table viewer sortable.
	- Make columns of MSI property/summary viewer sortable.
	- UI is coupled to MSI model. Would be nice to fix this up 
		- Well it is started anyway...
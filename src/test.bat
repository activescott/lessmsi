@setlocal
@set CONFIG=
@set /P CONFIG= Enter the config to test with (Release or Debug, defaults to Release): 
@if NOT DEFINED CONFIG set CONFIG=Release
echo Config: %CONFIG%
.\packages\NUnit.ConsoleRunner.3.2.1\tools\nunit3-console.exe .\Lessmsi.Tests\bin\%CONFIG%\LessMsi.Tests.dll /labels=All

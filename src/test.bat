
@set /P CONFIG= Enter the config to test with (Release or Debug, defaults to Release): 
@if NOT DEFINED CONFIG set CONFIG=Release

.\packages\NUnit.Runners.2.6.3\tools\nunit-console.exe .\Lessmsi.Tests\bin\%CONFIG%\LessMsi.Tests.dll /labels
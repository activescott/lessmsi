@setlocal
@set CONFIG=
@set /P CONFIG= Enter the config to test with (Release or Debug, defaults to Release): 
@if NOT DEFINED CONFIG set CONFIG=Release
echo Config: %CONFIG%

@set PAUSEFORDEBUG=
REM @set PAUSEFORDEBUG=--debug
packages\xunit.runner.console.2.1.0\tools\xunit.console.x86.exe .\Lessmsi.Tests\bin\%CONFIG%\LessMsi.Tests.dll %PAUSEFORDEBUG%

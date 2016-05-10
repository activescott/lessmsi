@setlocal
@set CONFIG=
@set TESTSTORUN=
@set /P CONFIG= Enter the config to test with (Release or Debug, defaults to Release): 
@set /P TESTSTORUN= Enter test names to run (defaults to All): 
@if NOT DEFINED CONFIG set CONFIG=Release
echo Config: %CONFIG%

if NOT "%TESTSTORUN%"=="" set TESTSTORUN=--test=%TESTSTORUN%
@set PAUSEFORDEBUG=
@set PAUSEFORDEBUG=--debug
.\packages\NUnit.ConsoleRunner.3.2.1\tools\nunit3-console.exe .\Lessmsi.Tests\bin\%CONFIG%\LessMsi.Tests.dll --labels=All %PAUSEFORDEBUG% %TESTSTORUN%

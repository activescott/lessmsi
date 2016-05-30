@setlocal 
@set /P TEMPVER= Enter the version number to build for:

msbuild.exe .\.build\lessmsi.msbuild /p:TheVersion=%TEMPVER% /fileLogger

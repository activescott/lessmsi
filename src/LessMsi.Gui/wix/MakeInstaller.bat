@Echo off
candle src\LessMsi.Gui\wix\Installer.wxs
light Installer.wixobj -ext WixUIExtension -out ..\..\..\bin\LesMSI_1.11.%1%.msi
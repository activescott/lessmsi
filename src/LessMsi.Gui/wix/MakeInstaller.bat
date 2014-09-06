@Echo off
candle Installer.wxs
light Installer.wixobj -ext WixUIExtension -out ..\..\..\bin\LesMSI_1.11.%1%.msi
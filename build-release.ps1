# Builds lessmsi (Release/x86) with the classic .NET Framework MSBuild plus a modern
# Roslyn compiler (the projects use C# 9 syntax that MSBuild 4.8's bundled csc rejects).
param(
    [string]$RoslynToolPath = "C:\Users\Administrator\Downloads\Compressed\build-tools\roslyn\tasks\net472",
    [string]$SdkToolsPath   = "C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8.1 Tools"
)

$ErrorActionPreference = "Stop"
$msbuild = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"
$srcDir  = Join-Path $PSScriptRoot "src"

$projects = @(
    "ExplorerShortcutHelper\ExplorerShortcutHelper.csproj",
    "LessMsi.Core\LessMsi.Core.csproj",
    "LessMsi.Cli\LessMsi.Cli.csproj",
    "LessMsi.Gui\LessMsi.Gui.csproj"
)

# The solution-level build does not resolve the net48 reference assemblies, so build project by project.
foreach ($proj in $projects) {
    $path = Join-Path $srcDir $proj
    Write-Host "=== Building $proj ===" -ForegroundColor Cyan
    & $msbuild $path /p:Configuration=Release /p:Platform=x86 `
        "/p:SolutionDir=$srcDir\" `
        "/p:CscToolPath=$RoslynToolPath" /p:CscToolExe=csc.exe /p:UseSharedCompilation=false `
        "/p:SdkToolsPath=$SdkToolsPath" "/p:TargetFrameworkSDKToolsDirectory=$SdkToolsPath" `
        /v:minimal /nologo /m
    if ($LASTEXITCODE -ne 0) { throw "Build failed for $proj (exit $LASTEXITCODE)" }
}

Write-Host "Build succeeded." -ForegroundColor Green

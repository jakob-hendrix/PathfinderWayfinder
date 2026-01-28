@echo off

SET RELEASE=Debug

@REM This should match what is set in the Wayfinder.App.csprog file
@REM     <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">
@REM       $(TargetFrameworks);net10.0-windows10.0.19041.0
@REM     </TargetFrameworks>

SET TARGET_FRAMEWORK=net10.0-windows10.0.19041.0

ECHO Cleaning old builds...
IF EXIST "bin\%RELEASE%\%TARGET_FRAMEWORK%" RD /s /q "bin\%RELEASE%\%TARGET_FRAMEWORK%"

ECHO Publishing Unpackaged Self-Contained App...
dotnet publish -f %TARGET_FRAMEWORK% -c %RELEASE% -p:WindowsPackageType=None -p:RuntimeIdentifierOverride=win10-x64 --self-contained true

ECHO Done! Your "No-Install" app is in:
ECHO bin\%RELEASE%\%TARGET_FRAMEWORK%\win-x64\publish
PAUSE


@echo off

REM Set nuget package versions project

SET NUGET_ROOT_VERSION=1.0.0
SET NUGET_IS_PREVIEW=preview
SET NUGET_BUILD_NUMBER=004

SET NUGET_NAME=nanoFramework.Companion.Drivers.Memory.%NUGET_ROOT_VERSION%-%NUGET_IS_PREVIEW%%NUGET_BUILD_NUMBER%

echo ********************************************************
echo Building %NUGET_NAME%
echo ********************************************************

REM Delete any prior package
if exist %NUGET_NAME%.nupkg (
	echo "Deleting older package...before creating a new one"
	del %NUGET_NAME%.nupkg
)

Nuget pack %NUGET_NAME%.nuspec

echo Moving package to local Nuget repository

REM Delete any prior package

if exist ../../nf-companion-local-private-nuget/%NUGET_NAME%.nupkg (
	del ..\\..\\nf-companion-local-private-nuget\\%NUGET_NAME%.nupkg
) 
move %NUGET_NAME%.nupkg ../../nf-companion-local-private-nuget 

pause
@echo off
set name=KsWare.F35FEB77-A8AA-4634-AA21-012EB75BA3C1
notepad %name%.nuspec

echo create folder structure..
mkdir content>nul
mkdir lib>nul
mkdir lib\net40>nul
mkdir lib\net45>nul

echo copy files...
set options=/Y /D
xcopy ..\KsWare.Presentation\bin\Debug\KsWare.JsonFx.dll lib\net40\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\KsWare.Presentation\bin\Debug\KsWare.Presentation.dll lib\net40\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\KsWare.Presentation\bin\Debug\KsWare.Presentation.xml lib\net40\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\KsWare.Presentation\bin\Debug_45\KsWare.JsonFx.dll lib\net45\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\KsWare.Presentation\bin\Debug_45\KsWare.Presentation.dll lib\net45\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\KsWare.Presentation\bin\Debug_45\KsWare.Presentation.xml lib\net45\ %options%
if errorlevel 1 set hasCopyError=true
if '%hasCopyError%' equ 'true' (
	echo ERROR
	goto PAUSEEXIT
)

echo pack...
nuget pack %name%.nuspec
if %ERRORLEVEL% geq 1 (
	echo ERROR %ERRORLEVEL%
	goto PAUSEEXIT
)
echo.
echo Provide the package? 
pause>nul
echo provide...
nuget setApiKey 5dbc1fca-4a7b-4916-80cc-8d9d44626456
if %ERRORLEVEL% geq 1 (
	echo ERROR %ERRORLEVEL%
	goto PAUSEEXIT
)
FOR %%f IN (*.nupkg) DO (
	::echo %%f
	nuget push %%f
	if %ERRORLEVEL% geq 1 (
		echo ERROR %ERRORLEVEL%
		goto PAUSEEXIT
	)
	move %%f old\
)

:PAUSEEXIT
pause
:EXIT
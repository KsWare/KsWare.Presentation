@echo off
SETLOCAL
::set name=KsWare.F35FEB77-A8AA-4634-AA21-012EB75BA3C1
::set name=KsWare.F35FEB78
set name=KsWare.Presentation
set Devenv="C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe"

:checkNuGet
if not exist nuget.exe (
	echo ERROR NuGet Command-Line Utility nuget.exe not found!
	echo Please copy the file in the current folder.
	echo starting download...
	start /wait http://dist.nuget.org/win-x86-commandline/latest/nuget.exe
	pause
	if not exist nuget.exe (
		echo a general download page can be found here:
		echo https://dist.nuget.org/index.html
		start /wait https://dist.nuget.org/index.html
		pause
	)
	goto checkNuGet
)

CHOICE /C YN /T 5 /D Y /M "Compile? "
if %ERRORLEVEL% equ 2 goto compile.end
:compile.start
echo Building Release...
%Devenv% "..\src\KsWare.Presentation.Solution\KsWare.Presentation.sln" /build Release
if %ERRORLEVEL% neq 0 (
	echo exitcode: %ERRORLEVEL%
	goto PAUSEEXIT
)
echo Building Release_40...
%Devenv% "..\src\KsWare.Presentation.Solution\KsWare.Presentation.sln" /build Release_40
if %ERRORLEVEL% neq 0 (
	echo exitcode: %ERRORLEVEL%
	goto PAUSEEXIT
)
pause
:compile.end

echo clean folder structure..
del lib\net40\*.* /Q>nul
del lib\net45\*.* /Q>nul

echo create folder structure..
mkdir content>nul
mkdir lib>nul
mkdir lib\net40>nul
mkdir lib\net45>nul
mkdir archive>nul


echo copy files...
set options=/Y /D

xcopy ..\src\KsWare.Presentation.Solution\History.txt lib\ %options%
if errorlevel 1 set hasCopyError=true

::net40

::xcopy ..\src\KsWare.Presentation\bin\Release\KsWare.JsonFx.dll lib\net40\ %options%
::if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.Core\bin\Release_40\KsWare.Presentation.Core.dll lib\net40\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.Core\bin\Release_40\KsWare.Presentation.Core.xml lib\net40\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.BusinessFramework\bin\Release\KsWare.Presentation.BusinessFramework.dll lib\net40\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.BusinessFramework\bin\Release\KsWare.Presentation.BusinessFramework.xml lib\net40\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.ViewModelFramework\bin\Release\KsWare.Presentation.ViewModelFramework.dll lib\net40\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.ViewModelFramework\bin\Release\KsWare.Presentation.ViewModelFramework.xml lib\net40\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.ViewFramework\bin\Release\KsWare.Presentation.ViewFramework.dll lib\net40\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.ViewFramework\bin\Release\KsWare.Presentation.ViewFramework.xml lib\net40\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.Labs\bin\Release\KsWare.Presentation.Labs.dll lib\net40\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.Labs\bin\Release\KsWare.Presentation.Labs.xml lib\net40\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.Labs\bin\Release\KsWare.Presentation.Compatibility40.dll lib\net40\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.Labs\bin\Release\KsWare.Presentation.Compatibility40.xml lib\net40\ %options%
if errorlevel 1 set hasCopyError=true

:: net45
::xcopy ..\src\KsWare.Presentation\bin\Release\KsWare.JsonFx.dll lib\net45\ %options%
::if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.Core\bin\Release\KsWare.Presentation.Core.dll lib\net45\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.Core\bin\Release\KsWare.Presentation.Core.xml lib\net45\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.BusinessFramework\bin\Release\KsWare.Presentation.BusinessFramework.dll lib\net45\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.BusinessFramework\bin\Release\KsWare.Presentation.BusinessFramework.xml lib\net45\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.ViewModelFramework\bin\Release\KsWare.Presentation.ViewModelFramework.dll lib\net45\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.ViewModelFramework\bin\Release\KsWare.Presentation.ViewModelFramework.xml lib\net45\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.ViewFramework\bin\Release\KsWare.Presentation.ViewFramework.dll lib\net45\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.ViewFramework\bin\Release\KsWare.Presentation.ViewFramework.xml lib\net45\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.Labs\bin\Release\KsWare.Presentation.Labs.dll lib\net45\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.Labs\bin\Release\KsWare.Presentation.Labs.xml lib\net45\ %options%
if errorlevel 1 set hasCopyError=true
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.Labs\bin\Release\KsWare.Presentation.Compatibility40.dll lib\net45\ %options%
if errorlevel 1 set hasCopyError=true
xcopy ..\src\KsWare.Presentation.Labs\bin\Release\KsWare.Presentation.Compatibility40.xml lib\net45\ %options%
if errorlevel 1 set hasCopyError=true


if '%hasCopyError%' equ 'true' (
	echo ERROR
	goto PAUSEEXIT
)

echo Edit nuspec. Set correct version and save the file.
notepad %name%.nuspec
::pause 

echo pack...
nuget pack %name%.nuspec
if %ERRORLEVEL% geq 1 (
	echo ERROR %ERRORLEVEL%
	goto PAUSEEXIT
)

:provide
if "%USERNAME%" -ne "KayS" (
	echo WARNING: publish nuget only at owners machine!
	echo It is NOT ALLOWED to publish own builds with "KsWare." in the name of nuget package.
	echo publishing skipped...
	goto PAUSEEXIT
)
echo.
echo Provide the package? 
pause>nul
:: C:\Users\%USERNAME%\AppData\Local
if exist %LOCALAPPDATA%\.nuget\SetApiKey.cmd (
	call %LOCALAPPDATA%\.nuget\SetApiKey.cmd
) 
if "%KsWare_NuGet_API_Key%" -eq "" (
	echo ERROR API Key not configured.
	echo notepad.exe %LOCALAPPDATA%\.nuget\SetApiKey.cmd
	goto provide
)
echo provide...
nuget setApiKey %KsWare_NuGet_API_Key%
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
	move %%f archive\
)

:PAUSEEXIT
pause
:EXIT
ENDLOCAL
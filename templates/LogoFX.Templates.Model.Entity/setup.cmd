REM Modify the code

dotnet .\utils\ModifyTool.dll .\LogoFX.Templates.Model --entity SampleModel

if %ERRORLEVEL% NEQ 0 ( 
	exit /b %ERRORLEVEL%
)

REM Delete utils

rmdir utils /s /q

REM Delete setup.cmd

start /b "" cmd /c del "%~f0"&exit /b
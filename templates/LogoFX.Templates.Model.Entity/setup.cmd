REM Modify the code

dotnet .\entity-utils\ModifyTool.dll .\LogoFX.Templates --model SampleModel

if %ERRORLEVEL% NEQ 0 ( 
	exit /b %ERRORLEVEL%
)

REM Delete utils

rmdir model-utils /s /q

if %ERRORLEVEL% NEQ 0 ( 
	exit /b %ERRORLEVEL%
)

REM Delete setup.cmd

start /b "" cmd /c del "%~f0"&exit /b
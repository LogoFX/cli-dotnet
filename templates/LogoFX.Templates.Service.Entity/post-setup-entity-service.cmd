REM Launch dotnet new

dotnet new logofx-entity -n SampleModel -sn LogoFX.Templates --allow-scripts yes

if %ERRORLEVEL% NEQ 0 ( 
	exit /b %ERRORLEVEL%
)

dotnet .\service-utils\ModifyTool.dll .\LogoFX.Templates --service SampleModel

if %ERRORLEVEL% NEQ 0 ( 
	exit /b %ERRORLEVEL%
)

REM Delete utils

rmdir service-utils /s /q

if %ERRORLEVEL% NEQ 0 ( 
	exit /b %ERRORLEVEL%
)

REM Delete setup.cmd

start /b "" cmd /c del "*.cmd"&exit /b
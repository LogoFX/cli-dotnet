REM TODO: read from csproj or set during pack process
set package_name=LogoFX.Templates
REM TODO: read from csproj or set during pack process
set package_version=0.2.0-rc3

call pack-templates.cmd

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

call install-package.cmd %package_name% %package_version%

cd ..

if exist output (
	echo Removing output
	rmdir output /s /q
)

if %ERRORLEVEL% NEQ 0 (
	echo Error removing output
	goto EXIT
)

if exist bin (
	echo Removing bin
	rmdir bin /s /q
)

if %ERRORLEVEL% NEQ 0 (
	echo Error removing bin
	goto EXIT
)

if exist obj (
	echo Removing obj
	rmdir obj /s /q
)

if %ERRORLEVEL% NEQ 0 (
	echo Error removing obj
	goto EXIT
)

cd devops

:EXIT
EXIT /B %ERRORLEVEL%
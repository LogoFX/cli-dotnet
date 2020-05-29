REM install-template-pack.cmd 

set path=%path%;%cd%

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

if exist generated (
	call remove-folder generated
)

if %ERRORLEVEL% NEQ 0 (
	goto EXIT
)

if exist output (
	call remove-folder output
)

if %ERRORLEVEL% NEQ 0 (
	goto EXIT
)

if exist bin (
	call remove-folder bin
)

if %ERRORLEVEL% NEQ 0 (
	goto EXIT
)

if exist obj (
	call remove-folder obj
)

if %ERRORLEVEL% NEQ 0 (
	goto EXIT
)

cd devops

:EXIT
REM /install-template-pack.cmd 
EXIT /B %ERRORLEVEL%
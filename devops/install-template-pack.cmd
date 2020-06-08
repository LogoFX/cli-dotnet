REM install-template-pack.cmd 

set path=%path%;%cd%

utils\GetProjValue ..\templatepack.csproj PackageId > tmp.txt
set /P package_name=<tmp.txt
utils\GetProjValue ..\templatepack.csproj PackageVersion > tmp.txt
set /P package_version=<tmp.txt
del tmp.txt

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

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
	rem call remove-folder output
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
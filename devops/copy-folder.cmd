REM copy-folder.cmd

REM %1 - source dir in 'templates' folder; %2 - additional parameters for xcopy

set templateDir=%1
set sourceDir=templates
set targetDir=generated

REM Prepare 'Generated' folder

cd ..
if not exist %targetDir% (
	md %targetDir%
)

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

cd %targetDir%

if exist %templateDir% (
	call remove-folder %templateDir%
)

if %ERRORLEVEL% NEQ 0 (
	goto EXIT
)

xcopy /e /i /y /h ..\%sourceDir%\%templateDir% .\%templateDir% %2

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

cd ..\devops

:EXIT
REM /copy-folder.cmd
exit /b %ERRORLEVEL%
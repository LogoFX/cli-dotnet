REM LogoFX.Templates.Model.Entity
set templateDir=%1
set sourceDir=templates
set targetDir=generated

cd ..\%targetDir%
if exist %templateDir% (
	echo Removing %templateDir%
	rmdir %templateDir%
)

if %ERRORLEVEL% NEQ 0 (
	echo Error removing %templateDir%
	goto EXIT
)

xcopy /e /i /y /h ..\%sourceDir%\%templateDir% .\%templateDir% /exclude:..\devops\excludefiles.txt+..\devops\excludeprojects.txt 

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

cd ..\devops

call generate-utils.cmd %1

:EXIT
exit /b %ERRORLEVEL%
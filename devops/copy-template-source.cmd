REM LogoFX.Templates.Model.Entity
set templateDir=%1
set sourceDir=templates
set targetDir=generated

cd ..\%targetDir%
if exist %templateDir% (
	rmdir %templateDir%
)

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

xcopy /e /i /y ..\%sourceDir%\%templateDir% .\%templateDir% /exclude:..\devops\excludefiles.txt+..\devops\excludeprojects.txt 

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

cd ..\devops

:EXIT
exit /b %ERRORLEVEL%
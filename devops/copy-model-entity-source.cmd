set templatesDir=templates
set sourceDir=source
set targetDir=LogoFX.Templates.Model.Entity

cd ..
if not exist %templatesDir% (
	md %templatesDir%
)
cd %templatesDir%

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

if exist %targetDir% (
	rmdir %targetDir% /s /q
)
md %targetDir%

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

xcopy /e /i /y ..\%sourceDir%\%targetDir% .\%targetDir% /exclude:..\devops\excludefiles.txt+..\devops\excludeprojects.txt 

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

cd ..\devops

:EXIT
exit /b %ERRORLEVEL%
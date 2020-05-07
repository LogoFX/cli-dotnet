call copy-template.cmd LogoFX.Templates.WPF --use-common

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

call copy-template.cmd LogoFX.Templates.Model.Entity

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

cd ..\
dotnet pack

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

cd devops

:EXIT
EXIT /B %ERRORLEVEL%
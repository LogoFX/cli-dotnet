call copy-template.cmd LogoFX.Templates.WPF --use-common

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

call copy-template-source.cmd LogoFX.Templates.Model.Entity

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

cd ..
dotnet pack -o output

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

cd devops

:EXIT
EXIT /B %ERRORLEVEL%
REM pack-templates.cmd 

call copy-template-logofx-wpf.cmd

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

call copy-template-logofx-model.cmd

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

call copy-template-logofx-service.cmd

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
REM /pack-templates.cmd 
EXIT /B %ERRORLEVEL%
REM Publish UninstallTemplate to \devops\utils folder

set utils=utils

if exist %utils% (
	call remove-folder %utils%
)

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

cd ..\utils\UninstallTemplate

dotnet publish -c release -o ..\..\devops\%utils%

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

cd ..\..\devops

:EXIT
EXIT /B %ERRORLEVEL%
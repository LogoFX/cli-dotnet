REM generate-utils.cmd 

set generated=generated
set utils=utils

REM Prepare 'Generated' folder

cd ..
if not exist %generated% (
	md %generated%
)

cd %generated%

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

if exist %utils% (
	call remove-folder %utils%
)

if %ERRORLEVEL% NEQ 0 (
	goto EXIT
)

cd ..\utils

cd ModifyTool
dotnet publish -c release -o ..\..\%generated%\%1\%utils%

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

cd ..\..\devops

:EXIT
REM /generate-utils.cmd 
EXIT /B %ERRORLEVEL%
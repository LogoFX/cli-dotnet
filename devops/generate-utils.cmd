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
	echo Removing %utils%
	rmdir %utils% /s /q
)

if %ERRORLEVEL% NEQ 0 (
	echo Error removing %utils%
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
EXIT /B %ERRORLEVEL%
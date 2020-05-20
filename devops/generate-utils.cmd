set temp=generated
set utils=utils

REM Prepare 'Generated' folder

cd ..
if not exist %temp% (
	md %temp%
)

cd %temp%

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
dotnet publish -c release -o ..\..\%temp%\%1\%utils%

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

cd ..\..\devops

:EXIT
EXIT /B %ERRORLEVEL%
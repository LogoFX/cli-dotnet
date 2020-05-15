set temp=generated
set utils=utils

cd ..\%temp%

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

if exist %utils% (
	rmdir %utils% /s /q
)

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

cd ..\utils

cd ModifyTool
dotnet publish -c release -o ..\..\%temp%\%utils%

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

cd ..\..\devops

:EXIT
EXIT /B %ERRORLEVEL%
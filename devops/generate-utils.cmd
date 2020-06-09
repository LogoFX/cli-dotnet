REM generate-utils.cmd 
REM %1 - template folder name
REM %2 - destination folder

set generated=generated
set utils=utils
set destination=%2

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
dotnet publish -c release -o ..\..\%generated%\%1\%destination%

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

call remove-folder ..\..\%generated%\%1\%destination%\runtimes

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

del ..\..\%generated%\%1\%destination%\ModifyTool.deps.json

cd ..\..\devops

:EXIT
REM /generate-utils.cmd 
EXIT /B %ERRORLEVEL%
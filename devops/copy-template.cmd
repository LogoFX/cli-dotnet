REM %1 - Template Solution Name
REM %2 - Flag to Use Common

set generated=generated
set common1=Common.Bootstrapping\Common.Bootstrapping.csproj
set common2=Common.Data.Fake.Setup\Common.Data.Fake.Setup.csproj

REM Prepare 'Generated' folder

cd ..
if not exist %generated% (
	md %generated%
)

cd %generated%

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

REM Copy solution folder

if exist %1 (
	rmdir %1 /s /q
)
if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

md %1
if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

xcopy /e /i /y /h ..\templates\%1 .\%1 /exclude:..\devops\exclude-common.txt

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

REM Copy 'Common' projects

if not "%2" == "--use-common" (
	GOTO EXIT
)

xcopy /e /i /y ..\common .\%1 /exclude:..\devops\exclude-common.txt

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

REM Remove 'Common' project references

cd %1\%1.Launcher
dotnet remove reference ..\..\..\common\%common1% ..\..\..\common\%common2%

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

cd ..
dotnet sln remove ..\..\common\%common1% ..\..\common\%common2%

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

REM Add 'Common' project references

dotnet sln add -s Common %common1% %common2%

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

cd %1.Launcher
dotnet add reference ..\%common1% ..\%common2%

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

cd ..\..\..\devops

:EXIT
EXIT /B %ERRORLEVEL%
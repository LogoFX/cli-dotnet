REM copy-template-logofx-wpf.cmd 

set source=LogoFX.Templates.WPF
set common1=Common.Bootstrapping\Common.Bootstrapping.csproj
set common2=Common.Data.Fake.Setup\Common.Data.Fake.Setup.csproj
set exclude=/exclude:..\devops\exclude-common.txt

call copy-folder %source% %exclude%

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

REM Copy 'Common' projects

cd ..\generated
xcopy /e /i /y ..\common .\%source% %exclude%

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

REM Remove 'Common' project references

cd %source%\%source%.Launcher

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

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

cd %source%.Launcher
dotnet add reference ..\%common1% ..\%common2%

if %ERRORLEVEL% NEQ 0 ( 
	goto EXIT
)

cd ..\..\..\devops

:EXIT
REM /copy-template-logofx-wpf.cmd 
EXIT /B %ERRORLEVEL%
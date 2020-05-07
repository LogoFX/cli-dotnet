REM %1 - Template Solution Name
REM %2 - Flag to Use Common

set temp=generated
set common1=Common.Bootstrapping\Common.Bootstrapping.csproj
set common2=Common.Data.Fake.Setup\Common.Data.Fake.Setup.csproj

REM Prepare 'Generated' folder

cd ..
if NOT EXIST %temp% (
	md %temp%
)

cd %temp%

REM Copy solution folder

if EXIST %1 (
	rmdir %1 /s /q
)
md %1
xcopy /e /i /y /h ..\templates\%1 .\%1 /exclude:..\devops\excludefiles.txt

if "%2" == "--use-common" (
REM Copy 'Common' projects

xcopy /e /i /y ..\common .\%1 /exclude:..\devops\excludefiles.txt

REM Remove 'Common' project references

cd %1\%1.Launcher
dotnet remove reference ..\..\..\common\%common1% ..\..\..\common\%common2%
cd ..
dotnet sln remove ..\..\common\%common1% ..\..\common\%common2%

REM Add 'Common' project references

dotnet sln add -s Common %common1% %common2%
cd %1.Launcher
dotnet add reference ..\%common1% ..\%common2%
) 

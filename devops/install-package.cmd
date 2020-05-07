REM %1 - Package name
REM %2 - Package version
REM TODO: read from csproj or set during pack process
set package_file_name=%1.%2.nupkg
echo %package_file_name%

REM Uninstall package
dotnet .\utils\UninstallTemplate.dll -d %1
cd ..\bin\Debug
dotnet new -i %package_file_name%

cd ..\..\devops

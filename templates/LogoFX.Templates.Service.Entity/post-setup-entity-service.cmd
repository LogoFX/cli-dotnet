REM Launch dotnet new

dotnet new logofx-model -n SampleModel -sn LogoFX.Templates --allow-scripts yes

REM Delete utils

rem rmdir utils /s /q

REM Delete setup.cmd

start /b "" cmd /c del "*.cmd"&exit /b
REM Modify the code

dotnet .\utils\ModifyTool.dll .\LogoFX.Templates.Model --entity SampleModel

REM Delete utils

rmdir utils /s /q

REM Delete setup.cmd

start /b "" cmd /c del "%~f0"&exit /b
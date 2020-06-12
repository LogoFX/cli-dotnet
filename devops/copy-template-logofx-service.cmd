REM copy-template-logofx-service.cmd 

call copy-folder LogoFX.Templates.Service /exclude:..\devops\exclude-common.txt+..\devops\exclude-for-service.txt

if %ERRORLEVEL% NEQ 0 (
	goto EXIT
)

call generate-utils.cmd LogoFX.Templates.Service service-utils

:EXIT
REM /copy-template-logofx-service.cmd 
exit /b %ERRORLEVEL%
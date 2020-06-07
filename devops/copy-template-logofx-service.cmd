REM copy-template-logofx-service.cmd 

call copy-folder LogoFX.Templates.Service.Entity /exclude:..\devops\exclude-common.txt+..\devops\exclude-for-service.txt

if %ERRORLEVEL% NEQ 0 (
	goto EXIT
)


:EXIT
REM /copy-template-logofx-service.cmd 
exit /b %ERRORLEVEL%
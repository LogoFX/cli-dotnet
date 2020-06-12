REM copy-template-logofx-model.cmd 

call copy-folder LogoFX.Templates.Entity /exclude:..\devops\exclude-common.txt+..\devops\exclude-for-model.txt

if %ERRORLEVEL% NEQ 0 (
	goto EXIT
)

call generate-utils.cmd LogoFX.Templates.Entity entity-utils

:EXIT
REM /copy-template-logofx-model.cmd 
exit /b %ERRORLEVEL%
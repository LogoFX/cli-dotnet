set package_name=LogoFX.Templates
REM TODO: read from csproj or set during pack process
set package_version=0.2.0-rc1

call pack-templates.cmd

cd .\devops

call install-package.cmd %package_name% %package_version%
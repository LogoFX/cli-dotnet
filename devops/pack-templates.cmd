call copy-template.cmd LogoFX.Templates.WPF --use-common
cd ..\..\..\devops
call copy-template.cmd LogoFX.Templates.Model.Entity

cd ..\
dotnet pack

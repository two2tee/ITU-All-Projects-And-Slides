@echo off
echo Welcome to LiftLog dependency setup script - Thor Olesen - Dennis T.T. Nguyen - All rights reserved 
echo
echo Press any keys to continue installation
pause
echo Checking if dot net core is installed...
dotnet --version

choice /c yn /m "Was a dot net version printed on your display?"
if %errorlevel% equ 2 GOTO errordotnet
GOTO checknpm

:errordotnet
echo Please install dotnet on your machine and run this setup again
PAUSE
GOTO END

:checknpm
choice /c yn /m "Do you have npmJS installed?"
if %errorlevel% equ 2 GOTO :errornpm
GOTO installAPIKeys


:errornpm
echo Please install npm on your machine and run this setup again
echo https://www.npmjs.com/
PAUSE
GOTO END


:installAPIKeys
echo --Building Project
dotnet clean
dotnet restore
echo --Project restored
cd Liftlog.Web
echo --Installing API-Keys
dotnet user-secrets set SendGridUser "LiftLog"
dotnet user-secrets set SendGridKey "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"
dotnet user-secrets set Facebook:AppSecret "790d4a44c5baa42129d5ee29d6fc07f0"
dotnet user-secrets set Facebook:AppId "1807178839603521"
echo --Installed API-Keys

echo DONE!
PAUSE


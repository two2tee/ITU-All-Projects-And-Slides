@echo off
echo LiftLog - Thor Olesen - Dennis T.T. Nguyen - All rights reserved 
echo
choice /c yn /m "Do you wish to start the liftLog?"
if %errorlevel% equ 2 GOTO END
dotnet run
pause

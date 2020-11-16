@echo off

set SOURCE=%2
set TARGET=%SOURCE%\ServiceInitial.cs
echo [PreBuild.ScanService] %TARGET%

:: Remove old file.
echo [PreBuild.ScanService] Clean.
del /f "%TARGET%"

:: Scan folders and generate the new file.
echo [PreBuild.ScanService] Generating service touch file.
echo // This file is automatic generated by script.            >> %TARGET%
echo // DO NOT EDIT DIRECTLY.                                  >> %TARGET%
echo.                                                          >> %TARGET%
echo using System;                                             >> %TARGET%
echo using Konata.Events;                                      >> %TARGET%
echo.                                                          >> %TARGET%
echo namespace Konata                                          >> %TARGET%
echo {                                                         >> %TARGET%
echo     public partial class ServiceMan : EventComponent      >> %TARGET%
echo     {                                                     >> %TARGET%
echo         private bool Initialize(EventPumper eventPumper)  >> %TARGET%
echo         {                                                 >> %TARGET%

:: Write classes to.
for /r %SOURCE%\Services\ %%f in (*.cs) do (
	call :write %%f
)

echo             return true;                                  >> %TARGET%
echo         }                                                 >> %TARGET%
echo     }                                                     >> %TARGET%
echo }                                                         >> %TARGET%

echo [PreBuild.ScanService] Success.
goto :eof

:write
set CLASS=%1
set CLASS=%CLASS:*Services\=%
set CLASS=%CLASS:.cs=%
set CLASS=%CLASS:\=.%
echo             RegisterRoutines(new Services.%CLASS%(eventPumper));     >> %TARGET%

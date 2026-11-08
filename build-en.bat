@echo off
chcp 65001 > nul
echo ====================================================================
echo        BUILD UNIPINYIN (ENGLISH EDITION) - WIN32 EXECUTABLE
echo ====================================================================
echo.

set CSC="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist %CSC% (
    set CSC="C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe"
)

if not exist %CSC% (
    echo [ERROR] Microsoft .NET Framework C# compiler (csc.exe) not found.
    pause
    exit /b 1
)

echo Compiling unipinyin-en.exe with panda application icon and sound chime...
%CSC% /target:winexe /optimize+ /platform:anycpu /win32icon:"%~dp0app.ico" /resource:"%~dp0toggle.wav",toggle.wav /r:System.dll /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Web.Extensions.dll /r:System.Core.dll /out:"%~dp0unipinyin-en.exe" "%~dp0src-en\*.cs"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ====================================================================
    echo [SUCCESS] Built executable: unipinyin-en.exe (~81 KB)
    echo ====================================================================
    echo.
    echo You can now launch unipinyin-en.exe directly!
    echo.
) else (
    echo.
    echo [FAILED] Compilation failed with errors.
)

pause

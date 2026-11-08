@echo off
chcp 65001 > nul
echo ====================================================================
echo        BIÊN DỊCH BỘ GÕ TIẾNG TRUNG UNIPINYIN CHO WINDOWS
echo ====================================================================
echo.

set CSC="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist %CSC% (
    set CSC="C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe"
)

if not exist %CSC% (
    echo [LỖI] Không tìm thấy trình biên dịch C# (csc.exe) của .NET Framework trên máy.
    pause
    exit /b 1
)

echo Đang tiến hành biên dịch UniPinyin.exe với biểu tượng chú gấu trúc và âm báo...
%CSC% /target:winexe /optimize+ /platform:anycpu /win32icon:"%~dp0app.ico" /resource:"%~dp0toggle.wav",toggle.wav /r:System.dll /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Web.Extensions.dll /r:System.Core.dll /out:"%~dp0UniPinyin.exe" "%~dp0*.cs"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ====================================================================
    echo [THÀNH CÔNG] Đã tạo file thực thi: UniPinyin.exe (Dung lượng siêu nhẹ ~80KB)
    echo ====================================================================
    echo.
    echo Bạn có thể khởi động ngay bằng cách nhấp đúp vào UniPinyin.exe!
    echo.
) else (
    echo.
    echo [THẤT BÀI] Có lỗi trong quá trình biên dịch.
)

pause

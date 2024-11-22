@echo off

net session >nul 2>nul

if %errorlevel% neq 0 (
PowerShell -Command "Start-Process cmd.exe -ArgumentList '/c %~f0' -Verb RunAs"
exit
)

mode 32,10
title NetInf

:upd

for /f "tokens=2* delims=:" %%i in ('netsh wlan show interface ^| find "SSID" ^| findstr /v "BSSID"') do set ssid=%%i
for /f "tokens=2* delims=:" %%i in ('netsh wlan show interface ^| find "State"') do set state=%%i

for /f "tokens=2* delims=:" %%i in ('netsh wlan show interface ^| find "Radio type"') do set type=%%i
for /f "tokens=2* delims=:" %%i in ('netsh wlan show interface ^| find "Band"') do set band=%%i

for /f "tokens=2* delims=:" %%i in ('netsh wlan show interface ^| find "Receive rate"') do set rec=%%i
for /f "tokens=2* delims=:" %%i in ('netsh wlan show interface ^| find "Transmit rate"') do set tra=%%i

for /f "tokens=2* delims=:" %%i in ('netsh wlan show interface ^| find "Signal"') do set signal=%%i

cls

echo SSID:          %ssid%
echo STATE:         %state%
echo TYPE:          %type%
echo BAND:          %band%
echo RECEIVE RATE:  %rec% (Mbps)
echo TRANSMIT RATE: %tra% (Mbps)
echo SIGNAL:        %signal%

goto upd
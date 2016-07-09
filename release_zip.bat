@echo off

set VERSION=%1

pushd GW2PAO\bin\x86\Release
del /Q Logs\*
del /Q UserData\*
del /Q debug.log
7z a ..\..\..\..\GW2PAO_%VERSION%.zip *
popd
pushd GW2PAO\bin\x86\Release_WithoutBrowser
del /Q Logs\*
del /Q UserData\*
del /Q debug.log
7z a ..\..\..\..\GW2PAO_%VERSION%_NoBrowser.zip *
popd
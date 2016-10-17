@echo off

REM --------------------- Release Cleanup ---------------------
pushd GW2PAO\bin\x86\Release
xcopy de\GW2PAO.resources.dll Locale\de\ /Y
xcopy es\GW2PAO.resources.dll Locale\es\ /Y
xcopy fr\GW2PAO.resources.dll Locale\fr\ /Y
xcopy ru\GW2PAO.resources.dll Locale\ru\ /Y
rd /S /Q en
rd /S /Q it
rd /S /Q ja
rd /S /Q ko
rd /S /Q ru
rd /S /Q zh-Hans
rd /S /Q zh-Hant
rd /S /Q de
rd /S /Q es
rd /S /Q fr
rd /S /Q hu
rd /S /Q ro
rd /S /Q sv
rd /S /Q pt-BR
del /Q *.xml
del /Q *.pdb
del /Q BitFactory.dll
del /Q GW2NET.*
del /Q GW2PAO.*.dll
del /Q GwApiNET.dll
del /Q Hardcodet.Wpf.TaskbarNotification.dll
del /Q Intellibox.dll
del /Q Microsoft.Expression.*
del /Q Microsoft.Practices.*
del /Q Newtonsoft.Json.dll
del /Q NHotkey.*
del /Q NLog.dll
del /Q OxyPlot.*
del /Q RestSharp.dll
del /Q SLF.*
del /Q System.Windows.Interactivity.dll
del /Q TS3QueryLib.Core.Framework.dll
del /Q Xceed.Wpf.*
del /Q FileDb*
del /Q ImageFileCache.WPF.*
del /Q MapControl.WPF.*
del /Q FontAwesome.Sharp.*
del /Q *.vshost.*
del /Q Logs/*
del /Q UserData/*
popd

xcopy AnetCopyright.txt GW2PAO\bin\x86\Release\ /Y
xcopy LICENSE.txt GW2PAO\bin\x86\Release\ /Y
xcopy ThirdPartyLicenses.txt GW2PAO\bin\x86\Release\ /Y
xcopy Tasks GW2PAO\bin\x86\Release\Tasks\ /Y /S /E

REM --------------------- Release_NoBrowser ---------------------
pushd GW2PAO\bin\x86\Release_WithoutBrowser
xcopy de\GW2PAO.resources.dll Locale\de\ /Y
xcopy es\GW2PAO.resources.dll Locale\es\ /Y
xcopy fr\GW2PAO.resources.dll Locale\fr\ /Y
xcopy ru\GW2PAO.resources.dll Locale\ru\ /Y
rd /S /Q de
rd /S /Q en
rd /S /Q es
rd /S /Q fr
rd /S /Q it
rd /S /Q ja
rd /S /Q ko
rd /S /Q ru
rd /S /Q zh-Hans
rd /S /Q zh-Hant
rd /S /Q hu
rd /S /Q ro
rd /S /Q sv
rd /S /Q pt-BR
del /Q *.xml
del /Q *.pdb
del /Q BitFactory.dll
del /Q GW2NET.*
del /Q GW2PAO.*.dll
del /Q GwApiNET.dll
del /Q Hardcodet.Wpf.TaskbarNotification.dll
del /Q Intellibox.dll
del /Q Microsoft.Expression.*
del /Q Microsoft.Practices.*
del /Q Newtonsoft.Json.dll
del /Q NHotkey.*
del /Q NLog.dll
del /Q OxyPlot.*
del /Q RestSharp.dll
del /Q SLF.*
del /Q System.Windows.Interactivity.dll
del /Q TS3QueryLib.Core.Framework.dll
del /Q Xceed.Wpf.*
del /Q avcodec-53.dll
del /Q avformat-53.dll
del /Q avutil-51.dll
del /Q Awesomium*
del /Q icudt.dll
del /Q inspector.pak
del /Q libEGL.dll
del /Q libGLESv2.dll
del /Q xinput9_1_0.dll
del /Q pdf_js.pak
del /Q FileDb*
del /Q ImageFileCache.WPF.*
del /Q MapControl.WPF.*
del /Q FontAwesome.Sharp.*
del /Q *.vshost.*
del /Q Logs/*
del /Q UserData/*
popd

xcopy AnetCopyright.txt GW2PAO\bin\x86\Release_WithoutBrowser\ /Y
xcopy LICENSE.txt GW2PAO\bin\x86\Release_WithoutBrowser\ /Y
xcopy ThirdPartyLicenses.txt GW2PAO\bin\x86\Release_WithoutBrowser\ /Y
xcopy Tasks GW2PAO\bin\x86\Release_WithoutBrowser\Tasks\ /Y /S /E
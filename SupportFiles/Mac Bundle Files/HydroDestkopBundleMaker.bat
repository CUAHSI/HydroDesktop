:: This batch file copies the necessary files to create a Mac .app bundle.
:: Use the resulting folder (HydroDesktop.app) on Mac OS as an application.
:: The bundle can be used to create a .pkg with Apple's PackageMaker.app.

SET SOURCE=
SET DESTINATION=

xcopy "%SOURCE%\Binaries\*.dll" /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\"
xcopy "%SOURCE%\Binaries\*.exe" /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\"
xcopy "%SOURCE%\Binaries\*.png" /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\"
xcopy "%SOURCE%\Binaries\Mono Extensions\*.dll" /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\Mono Extensions\"
xcopy "%SOURCE%\Binaries\Application Extensions\*.dll" /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\Application Extensions\"
xcopy "%SOURCE%\Binaries\Plugins\*.dll" /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\Plugins\"
xcopy "%SOURCE%\Binaries\Support\Mono\*.*" /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\Support\Mono\"
xcopy "%SOURCE%\Binaries\Plugins\*" /s /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\Plugins\"
xcopy "%SOURCE%\Binaries\de\*" /s /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\de\"
xcopy "%SOURCE%\Binaries\es\*" /s /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\es\"
xcopy "%SOURCE%\Binaries\fr\*" /s /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\fr\"
xcopy "%SOURCE%\Binaries\hu\*" /s /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\hu\"
xcopy "%SOURCE%\Binaries\it\*" /s /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\it\"
xcopy "%SOURCE%\Binaries\ja\*" /s /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\ja\"
xcopy "%SOURCE%\Binaries\pt\*" /s /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\pt\"
xcopy "%SOURCE%\Binaries\ru\*" /s /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\ru\"
xcopy "%SOURCE%\Binaries\sk\*" /s /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\sk\"
xcopy "%SOURCE%\Binaries\sv\*" /s /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\sv\"
xcopy "%SOURCE%\Binaries\tr\*" /s /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\tr\"
xcopy "%SOURCE%\Binaries\zh-cn\*" /s /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\zh-cn\"
xcopy "%SOURCE%\Binaries\zh-tw\*" /s /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\zh-tw\"

xcopy "%SOURCE%\SupportFiles\Mac Bundle Files\Info.plist" /s /y "%DESTINATION%\HydroDesktop.app\Contents\"
xcopy "%SOURCE%\SupportFiles\Mac Bundle Files\HydroDesktopScript" /s /y "%DESTINATION%\HydroDesktop.app\Contents\MacOS\"
xcopy "%SOURCE%\SupportFiles\Mac Bundle Files\HydroDesktop.icns" /s /y "%DESTINATION%\HydroDesktop.app\Contents\Resources\"

pause
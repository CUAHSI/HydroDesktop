@echo off
set starttime=%time%

bin\Oatc.OpenMI.CommandLine.exe -r SmithBranchModel.opr
set endtime=%time%

set /a hrs=%endtime:~0,2%
set /a hrs=%hrs%-%starttime:~0,2%

set /a mins=%endtime:~3,2%
set /a mins=%mins%-%starttime:~3,2%

set /a secs=%endtime:~6,2%
set /a secs=%secs%-%starttime:~6,2%

if %secs% lss 0 (
    set /a secs=!secs!+60
    set /a mins=!mins!-1
)
if %mins% lss 0 (
    set /a mins=!mins!+60
    set /a hrs=!hrs!-1
)
if %hrs% lss 0 (
    set /a hrs=!hrs!+24
)
set /a tot=%secs%+%mins%*60+%hrs%*3600

echo End     = %endtime%
echo Start   = %starttime%
echo Hours   = %hrs%
echo Minutes = %mins%
echo Seconds = %secs%
echo Total   = %tot%

pause
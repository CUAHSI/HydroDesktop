echo "downloading DotSpatial package ... please wait"
cd %1
Nuget\nuget.exe install %1HydroDesktop.Main\packages.config -o %1Packages  -s http://hydro10.sdsc.edu/nuget/nuget
Nuget\nuget.exe install %1HydroDesktop.Data\packages.config -o %1Packages  -s http://hydro10.sdsc.edu/nuget/nuget
Nuget\nuget.exe install %1HydroDesktop.Controls\packages.config -o %1Packages  -s http://hydro10.sdsc.edu/nuget/nuget
Nuget\nuget.exe install %1HydroDesktop.WebServices\HydroDesktop.WebServices\packages.config -o %1Packages  -s http://hydro10.sdsc.edu/nuget/nuget




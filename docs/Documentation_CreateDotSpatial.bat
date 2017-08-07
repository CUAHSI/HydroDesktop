SET build.number=1.5.0
CD C:\Users\shieldst\Documents\Visual Studio 2010\Projects\DotSpatial2

@echo  Deleting all packages since we are going to upload all of the packages we find...

del *.nupkg



SupportFiles\Nuget\nuget.exe pack "DotSpatial.Analysis\DotSpatial.Analysis.csproj" -Version %build.number% -Build -Properties Configuration=Release 
SupportFiles\Nuget\nuget.exe pack "DotSpatial.Compatibility\DotSpatial.Compatibility.csproj" -Version %build.number% -Build -Properties Configuration=Release
SupportFiles\Nuget\nuget.exe pack "DotSpatial.Controls\DotSpatial.Controls.csproj" -Version %build.number% -Build -Properties Configuration=Release
SupportFiles\Nuget\nuget.exe pack "DotSpatial.Data\DotSpatial.Data.csproj" -Version %build.number% -Build -Properties Configuration=Release
SupportFiles\Nuget\nuget.exe pack "DotSpatial.Data.Forms\DotSpatial.Data.Forms.csproj" -Version %build.number% -Build -Properties Configuration=Release
SupportFiles\Nuget\nuget.exe pack "DotSpatial.Extensions\DotSpatial.Extensions.csproj" -Version %build.number% -Build -Properties Configuration=Release
SupportFiles\Nuget\nuget.exe pack "DotSpatial.Modeling.Forms\DotSpatial.Modeling.Forms.csproj" -Version %build.number% -Build -Properties Configuration=Release
SupportFiles\Nuget\nuget.exe pack "DotSpatial.Mono\DotSpatial.Mono.csproj" -Version %build.number% -Build -Properties Configuration=Release
SupportFiles\Nuget\nuget.exe pack "DotSpatial.Positioning\DotSpatial.Positioning.csproj" -Version %build.number% -Build -Properties Configuration=Release
::SupportFiles\Nuget\nuget.exe pack "DotSpatial.Positioning\DotSpatial.Positioning Designers.csproj" -Version %build.number% -Build -Properties Configuration=Release
::SupportFiles\Nuget\nuget.exe pack "DotSpatial.Positioning\DotSpatial.Positioning.GPS.csproj" -Version %build.number% -Build -Properties Configuration=Release
::SupportFiles\Nuget\nuget.exe pack "DotSpatial.Positioning\DotSpatial.Positioning.Design.csproj" -Version %build.number% -Build -Properties Configuration=Release
::SupportFiles\Nuget\nuget.exe pack "DotSpatial.Positioning.Forms\DotSpatial.Positioning.Forms.csproj" -Version %build.number% -Build -Properties Configuration=Release
SupportFiles\Nuget\nuget.exe pack "DotSpatial.Projections\DotSpatial.Projections.csproj" -Version %build.number% -Build -Properties Configuration=Release
SupportFiles\Nuget\nuget.exe pack "DotSpatial.Projections.Forms\DotSpatial.Projections.Forms.csproj" -Version %build.number% -Build -Properties Configuration=Release
SupportFiles\Nuget\nuget.exe pack "DotSpatial.Serialization\DotSpatial.Serialization.csproj" -Version %build.number% -Build -Properties Configuration=Release
SupportFiles\Nuget\nuget.exe pack "DotSpatial.Symbology\DotSpatial.Symbology.csproj" -Version %build.number% -Build -Properties Configuration=Release
SupportFiles\Nuget\nuget.exe pack "DotSpatial.Symbology.Forms\DotSpatial.Symbology.Forms.csproj" -Version %build.number% -Build -Properties Configuration=Release
:: Tools doesn't need to be pushed to nuget. It is on the myget feed.
::SupportFiles\Nuget\nuget.exe pack "DotSpatial.Tools\DotSpatial.Tools.csproj" -Version 1.0.1186 -Build -Properties Configuration=Release
SupportFiles\Nuget\nuget.exe pack "DotSpatial.Topology\DotSpatial.Topology.csproj" -Version %build.number% -Build -Properties Configuration=Release

@echo 
@echo 
@echo 
@echo  Ready to publish to nuget.org
@echo 
@echo 
@echo 
pause



forfiles /m DotSpatial.*.nupkg /c "cmd /c  SupportFiles\Nuget\nuget.exe  push @FILE 42be5d50-8682-4d36-9823-88a7d450a702
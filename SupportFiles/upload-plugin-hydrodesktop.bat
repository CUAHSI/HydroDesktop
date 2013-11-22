SET build.number=1.5.11
CD E:\dev\HydroDesktop\source\

@echo  Deleting all packages since we are going to upload all of the packages we find...
del *.nupkg

CD DataAggregation\
REN packages.config tempfile
CD ..

.nuget\nuget.exe pack "DataAggregation\DataAggregation.csproj" -Version %build.number% -Build -Properties Configuration=Release

CD DataAggregation\
REN tempfile packages.config
CD ..

@echo 
@echo 
@echo 
@echo  Ready to publish to nuget.org
@echo 
@echo 
@echo 
pause



forfiles /m DataAggregation.*.nupkg /c "cmd /c  .nuget\nuget.exe  push @FILE 546086eb-759c-4006-8209-787d4a4f448d -Source http://www.myget.org/F/hydrodesktop/api/v2/package
Automated Builds are now done on a JetBrains TeamCity Server at [http://hydro10.sdsc.edu:88](http://hydro10.sdsc.edu:88)
For a release, a developer will still need to commit the binaries using msbuild /t:UpdateBinaries

# NEEDS NEW PROCESS FOR Hydrodestop 1.2

# Required for Build:
* Net 3.5
* MsBuild (Instructions based on  VS 2008 Sp1).
* InnoSetup 5.3.10 
	* Innosetup quickstart pack from [ http://www.jrsoftware.org/isdl.php]( http://www.jrsoftware.org/isdl.php)
* [Slik Subversion](http://www.sliksvn.com/en/download)
## Dependencies
* [MS Community Build Tasks](http://msbuildtasks.tigris.org) (in build/trunk)
* [Codeplex Api](http://codeplex.codeplex.com/wikipage?title=CodePlexMSBuildTasks&referringTitle=CodePlexAPI) (in build/trunk)
* + InnoSetup, Slik Subversion (must be installed, see required)

 note on how to clean a mercurial repo folder
hg purge --print --all

# Building the Hydrodesktop Application:
## Overview
You check out the build script, run the script using MsBuild. This will pull all the code from source control (subversion), compile the Visual Studio solutions, and run the installer. When a release is completed, the release manager will then "check-in" the binaries to a "tagged" directory the subversion. The tagged directory preserves the dll's as a single package. 

## Steps
* Create a new directory, eg C:\HydrodesktopBuilds
* start a "visual studio command prompt"
	* Program Files>Microsoft Visual Studio 2008>Visual Studio Tools>visual studio command prompt
* cd C:\HydrodesktopBuilds
* check out\export "Build/trunk" Directory from subversion
	* svn co https://hydrodesktop.svn.codeplex.com/svn/hydrodesktop/trunk/Build/trunk   Build
* cd Build
* Run MsBuild script
	*  msbuild BuildHydroDesktop.proj
* wait...
* If you are confident, then commit the binaries
	* msbuild /t:UpdateBinaries
	* will commit with comment "update of binary files from build script"
* commit build script: BuildHydroDesktop.proj

* ANY EDITS YOU DO TO THE InnoSetup scripts HAVE NOT BEEN DONE IN SOURCE CONTROL, aka commit separately*

--You can now use a response file to setup an automated email. See the hydrodesktopbuild.rsp

 msbuild BuildHydroDesktop.proj @hydrodesktopbuild.rsp-- No longer needed Being built on River

# Include a plug-in in the build
Nothing. All plugins and solutions are built automatically. You can exclude a plug-in.

# Excluding a Plug-in to the TRUNK Build
Steps:
* Checkout hydrodesktop/trunk/Build/Trunk
* Edit BuildHydroDesktop.proj
In the section EXCLUDED SOLUTIONS edit the item group. To test, run a clean
"msbuild /t:Clean  BuildHydroDesktop.proj" and look to see if your project was excluded.

{{	
	<ItemGroup>
	   <excludedSolution Include="$(SourceDirectory)\Source\Libraries\UserControls\HydroDesktopControls.sln" />
	   <excludedSolution Include="$(SourceDirectory)\**\Search2.sln" />
	   <excludedSolution Include="$(SourceDirectory)\****\HydroModeler 2.0\****\**.sln" />
	   <excludedSolution Include="$(SourceDirectory)\****\HydroModeler\****\**.sln" />
	   <excludedSolution Include="$(SourceDirectory)\**\RibbonSamplePlugin.sln"/>
	</ItemGroup>
}}
Examples:
* several solutions lists are built
	* 		    Project/Solution with Single Solution:
******		       $(SourceDirectory)\****\Search2.sln
**			Item with Multiple Soultions
******			   $(SourceDirectory)\****\HydroModeler 2.0\****\**.sln
******			   $(SourceDirectory)\****\PROJECTDIRECTORY\****\**.sln
			
* Commit
If you create a "tag" (svnCopy), then you can create a custom build script customized to add only your plug ins

# Testing
Things that need to be tested:
* Manual testing for Functional Approval:
	* Run HydroDesktop Quick Start Guide [http://hydrodesktop.codeplex.com/documentation](http___hydrodesktop.codeplex.com_documentation)
* Other Manual tasks (at present)
# Does .NET framework get correctly installed on machines that don't have .NET Framework 2.0 or 3.5?
# Launch HydroDesktop on 32bit and 64bit Windows operating systems.
# Can we run a search (HIS Central option) and download data? With previous installers we tested the keywords: Streamflow, Nutrient, Nitrate Nitrogen and area: randomly selected county or HUC in the U.S.
# Can we add or update web service metadata using metadata fetcher?
# Can we display one or more data series using the graph view and table view?
# Can we export of one or more of the data series to a text file.

# Publishing a Release
* UPLOADS TO CODEPLEX AND THE SUBVERSION BRANCHING PORTION ARE NOT WORKING CORRECTLY *
Processes done completing a release:
* commit build script and binaries to svn (working)
* build zip file with source (after running "clean" on solutions)
* **Create CodePlex Release**
* Upload Files to Release (seems to fail, often)

Run msbuild /CreateRelease BuildHydroDesktop.proj
{{
msbuild /t:CreateRelease /p:User={CodePlexUser} /p:Password={CodePlexPassword} BuildHydroDesktop.proj
 }}
* If the upload fails. try just the upload:
* UPLOADS PORTION IS NOT WORKING CORRECTLY *
{{
msbuild /t:UploadToCodeplex /p:User={CodePlexUser} /p:Password={CodePlexPassword} BuildHydroDesktop.proj /
}}

Just tag branches in repository (uploads new binaries)
* THE SUBVERSION BRANCHING PORTION IS NOT WORKING CORRECTLY *
{{
msbuild /t:UpdateSourceRepository 
 }}

# testing Software
* [UserInterfaceTesting](UserInterfaceTesting)

Branching in the Codeplex SVN bridge does not always work, 
so for branching the hydrodesktop that has a lot of binaries, 
we need to do it in steps

# Summary
# create folder
# svn copy folders from trunk 
# add externals
# add build to teamcity
# checkout instller Build folder
## test installer build 
# Configure the TeamCity build

# Steps
## Branch in SVN
{{
Note for some reason, just branching entire hydrodesktop/trunk does not work in codeplex.
 It is probably a memory issue since we have multiple large binaries in the tree.
 These steps are the workaround (as of Oct 21th, 2010
}}
* create a folder in hydrodesktop/branch/BRANCH_NAME
* TortoiseSVN branch tag (svn copy  the folders
	* binaries
	* source
	* build
	* Databases
	* Installer
	* UItests
	* file: subversion.externals.txt
	* Commit Source 1 at a time
		* create a source directory
		* add
		* commit
		* branch each folder in the trunk/source
			* Libraries
			* Main
			* Plugins
			* Tools
			* IGNORE TESTING
* add a README (aka README_branch_v11_oct19)
	* add to svn
* ADD EXTERNALS: 
	*  right click on hydrodesktop/trunk, select properties
	* Export properties
	* right click on BRANCH_NAME, select properties
	* click import the saved properties file
* COMMIT TO SVN

## Test the autobuild
Do the steps indicated in [HydroDesktop Building and Testing](HydroDesktop-Building-and-Testing) with the following modifications
* checkout the build (hydrodesktop/branch/BracnhName/Build)
	* pass msbuild a property wiht the branch location eg for branch/v11_oct19
	* msbuild /p:SvnRoot=https://hydrodesktop.svn.codeplex.com/svn/hydrodesktop/branch/v11_oct19/

## Configure the Team City Build


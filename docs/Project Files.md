# HydroDesktop Project Files
HydroDesktop uses a **project** to store downloaded time series, map symbology and geographic data.
The HydroDesktop project is a folder with three types of files: The project file (.dspx), the SQLite Database (.sqlite) files and the Shapefile (.shp) geographic data files. 
The **.dspx and the **.sqlite files should have the same name, for example project1.dspx and project1.sqlite.

## .dspx file format description
The .dspx file is a XML (eXtensible markup language) text file with information about the symbology of map layers (color, transparency, visibility, line width), names of geographic data files, map view extent, and map projection.

## Project File Scenarios for testing
Following scenarios require detailed testing:
# start  HydroDesktop
# Create new project
# Open Project
# Save Project
# Save Project As
# Change Database
# Create new database
# Close HydroDesktop
# Double-click on a *.hdprj file

## 0. At first run of a new version
* If settings.xml does not exist, assume first run.
*  Create a settings.xml, with the default paths (which is where files are store in the install)
* ASK user where to create a new project
	* should discourage writing to the "default" project
Expected behavior: User starts HydroDesktop. The file c:\\Program Files\CUAHSI HIS\HydroDesktop\projects\default\Default.dspx is opened.
Exceptional cases:
* Default.hdprj file doesn't exist
* DataRepository.sqlite database specified in the Default.hdprj file doesn't exist

## 1. Start HydroDesktop
*  read settings.xml 
	* check version
	* Update if needed.
	* validate paths to informationversion. 
Expected behavior: User starts HydroDesktop. The file c:\\Program Files\CUAHSI HIS\HydroDesktop\projects\default\Default.hdprj is opened.
Exceptional cases:
* Default.hdprj file doesn't exist 
	* (ask user to create a select project, or create a new project)
* DataRepository.sqlite database specified in the Default.hdprj file doesn't exist

## 2. Create New Project
Expected behavior: 
# User specifies the path of the new hdprj file. 
# The new hdprj file is opened with default base map shapefiles. 
# A new default SQLite database in the same folder as the new project is also created.
Exceptional cases: 
* The user doesn't have write permission for the specified folder

## 3. Open Project
Expected behavior: 
# User specifies the hdprj project file to be opened. 
## If changes have been made to the current project, a message 'Save Changes to Current Project?' is first shown. 
# The map layers are loaded according to the hdprj file. 
# The database connection is set according to the hdprj file and the theme layers are loaded. 
# Plugins are activated or deactivated according to the hdprj file.
Exceptional cases:
* The existing hdprj file is in incorrect format (changes have been made by external editor)
* Some of the shapefiles specified in the hdprj file don't exist
* The SQLite database specified in the hdprj file doesn't exist
* The SQLite database is in incorrect format (some tables have been deleted)

## 4. Save Project
Expected behavior: 
# The current path to the SQLite database, the names of currently active plugins and the map layer names and symbology are saved to the existing hdprj project file.

## 5. Save Project As
Expected behavior:
# user dialog appears to allow user to select a directory for the new project
# The current project file is saved to the user specified directory. 
# The SQLite database is also copied to the user specified directory. The relative paths to the map layers and to the SQLite database path are updated.
* The move logic is missing from the code

## 6. Change Database
Expected behavior: 
# user select database.
## where should this open up? in the project, or in the directory of the present database
## the file dialog should open up in the directory of the present database
# The database path is changed. 
## warning if this is not in the same directory as the project
# Changed database Event is posted.
##  All plugins and user controls are refreshed to use the modified database connection.
Exceptional cases:
* The SQLite database file is in incorrect format.

## 7. Create New Database
Expected behavior: 
# dialog appears, opened in same directory as project (or in database directory?)
## The user specifies the name of the new database. 
# The new SQLite database is created by copying the dbTemplate.bin file. 
Exceptional cases:
* The user doesn't have write permission for the specified folder

## 8. Close HydroDesktop
Expected behavior:
If changes have been made to the current project, a message 'Save Changes to Current Project?'
# if the default project is opened:
## User selects yes: show a 'Save As' dialog and let the user specify the file name
## User selects no: undo all changes to default.hdprj project file and default.sqlite database.
# the proposed behavior is similar to MS Word, ArcGIS and MapWindow applications.

## 9. Double-Click on the *.dspx project file in Windows Explorer
Expected behavior: The installer sets the file association of *.hdprj extension with HydroDesktop. The HydroDesktop application is opened with the map layers and database connection as specified in the hdprj project file.
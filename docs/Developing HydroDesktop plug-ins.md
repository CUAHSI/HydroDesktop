The following plug-ins can be used as a sample starting point for HydroDesktop plug-in developers:
## Sample Plug-ins
Shows how to add a ribbon button, ribbon tab, ribbon panel and a separate main view from a plug-in. The source code can be found under [Source/Plugins/RibbonSampleCSharp](https://hydrodesktop.svn.codeplex.com/svn/hydrodesktop/trunk/source/plugins/ribbonsamplecsharp/) (c#) and [Source/Plugins/RibbonSampleVB](https://hydrodesktop.svn.codeplex.com/svn/hydrodesktop/trunk/source/plugins/RibbonSampleVB/) (VB.NET). Use one of the sample plug-in projects as a template for a new HydroDesktop plugin.

## Testing the Sample Plugin
To test the sample plugin, copy the plugin assembly DLL file to the **plugins** subdirectory of the directory where HydroDesktop is installed. (c:\Program Files\CUAHSI HIS\HydroDesktop\Plugins\)

## Additional Sample Plugins
## Export to CSV
Shows how to create a new dialog form, read data from the main DataRepository database using SQL queries and how to export the data to a CSV file in a BackgroundWorker. Source code can be found under Source/Plugins/ExportToCSV

## Import from WaterML
Shows how to use the HydroDesktop web service functions, data access layer and object model to read a list of time series from a WaterML document and save the data values and related information to the database.

## Plugin Developer Documentation
[HydroDesktop Plugin Development Guide](Developing HydroDesktop plug-ins_HydroDesktop_Plugin_Development_V1.1.docx)
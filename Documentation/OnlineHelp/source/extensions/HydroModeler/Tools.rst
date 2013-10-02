.. index:: Tools

Tools
=====

These components are designed by the HydroModeler development team to assist in building and running user defined configurations. 

Db Reader
---------

Db Reader is a tool to help users extract data from HydroDesktop database and provide this data to the model simulation.  Because not all data will follow the same conformity, Db Reader will gather the required information and format it to a usable type, an OpenMI Exchange Item.  Db Reader gathers this information in two steps.  It extracts all available information to be potentially utilized from the database.  Then, Db Reader analyzes the data's time series information so that duplicate or reduntant data is not loaded.  This helps to reduce the amount of resources used by your computer.  Db Reader is seperated into three methods; Initialize, On Add Link, and Get Values.  Initialize extracts the data from the HydroDesktop database and also pulls the data characteristics such as the name, description and ID.  On Add Link will run once Db Reader has been linked to another component and will gather the specific data series required and store them to a buffer (Oatc.SmartBuffer).  Get Values will find the required data buffer and filter the results to include only those in the desire time range and return these values.  If the data series is too sparse or the output and input data do not align properly, Db Reader will interpolate to create a more appropriate data series.

Db Writer
---------

Db Writer is a tool to help users save computed, simulation data to the HydroDesktop database.  This allows users to manipulate the data, using other plug-ins present in HydroDesktop, to view, edit, and manage the simulation data.  This tool is comprised of four methods; Initialize, Add Link, Data Changed and Finish.  Initialize will create generic output exchange items that will be detailed after the model simulation has run.  Add Link will set up Db Writer to grab data from a component immediately after it has been computed after one simulated time step.  Db will also set up a data model that will include variable parameters like time unit, variable unit, measurement method and measurment source.  Data Changed operates when a data transfer occurs.  When this does occur, data is obtained from the preceeding component and added to its respective data series contained within the data model.  The data model is only kept in memory during the duration of the simulation to alleviate the demand on computer resources as well as save computational time.  Finish will take the data model and either create a new instance for the data or ammend it to a pre existing series of data within the HydroDesktop database.

Hydro Link
----------

*obsolete component*

Load Calculator
---------------

to come...

wtmp Reader
-----------

*obsolete component*

Random Input Generator
----------------------

to come...
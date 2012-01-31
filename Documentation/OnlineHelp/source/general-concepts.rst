.. index:: 
   single: general concepts
   single: concepts; general


General Concepts
================

WaterOneFlow Web Service
------------------------

Web services are like programs that you access on the Internet, which typically have one distinct "service" that they provide to you.  WaterOneFlow is a Web service whereby you provide a location, a variable of interest (e.g., streamflow), and a time period, and it returns a time series of data.  The output format from WaterOneFlow is an XML language called WaterML that includes both the time series data and also the metadata to fully describe the data.  This is a standard design that dozens of agencies and universities now use to publish their data.

HydroDesktop knows how to request data from a WaterOneFlow Web service and translate the WaterML response into your local database so that you can get on with your analysis.  It handles this details of communicating with Web services so that you don't have to.

Data Management
---------------

HydroDesktop works with both spatial and temporal data.  Spatial data tend to be in the form of shapefiles, and some shapefiles such as basemap data are distributed with HydroDesktop.  Extensions can enable HydroDesktop to work with additional spatial data types such as online map services.

Temporal data are stored in a relational database called the Data Repository.  By default, SQLite is used as the database format.  Tools in HydroDesktop know how to read this database and present information from it to the user.  A default database is included with HydroDesktop, but you can and are encouraged to create and use your own project-specific databases.  These databases are filled with temporal data that you acquire through data searches, data import, or data generation using models and analytical tools.

HydroDesktop also has the ability to query a given data service to figure out what data are available through the service.  It stores this catalog of available data in another database called the Metadata Cache.  To keep things clean, this database is kept separate from the Data Repository.  Think of the Metadata Cache as a description of all data from all remote data sources that your installation of HydroDesktop knows about, and the Data Repository as a collection of data that you've actually downloaded from data sources and saved.

Search
------

HydroDesktop can search for hydrologic time series data, download it, and save it to your local Data Repository database.
When searching for data, you can specify the following filters: region of interest, parameter, time range, and data source.
When the search results are returned, you can further filter the results and then choose which data you want to actually download.

For more information on search see :doc:`guide-books/Search/Search`.

Projects
--------

You save elements of a HydroDesktop session in a project file.  The project file keeps track of which Data Repository database you were using, what layers you had in your map, how those layers were symbolized, etc.

Getting Help
------------

There are a variety ways of getting help with HydroDesktop:

* Click buttons on the Help tab of the ribbon. The Help tab has buttons for opening the help system, leaving a comment, etc.
* Click help buttons on the specific HydroDesktop tools that you are using, if available.
* Add to discussions and issues on the HydroDesktop Web site at http://hydrodesktop.codeplex.com/.

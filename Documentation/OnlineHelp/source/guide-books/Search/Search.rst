.. index:: Search


Search and Download Data
========================

When searching for data in HydroDesktop, you can specify the following filters: region of interest, time period of interest, data source and variables of interest. 
HydroDesktop then searches the CUAHSI-HIS national catalog of known time series data to find locations of time series that match your search. 
Locations of time series data that match your search are presented in the map. These results include information that HydroDesktop 
can use to connect to each individual data provider for data access. You can further filter the results and then choose which data you want to actually download and store in your database.

When you save data to your database, it is stored as a theme. A theme is a collection of hydrologic time series data that share a common relationship. 
A theme can be anything from a geographic space (e.g., Texas, Colorado) to a hydrologic event (e.g., flood, hurricane) to a combination of both (e.g., Texas Flood). 
Simply put, a theme organizes a collection of related time series. HydroDesktop can save data to a new theme or append data to an existing theme. 
The workflow for finding data and saving it to a theme is shown in the figure.

.. figure:: ./images/Search_fig01.png
   :align: center

The search and data download steps are explained in more detail below:
  
.. toctree::
  :maxdepth: 2


  SearchArea

  SearchOptions 

  SearchKeywords  

  RunSearch

  SelectSeriesToDownload

  DownloadingData

  AdvancedSettings

  SearchManagement

As you build your query, the current search parameters are shown in the Search Summary at the bottom of the Search Panel. 
To give yourself more room to work, you can hide the Search Summary by clicking the Hide Search Summary button.

By default the *HIS Central Catalog* is used for searching. to be able to search data from services that are not registered at the HIS Central catalog,
use the **Metadata Cache** search option. To use the Metadata Cache search option, select *Metadata Cache* from the drop-down under the Search button in the main toolbar.
See the following detailed instructions for searching data from a service that is not registered at HIS Central:

.. toctree::
   :maxdepth: 1
   
   ../TableView/MetadataFetcher
   
   
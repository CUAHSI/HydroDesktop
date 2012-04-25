.. index:: Search


Searching for Data
==================

When searching for data in HydroDesktop, you can specify the following filters:

#. :ref:`Area of interest <search-specifying-area-of-interest>`
#. :ref:`Keywords (e.g., Precipitation) <search-choosing-keywords>`
#. :ref:`Time range <search-setting-a-time-range>`
#. :ref:`Data sources <search-choosing-data-sources>`

Once you :ref:`initiate a search<search-running-a-search>`, HydroDesktop queries the CUAHSI-HIS national catalog of known time series data to find locations of time series that match your search. 
Search results are presented in the map and include information that HydroDesktop 
can use to connect to each individual data provider for data access.

You can then further filter the results and choose which data you want to actually :ref:`download<search-downloading-data>`.

.. figure:: ./images/Search_fig01.png

The tools for running a search are located on the *Search* tab of the HydroDesktop ribbon.
The panels in this tab represent the steps in the workflow above.

.. figure:: ./images/Search_fig02.png

.. _search-specifying-area-of-interest:

Specifying Area of Interest
---------------------------

There are two ways of specifying an area of interest for the search: *drawing a box* or *selecting a polygon*.

Drawing a Box
'''''''''''''

If you are very familiar with the location of your study area, then drawing a box may be the easiest option.

To draw a box around your area of interest:

#. Use the tools on the *Map* tab to locate and zoom in to your area of interest.
#. On the *Search* tab, click the **Draw Rectangle** tool.
#. With the left mouse button, click and drag in the map to draw a box around your study area. 

.. figure:: ./images/Search_fig03.png

   Once a box is drawn, it is highlighted in the map

Selecting a Polygon
'''''''''''''''''''

If a polygon feature in the map represents your area of interest, you can select that polygon to be used in the search.
For example, the polygon could represent a county as found in the U.S. Counties dataset included with HydroDesktop,
or the polygon could come from a shapefile that you created and added to the map.

You can either select polygons by location or by attributes of the polygon feature.

To select polygons by location:

#. In the *Legend*, left-click to select the map layer that you want to use.  Note that you must select a polygon layer.  The search will not work with online basemap data, point layers, or line layers.
#. In the *Legend*, make sure the desired map layer is visible by placing a check in the box next to the layer's name.
#. On the *Search* tab, click the **Select Polygons** tool.  This tool works like the **Select** tool on the Map tab, and either tool can be used to select features.
#. With the left mouse button, click a polygon in the map or draw a box that intersects one or more polygons to select those features.  

.. figure:: ./images/Search_fig04.png

   A selected polygon is highlighted in the map

Alternately, you can select polygons using attributes of polygon features.

To select polygons by attributes:

#. In the *Legend*, make sure the desired map layer is visible by placing a check in the box next to the layer's name.
#. On the *Search* tab, click the **Select by Attribute** button. 
#. In the dialog that opens, choose the layer of interest, an attribute of that layer, and then the value of the attribute that will be used to identify the polygon of interest.  To choose the value, you can either select it from the list or type in the desired value.
#. With the attribute value highlighted, click **OK** to close the dialog and select the feature.

.. figure:: ./images/Search_fig05.png

.. _search-choosing-keywords:

Choosing Keywords
-----------------

To faciliate searching across a variety of data sources, each with their own naming conventions for hydrologic parameters, CUAHSI maintains an ontology of hydrologic *keywords* to which a given data source's parameters are mapped.
This means a search for the keyword *Precipitation* will return results even if a data source calls that parameter "rainfall" or "precip" instead.

You can choose from one or more of these keywords when conducting a search.  

To quickly choose a single keyword:

#. In the *Keyword* panel on the *Search* tab, locate the drop down box where you can type a keyword.
#. Start typing the keyword, e.g., *streamflow*, in the box.  The box autocompletes to a valid keyword based on what you type.  Or, you can click the drop down arrow to choose a keyword from the list.

To select multiple keywords:

#. In the *Search* tab, click the **Keyword Selection** button.
#. In the dialog that opens, choose a keyword by either typing it in the text box at the top or by browsing the keywords and selecting a desired keyword.
#. With a keyword highlighted, click the green plus sign to add it to the list of selected keywords.
#. Repeat steps 2 and 3 to add more keywords.
#. Click **OK** when all desired keywords have been added.

.. figure:: ./images/Search_fig06.png

.. note:: If you are interested in all variables, then select the top-level **Hydrosphere** keyword.

.. _search-setting-a-time-range:
   
Setting a Time Range
--------------------

Search results will only be returned if the period of record for a time series intersects the time range that you specify.

To quickly set the time range, enter the start and end dates into the boxes in the *Time Range* panel of the *Search* tab.  Default values are provided as a guide.

For more advanced options regarding time ranges, click the **Select Time** button.  This button opens a dialog in which you can manually set the start and end dates or quickly select a recent period such as the last month.

.. _search-choosing-data-sources:

Choosing Data Sources
---------------------

Dozens of data source publish data using WaterOneFlow, making them accessible in HydroDesktop.  These data sources register with CUAHSI's HIS Central, where a catalog of all data sources and what parameters those data sources publish is maintained.

By default, when HydroDesktop performs a search, it sends the request to HIS Central.  HIS Central searches its catalog to see who has time series that match the search criteria, and is sends back search results to HydroDesktop.

In many cases, this default behavior is fine and there is no need to modify this aspect of search.  However, you do have a couple of advanced options available to you.

One of these options is to **restrict the search to specific data sources**.  You might do this when you know you only want to work with USGS data.

To restrict the option to particular data sources:

#. On the *Search* tab, click the **All Services** button.
#. In the dialog that opens, place a check next to services that you do not want to exclude from the search, and click **OK**.

Another option you have is to search the local metadata cache instead of HIS Central.
The *metadata cache* is a database, much like the one at HIS Central, which catalogs what parameters are available from certain data sources.
However, the metadata cache is created by you and managed by you.  It has no ties to HIS Central.
Creating a metadata cache is useful if you are aware of services that are not registered at HIS Central.  These services wouldn't be returned in HIS Central search results, but may still be useful to you.
For more information on creating a metadata cache, see :doc:`/extensions/MetadataFetcher/MetadataFetcher`.

To search the metadata cache instead of HIS Central:

#. On the *Search* tab, click the **HIS Central** button.
#. Select the **Local Metadata Cache** option and click **OK**.

.. note::  You must have already harvested data into the metadata cache before attempting to search it.

.. _search-running-a-search:

Running a Search
----------------

With search parameters set, follow these steps to run a search:

#. In the *Search* tab, click the **Run Search** button.
#. When the dialog indicates the search has finished, click to **Hide** the dialog.

Locations of time series that match your search criteria are displayed in the map and symbolized by data source and number of data values available.
You can hover your mouse over one of the symbols in the map to see more about that location.

.. figure:: ./images/Search_fig07.png

.. _search-downloading-data:

Downloading Data
----------------

There are a couple of ways to download data from search results.  The easiest is to hover the mouse over the location of a desired time series in the map, and in the pop-up window that opens, click the **Download** button.

If you would like to download multiple time series, then you can select them and click the **Download** button on the *Search* tab.
You select series the same way you select other features in the map.
For example, you can use the **Select** tool on the *Map* tab to click on or draw a box around the time series you want to download.

When the download begins, the Download Manager shows detailed progress of each download.  

.. figure:: ./images/Search_fig08.png

   Download Manager

When the download completes, click to **Hide** the dialog.
If any errors occur during the download, you can click to view the details of the download attempt or attempt to re-download the series with errors.
You can also copy the error log so that you can post it to the HydroDesktop issue tracker, thereby alerting the development team to potential issues with the system.

With the download complete, you can now visualize, analyze, and export the data using HydroDesktop's other capabilities.
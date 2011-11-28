.. index:: Search

Select Series to Download
======================

The search results are shown in the "Search Results" map layer and in the **Results** tab of the search panel. 

To select data series for download use one of the following options:

1. Select sites using the **map** by dragging a box around the sites. The selected sites for download are highlighted in the map.
For each selected site in the map, all series will be downloaded.
2. Select series using the **Results** tab in the search panel. Multiple series can be selected by holding the **CTRL** or **SHIFT** key.
If you want to select all series, click on a series in the **Results** tab and press **CTRL+A** keys.
3. Select series using the **Attribute Table**

To select series using the attribute table, click the **Attribute** button in the main *Home* toolbar.

A new dialog **Attribute Table Editor** is shown.
- You can sort each column by clicking on the column header.
- You can select a series by clicking on the row
- You can select multiple series by holding the **SHIFT** or **CTRL** key.

Some key columns to note in the attribute table editor are:

- DataSource: The original data source of the data
- SiteName: The name of the monitoring point where the time series is recorded.
- VarName: The name of the variable represented by the time series.
- DataType: Some sites report several statistics of data. For example at a streamflow location, you can find minimum, maximum, and average streamflow values computed on daily time step.
- ValueType: Information how the values were obtained (field observation, derived value)
- StartDate, EndDate, ValueCount: These fields give you sense of the overall period of record for a time series and the number of values for its period of record.
- ServiceURL, ServiceCode: These fields give you information about the web service that holds the data
- TimeSupport, TimeUnits: the length of period of each observation. For example, if the variable is daily precipitation amount, then the time support is one day.
- Latitude, Longitude: the coordinates of the site in WGS84 coordinate system


*Selecting series to download using the map and the attribute table:*


.. figure:: ./images/Search_fig06.png
   :align: center
.
   
After selecting the series to download from the search results, specify the **Theme name**. The theme name is a grouping of the downloaded data.  You can select *New Theme* or *Existing Theme* 
if there already are themes from previous downloads in the map.

**Next Step:**

.. toctree::
   :maxdepth: 1
   
   DownloadingData
.. index:: 
   single: extensions; Metadata Fetcher
   single: Metadata Fetcher

Metadata Fetcher
================

HIS Central maintains a catalog of time series metadata, i.e., where time series variables are measured and from which Web services they may be obtained.  HydroDesktop queries HIS Central's catalog when searching for data.  But what happens when a service isn't registered with HIS Central?  This may happen when a service is developmental or if the service is private.  HydroDesktop can still search for data from such services, but instead of searching HIS Central, the search is performed on a local metadata cache database.

The local metadata cache database is similar to the HIS Central catalog.  Like the HIS Central catalog, the local metadata cache database doesn't store time series values; it stores information about where time series variables are measured and from which Web services they may be obtained.  One difference is that the variables in the local metadata cache database are not mapped to the CUAHSI hydrologic variable ontolgoy, because the ontology and mappings are maintained by HIS Central.  Another difference is that the local metadata cache database only stores metadata from the services that you choose to harvest into it.  

The *Metadata Fetcher* is the tool that you use to harvest metadata into the local metadata cache database.  When this extension is enabled, you'll find a *Metadata* panel added to the *Table* tab.  The panel has buttons for working with the metadata cache.  

The general workflow for harvesting metadata into your local cache is:

#. Use the **Add** button to add WaterOneFlow services to the list of services that the metadata cache will harvest.  This just adds services to a list and doesn't actually perform any harvesting.
#. Use the **Manage** button to harvest metadata for selected services.

Once metadata has been harvested, you can search the local metadata cache database by using the Search extension.  See the documentation on the Search extension for more information.


Managing Services
-----------------

When you click the *Manage* button, the *Metadata Fetcher* window opens showing a list of WaterOneFlow Web services that can be harvested.  The title of the service, it's URL, and a timestamp indicating when the service was last harvested is displayed.  

.. note:: If you see a timestamp for the year 0001, this indicates that metadata has not yet been harvested for the given service.

To update the local metadata cache database, place a check next to the service(s) that you want to harvest and click **Download Metadata**.  The database will be updated with the latest metadata available from the service.  Remember that you are not downloading time series values here.  You are simply downloading metadata about where time series are measured to enable efficient searching for data later on.

In the *Service Management* menu, you'll find options for adding services and removing checked services from the local metadata cache database.  You can also refresh the service list if you suspect that the list of services displayed in the *Metadata Fetcher* window is not current.

Adding Services
---------------

The *Add WaterOneFlow Service Info* window is used to add WaterOneFlow Web services to the list of services for which metadata can be harvested into the local metadata cache database.  You can open the *Add WaterOneFlow Service Info* window either by clicking **Add** in the *Metadata* panel of the *Table* tab in HydroDesktop, or by accessing the *Service Management* menu in the *Metadata Fetcher window*.

In the *Add WaterOneFlow Service Info* window, you can add information for one service at a time or for multiple services.  Before updating the local metadata cache database with information about the services you want to add, you can check for any identical services already in the database by clicking the **Check Existing** button.  This helps to avoid duplicate entries in the database.  Click **Update Database** to add information about the services to the database.

Adding a Single Service
'''''''''''''''''''''''

To add information about a single WaterOneFlow Web service:

#. In the **Add WaterOneFlow Service Info** window, activate the **Add Single Service** tab.
#. Fill out the information for a single service.
#. Optionally, click to check if the service already exists in the database.
#. Click to update the database.

Adding Multiple Services
''''''''''''''''''''''''

To add information about multiple WaterOneFlow Web services, you have three options:

* In the **Add WaterOneFlow Service Info** window, activate the **Add Multiple Services** tab and complete the information for each service to add.
* In the **Import** menu, choose **From HydroServer** and input the URL to a HydroServer's Capabilities service.
* In the **Import** menu, choose **From File** and input the path to a comma delimited file containing information about the services to add.  The names of the columns in the file must match the names in the **Add Multiple Services** tab.

For any of the options above, the result is a list of services in the **Add Multiple Services** tab.  Once the list is complete, you can optionally click to check if the service already exists in the database.  Then, click to update the database.

.. note:: A HydroServer is a Web server hosting one or more WaterOneFlow services.  HydroServers also publish a Capabilities service which enables clients to automatically determine what services the HydroServer has to offer.

Options
'''''''

In the *Add WaterOneFlow Service Info* window, from the *Options* menu, you can choose whether or not each service in the list should be checked to see if it matches the WaterOneFlow service signature before adding the service to the database.  Checking the service requires following the given URL to see if a Web service is present, and if so, whether or not it has the same methods as defined by WaterOneFlow.  This checking requires additional processing time, but helps to insure that only operational WaterOneFlow services are added to the metadata cache database.
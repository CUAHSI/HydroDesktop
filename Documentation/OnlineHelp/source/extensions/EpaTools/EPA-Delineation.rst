.. index:: EPA Delineation

EPA Delineation
===============

After you specify one point on the main map, EPA Delineation tool returns back to you the watershed delineated from that point based on NHD, as well as all NHD flowlines that flow down to the point in that watershed. It uses EPA's WATERS Web and Database Services.

Delineating Your Watershed
--------------------------

Using EPA Delineation tool to get the watershed shapefile makes it possible to search hydrological data within particular watersheds. It follows several steps:

#. Click the **Delineate** button in the **EPA Delineation** Panel in the **Home** Tab.
#. Specify the names of *Watershed Point*, *Watershed* and *Streamline* for the files that you want to save.
#. Click **OK**. You will find the cursor turns to a cross shape.
#. Click at one point that you want to use as the outlet on the map.
#. The Delineation tool is now starting to collect the coordinate information of the point you clicked and send it to the EPA Web Services to get responses.
#. Wait until the message telling you *"Drawing Features on the Map..."*. The message box will close automatically after all procedures ending.
#. You will find three shapefiles with the names you specified shown on the top of the map. They are temporarily saved in *Users\Application Data\HydroDesktop\Delineation* folder in your system.
#. You can export the features as shapefiles to other locations by right clicking on the name and selecting **Data --> Export Data**.
#. You can also check the attributes of these shapefiles by right clicking on the name and selecting **View Attributes**, or directly click the **Attribute** button in the ribbon tab.

Now you have a pretty hydrological view of all the upstream of your particular point.

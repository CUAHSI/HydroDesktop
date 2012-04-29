.. index:: Map


Working with the Map
====================



#. :ref:`Map Layers <map-working-with-map-layers>`
#. :ref:`Selection  <map-selection>`
#. :ref:`Attribute Table <map-attribute-table>`
#. :ref:`Map Layout <map-layout>`

.. _map-working-with-map-layers:

Working with Map Layers
-----------------------

The spatial data in HydroDesktop is organized in map layers. The list of all map layers is shown in the legend. Checking or unchecking a layer checkbox shows or hides the layer in the map.
There are five main types of layers:
* Point Layer (for example, cities)
* Line Layer (for example, rivers)
* Polygon Layer (for example, lakes)
* Raster Layer (for example, elevation)
* Background Map Layer (for example, satellite images)

Each layer has a data set storing the associated spatial data. The data set can be a file on the local computer or it can be an online dataset (web map service). Some spatial data are
shipped with HydroDesktop. These include the 'World' and 'North America' templates and the 'jacobs_well_spring' and 'elbe' sample projects.

  To add a new layer to the map, click the *Add* button in the main toolbar.
  
  To change the color scheme or symbol of a layer, right-click on the layer in the legend and select *Properties*.
  
  To add labels to the layer, right-click on the layer in the legend and select *Labeling - Label Setup*.

.. _map-selection:

Selection
---------

You can highlight features in a map layer by using *Selection*. To enable selection, use the *Select* tool in the main toolbar. Click on a feature in the map to select it. The selected 
feature is highlighted in light blue color. Holde the *CTRL* button to select more features. To deselect all features, click the *Deselect All* button in the main toolbar.

.. _map-attribute-table:

Attribute Table
---------------

Each point, line or polygon layer has an *Attribute table* associated with it. This table shows additional information describing the layer. For example, the attribute table of the 
Countries layer has fields describing the country names, areas and populations.

There are two ways to view or edit the attribute table:
  * Click the *View Attribute Table* button in the main toolbar.
  * Right-click on layer in the legend and select *Attribute Table Editor*.
  
If some features in the layer are selected, then the corresponding rows in the attribute table are also selected. You can also use the *Query* tool in the selection menu of the attribute
table editor to filter the data using a custom query expression.

.. _map-layout:

Layout
------

To print or export a map, or set up the map layout by going to the *File* menu in the main toolbar and selecting *Print*. This displays the Print Layout window. 
  * To add a map to the layout, click on the *Insert Map* button and drag-drop it to the layout. You can change the scale of the map by changing the Scale value in the map properties
  of the layout.
  
  * To add a legend to the layout, click on the *Insert Legend* button and drag-drop it to the layout
  
  * You can also add a scale bar, north arrow, text or picture to the layout.
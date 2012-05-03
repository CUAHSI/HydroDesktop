.. index:: extensions

Extensions
==========

Extensions add functionality to HydroDesktop.  For example, a Fetch Basemap extension might enable HydroDesktop to display basemaps from online maps services.  This extension might add a few buttons to the HydroDesktop user interface to give the user control over the choice and display properties of the basemap.  A nice thing about extensions is that they can contain very specific functionality when it does not make sense to include such specific functionality in the general HydroDesktop user interface.  Extensions can also be created and added to HydroDesktop without requiring access to the HydroDesktop source code and without requiring rebuilding HydroDesktop from the source code.  

.. note:: While it might be temping to enable all extensions, keep in mind that extensions often add buttons or other items to the user interface, and require memory to operate.  To keep your user interface from getting cluttered, and to keep HydroDesktop running as efficiently as possible, it's a good idea to only enable extensions that you use.

.. tip:: Extensions can be enabled by clicking the HydroDesktop Orb Button.

HydroDesktop Extensions:

.. toctree::
   :maxdepth: 6

   EpaTools/EPA-tool
   FetchBasemap/FetchBasemap
   HydroModeler/HydroModeler
   HydroR/HydroR
   GraphView/GraphView
   EditView/EditView
   MetadataFetcher/MetadataFetcher
.. index:: Tutorial02

Tutorial 2: Creating and Running a Model Composition
====================================================

The purpose of creating a model composite is to link the components that  are already created so they can exchange data in between and finally  
you get the final product of the project. In this demonstration, we will be create example configuration 4. 

Example 4 structure
------------
..In this exercise we are interested in calculating the runoff (mm/day) using TopModel component for watershed number 18 in cowetta watershed.  
TopModel component need two input exchange items First, daily precipitation rate (mm) which is obtained from the Db reader, Second, The daily  
evapotranspiration rate (mm/day) which is obtained from Hargreaves component. The evapotranspiration rate depends on three input  exchange  
values (maximum temperature, minimum temperature, temperature), The Db reader connect the stored data of watershed 18 and offer the  
temperature variables as an input exchange item for the Hargreaves component, and the precipitation rate for the TopModel component. Finally  
the TopModel needs to be connected with a trigger to start the calculation at every time step (day),and the Db writer component to store the runoff  
value to data base. The stored runoff can be plotted as a time series using hydrodesktop graph. 

Procedures of creating a Model Composition
------------
1	Open the start menu and from program choose the CAUHSI HIS, then press hydrodesktop icon.
.. figure:: ./images/Totorial02/Hydrodesktop_start.png
:align: center
2-	check that you have the latest version of hydrodesktop by comparing the version number  of the CUAHSI Hydro Desktop you have and the latest available on  http://hydrodesktop.codeplex.com/releases/view/59853	 ( ex; Hydro Desktop version is 1.2537.0). Choose Create New Empty Project button.

.. figure:: ./images/Totorial02/newproject.png
:align: center
3-	Load the HydroModeler plugin by selecting the icon in the upper left corner of the screen - Extensions - HydroModeler
.. figure:: ./images/Totorial02/extensions.png
:align: center
4-	Navigate to C:\Hydrodesktop\Installer\HydroModeler_example_configurations\example_configuration_04\models\TOPMODEL



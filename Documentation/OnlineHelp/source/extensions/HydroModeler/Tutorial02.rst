.. index:: Tutorial02

Tutorial 2: Creating and Running a Model Composition
====================================================

The purpose of creating a model composite is to link the components that  are already created so they can exchange data in between and finally  
you get the final product of the project. In this demonstration, we will be create example configuration 4. 

Example 4 structure
------------
..In this exercise we are interested in calculating the runoff (mm/day) using TopModel component for watershed number 18 in cowetta watershed.  
TopModel component needs an ascii raster contain the Topographic index of each pixel in the watershed, model parameter, and  two input exchange items changing with every time step. First, daily precipitation rate (mm) which is obtained from the Db reader, Second, The daily  
evapotranspiration rate (mm/day) which is obtained from Hargreaves component. The evapotranspiration rate depends on three input  exchange  
values (maximum temperature, minimum temperature, temperature), The Db reader connect the stored data of watershed 18 and offer the  
temperature variables as an input exchange item for the Hargreaves component, and the precipitation rate for the TopModel component. Finally  
the TopModel needs to be connected with a trigger to start the calculation at every time step (day),and the Db writer component to store the runoff  
value to data base. The stored runoff can be plotted as a time series using hydrodesktop graph. 

Procedures of creating a Model Composition
------------

1-	Open the start menu and from program choose the CAUHSI HIS, then press hydrodesktop icon.

.. figure:: ./images/Totorial02/Hydrodesktop_start.png
	:align: center

2-	check that you have the latest version of hydrodesktop by comparing the version number  of the CUAHSI Hydro Desktop you have and the latest available on  http://hydrodesktop.codeplex.com/releases/view/59853	 ( ex; Hydro Desktop version is 1.2537.0). Choose Create New Empty Project button.

.. figure:: ./images/Totorial02/newproject.png
:align: center

3-	Load the HydroModeler plugin by selecting the icon in the upper left corner of the screen - Extensions - HydroModeler

.. figure:: ./images/Totorial02/extensions.png
:align: center

4-	Navigate to C:\Hydrodesktop\Installer\HydroModeler_example_configurations\example_configuration_04\models\TOPMODEL. 

 .. figure:: ./images/Totorial02/Current Directory.png
:align: center

5-	Edit the Topmodel.omi and be sure that the TopModel.Linkable_Component" Assembly  is pointing relatively toward the right TopModel.dll file of the model. check the path of both the input TI_raster,configuration file.xml, and revise the values of the input model parameters

 .. figure:: ./images/Totorial02/TopModel.omi.png
:align: center

6-	Follow the same procedures to be sure that  the Hargreaves, Db reader, Dbwriter omi files are  pointing toward the right .dll file.

7-	Right click on the HydroModeler workspace and select Add Model.
 .. figure:: ./images/Totorial02/Model Adding.png
:align: center
8-	 Navigate to C:\Hydrodesktop\Installer\HydroModeler_example_configurations\example_configuration_04\models and add all two models (TopModel-Hargreaves). Also add the DbReader and DbWriter components from C:\Hydrodesktop\Installer\HydroModeler_example_configurations\example_configuration_04\Data\cuahsi-his. now all the models should be added to the HydroModeler workspace. Right click in the HydroModeler workspace and select Add Trigger. A Trigger starts the simulation by invoking the action GetValues on the model at a specified time. 

 .. figure:: ./images/Totorial02/Component.png
:align: center

9-	From the top bar choose table and press change button to define the path for the SQlite database file, navigate to C:\Hydrodesktop\Installer\HydroModeler_example_configurations\example_configuration_04\Data\cuahsi-his and select weather Data repository

 .. figure:: ./images/Totorial02/table.png
:align: center

10-	return back to the HydroModeler tab. Right click in the HydroModeler workspace and select Add Connection. Next, click on the DbReader to assign it as the source component and then click on the Hargreaves to assign as a target component.

11-	Click on the arrow mark to open a connection properties window. Define the output exchange item that will be supplied as an input exchange item.  check the (+) mark for Temperature  of the Dbreader Output Exchange Items to show the three exchanging Temperature items,  check Coweeta max Temperature box and Hargreaves PET Max Temp box, and then press apply to activate the link. Repeat for all links. 

 .. figure:: ./images/Totorial02/Dbconnection.png
:align: center	

12-	Connect the Dbreader to the TopModel to supply the TopModel with the precipitation data of Coweeta watershed. choose Dbreader as a source and TopModel as a target component. 

 .. figure:: ./images/Totorial02/Db-TopModel.png
:align: center	

13-	Connect the Hargreaves PET as a source component to supply the daily calculated PET  to TopModel component. 	

 .. figure:: ./images/Totorial02/hargreaves-TopModel.png
:align: center

14-	Link the TopModel Component as a source to the Db Writer to store the output data (daily runoff hydrograph).

 .. figure:: ./images/Totorial02/DbWriter-TopModel.png
:align: center

15-	Link the TopModel as a source component to the trigger 

 .. figure:: ./images/Totorial02/TopModel-Trigger.png
:align: center

16-	Right click in the HydroModeler workspace and select Run.	

 .. figure:: ./images/Totorial02/run.png
:align: center

17-	Select Set all within Events listened during calculation, click Latest overlapping to determine the simulation end time, and finally click RUN!!

 .. figure:: ./images/Totorial02/runsetup.png
:align: center
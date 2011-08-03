.. index:: Runoff

Runoff
======

These components are designed to predict the tendencies and characteristics of surface water runoff.  The watershed characteristics and upstream information help to determine the outcome of excess rainfall.

2D Diffusive Wave
-----------------

*component currently not working correctly*

This component was created to estimate surface runoff.  It is designed off of the Saint-Venant momentum equation, which uses multiple terms to predict surface runoff.  Diffusive Wave ignores terms for the local and convective acceleration and only includes the pressure, gravity and friction forces.  The pressue force is dependant of the water depth along the channel.  The gravity force is dependant upon the slope of the channel bed.  The frictional force is dependant on the friction slope.  Kinematic wave is a simpler model, which does not include the pressure force term, where as dynamic wave considers all terms of the Saint-Venant momentum equation, including the local and convective acceleration.

Unit Hydrograph
---------------

This component was designed to approximate the watershed response to excess rainfall.  It is based off of the synthetic Soil Conservation Service (SCS) Unitless Hydrograph.  Using excess precipitation as inputs, this method can determine the streamflow at an outlet to a subbasin.  This component also uses the time of peak flow, the peak flow rate and the subbasin lag time as supporting data.  By using this data, the component can build a unit hydrograph that represents one inch of uniform runoff from a watershed in a set amount of time.  This unit hydrograph is then used to approximate the total runoff values.

TOPMODEL
--------

This component is design around the concept that the topography can determine flow routing through upland catchments.  The component relies on the conservation of mass equation, to deteremine the inflow, outflow and change in storage, as well as Darcy's law, to determine the water flow rate through soil.  The Topographic Intdex (TI) is used to determine the index of hydrological similarity so that TOPMODEL can predict the hydrological responses.  From a Digital Elevation Model (DEM), tools referred to as Terrain Analysis Using Digital Elevation Models (TauDEM), are used to obtain hydrological information.  From here, we are able to analyse the watershed topography to derive an appropriate TI.
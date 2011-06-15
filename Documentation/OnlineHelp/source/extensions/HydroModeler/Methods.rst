.. index:: Methods

Methods
=======

This page will give you a brief overview of the inherent methods used in HydroModeler to create and run your projects.  Each of the built in components are found within their respective categories.  The other sub category will contain information about the tools that are used to collect, store, and maniupulate your data.


Concentration
-------------

+ **Water_adv:**

*Advection.*  The Water_adv component is used to calculate the dispersion of contaminants by water flow.  As a contaminant is introduced at a specified location in a flowing body of water, such as a river, it begins to migrate downstream over time.  Water_adv can find the concentration of contaminants at any given time by considering things like the flow rate and dispersion coefficient.

+ **Sediment_Diff:**

*Diffusion.*  The Sediment_Diff component is used to calculate	the diffusion of contaminants to areas of lower levels of contamination.  As a contaminated river continues to flow, some of these contaminants will infiltrate into the soil.  Again, this component can calculate the level of contamination in the soil at any time, given certain constants, like the flow rate and dispersion coefficient.

EvapoTranspiration
------------------

+ **Hargreaves:**

+ **ET:**

+ **PT_PET:**

Infiltration
------------

+ **GreenAmpt:**

+ **CNMethod:**

Routing
-------

+ **Muskingum:**	

Runoff
------

+ **2dDiffusiveWave:**

+ **NRCS UnitHydrograph:**

+ **TopModel:**

SolarRadiation
--------------

+ **NSR:**

Other
-----

+ **LoadCalculator:**

+ **wtmpReader:**

+ **RandomInputGenerator:**

+ **HydroLink:**

+ **DbReader:**

+ **DbWriter:**

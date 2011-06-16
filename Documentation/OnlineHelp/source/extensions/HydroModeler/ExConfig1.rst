.. index:: ExConfig1

Example Configuration 1 - Smith Branch
======================================

.. figure:: ./images/ExampleConfig/HM_fig1.png
   :align: center


The major purpose of this configuration is to find the streamflow at the Smith Branch watershed outlet using a given amount of rainfall.  The rainfall is imported from the HIS database using the DbReader method.  This data is then sent to the Curve Number Method, which can estimate the infiltration of water that occurs in the soil.  This method uses existing conditions such as land use, soil type, and antecedent soil conditions to get an approximation for the excess rainfall.  This newly calculated data is now sent to the Unit Hydrograph method.  This method takes the new data and computes an estimated streamflow of an outlet for a specific subbasin.  The streamflow for each of these outlets are then exported to the Muskingum Method so that a channel network can be applied to determine the streamflow to the watershed outlet.  DbWriter will then store this information.

The following equations are used in the estimation of excess rainfall using the Curved Numbers Method.  The excess precipitation can be obtained using equation 1.  Equation 2 is a conceptual model used to approximate the continuing abstraction.  Equation 3 is an empirical formula for soil water storage.  Equation 4 is an estimation of the initial abstraction using an empirically derived relationship.

.. figure:: ./images/ExampleConfig/HM_fig2.png
   :align: center


The following equations are used in the estimation of the streamflow for an outlet.  Equation 1 is used to obtain the lag time.  Equation 2 estimates the peak flow rate.  Equation 3 defines the peak flow.  Equation 4 uses the excess precipitation and instantaneous unit hydrograph to solve for the direct runoff hydrograph.

.. figure:: ./images/ExampleConfig/HM_fig3.png
   :align: center


The following equations are used in the estimation of the streamflow at the watershed outlet.  In Equation 1, the total water storage in a channel is computed using wedge and prism volumes.  The wedge volumes account for the back water or flood wave effects.  The prism volumes account for the volume of the water residing in the cross sectional area of the channel.  Equation 1 can be simplified to equation 2 by the assumption that the storage can be expressed as a function of the weighting factor and proportionality coefficient.

.. figure:: ./images/ExampleConfig/HM_fig4.png
   :align: center
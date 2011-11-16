.. index:: ExConfig3

Example Configuration 3 - Bidirectional Link
============================================

.. figure:: ./images/ExampleConfig/HM_fig9m.png
   :align: center

|


Example configuration 3's primary objective is to solve for the concentration of a contaminant in a river and its riverbed by simultaneously solving for both the soil and the water concentrations using a bidirectional link.  To do this, the process is broken up into two seperate components.  The two components are the water advection and sediment diffusion models.  Advection is generally associated with the lateral movement of the contaminant and diffusion with the infiltration into the soil.  The water advection model is comprised of movement in both the x and z direction.  The sediment diffusion model is only concerned with contanimants moving in the z direction.  A clearer representation of this is given in the image below.  The flow velocity and diffusion coefficients are required to help use determine the level of contamination in the soil and water.

.. figure:: ./images/ExampleConfig/HM_fig10m.png
   :align: center

|


These two components are both derived from the equation shown below.  D represents the diffusion coefficients in the x and z directions.  u represents the velocity in the x direction and v represents the velocity in the z direction.

.. figure:: ./images/ExampleConfig/HM_fig11m.png
   :align: center

|

To obtain the equation for the water advection component, we only concern ourselves with the x and z direction since we assume that the dominate means of contaminant transportation is advection in the x direction and diffusion in the z direction.

.. figure:: ./images/ExampleConfig/HM_fig12m.png
   :align: center

|

We use similar assumptions to help us derive the sediment diffusion component.  Only concerned with the diffusion in the z direction, we can reduce the equation as follows.

.. figure:: ./images/ExampleConfig/HM_fig13m.png
   :align: center
.. index:: Solar Radiation

Solar Radiation
===============

This component is used to determine the amount of solar radiation that is present at Earth's surface.  Radiation can either be absorbed or deflected.  A surfaced ability to deflect radiation is described by the albedo, and this can vary based on the type of surface that is present.  For example, snow will have a different albedo than vegitation.

Net Solar Radiation
-------------------

This component is design to compute the total amount of solar radiation.  The net radiation is the total of short and long wave radiation.  The short wave radiation is determined by the canopy reflection coefficient and the incoming solar radiation.  For this component, the canopy coefficient will be assumed to be a constant value of 0.23 because the variables that characterize the canopy coefficient, such as the time of day, angle of exposure and surface vegetation, vary only slighty.  The long wave radiation is determined using standarized equations for long wave radiation, cloudiness factor and relative solar radiation.

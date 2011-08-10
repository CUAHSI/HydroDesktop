.. index:: Runoff


.. role:: raw-latex(raw)
    :format: latex html

.. raw:: html

	<script type="text/javascript" src="http://cdn.mathjax.org/mathjax/latest/MathJax.js?config=TeX-AMS-MML_HTMLorMML"> </script>



Runoff
======

These components are designed to predict the tendencies and characteristics of surface water runoff.  The watershed characteristics and upstream information help to determine the outcome of excess rainfall.

2D Diffusive Wave
-----------------

*component currently not working correctly*

This component was created to estimate surface runoff.  It is designed off of the Saint-Venant momentum equation, which uses multiple terms to predict surface runoff.  Diffusive Wave ignores terms for the local and convective acceleration and only includes the pressure, gravity and friction forces.  The pressue force is dependant of the water depth along the channel.  The gravity force is dependant upon the slope of the channel bed.  The frictional force is dependant on the friction slope.  Kinematic wave is a simpler model, which does not include the pressure force term, where as dynamic wave considers all terms of the Saint-Venant momentum equation, including the local and convective acceleration.

This is the Saint-Venant momentum equation.

.. raw:: latex html

	\[\frac{\partial V}{\partial t} + V\frac{\partial V}{\partial x} + g\frac{\partial y}{\partial x}-g(S_0-S_f) = 0\]

.. Note::

	Where :raw-latex:`\(\frac{\partial V}{\partial t}\)` is the local acceleration term, :raw-latex:`\(V\frac{\partial V}{\partial x}\)` is the convective acceleration term, :raw-latex:`\(g\frac{\partial y}{\partial x}\)` is the pressure force term, :raw-latex:`\(gS_0\)` is the gravity force term, and :raw-latex:`\(gS_f\)` is the friction force term.

This is the equation used for the 2D Diffusive Wave component after reducing the Saint-Venant momentum equation.

.. raw:: latex html

	\[g\frac{\partial y}{\partial x}-g(S_0-S_f) = 0\]

.. Note::

	Where :raw-latex:`\(g\frac{\partial y}{\partial x}\)` is the pressure force term, :raw-latex:`\(gS_0\)` is the gravity force term, and :raw-latex:`\(gS_f\)` is the friction force term.


Unit Hydrograph
---------------

This component was designed to approximate the watershed response to excess rainfall.  It is based off of the synthetic Soil Conservation Service (SCS) Unitless Hydrograph.  Using excess precipitation as inputs, this method can determine the streamflow at an outlet to a subbasin.  This component also uses the time of peak flow, the peak flow rate and the subbasin lag time as supporting data.  By using this data, the component can build a unit hydrograph that represents one inch of uniform runoff from a watershed in a set amount of time.  This unit hydrograph is then used to approximate the total runoff values.

This is the Unit Hydrograph equation, which solves for the direct runoff hydrograph using excess precipitation and the instantaneous unit hydrograph.

.. raw:: latex html

	\[Q_n = \sum_{m=1}^{n\le M}P_{e,m}U_{n-m+1}\]

.. Note::

	Where :raw-latex:`\((Q_n)\)` is the direct runoff hydrograph, :raw-latex:`\((P_{e,m})\)` is the excess precipitation at time :raw-latex:`\(m\)`, and :raw-latex:`\((U_{n-m+1})\)` is the unit hydrograph ordinate at current index :raw-latex:`\(n-m+1\)`.

The lag time is obtained using this equation.

.. raw:: latex html

	\[t_p = 0.6t_c\]

.. Note::

	Where :raw-latex:`\((t_p)\)` is the lag time, and :raw-latex:`\((t_c)\)` is the time of concentration.

The peak flow rate is estimated using this equation.


.. raw:: latex html

	\[q_p = \frac{483.4A}{T_p}\]

.. Note::

	Where :raw-latex:`\((q_p)\)` is the peak flow rate, :raw-latex:`\((A)\)` is the watershed area, and :raw-latex:`\((T_p)\)` is the peak flow.

The peak flow is determined using this equation.

.. raw:: latex html

	\[T_p = \frac{t_r}{2}+t_p\]

.. Note::

	Where :raw-latex:`\((T_p)\)` is the peak flow, :raw-latex:`\((t_r)\)` is the rainfall duration, and :raw-latex:`\((t_p)\)` is the lag time.

TOPMODEL
--------

This component is design around the concept that the topography can determine flow routing through upland catchments.  The component relies on the conservation of mass equation, to deteremine the inflow, outflow and change in storage, as well as Darcy's law, to determine the water flow rate through soil.  The Topographic Intdex (TI) is used to determine the index of hydrological similarity so that TOPMODEL can predict the hydrological responses.  From a Digital Elevation Model (DEM), tools referred to as Terrain Analysis Using Digital Elevation Models (TauDEM), are used to obtain hydrological information.  From here, we are able to analyse the watershed topography to derive an appropriate TI.

The following equation is used to determine the total flow rate.

.. raw:: latex html

	\[q_{total} = q_{subsurface}+q_{overland}\]

.. Note::

	Where :raw-latex:`\((q_{total})\)` is the total flow rate, :raw-latex:`\((q_{subsurface})\)` is the subsurface flow rate, and :raw-latex:`\((q_{overland})\)` is the flow rate from the saturated contributing area.


The 2 following equations are used to deteremine the overland flow rate.

.. raw:: latex html

	\[q_{overland} = \frac{A_{sat}}{A}\times P+q_{return}\]

.. Note::

	Where :raw-latex:`\((q_{overland})\)` is the flow rate from the saturated contributing area, :raw-latex:`\((A_{sat})\)` is the saturated area, :raw-latex:`\((P)\)` is the precipitation, and :raw-latex:`\((q_{return})\)` is the return flow.

.. raw:: latex html

	\[q_{overland} = T_{max}\times e^{-\frac{s}{m}}\times\tan\beta\]

.. Note::

	Where :raw-latex:`\((q_{overland})\)` is the flow rate from the saturated contributing area, :raw-latex:`\((T_{max})\)` is the average transmissivity of saturated soil, :raw-latex:`\((s)\)` is the saturation deficit, :raw-latex:`\((m)\)` is the soil parameter, and :raw-latex:`\((\beta)\)` is the local slope.

The following equation is used to determine the subsurface flow rate.

.. raw:: latex html

	\[q_{subsurface} = T_{max}\times e^{-\lambda}\times e^{-\frac{s_{average}}{m}}\]

.. Note::

	Where :raw-latex:`\((q_{subsurface})\)` is the subsurface flow rate, :raw-latex:`\((T_{max})\)` is the average transmissivity of saturated soil, :raw-latex:`\((\lambda)\)` is the average topographic index, :raw-latex:`\((s_{average})\)` is the average saturation deficit, and :raw-latex:`\((m)\)` is the soil parameter.

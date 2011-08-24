.. index:: ExConfig4


.. role:: raw-latex(raw)
    :format: latex html

.. raw:: html

	<script type="text/javascript" src="http://cdn.mathjax.org/mathjax/latest/MathJax.js?config=TeX-AMS-MML_HTMLorMML"> </script>


Example Configuration 4 - TOPMODEL
==================================

.. figure:: ./images/ExampleConfig/HM_fig5.png
   :align: center

This example configuration was created using the conservation of mass equation, which states that the inflow rate minus the outflow rate equals the rate of change in the storage, and Darcy's law, which can be used to calculate the water flow rate through soil.  The TOPMODEL component is also dependent upon the Topographic Index(TI), which is derived from the analysis of the watershed topography by using Terrain Analysis Using Digital Elevation Models(TauDEM).

Located below are the equations used to model the TOPMODEL component.  The first equation determines the total flow rate.  Equations 2 and 3 are used to determine the overland flow rate.  Equation 4 determines the subsurface flow rate.

.. raw:: latex html

	\[q_{total} = q_{subsurface}+q_{overland}\]

.. raw:: latex html

	\[q_{overland} = \frac{A_{sat}}{A}\times P+q_{return}\]

.. raw:: latex html

	\[q_{overland} = T_{max}\times e^{-\frac{s}{m}}\times\tan\beta\]

.. raw:: latex html

	\[q_{subsurface} = T_{max}\times e^{-\lambda}\times e^{-\frac{s_{average}}{m}}\]

.. Note::

	Where :raw-latex:`\((q_{total})\)` is the total flow rate, :raw-latex:`\((q_{subsurface})\)` is the subsurface flow rate, :raw-latex:`\((q_{overland})\)` is the flow rate from the saturated contributing area, :raw-latex:`\((A_{sat})\)` is the saturated area, :raw-latex:`\((P)\)` is the precipitation, :raw-latex:`\((q_{return})\)` is the return flow, :raw-latex:`\((T_{max})\)` is the average transmissivity of saturated soil, :raw-latex:`\((s)\)` is the saturation deficit, :raw-latex:`\((m)\)` is the soil parameter, :raw-latex:`\((\beta)\)` is the local slope, :raw-latex:`\((\lambda)\)` is the average topographic index, and :raw-latex:`\((s_{average})\)` is the average saturation deficit.
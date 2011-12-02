.. index:: Infiltration


.. role:: raw-latex(raw)
    :format: latex html

.. raw:: html

	<script type="text/javascript" src="http://cdn.mathjax.org/mathjax/latest/MathJax.js?config=TeX-AMS-MML_HTMLorMML"> </script>


Infiltration
============

Infiltration is the process of water penetrating through the surface and into the soil.  The amount of infiltration that occurs is dictated by the characteristics of the soil, ranging from the porosity to the current moisture content, as well as surface covering, like vegitation.

Green Ampt
----------

The component uses the Green and Ampt method to determine the amount of excess precipitation that has occured.  Green Ampt does a better approximation of excess rainfall by incorporation more than just the amount of rainfall by including the intensity and the time frame that the rainfall occurs.  This component also requires soil charateristics, such as the current moisture content and the moisure content at complete saturation.  The Green and Ampt method uses a few contants such as the porosity, effective porosity, soil suction head and hydraulic conductivity,  all of which have estimated values, for varying soil types, found by Rawls, Brakensiek and Miller.  Because this method requires more initial data, the estimation of excess rainfall becomes a more accurate model than the Curve Number method.

This is the Green Ampt equation.

.. raw:: latex html

	\[F(t) = Kt+\psi\Delta\theta ln\left(1+\frac{F(t)}{\psi\Delta\theta}\right)\]

.. Note::

	Where :raw-latex:`\((K)\)` is the hydraulic conductivity, and :raw-latex:`\((\psi)\)` is the wetting front soil suction head.

Curve Number Method
-------------------

The purpose of this component is to compute the amount of excess precipitation that occurs based on the amount of water that infiltrates into the soil.  This component is designed on a commonly used practice in hydrological engineering, which calculates the excess precipitation using continuing extraction and soil water storage as supporting data.  Geospatial information, like the land cover and soil data obtained from the Enviromental Protection Agncy (EPA) and National Resources Conservation Service (NRCS), are used to model this method.  Using this information allows for the calculation of curve numbers for each subbasin within the watershed.  Because some data is not readily available,  this component would be more advantageous to use than the Green Ampt.  For example,  if the moisture content of the soil is unknown, then the Curve Number component can approximate the excess rainfall based on assumed soil conditions.

Excess precipitation can be obtained using the Curve Numbers equation.

.. raw:: latex html

	\[P_e = P-I_a-F_a\]

.. Note::

	Where :raw-latex:`\((P_e)\)` is the excess precipitation, :raw-latex:`\((P)\)` is the precipitation, :raw-latex:`\((I_a)\)` is the initial abstraction, and :raw-latex:`\((F_a)\)` is the continuing abstraction.

The following equation is a conceptual model used to approximate the continuing abstraction.

.. raw:: latex html

	\[F_a = \frac{S(P-I_a)}{P-I_a+S}\]

.. Note::

	Where :raw-latex:`\((F_a)\)` is the continuing abstraction, :raw-latex:`\((S)\)` is soil water storage, :raw-latex:`\((P)\)` is the precipitation, and :raw-latex:`\((I_a)\)` is the initial abstraction.

The following equation is an empirical formula for soil water storage.

.. raw:: latex html

	\[S = \frac{1000}{CN}-10\]

.. Note::

	Where :raw-latex:`\((S)\)` is the soil water storage, and :raw-latex:`\((CN)\)` is the curve number parameter.

The following equation is an estimation of the initial abstraction using an empirically derived relationship.

.. raw:: latex html

	\[I_a = 0.2S\]

.. Note::

	Where :raw-latex:`\((I_a)\)` is the initial abstraction, and :raw-latex:`\((S)\)` is the soil water storage.

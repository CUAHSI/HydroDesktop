.. index:: Solar Radiation


.. role:: raw-latex(raw)
    :format: latex html

.. raw:: html

	<script type="text/javascript" src="http://cdn.mathjax.org/mathjax/latest/MathJax.js?config=TeX-AMS-MML_HTMLorMML"> </script>


Solar Radiation
===============

This component is used to determine the amount of solar radiation that is present at Earth's surface.  Radiation can either be absorbed or deflected.  A surfaced ability to deflect radiation is described by the albedo, and this can vary based on the type of surface that is present.  For example, snow will have a different albedo than vegitation.

Net Solar Radiation
-------------------

This component is design to compute the total amount of solar radiation.  The net radiation is the total of short and long wave radiation.  The short wave radiation is determined by the canopy reflection coefficient and the incoming solar radiation.  For this component, the canopy coefficient will be assumed to be a constant value of 0.23 because the variables that characterize the canopy coefficient, such as the time of day, angle of exposure and surface vegetation, vary only slighty.  The long wave radiation is determined using standarized equations for long wave radiation, cloudiness factor and relative solar radiation.

This is the net solar radiation equation.

.. raw:: latex html

	\[R_n = S_n + L_n\]

.. Note::

	Where :raw-latex:`\((R_n)\)` is the net radiation, :raw-latex:`\((S_n)\)` is the short wave radiation, and :raw-latex:`\((L_n)\)` is the long wave radiation.

The following equation is used to determine the short wave radiation.

.. raw:: latex html

	\[S_n = (1-\alpha)R_s\]

.. Note::

	Where :raw-latex:`\((S_n)\)` is the short wave radiation, :raw-latex:`\((\alpha)\)` is the canopy coefficient held constant at 0.23, and :raw-latex:`\((R_s)\)` is the incoming solar radiation.

The following equation is used to determine the long wave radiation.

.. raw:: latex html

	\[R_s = T_fR_a\]

.. Note::

	Where :raw-latex:`\((R_s)\)` is the incoming solar radiation, :raw-latex:`\((T_f)\)` is the atmospheric transmittance, and :raw-latex:`\((R_a)\)` is the extraterrestrial radiation.

The following equation is used to find the extraterrestrial radation.

.. raw:: latex html

	\[L_n = \frac{-c_f\sigma(0.34-0.14\sqrt{e_a})(T_{kmax}^4+T_{kmin}^4)}{2}\]

.. Note::

	Where :raw-latex:`\((L_n)\)` is the long wave radiation, :raw-latex:`\((c_f)\)` is the cloudiness factor, :raw-latex:`\((\sigma)\)` is the Stefan-Boltzmann constant, :raw-latex:`\((T_{kmax})\)` is the maximum temperature, and :raw-latex:`\((T_{kmin})\)` is the minimum temperature. 

The following equation is used to find the cloudiness factor.

.. raw:: latex html

	\[c_f = 1.35\frac{R_s}{R_{s0}}-0.35\]

.. Note::

	Where :raw-latex:`\((c_f)\)` is the cloudiness factor, :raw-latex:`\((R_s)\)` is the incoming solar radiation, and :raw-latex:`\((R_{s0})\)` is the clear sky radiation.

The following equation is used to determine the relative solar radiation and can have values that range from 0.3 to 1.0.

.. raw:: latex html

	\[\frac{R_s}{R_{s0}} = \frac{T_f}{0.75+2\times10^{-5}z}\]

.. Note::

	Where :raw-latex:`\((R_s)\)` is the incoming solar radiation, :raw-latex:`\((R_{s0})\)` is the clear sky radiation, and :raw-latex:`\((T_f)\)` is the atmospheric transmittance.
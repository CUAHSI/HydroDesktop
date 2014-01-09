.. index:: Evapotranspiration


.. role:: raw-latex(raw)
    :format: latex html

.. raw:: html

	<script type="text/javascript" src="http://cdn.mathjax.org/mathjax/latest/MathJax.js?config=TeX-AMS-MML_HTMLorMML"> </script>


Evapotranspiration
==================

Evapotranspiration is a hydrological process that describes the loss of surface water to the atmosphere by means of evaporation from soil and water bodies as well as transpiration from vegitation.

Hargreaves
----------

The Hargreaves component is the simplest of the three methods used to compute evapotranspiration.  This method requires only two inputs, temperature and solar radiation; both of which are easily obtainable sets of data.  Although this method requires less initial information, it can be found to be as accurate as Penman Monteith and Preistly Taylor given that the model spans a larger time frame.

This is the Hargreaves equation.

.. raw:: latex html

	\[ET_0 = 0.0135(KT)(R_a)(\sqrt{TD})(TC+17.8)\]

.. Note::

	Where :raw-latex:`\((ET_0)\)` is the evapotranspiration, :raw-latex:`\((KT)\)` is the temperature reduction coefficient, :raw-latex:`\((R_a)\)` is the incoming extraterrestrial radiation, :raw-latex:`\((TD)\)` is the difference of the maximum and minimum daily temperatures, and :raw-latex:`\((TC)\)` is the average daily temperature.


Penman Monteith
---------------

Penman Monteith is an American Society of Civil Engineering (ASCE) recognized method for determining evapotranspiration.  This component is designed off of a variation of the ASCE Penman Monteith approximation referred to as the standarized reference evapotranspiration.  This component takes a number of variables into consideration, ranging from time series data, which includes temperature and windspeed, and geospatial data, which includes land cover and elevation, to evaporation from surfaces and vegitation.  This component is primarily used for smaller scale models where input data is readily available.  Net radiation, air temperature, humidy, wind speed and air pressure are some of the required known variables for Penman Monteith to compute the evapotranspiration. 

This is the Penman Monteith equation.

.. raw:: latex html

	\[ET_{sz} = \frac{\frac{1}{\lambda\rho_w}\Delta(R_n-G)+\gamma\frac{C_n}{T+273}u_2(e_s-e_a)}{\Delta+\gamma(1+C_du_2)}\]

.. Note::

	Where :raw-latex:`\((ET_{sz})\)` is the standardized evapotranspiration, :raw-latex:`\((R_n)\)` is the net radiation, :raw-latex:`\((G)\)` is the soil heat flux, :raw-latex:`\((T)\)` is the daily average temperature, :raw-latex:`\((u_2)\)` is the daily average wind speed, :raw-latex:`\((e_s)\)` is the saturation vapor pressure, :raw-latex:`\((e_a)\)` is the mean actual vapor pressure, :raw-latex:`\((\Delta)\)` is the slope of the saturation vapor pressure - temperature curve, :raw-latex:`\((\gamma)\)` is the psychometric constant, :raw-latex:`\((C_n)\)` and :raw-latex:`\((C_d)\)` are constants based on crop reference type and simulation time step, :raw-latex:`\((\lambda)\)` is the latent heat of vaporization, and :raw-latex:`\((\rho_w)\)` is water density.

Priestly Taylor
---------------

Priestly Taylor is a component that calculates evapotranspirtation similarly to Penman Monteith.  However, this method requires less initial data and is therefore the more desirable technique when looking at a broader land span.  Because energy balance has a greater effect on evapotranspiration on larger areas, the Penman Monteith equation can be reduced by the assumption that the aerodynamic properties are attributed to approximately 30% of the net radiation's influence on the total evapotranspiration.  This assumption varies slighty when looking from one region to the next, but allows the user to effectively compute evapotranspiration with a minimal amount of required data.

This is the combination method for computing evaporation that was developed by Penman.

.. raw:: latex html

	\[E = \frac{\Delta}{\Delta+\gamma}E_r+\frac{\gamma}{\Delta+\gamma}E_a\]

.. Note::

	Where :raw-latex:`\((E)\)` is the weighted evaporation, :raw-latex:`\((\Delta)\)` is the gradient of the saturated vapor pressure curve, :raw-latex:`\((\gamma)\)` is the psychrometric constant, :raw-latex:`\((E_r)\)` is the evaporation rate determined from the net radiation, and :raw-latex:`\((E_a)\)` is the evaporation rate determined from the aerodynamic method.

The following equation can be derived from the previous, using the aforementioned assumption.

.. raw:: latex html

	\[E = \alpha\frac{\Delta}{\Delta+\gamma}E_r\]

.. Note::

	Where :raw-latex:`\((E)\)` is the weighted evaporation, :raw-latex:`\((\alpha)\)` is a constant held at 1.3, :raw-latex:`\((\Delta)\)` is the gradient of the saturated vapor pressure curve, :raw-latex:`\((\gamma)\)` is the psychrometric constant, and :raw-latex:`\((E_r)\)` is the evaporation rate determined from the net radiation.
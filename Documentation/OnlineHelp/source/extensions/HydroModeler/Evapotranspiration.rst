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


Penman Monteith
---------------

Penman Monteith is an American Society of Civil Engineering (ASCE) recognized method for determining evapotranspiration.  This component is designed off of a variation of the ASCE Penman Monteith approximation referred to as the standarized reference evapotranspiration.  This component takes a number of variables into consideration, ranging from time series data, which includes temperature and windspeed, and geospatial data, which includes land cover and elevation, to evaporation from surfaces and vegitation.  This component is primarily used for smaller scale models where input data is readily available.  Net radiation, air temperature, humidy, wind speed and air pressure are some of the required known variables for Penman Monteith to compute the evapotranspiration. 

This is the Penman Monteith equation.

.. raw:: latex html

	\[ET_{sz} = \frac{\frac{1}{\lambda\rho_w}\Delta(R_n-G)+\gamma\frac{C_n}{T+273}u_2(e_s-e_a)}{\Delta+\gamma(1+C_du_2)}\]


Priestly Taylor
---------------

Priestly Taylor is a component that calculates evapotranspirtation similarly to Penman Monteith.  However, this method requires less initial data and is therefore the more desirable technique when looking at a broader land span.  Because energy balance has a greater effect on evapotranspiration on larger areas, the Penman Monteith equation can be reduce by the assumption that the aerodynamic properties are attributed to approximately 30% of the net radiation's inflence on the total evapotranspiration.  This assumption varies slighty when looking from one region to the next, but allows the user to effectively compute evapotranspiration with a minimal amount of required data.

This is the Priestly Taylor equation.

.. raw:: latex html

	\[E = \alpha\frac{\Delta}{\Delta+\gamma}E_r\]
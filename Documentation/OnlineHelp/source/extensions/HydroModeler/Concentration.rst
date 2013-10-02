.. index:: Concentration


.. role:: raw-latex(raw)
    :format: latex html

.. raw:: html

	<script type="text/javascript" src="http://cdn.mathjax.org/mathjax/latest/MathJax.js?config=TeX-AMS-MML_HTMLorMML"> </script>


Concentration
=============

These components were created to find the concentrations of a contaminant in soil and water at any given timestep.  They were designed to model the concentration of a contaminant in a river or channel.

Water Advection and Sediment Diffusion
--------------------------------------

These comonents were designed assuming a state where a water layer resides above a sediment column and, as a result, must work in tandem using a bidirectional link because of the shared boundary condition.  Assuming a pollutant is introduced, we can model it's dilution based on the advection-diffusion equation.  Advection describes the horizontal displacement of the contaminant, governed by the flow characteristics of the river, where as diffusion describes the infiltration of the conaminant to or from the soil bed, moving from areas of higher concentration to lower concentration.  Assuming that the primary means of transportation are advection in the x direction and diffusion in the z direction, the equations for the concentration of the water and the concentration of the sediment column are derived from the advection-diffusion equation. Using these equations, we are able to estimate the concentration of pollutant, at any given time step, that is present in the water or in the soil.   


The two components are derived from this equation.

.. raw:: latex html

	\[\frac{\partial C}{\partial t} = D_x \frac{\partial^2C}{\partial x^2}+D_z \frac{\partial^2C}{\partial z^2} - u \frac{\partial C}{\partial x} - v \frac{\partial C}{\partial z}\]


The following equation was used to model the Water Advection Component.

.. raw:: latex html

	\[\frac{\partial C_w}{\partial t} = D_{w,z} \frac{\partial^2C_w}{\partial z^2} - v \frac{\partial C_w}{\partial x}\]


The following equation was used to model the Sediment Diffusion Component.

.. raw:: latex html

	\[\frac{\partial C_s}{\partial t} = D_{s,z} \frac{\partial^2C_s}{\partial z^2}\]
.. index:: Routing

Routing
=======

Routing is the determination of the strength and time span of water flow using upstream points to help approximate these values.

Muskingum Method
----------------

This component will transform flow data through a channel network at each subbasin to a watershed outlet.  The method behind this component is derived from a variable discharge-storage relationship that is comprised of wedge and prism volumes.  The wedge shape accounts for the wave effect caused in moving water, which is controlled by a weighing factor that ranges from 0 to 0.5, while the prism shape accounts for the water residing in the cross sectional area of the channel, which is weighted by proportionality constants, the time it takes a flood wave to move through the channel.  By looking at these pieces we are able to determine the total storage of the channel.
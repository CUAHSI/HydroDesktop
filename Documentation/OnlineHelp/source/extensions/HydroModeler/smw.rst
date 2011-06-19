.. index:: smw

SMW
===

*Simple Model Wrapper*. Simple Model Wrapper (or SMW) is an approach for creating OpenMI model components that simplifies the OpenMI interface in order to create new components more easily by reducing the amount of code required.  The Simple Model Wrapper will reduce the required number of methods from 19 to 3.  When designing your components, they will consist of three methods, called Initialize, Preform Time Step, and Finish.  Initialize will set up the model with initial conditions, units, and system parameters.  Perform Time Step will take all the initialized information and process it to output your calculated values.  It will return these values so that they may be accessed by other components.  The Finish method will close out any files opened to retrieve any information, for example, the configuration file.  This method helps to conserve computer memory and allow processes to run faster.  SMW is described in more detail in the following journal article.

Castronova, A. M., and J. L. Goodall (2010), A generic approach for developing process-level hydrologic modeling components, Environmental Modelling & Software, 25(7), 819-825, http://dx.doi.org/10.1016/j.envsoft.2010.01.003. 
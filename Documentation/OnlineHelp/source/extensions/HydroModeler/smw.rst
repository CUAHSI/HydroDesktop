.. index:: smw

SMW
===

*Simple Model Wrapper*. This is an approach for creating OpenMI model components that simplifies the OpenMI interface, which in turn, allows you to create your own components with ease by reducing the amount of code required.  The Simple Model Wrapper will reduce the required number of methods from 19 to 3.  When designing your components, they will consist of three methods, called Initialize, Preform Time Step, and Finish.  Initialize will set up the model with initial conditions, units, and system parameters.  Perform Time Step will take all the initialized information and process it to output your calculated values.  It will return these values so that they may be accessed by other components.  The Finish method will close out any files opened to retrieve any information, for example, the configuration file.  This method helps to conserve computer memory and allow processes to run faster.
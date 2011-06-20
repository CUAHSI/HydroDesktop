.. index:: HydroModeler

HydroModeler
============

This help system for HydroModeler provides a brief introduction to HydroModeler, documentation for various general help topics associated with using HydroModeler, tutorials for using and extending HydroModeler, an explanation of the sample configurations installed with HydroModeler, and a glossary of common terms used in association with HydroModeler.  


Introduction
------------

HydroModeler is a HydroDesktop plugin that extends its capabilities to include component-based model development. HydroModeler is based on an open-source model editor provided by the OpenMI Association Technical Committee (OATC) Configuration Editor version 1.4. Its integration within the HydroDesktop environment provides the ability to both retrieve data from remote sources and to utilize this data in a model simulation. This help document includes tutorials that show how to run the installed example configurations and how to extend HydroModeler through the developement of new model components.

HydroModeler adpots the Open Modeling Interface (OpenMI) standard for model coupling.  This allows any OpenMI-compliant model to be used within the application. Tutorial 3 outlines how to create a new model component.  It is also possible to follow the development strategy outlined in the OpenMI document series which can be found at http://openmi.com . The basic functionality of HydroModeler includes the ability to open and save model configurations, adding and removing components and connections, and executing a model configuration.  Output data can be written to the HydroDesktop database so that it can be viewed and edited using other HydroDesktop plug-ins (e.g., Graph View, Edit View, HydroR, etc.)


Help Topics
-----------

.. toctree::
   :maxdepth: 2
   
   OpeningOprFile
   
   LinkingModels
   
   RunningComposition

   ChangingInputs


Tutorials
---------

.. toctree::
   :maxdepth: 2
   
   Tutorial01
   
   Tutorial02
   
   Tutorial03


Example Configurations
----------------------

.. toctree::
   :maxdepth: 2

   ExConfig1

   ExConfig2

   ExConfig3


Glossary
--------

.. toctree::
   :maxdepth: 2

   config

   dll

   omi

   opr

   shp

   smw
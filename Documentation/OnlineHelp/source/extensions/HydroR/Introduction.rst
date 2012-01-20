.. index:: Introduction


Introduction
============

.. index:: 
  single: Introduction
  
HydroR is a HydroDesktop extension that provides an interface between HydroDesktop and the R statistical computing environment.  In a nutshell, R is a language and environment for statistical computing and graphics.  R provides a wide variety of statistical and graphical techniques and is highly extensible.  R is available as free software, and can be downloaded from http://www.r-project.org/.

HydroR provides a scripting interface within which R commands can be written and then sent to the R Console for execution.  HydroR provides functionality for automatically generating the R code needed to retrieve any of the hydrologic time series data that have been downloaded and stored within HydroDesktop's data repository database into an R List object.  The R list object contains all of the metadata for a time series and a Data Frame object that contains the time series data values.  One the data have been transferred from the HydroDesktop database into an R List object, they can be manipulated using any of the graphical or statistical tools that R, or any extended R packages, provides.

HydroR includes a package for R called "HydroR."  This package is installed the first time you open the R Console from the HydroR extension.  Once installed, this R package can be used independently from HydroDesktop given some information about the syntax used in the R commands that are included in the HydroR package.

.. index:: 
  single: Prerequisites
  
Prerequisites
---------------------

Before using the HydroDesktop HydroR extension, you must download and install the latest version of R from http://www.r-project.org/. 

 

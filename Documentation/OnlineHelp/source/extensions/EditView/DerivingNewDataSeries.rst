.. index:: Deriving New Data Series

Deriving New Data Series
======================================================
  
The Edit View extension provides some functionality for deriving new data series from existing data series in the HydroDesktop database.  You can access this functionality by selecting an existing data series and then clicking on the "Derive Series" button on the Edit View ribbon.  The following are the options for deriving new series:

	* Derive a Copy of Data For Editing - this option will create an exact copy of an existing data series so that you can edit the copy and preserve the original.
	* Derive Using an Aggregate Function - this option will derive a new data series by aggregating an existing data series to a new time step (e.g., daily, monthly, quarterly) using a selected statistic (e.g., maximum, minimum, average, sum). 
	* Derive Using an Algebraic Equation - this option will derive a new data series from an existing data series by applying a user defined algebraic transformation to each data value.

Use the following steps to derive a new data series using Edit View:

1. If you have not already, select a data series in the Series Selection tool.
2. Click the "Derive Series" button on the Edit View tab.  The following window will appear.

.. figure:: ./images/Edit_image014.png
  :align: center

3. Select the derivation options that you want to apply by selecting the appropriate radio buttons.
4. Modify any parameters at the bottom of the form that need to be modified.  If you need to create a new method, variable, or quality control level, select "New Method," "New Variable," or "New QualityControlLevel" from the drop down lists at the bottom of the form.  The example in the figure above will create an exact copy of the data series, maintaining the same method description, variable description, and quality control level description.
5. When you have selected all of the appropriate options on the form, click the "New Data Series" button to derive the new data series.

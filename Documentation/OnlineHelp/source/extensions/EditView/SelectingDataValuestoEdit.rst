.. index:: Selecting Data Values to Edit

Selecting Data Values to Edit
=====================================================
  
Before data values within a data series can be edited, they must first be selected.  You can select individual data values by clicking on their associated record in the tabular view at the bottom of the Edit View tab.  When you do so, you will notice that the corresponding point on the plot changes color from black to red.  Additionally, the Edit View extension provides several filters that can be used for selecting multiple data values that you would like to edit.  These include the following:

	* Value Threshold Filter - use this filter to select all data values greater than or less than a given threshold or between two threshold values.  Useful for selecting out of range values for interpolation.
	* Data Gaps Filter - use this filter to select data values where the time gap between two values is greater than a threshold.  Useful for finding data gaps.
	* Date Filter - use this filter to select all data values greater than or less than a give date or between two dates.  Useful for selecting blocks of data for deletion.
	* Value Change Threshold Filter - use this filter to select all data values where the change from one value to the next is greater than some threshold value.  Useful for detecting anomalies in time series.

The following is an example of how to apply a value threshold data filter:

1. If you have not already, select a data series for editing and click the "Start Editing" button on the Edit View ribbon.  
2. Type a value into one of the threshold value boxes and then click the "Apply Filter" button. In the example below, a filter has been applied to select dissolved oxygen data values greater than 15 mg/L, which are obvious out of range values that need to be edited.

.. figure:: ./images/Edit_image003.png
  :align: center 


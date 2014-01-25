.. index:: Interpolating Selected Data Values

Interpolating Selected Data Values
=====================================================
  
The Edit View extension provides a function for linearly interpolating data values based on the numeric values of the data values that come before and after the selected data values in time.  This can be useful for correcting anomalies and obvious out of range values within a dataset.  Use the following steps to interpolate selected data values:

1. If you have not already, select a data series for editing and click the "Start Editing" button on the Edit View ribbon.
2. Use the data filters or click on records in the Edit View table to select the data values that you would like to modify.  In the following example, all dissolved oxygen data values greater than 15 mg/L have been selected using a value threshold filter as it is known that these are obvious out of range data values that need to be corrected.

.. figure:: ./images/Edit_image006.png
  :align: center 

3. Click the "Interpolate" button on the Edit View tab.  A window will pop up asking you if you are sure that you want to Interpolate.  Click the "Yes" button.  After a moment you will notice that the data values you selected have now been linearly interpolated and that the scale of your plot has been changed to reflect the new extents of your data.

.. figure:: ./images/Edit_image007.png
  :align: center 
# Series
The series represents a sequence of data values associated with a specific site, variable, method, source and quality control levels. The values can be measured observations or model simulation result values. 
## Constructor

## Properties
* **Site** The [Site](Site) associated with this time series. For a new series, it is required to specify the site. The site has a geographic location and additional information about the location. There may be multiple series available at one site.

* **Variable** The [Variable](Variable) associated with this time series. For a new series, it is required to specify the variable. The variable has units, time support information and additional information.

* **Method** The [Method](Method) used to measure, derive or simulate the data values in the series. The default value of this property is Method.Unknown.

* **QualityControlLevel** The [QualityControlLevel](QualityControlLevel) of the data values in the series. This property indicates whether the data values have been controlled for quality. If the quality control level is unknown, set this property to QualityControlLevel.Unknown.

* **Source** The [Source](Source) of the data values in the series. This includes information about the organization responsible for observing, deriving or calculating the values. If the data source is unknown, set this property to Source.Unknown.

* **DataValueList** The list of [DataValue](Data-Values) in this series. The data values are sorted by ascending time. Each data value has information about time and value as well as optional information about special observation conditions. The Time property of each data value must be unique. To add a new data value to the series, use the **AddDataValue()** method.

* **HasDataValues** Returns true if the series contains data values. Returns false if the series does not contain any data values.

* **ValueCount** The number of data values in this series

## Methods

## Sample Code
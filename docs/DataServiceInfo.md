# DataServiceInfo
Represents information about a WaterOneFlow web service. The WaterOneFlow web services are web services hosted at different servers which provide information about hydrological or meteorological observations in the WaterML data format. Some of the web services are registered in the [HIS Central](http://water.sdsc.edu/hiscentral/webservices/hiscentral.asmx) web service catalog. 
## Constructor

## Properties
* **Abstract** Optional more detailed description of the web service

* **Citation** (Optional) The official citation of the web service

* **ContactName** (Optional) The official contact name

* **ContactEmail** (Optional) The official contact email

* **DescriptionURL** The optional URL of the web site where this web service is documented

* **EndpointURL** The base URL of the web service. The url should not contain the ?wsdl parameter.

* **HISCentralID** If the web service is registered on HIS Central, the HISCentralID is the unique identifier assigned by HIS Central to this web service. This property is only used for web services registered by HIS Central. Please note: Sometimes HIS Central uses 'NetworkID' as a synonym for 'ServiceID'.

* **ServiceCode** The code of the web service. This property is only used by web services registered by HIS Central.

* **ServiceName** The official name of the web service. For most WaterOneFlow web services, the service name is **WaterOneFlow**. For the HIS Central metadata catalog web service, the service name is **hiscentral**.

* **ServiceType**

* **Version** The version of the web service. If the service is a WaterOneFlow service, use '1.0' for WaterOneFlow 1.0 and '1.1' for WaterOneFlow 1.1.

* **Protocol** The communication protocol used by the web service. Set this value to **SOAP** for WaterOneFlow and HIS Central. Some other services may use the more simple **REST** protocol.

* **NorthLatitude**

* **SouthLatitude**

* **EastLongitude**

* **WestLongitude**

* **Title** An optional brief description of the web service

* **ValueCount** (Optional) The total number of data values available at the web service. This property is only available for WaterOneFlow web services which are registered at HIS Central.

* **SiteCount** (Optional) The total number of sites available at the web service. this property is only available for WaterOneFlow web services which are registered at HIS Central.

* IsHarvested

* HarveDateTime

## Methods

## Sample Code
# Version 1.0 Task List

Major development tasks underway for HydroDesktop, including assignments indicating who is working on what. "Task items" are based on the descriptions in the [Functional Specifications](Functional-Specifications) document.

|| Task Item || Status || Responsible Entities || 
| **Core Functionality** |
| 3.1.1. Data Discovery Using the HIS Central Metadata Catalog | Initial version complete | ISU | 
| 3.1.2. Data Discovery Directly From WaterOneFlow Web Services | Initial version complete | UT | 
| 3.1.3. Data Discovery for Thematic Datasets | Not started | UT|  
| 3.1.4. Processing of Search Results | Initial version complete | ISU |  
| 3.2.1. Downloading Observational Data | Initial version complete | ISU |  
| 3.2.2. Downloading GIS Datasets | Not started. Need to define this more. Perhaps this is the WMS/WFS plugin? Add Google Earth, Virtual Earth, and/or ESRI ArcGIS online | UT|  
| 3.3.1. Visualization and Analysis of Spatial Data | Essentially complete through use of MW 6.0 components | ISU |  
| 3.3.2. Visualization and Analysis of Observational Data | First version graphing plug-in complete. | USU |  
| 3.4.1. Importing and Exporting Spatial Datasets | Importing GIS data from local files works. Need to add WMS/WFS service and exporting of themes and NetCDF support. | ISU using GDAL/MapWindow |  
| 3.4.2. Importing and Exporting Observational Datasets | Exporting - initial version complete (UT), Importing - under construction (USU) | UT/USU |  
| 3.5. Project Workspace | Not started | ISU |  
| 3.6. Plug-in Interface | Base interface fairly stable. Proposed changes include improving tab management and streamlined HIS database access through abstraction layer (see B.1. below) | ISU |  
| **Additional Described Plug-ins** | 
| A.1. Data Series Transformation Plug-in | Extract capabilities from ODM tools and use them here?  | USU?  |  
| A.2. NetCDF/UNIDATA Data Provider Plug-in | Not Started | None |  
| A.3. OpenMI Modeling Plug-in | Loosely integrated version done... working on more tightly integrated version? | SC |  
| A.4. Workflow Plug-in | Not Started - potentially do as a Trident based tool in collaboration with CSIRO? | CSIRO? |  
| A.5. Spatial Data Discovery and Download Plug-in | Not Started | ? |  
| A.6. BASINS Data Downloader | Not Started | ?|  
| A.7. Scripting Plug-in | USU working on (HydroR) | USU |  
| **Specific Additional Tasks** |
| B.1. Database abstraction layer | Under construction  | ISU/SDSC/UT |  
| **Additional Tasks Associated with INRA ICEWATER** |
| C.1. Security for Data Publication  | Not Started  | ISU (contributions from Montana?) |  
| C.2. QA/QC Data Checking and Editing| Not Started | ISU/USU|
| C.3. Data posting/publishing from HydroDesktop to server | Not Started | ISU|
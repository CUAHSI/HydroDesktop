[Getting HydroDesktop](Getting-HydroDesktop), [Presentations and Publications](Presentations-and-Publications), [Functional Specifications](Functional-Specifications), [Database Structure](Database-Structure), [Sample Data](Sample-Data), [Workshops and Training](Workshops-and-Training)
----
# HydroDesktop Database Structure

HydroDesktop uses two local databases to store hydrologic observation data and metadata. 

The local metadata cache is a database that is populated with search results from both HIS Central and from specific HIS servers. This local metadata cache is searchable using the data discovery plugin.

The observations database is the heart of HydroDesktop and contains raw data observations that are downloaded from HIS servers, or imported from local data. 

[Database Schemas](Database-Schemas) for the Data Repository database

**Default Database Engine**

By default HydroDesktop uses the SQLite Database Engine. You can learn more about it here: [http://sqlite.org/](http://sqlite.org/)
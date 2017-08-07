# HydroDesktop Database Abstraction Layer
The library HydroDesktop.Data.dll contains built-in functions for accessing the HydroDesktop database. To use the build-in functions in a plug-in, add a reference to HydroDesktop.data.dll

The class code should have the following using statements:
using HydroDesktop.Database
using HydroDesktop.ObjectModel

Important classes in HydroDesktop.Data.dll

## Config
This is a static class with current database connection information. 
* **DataRepositoryConnectionString** - Connection string of the currently used HydroDesktop data repository database
* **DataRepositoryOperations** - The DbOperations helper class for accessing the HydroDesktop data repository database
* **SQLRepositoryManager** - The DataManagerSQL helper class for saving values to the HydroDesktop data repository database
* **ThemeManager** - this class can be used for working with themes that are shown in the map.
## DbOperations
Contains methods for accessing the database using SQL queries
* **ExecuteNonQuery(string sqlString)** - Executes a SQL statement. This can be used for INSERT and DELETE statements.
* **LoadTable(string sqlQuery)** - Based on a SQL query, returns a DataTable that matches the query results
* **ExecuteSingleOutput(string sqlString)** - Executes a SQL query with a single output value. Returns the value of the first column of the first matching row of the query result.

## Sample Code
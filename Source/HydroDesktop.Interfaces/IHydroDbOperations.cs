using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Commonly used database operations
    /// These don't depend on the specific database type
    /// </summary>
    public interface IHydroDbOperations
    {
        

        /// <summary>
        /// The database provider factory currently used
        /// </summary>
        DbProviderFactory DbFactory { get; }
        

        /// <summary>
        /// Error message displayed in case of unsuccessful connection
        /// </summary>
        string ErrorMessage { get; }
        

        /// <summary>
        /// Get the database connection string
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Database type (supported type is SQLite)
        /// </summary>
        DatabaseTypes DatabaseType { get; }

        /// <summary>
        /// Test if we are able to connect to the database specified in the constructor
        /// </summary>
        /// <returns>true if connection successful, false otherwise</returns>
        bool TestConnection();
        

        /// <summary>
        /// Creates a new instance of a database connection
        /// </summary>
        /// <returns>the database connection</returns>
        DbConnection CreateConnection();
        

        /// <summary>
        /// Creates a new instance of a database command
        /// </summary>
        /// <param name="txtQuery">the SQL query</param>
        /// <returns>the database command</returns>
        DbCommand CreateCommand(string txtQuery);
        

        /// <summary>
        /// Creates a new command parameter with the specified data type
        /// </summary>
        /// <param name="parameterType">the parameter data type</param>
        /// <returns>The parameter object</returns>
        DbParameter CreateParameter(DbType parameterType);
        

        /// <summary>
        /// Creates a new instance of a database command parameter with the specified name 
        /// and data type
        /// </summary>
        /// <param name="name">the parameter name</param>
        /// <param name="parameterType">the parameter data type</param>
        /// <returns>the database command parameter</returns>
        DbParameter CreateParameter(string name, DbType parameterType);
        

        /// <summary>
        /// Creates a new dB command parameter with the specified name and value
        /// </summary>
        /// <param name="parameterType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        DbParameter CreateParameter(DbType parameterType, object value);
        
        /// <summary>
        /// Adds a parameter to an existing command
        /// </summary>
        DbParameter AddParameter(DbCommand cmd, string parameterName, DbType parameterType);
        

        /// <summary>
        /// Executes a SQL statement without returning any results.
        /// This is used for INSERT or DELETE statements.
        /// </summary>
        /// <param name="sqlString">the SQL string</param>
        void ExecuteNonQuery(string sqlString);
        

        /// <summary>
        /// Executes a SQL command without returning any results.
        /// This is used for INSERT or DELETE commands.
        /// </summary>
        /// <param name="cmd">the database command</param>
        void ExecuteNonQuery(DbCommand cmd);
        

        /// <summary>
        /// Executes an SQL command without returning any results.
        /// This is used for INSERT or DELETE commands. The parameter values
        /// can be supplied in the parameter array
        /// </summary>
        /// <param name="txtQuery">the SQL query (parameters should be marked as
        /// ? or @)</param>
        /// <param name="parameterValues">the values of the command parameters</param>
        void ExecuteNonQuery(string txtQuery, object[] parameterValues);
        

        /// <summary>
        /// Generates an SQL Insert command for the given table name.
        /// We assume that the first column is the identifier column.
        /// </summary>
        /// <param name="tableName">name of the database table</param>
        /// <param name="table">corresponding DataTable object</param>
        /// <returns>the insert sql string (parametric query)</returns>
        string GenerateInsertCommand(string tableName, DataTable table);
        

        /// <summary>
        /// Inserts the content of the data table back to database. If a row already exists that has
        /// the unique fields, then an update is done instead of an insert. The primary key (ID) values are modified
        /// to reflect their values in the database
        /// </summary>
        /// <param name="tableName">name of the table in the database</param>
        /// <param name="primaryKey">the name of the primary key column</param>
        /// <param name="table">In-memory Datatable. This table must have exactly same structure as the database table</param>
        /// <param name="uniqueFields">an array of all field names that define an unique key ('business key')</param>
        void SaveTable(string tableName, DataTable table, string primaryKey, string[] uniqueFields);
        

        /// <summary>
        /// Creates a new database command with an array of parameters
        /// </summary>
        /// <param name="sqlString">the SQL string</param>
        /// <param name="numParameters">the number of command parameters</param>
        /// <returns>a new instance of a database command object</returns>
        DbCommand CreateCommand(string sqlString, int numParameters);
        

        /// <summary>
        /// Get the next auto-incremented (primary key) ID
        /// </summary>
        int GetNextID(string tableName, string primaryKeyName);
        

        /// <summary>
        /// Based on a SQL query, returns a data table with all rows that
        /// match the query results
        /// </summary>
        /// <param name="sqlQuery">the sql query statement</param>
        /// <returns>The resulting data table</returns>
        DataTable LoadTable(string sqlQuery);

        /// <summary>
        /// Updates the existing in-memory data table object by 
        /// the results of the SQL query
        /// </summary>
        /// <param name="existingTable">the existing table</param>
        /// <param name="sqlQuery">the SQL query</param>
        DataTable LoadTable(string sqlQuery, DataTable existingTable);


        /// <summary>
        /// Based on a SQL query, returns a data table with all rows that
        /// match the query results
        /// </summary>
        /// <param name="tableName">name of the resulting data table</param>
        /// <param name="sqlQuery">the SQL query string</param>
        /// <returns>The resulting data table</returns>
        DataTable LoadTable(string tableName, string sqlQuery);

        /// <summary>
        /// Gets the collection of entities
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="query">Query to select any data</param>
        /// <param name="rowReader">Delegate that converted row into entity</param>
        /// <returns>Collection of entities</returns>
        List<T> Read<T>(string query, Func<DbDataReader, T> rowReader);
        

        /// <summary>
        /// Based on a database command, returns a data table with all rows that
        /// match the query results
        /// </summary>
        /// <param name="tableName">name of the resulting data table</param>
        /// <param name="cmd">the database command (with parameter values set)</param>
        /// <returns>The resulting data table</returns>
        DataTable LoadTable(string tableName, DbCommand cmd);
        

        /// <summary>
        /// Creates a data adapter for the specified data table
        /// </summary>
        /// <param name="tableName">name of the table in the database</param>
        /// <param name="primaryKeyName">name of primary key column</param>
        /// <returns>a new data adapter associated with the table</returns>
        DbDataAdapter CreateDataAdapter(string tableName, string primaryKeyName);
        

        /// <summary>
        /// Executes an SQL query with a single output value
        /// </summary>
        /// <param name="inputString">the SQL query string</param>
        /// <param name="parameters">the values of command parameters</param>
        /// <returns>the query result (value of first matching row and column)</returns>
        object ExecuteSingleOutput(string inputString, params object[] parameters);
        
        
        /// <summary>
        /// returns a DataTable object that has the same schema as
        /// the user-specified table with the name
        /// </summary>
        /// <param name="tableName">name of the table in the database</param>
        /// <returns>an empty dataTable with the same column names and types</returns>
        DataTable GetTableSchema(string tableName);
        
    }
}

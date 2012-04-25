using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Diagnostics;
using HydroDesktop.Interfaces;
using System.Globalization;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Commonly used database operations
    /// </summary>
    public class DbOperations : IHydroDbOperations
    {
        private readonly DbProviderFactory dbFactory;
        
        private readonly string _connectionString;
        private string _errorMessage = "";

        private readonly Stopwatch _sw = new Stopwatch();
        /// <summary>
        /// Creates a new instance of the dbOperations object. 
        /// </summary>
        /// <param name="connectionString">the connection string</param>
        /// <param name="databaseType">the type of the DBMS (SQLite, SQLServer)</param>
        public DbOperations(string connectionString, DatabaseTypes databaseType)
        {
            _connectionString = connectionString;
            if (databaseType == DatabaseTypes.SQLite)
            {            
                dbFactory = new System.Data.SQLite.SQLiteFactory(); 
            }
            else if (databaseType == DatabaseTypes.SQLServer)
            {
                dbFactory = SqlClientFactory.Instance;
            }
        }

        /// <summary>
        /// The database provider factory currently used
        /// </summary>
        public DbProviderFactory DbFactory
        {
            get { return dbFactory; }
        }

        /// <summary>
        /// Error message displayed in case of unsuccessful connection
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        /// <summary>
        /// Get the database connection string
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
        }
        /// <summary>
        /// Database type (supported type is SQLite)
        /// </summary>
        public DatabaseTypes DatabaseType
        {
            get
            {
                if (dbFactory is System.Data.SQLite.SQLiteFactory)
                {
                    return DatabaseTypes.SQLite;
                }
                if (dbFactory is SqlClientFactory)
                {
                    return DatabaseTypes.SQLServer;
                }
                return DatabaseTypes.Unknown;
            }
        }

        

        /// <summary>
        /// Test if we are able to connect to the database specified in the constructor
        /// </summary>
        /// <returns>true if connection successful, false otherwise</returns>
        public bool TestConnection()
        {     
            DbConnection connection1 = CreateConnection();

            try
            {
                connection1.Open();
                const string commandText = "SELECT * FROM DataThemeDescriptions";
                var da = dbFactory.CreateDataAdapter();
                da.SelectCommand = dbFactory.CreateCommand();
                da.SelectCommand.CommandText = commandText;
                da.SelectCommand.Connection = connection1;
                DataTable dt = new DataTable();
                dt.TableName = "table";
                da.Fill(dt);
                
                return true;
            }
            catch (Exception ex)
            {
                _errorMessage = "Failed Test Connection connectionstring{" + connection1.ConnectionString + "} error" + ex.Message;

                return false;
            }
            finally
            {
                connection1.Close();
                connection1.Dispose();
            }
        }

        /// <summary>
        /// Creates a new instance of a database connection
        /// </summary>
        /// <returns>the database connection</returns>
        public DbConnection CreateConnection()
        {
            DbConnection conn = dbFactory.CreateConnection();
            conn.ConnectionString = _connectionString;
            return conn;
        }

        /// <summary>
        /// Creates a new instance of a database command
        /// </summary>
        /// <param name="txtQuery">the SQL query</param>
        /// <returns>the database command</returns>
        public DbCommand CreateCommand(string txtQuery)
        {
            DbCommand cmd = dbFactory.CreateCommand();
            cmd.Connection = CreateConnection();
            cmd.CommandText = txtQuery;
            return cmd;
        }

        /// <summary>
        /// Creates a new command parameter with the specified data type
        /// </summary>
        /// <param name="parameterType">the parameter data type</param>
        /// <returns>The parameter object</returns>
        public DbParameter CreateParameter(DbType parameterType)
        {
            DbParameter param = dbFactory.CreateParameter();
            param.DbType = parameterType;
            param.Direction = ParameterDirection.Input;
            return param;
        }

        /// <summary>
        /// Creates a new instance of a database command parameter with the specified name 
        /// and data type
        /// </summary>
        /// <param name="name">the parameter name</param>
        /// <param name="parameterType">the parameter data type</param>
        /// <returns>the database command parameter</returns>
        public DbParameter CreateParameter(string name, DbType parameterType)
        {
            DbParameter param = dbFactory.CreateParameter();
            param.ParameterName = name;
            param.DbType = parameterType;
            param.Direction = ParameterDirection.Input;
            return param;
        }

        /// <summary>
        /// Creates a new dB command parameter with the specified name and value
        /// </summary>
        /// <param name="parameterType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DbParameter CreateParameter(DbType parameterType, object value)
        {
            DbParameter param = dbFactory.CreateParameter();
            //param.ParameterName = name;
            param.DbType = parameterType;
            param.Direction = ParameterDirection.Input;
            param.Value = value;
            return param;
        }

        /// <summary>
        /// Adds a parameter to an existing command
        /// </summary>
        public DbParameter AddParameter(DbCommand cmd, string parameterName, DbType parameterType)
        {
            DbParameter param = dbFactory.CreateParameter();
            param.ParameterName = parameterName;
            param.DbType = parameterType;
            param.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(param);
            return param;
        }

        /// <summary>
        /// Executes a SQL statement without returning any results.
        /// This is used for INSERT or DELETE statements.
        /// </summary>
        /// <param name="sqlString">the SQL string</param>
        public void ExecuteNonQuery(string sqlString)
        {
            DbConnection conn = CreateConnection();
            conn.Open();

            DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sqlString;

            cmd.ExecuteNonQuery();
            conn.Close();
            cmd.Dispose();
            conn.Dispose();
        }

        /// <summary>
        /// Executes a SQL command without returning any results.
        /// This is used for INSERT or DELETE commands.
        /// </summary>
        /// <param name="cmd">the database command</param>
        public void ExecuteNonQuery(DbCommand cmd)
        {         
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

        /// <summary>
        /// Executes an SQL command without returning any results.
        /// This is used for INSERT or DELETE commands. The parameter values
        /// can be supplied in the parameter array
        /// </summary>
        /// <param name="txtQuery">the SQL query (parameters should be marked as
        /// ? or @)</param>
        /// <param name="parameterValues">the values of the command parameters</param>
        public void ExecuteNonQuery(string txtQuery, object[] parameterValues)
        {
            DbConnection conn = CreateConnection();
            conn.Open();

            DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = txtQuery;

            for (int p = 0; p < parameterValues.Length; p++)
            {
                DbParameter param = dbFactory.CreateParameter();
                param.Value = parameterValues[p];
                param.DbType = GetDbTypeFromValue(parameterValues[p]);
                param.ParameterName = "@p" + p.ToString(CultureInfo.InvariantCulture);
                cmd.Parameters.Add(param);
            }

            cmd.ExecuteNonQuery();
            conn.Close();
            cmd.Dispose();
            conn.Dispose();
        }

        private DbType GetDbTypeFromValue(object value)
        {
            if (value is long)
                return DbType.Int64;
            if (value is int)
                return DbType.Int32;
            if (value is double)
                return DbType.Double;
            if (value is DateTime)
                return DbType.DateTime;
            if (value is string)
                return DbType.String;
            return DbType.String;
        }

        /// <summary>
        /// Executes a batch SQL insert / update statement. The SQL string should have parameters
        /// marked as '?'. Number of parameters needs to be the same as the parameterValues[][]
        /// 2d-array row length
        /// </summary>
        /// <param name="sqlString">the sql insert statement(should use parameters)</param>
        /// <param name="parameterValues">the values of sql parameters</param>
        /// <returns>The array of new primary keys</returns>
        private void ExecuteBatchSQL(string sqlString, object[][] parameterValues)
        {
            if (parameterValues.Length == 0) return;
            
            //create the command
            DbCommand cmd1 = null;
            DbConnection conn = CreateConnection();

            try
            {
                
                conn.Open();
                cmd1 = conn.CreateCommand();
                cmd1.CommandText = sqlString;
                int numParameters = parameterValues[0].Length;

                for (int i = 0; i < numParameters; i++)
                {
                    cmd1.Parameters.Add(dbFactory.CreateParameter());
                }

                //execute the command for each item
                for (int r = 0; r < parameterValues.Length; r++)
                {
                    for (int c = 0; c < parameterValues[r].Length; c++)
                    {
                        cmd1.Parameters[c].Value = parameterValues[r][c];
                    }
                    cmd1.ExecuteNonQuery();
                }
            }
            finally
            {
                if (cmd1 != null) cmd1.Dispose();
                conn.Close();
                conn.Dispose();
            }
        }

        /// <summary>
        /// Executes a multiple-row insert or update operation (similar to DataAdapter.Update())
        /// </summary>
        /// <param name="sqlString">the sql string (should be an insert or an update command)</param>
        /// <param name="rowIndices">the indices of data rows to be inserted or updated</param>
        /// <param name="primaryKey">the primary key column name</param>
        /// <param name="table">the local copy of the data table</param>
        private void ExecuteBatchSQL(string sqlString, IList<int> rowIndices, string primaryKey, DataTable table)
        {
            if (rowIndices.Count == 0) return;

            //create the command
            using (DbConnection conn = CreateConnection())
            {
                conn.Open();

                using (DbTransaction tran = conn.BeginTransaction())
                {
                    bool isInsertCommand = false;
                    if (sqlString.ToLower().StartsWith("insert"))
                    {
                        isInsertCommand = true;
                        sqlString += ";select last_insert_rowid()";
                    }

                    //get the indices of parameters to be inserted or updated
                    int[] columnIndices;
                    if (isInsertCommand)
                    {
                        columnIndices = new int[table.Columns.Count - 1];
                        for (int c = 0; c < table.Columns.Count - 1; c++)
                        {
                            columnIndices[c] = c + 1;
                        }
                    }
                    else
                    {
                        columnIndices = new int[table.Columns.Count];
                        for (int c = 0; c < table.Columns.Count - 1; c++)
                        {
                            columnIndices[c] = c + 1;
                        }
                        columnIndices[columnIndices.Length - 1] = table.Columns.IndexOf(primaryKey);
                    }

                    
                    using (DbCommand cmd1 = conn.CreateCommand())
                    {
                        //cmd1 = conn.CreateCommand();
                        cmd1.CommandText = sqlString;
                        int numParameters = columnIndices.Length;

                        for (int i = 0; i < numParameters; i++)
                        {
                            cmd1.Parameters.Add(dbFactory.CreateParameter());
                        }

                        //assign the correct parameter types
                        for (int c = 0; c < columnIndices.Length; c++)
                        {
                            int colIndex = columnIndices[c];
                            if (table.Columns[colIndex].DataType == typeof(DateTime))
                            {
                                cmd1.Parameters[c].DbType = DbType.DateTime;
                            }
                        }

                        //execute the command for each item
                        for (int r = 0; r < rowIndices.Count; r++)
                        {
                            int rowIndex = rowIndices[r];

                            //populate command parameter values
                            for (int c = 0; c < columnIndices.Length; c++)
                            {
                                int columnIndex = columnIndices[c];

                                cmd1.Parameters[c].Value = table.Rows[rowIndex][columnIndex];
                            }

                            if (isInsertCommand)
                            {
                                //if it's an insert command then the value of the primary key
                                //in the in-memory data table gets modified.
                                table.Rows[rowIndex][primaryKey] = cmd1.ExecuteScalar();
                            }
                            else
                            {
                                //if it's an update command then nothing gets modified in the in-memory data table
                                cmd1.ExecuteNonQuery();
                            }
                        }
                    }

                    //commit the transaction
                    tran.Commit();
                    
                }//end the transaction
            }//dispose the connection
        }

        /// <summary>
        /// Generates an SQL Insert command for the given table name.
        /// We assume that the first column is the identifier column.
        /// </summary>
        /// <param name="tableName">name of the database table</param>
        /// <param name="table">corresponding DataTable object</param>
        /// <returns>the insert sql string (parametric query)</returns>
        public string GenerateInsertCommand(string tableName, DataTable table)
        {
            var sql = new StringBuilder("insert into " + tableName + " (");
            var sqlValues = new StringBuilder(" values(");

            for(int c = 1; c < table.Columns.Count - 1; c++)
            {
                sql.Append(table.Columns[c].ColumnName);
                sql.Append(",");
                sqlValues.Append("?,");
            }
            sql.Append(table.Columns[table.Columns.Count - 1]);
            sql.Append(") ");
            sqlValues.Append("?)");
            sql.Append(sqlValues);
            return sql.ToString();
        }

        /// <summary>
        /// Generates an SQL Update command for the given table name.
        /// </summary>
        /// <param name="tableName">name of the database table</param>
        /// <param name="primaryKeyName">name of the primary key column</param>
        /// <param name="table">corresponding DataTable object</param>
        /// <returns>the update sql string (parametric query)</returns>
        private string GenerateUpdateCommand(string tableName, string primaryKeyName, DataTable table)
        {
            string sqlUpdate = "update " + tableName + " set ";

            for(int c = 1; c < table.Columns.Count - 1; c++)
            {
                sqlUpdate += table.Columns[c].ColumnName + "=?, ";
            }
            sqlUpdate += table.Columns[table.Columns.Count - 1] + "=? where " + primaryKeyName + "=?";

            return sqlUpdate;
        }
        /// <summary>
        /// Creates an unique query command object
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="primaryKeyName">primary key name</param>
        /// <param name="uniqueFields">list of unique columnhs</param>
        /// <returns>the DB command object that can be used for running the query</returns>
        private string GenerateUniqueQueryCommand(string tableName, string primaryKeyName, string[] uniqueFields)
        {         
            string uniqueSQL = "select " + primaryKeyName + " from " + tableName + " where ";
           
            for (int i = 0; i < uniqueFields.Length - 1; i++)
            {
                uniqueSQL += uniqueFields[i] + "=? AND ";
            }
            uniqueSQL += uniqueFields[uniqueFields.Length - 1] + "=?";

            return uniqueSQL;
        }
        

        /// <summary>
        /// Inserts the content of the data table back to database. If a row already exists that has
        /// the unique fields, then an update is done instead of an insert. The primary key (ID) values are modified
        /// to reflect their values in the database
        /// </summary>
        /// <param name="tableName">name of the DataTable</param>
        /// <param name="primaryKey">the name of the primary key column</param>
        /// <param name="table">In-memory Datatable. This table must have exactly same structure as the database table</param>
        /// <param name="uniqueFields">an array of all field names that define an unique key ('business key')</param>
        public void SaveTable(string tableName, DataTable table, string primaryKey, string[] uniqueFields)
        {
            int nr = table.Rows.Count;

            //create an 'insert command'
            string sqlInsert = GenerateInsertCommand(tableName, table);
            string sqlUpdate = GenerateUpdateCommand(tableName, primaryKey, table);

            //integer array with new values of primary keys
            int[] primaryKeys = new int[nr];
            int numRowsToInsert = 0;
            int numRowsToUpdate = 0;

            if (uniqueFields != null)
            {
                using (DbConnection conn = CreateConnection())
                {
                    //to select unique id's
                    string sqlUnique = GenerateUniqueQueryCommand(tableName, primaryKey, uniqueFields);

                    //for each row in the table find if there is an existing database row with the same
                    //unique identifier
                    conn.Open();

                    using (DbCommand uniqueCmd = conn.CreateCommand())
                    {
                        uniqueCmd.CommandText = sqlUnique;
                        uniqueCmd.Connection = conn;
                        for (int p = 0; p < uniqueFields.Length; p++)
                        {
                            DbParameter param = dbFactory.CreateParameter();
                            param.ParameterName = uniqueFields[p];
                            uniqueCmd.Parameters.Add(param);
                        }

                        for (int r = 0; r < nr; r++)
                        {
                            for (int p = 0; p < uniqueFields.Length; p++)
                            {
                                uniqueCmd.Parameters[p].Value = table.Rows[r][uniqueFields[p]];
                            }

                            object uniqueID = uniqueCmd.ExecuteScalar();
                            if (uniqueID != null)
                            {
                                primaryKeys[r] = Convert.ToInt32(uniqueID);
                                //change the ID value in the table
                                table.Rows[r][primaryKey] = primaryKeys[r];
                                numRowsToUpdate++;
                            }
                            else
                            {
                                primaryKeys[r] = 0;
                                numRowsToInsert++;
                            }
                        }
                    } //dispose the command
                } //dispose the Connection

            }
            else
            {
                for (int r = 0; r < nr; r++)
                {
                    primaryKeys[r] = 0;
                    numRowsToInsert++;
                }
            }
            
            var insertRowIndices = new List<int>(numRowsToInsert);
            var updateRowIndices = new List<int>(numRowsToUpdate);

            for (int r=0; r<primaryKeys.Length; r++)
            {
                if (primaryKeys[r] > 0)
                {
                    updateRowIndices.Add(r);
                }
                else
                {
                    insertRowIndices.Add(r);
                }
            }
            
            //execute the batch insert command
            if (numRowsToInsert > 0)
            {
                ExecuteBatchSQL(sqlInsert, insertRowIndices, primaryKey, table);
            }

            //execute the batch update command
            if (numRowsToUpdate > 0)
            {
                ExecuteBatchSQL(sqlUpdate, updateRowIndices, primaryKey, table);
            }
        }

        /// <summary>
        /// Creates a new database command with an array of parameters
        /// </summary>
        /// <param name="sqlString">the SQL string</param>
        /// <param name="numParameters">the number of command parameters</param>
        /// <returns>a new instance of a database command object</returns>
        public DbCommand CreateCommand(string sqlString, int numParameters)
        {
            DbConnection conn = CreateConnection();
            conn.Open();
            
            DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sqlString;
            for (int i = 0; i < numParameters; i++)
            {
                DbParameter param = dbFactory.CreateParameter();
                cmd.Parameters.Add(param);
            }
            conn.Close();
            return cmd;
        }

        /// <summary>
        /// Get the next auto-incremented (primary key) ID
        /// </summary>
        public int GetNextID(string tableName, string primaryKeyName)
        {
            DbConnection conn = CreateConnection();
            conn.Open();
            DbCommand cmd = conn.CreateCommand();
              
            cmd.CommandText = "select max(" + primaryKeyName + ") FROM " + tableName;
            
            object obj = cmd.ExecuteScalar();

            conn.Close();
            cmd.Dispose();
            conn.Dispose();

            if (obj.ToString() == "")
            {
                return 1;
            }
            return Convert.ToInt32(obj) + 1;
        }

        /// <summary>
        /// Updates the existing in-memory data table object by 
        /// the results of the SQL query
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public DataTable LoadTable(string sqlQuery, DataTable table)
        {
            var conn = CreateConnection();
            conn.Open();
            var da = dbFactory.CreateDataAdapter();
            da.SelectCommand = dbFactory.CreateCommand();
            da.SelectCommand.CommandText = sqlQuery;
            da.SelectCommand.Connection = conn;
            if (table == null)
            {
                table = new DataTable {TableName = "table"};
            }
            
            da.Fill(table);
            conn.Close();
            return table;
        }

        /// <summary>
        /// Based on a SQL query, returns a data table with all rows that
        /// match the query results
        /// </summary>
        /// <param name="sqlQuery">the SQL query string</param>
        /// <returns>The resulting data table</returns>
        public DataTable LoadTable(string sqlQuery)
        {
            _sw.Reset();
            _sw.Start();
            
            var conn = CreateConnection();
            conn.Open();
            var da = dbFactory.CreateDataAdapter();
            da.SelectCommand = dbFactory.CreateCommand();
            da.SelectCommand.CommandText = sqlQuery;
            da.SelectCommand.Connection = conn;
            var dt = new DataTable();
            dt.TableName = "table";
            da.Fill(dt);
            conn.Close();

            _sw.Stop();
            Debug.WriteLine("LoadTable:" + sqlQuery + " " + _sw.ElapsedMilliseconds + "ms");

            return dt;
        }

        /// <summary>
        /// Based on a SQL query, returns a data table with all rows that
        /// match the query results
        /// </summary>
        /// <param name="tableName">name of the resulting data table</param>
        /// <param name="sqlQuery">the SQL query string</param>
        /// <returns>The resulting data table</returns>
        public DataTable LoadTable(string tableName, string sqlQuery)
        {
            _sw.Reset();
            _sw.Start();
            
            var conn = CreateConnection();
            conn.Open();
            var da = dbFactory.CreateDataAdapter();
            da.SelectCommand = dbFactory.CreateCommand();
            da.SelectCommand.CommandText = sqlQuery;
            da.SelectCommand.Connection = conn;
            var dt = new DataTable {TableName = tableName};
            da.Fill(dt);
            conn.Close();

            _sw.Stop();
            Debug.WriteLine("LoadTable:" + sqlQuery + " " + _sw.ElapsedMilliseconds + "ms");

            return dt;
        }

        public List<T> Read<T>(string query, Func<DbDataReader, T> rowReader, params object[] parameters)
        {
            var result = new List<T>();
            var cmd = CreateCommand(query);
            try
            {
                cmd.Connection.Open();
                for (int p = 0; p < parameters.Length; p++)
                {
                    var param = dbFactory.CreateParameter();
                    Debug.Assert(param != null);
                    param.Value = parameters[p];
                    param.DbType = GetDbTypeFromValue(parameters[p]);
                    param.ParameterName = "@p" + p.ToString(CultureInfo.InvariantCulture);

                    cmd.Parameters.Add(param);
                }

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(rowReader(reader));
                }
                reader.Close();
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Based on a database command, returns a data table with all rows that
        /// match the query results
        /// </summary>
        /// <param name="tableName">name of the resulting data table</param>
        /// <param name="cmd">the database command (with parameter values set)</param>
        /// <returns>The resulting data table</returns>
        public DataTable LoadTable(string tableName, DbCommand cmd)
        {
            bool openCloseConnection = false;
            
            if (cmd.Connection.State != ConnectionState.Open)
            {
                openCloseConnection = true;
                cmd.Connection.Open();
            }

            DbDataAdapter da = dbFactory.CreateDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            dt.TableName = tableName;
            da.Fill(dt);

            if (openCloseConnection == true)
            {
                cmd.Connection.Close();
            }
            return dt;           
        }

        /// <summary>
        /// Creates a data adapter for the specified data table
        /// </summary>
        /// <param name="tableName">name of the table in the database</param>
        /// <param name="primaryKeyName">name of primary key column</param>
        /// <returns>a new data adapter associated with the table</returns>
        public DbDataAdapter CreateDataAdapter(string tableName, string primaryKeyName)
        {
            DbConnection conn = CreateConnection();
            DbDataAdapter da = dbFactory.CreateDataAdapter();
            da.SelectCommand = conn.CreateCommand();
            da.SelectCommand.CommandText = "SELECT * FROM " + tableName;
            
            DbCommandBuilder cmdBuilder = dbFactory.CreateCommandBuilder();
            cmdBuilder.DataAdapter = da;
            
            da.InsertCommand = (DbCommand)((ICloneable)cmdBuilder.GetInsertCommand()).Clone();

            //sqLITE - specific...
            if (dbFactory is System.Data.SQLite.SQLiteFactory)
            {
                string pkColName = 
                da.InsertCommand.CommandText += String.Format(";SELECT last_insert_rowid() AS [{0}]", primaryKeyName);
                da.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
            }
            return da;
        }

        /// <summary>
        /// Executes an SQL query with a single output value
        /// </summary>
        /// <param name="inputString">the SQL query string</param>
        /// <param name="parameters">the values of command parameters</param>
        /// <returns>the query result (value of first matching row and column)</returns>
        public object ExecuteSingleOutput(string inputString, params object[] parameters)
        {
            object output;
            var cmd = CreateCommand(inputString);
            try
            {
                cmd.Connection.Open();     
                cmd.CommandText = inputString;

                for (int p = 0; p < parameters.Length; p++)
                {
                    var param = dbFactory.CreateParameter();
                    Debug.Assert(param != null);
                    param.Value = parameters[p];
                    param.DbType = GetDbTypeFromValue(parameters[p]);
                    param.ParameterName = "@p" + p.ToString(CultureInfo.InvariantCulture);

                    cmd.Parameters.Add(param);
                }

                output = cmd.ExecuteScalar();
                if (output != DBNull.Value)
                {
                    output = Convert.ToString(output, CultureInfo.InvariantCulture);
                }
            }
            catch
            {
                output = null;
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Dispose();
            }

            return output;

        }
        
        /// <summary>
        /// returns a DataTable object that has the same schema as
        /// the user-specified table with the name
        /// </summary>
        /// <param name="tableName">name of the table in the database</param>
        /// <returns>an empty dataTable with the same column names and types</returns>
        public DataTable GetTableSchema(string tableName)
        {
            DataTable table = null;
            DbConnection conn = CreateConnection();
            
            try
            {          
                table = new DataTable();
                
                DbDataAdapter da = dbFactory.CreateDataAdapter();
                
                da.SelectCommand = conn.CreateCommand();
                da.SelectCommand.CommandText = "SELECT * FROM " + tableName;
                da.FillSchema(table, SchemaType.Source);
                
                table = new DataTable();
                da.FillSchema(table, SchemaType.Source);
                table.TableName = tableName;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return table;
        }
    }
}

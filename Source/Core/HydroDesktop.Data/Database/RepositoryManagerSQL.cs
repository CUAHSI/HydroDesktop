using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Helper class for reading and writing HydroDesktop objects to
    /// and from the HydroDesktop data repository SQLite database
    /// </summary>
    class DbRepositoryManagerSQL : BaseRepository<Series>, IRepositoryManager
    {
        #region Constructor
        /// <summary>
        /// Creates a new instance of the manager given a connection string
        /// </summary>
        /// <param name="dbType">The type of the database (SQLite, SQLServer, ...)</param>
        /// <param name="connectionString">The connection string</param>
        public DbRepositoryManagerSQL(DatabaseTypes dbType, string connectionString)
            : base(dbType, connectionString)
        {
            
        }

        /// <summary>
        /// Creates a new RepositoryManager associated with the specified database
        /// </summary>
        /// <param name="db">The DbOperations object for handling the database</param>
        public DbRepositoryManagerSQL(IHydroDbOperations db)
            :base(db)
        {
        }

        #endregion

        #region Properties

        private IHydroDbOperations _db
        {
            get { return DbOperations; }
        }

        protected override string TableName
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Save Series

        /// <summary>
        /// Simplified version of SaveSeries (for HydroForecaster)
        /// </summary>
        /// <param name="siteID">site ID</param>
        /// <param name="variableID">variable ID</param>
        /// <param name="methodDescription">description of method</param>
        /// <param name="themeName">theme name</param>
        /// <param name="dataValues">The table with data values. First column must be DateTime and second column must be Double.</param>
        /// <returns>number of saved data values</returns>
        public int SaveSeries(int siteID, int variableID, string methodDescription, string themeName, DataTable dataValues)
        { 
            string sqlSaveSeries = "INSERT INTO DataSeries(SiteID, VariableID, MethodID, SourceID, QualityControlLevelID, " +
                "IsCategorical, BeginDateTime, EndDateTime, BeginDateTimeUTC, EndDateTimeUTC, ValueCount, CreationDateTime, " +
                "Subscribed, UpdateDateTime, LastCheckedDateTime) " +
                "VALUES(?, ?, ?, ?,?,?,?,?,?,?,?,?,?,?,?)" + LastRowIDSelect;

            string sqlSaveTheme1 = "INSERT INTO DataThemeDescriptions(ThemeName, ThemeDescription) VALUES (?,?)" + LastRowIDSelect;
            string sqlSaveTheme2 = "INSERT INTO DataThemes(ThemeID,SeriesID) VALUEs (?,?)";

            
            int methodID = 0;
            int qualityControlLevelID = 0;
            int sourceID = 0;
            int seriesID = 0;
            long themeID = 0;

            //create the series object
            Series series = new Series();
            series.Variable = new Variable();
            series.Method = Method.Unknown;
            series.Method.Description = methodDescription;

            //to add the data values
            foreach (DataRow row in dataValues.Rows)
            {
                series.AddDataValue(Convert.ToDateTime(row[0]), Convert.ToDouble(row[1]));
            }

            var theme = new Theme(themeName);
            
            int numSavedValues;

            //Step 1 Begin Transaction
            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();

                using (DbTransaction tran = conn.BeginTransaction())
                {
                    methodID = GetOrCreateMethodID(series.Method, conn);
                    qualityControlLevelID = GetOrCreateQualityControlLevelID(series.QualityControlLevel, conn);
                    sourceID = GetOrCreateSourceID(series.Source, conn);
                    
                    using (DbCommand cmd18 = conn.CreateCommand())
                    {
                        cmd18.CommandText = sqlSaveSeries;
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, siteID));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, variableID));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, methodID));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, sourceID));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, qualityControlLevelID));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.Boolean, series.IsCategorical));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.BeginDateTime));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.EndDateTime));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.BeginDateTimeUTC));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.EndDateTimeUTC));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, series.ValueCount));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.CreationDateTime));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.Boolean, series.Subscribed));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.UpdateDateTime));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.LastCheckedDateTime));

                        var seriesIDResult = cmd18.ExecuteScalar();
                        seriesID = Convert.ToInt32(seriesIDResult);
                    }

                    Dictionary<string, Qualifier> qualifierLookup;
                    Dictionary<string, Sample> sampleLookup;
                    Dictionary<string, OffsetType> offsetLookup;
                    GetLookups(series, out qualifierLookup, out sampleLookup, out offsetLookup);

                    SaveQualifiers(conn, qualifierLookup);
                    SaveSamplesAndLabMethods(conn, sampleLookup);
                    SaveOffsets(conn, offsetLookup);
                    numSavedValues = SaveDataValues(conn, seriesID, series.DataValueList);

                    //****************************************************************
                    //*** Step 14 Data Theme                               ***********
                    //****************************************************************
                    var themeIDResult = GetThemeID(conn, theme);
                    if (themeIDResult.HasValue)
                    {
                        themeID = themeIDResult.Value;
                    }
                   
                    if (themeID == 0)
                    {
                        using (DbCommand cmd23 = conn.CreateCommand())
                        {
                            cmd23.CommandText = sqlSaveTheme1;
                            cmd23.Parameters.Add(_db.CreateParameter(DbType.String, theme.Name));
                            cmd23.Parameters.Add(_db.CreateParameter(DbType.String, theme.Description));
                            themeID = Convert.ToInt32(cmd23.ExecuteScalar());
                        }
                    }

                    using (DbCommand cmd24 = conn.CreateCommand())
                    {
                        cmd24.CommandText = sqlSaveTheme2;
                        cmd24.Parameters.Add(_db.CreateParameter(DbType.Int32, themeID));
                        cmd24.Parameters.Add(_db.CreateParameter(DbType.Int32, seriesID));
                        cmd24.ExecuteNonQuery();
                    }

                    //Step 13 Commit Transaction
                    tran.Commit();
                }
                conn.Close();
            }
            return numSavedValues;
        }

        /// <summary>
        /// Saves a data series to the database. The series will be associated with the 
        /// specified theme. Depending on the OverwriteOptions, this will check if the series
        /// already exists in the database and overwrite data values in the database if required. 
        /// </summary>
        /// <param name="seriesToSave">The data series to be saved. This should contain
        /// information about site, variable, method, source and quality control level.</param>
        /// <param name="theme">The theme where this series should belong to</param>
        /// <param name="overwrite">The overwrite options. Set this to 'Copy' if 
        /// a new series should be created in the database. For options other than 'Copy',
        /// some of the existing data values in the database may be overwritten.</param>
        public int SaveSeries(Series seriesToSave, Theme theme, OverwriteOptions overwrite)
        {
            switch (overwrite)
            {
                case OverwriteOptions.Fill:
                case OverwriteOptions.Append:
                    return SaveSeriesAppend(seriesToSave, theme);
                case OverwriteOptions.Copy:
                    return SaveSeriesAsCopy(seriesToSave, theme);
                case OverwriteOptions.Overwrite:
                    return SaveSeriesOverwrite(seriesToSave, theme);
                default:
                    goto case OverwriteOptions.Append;
            }
        }

        /// <summary>
        /// Saves a data series to the database. The series will be associated with the 
        /// specified theme. This method checks whether there are any existing series with 
        /// the same properties in the database. If there are existing series with the same
        /// properties, the new data values are 'appended' to the existing series (no duplicate
        /// series or data values are created)
        /// </summary>
        /// <param name="series">The time series</param>
        /// <param name="theme">The associated theme</param>
        /// <returns>Number of DataValue saved</returns>
        private int SaveSeriesAppend(Series series, Theme theme)
        {
            const string sqlThemeSeries = "SELECT ThemeID FROM DataThemes WHERE ThemeID = ? AND SeriesID = ?";
            string sqlSeries = "SELECT SeriesID, BeginDateTime, BeginDateTimeUTC, EndDateTime, EndDateTimeUTC, ValueCount FROM DataSeries WHERE SiteID = ? AND VariableID = ? AND MethodID = ? AND QualityControlLevelID = ? AND SourceID = ?";
            string sqlSaveSeries = "INSERT INTO DataSeries(SiteID, VariableID, MethodID, SourceID, QualityControlLevelID, " +
                "IsCategorical, BeginDateTime, EndDateTime, BeginDateTimeUTC, EndDateTimeUTC, ValueCount, CreationDateTime, " +
                "Subscribed, UpdateDateTime, LastCheckedDateTime) " +
                "VALUES(?, ?, ?, ?,?,?,?,?,?,?,?,?,?,?,?)" + LastRowIDSelect;

            string sqlSaveTheme1 = "INSERT INTO DataThemeDescriptions(ThemeName, ThemeDescription) VALUES (?,?)" + LastRowIDSelect;
            string sqlSaveTheme2 = "INSERT INTO DataThemes(ThemeID,SeriesID) VALUEs (?,?)";

            string sqlUpdateSeries = "UPDATE DataSeries SET BeginDateTime = ?, BeginDateTimeUTC = ?, EndDateTime = ?, EndDateTimeUTC = ?, " +
                "ValueCount = ?, UpdateDateTime = ? WHERE SeriesID = ?";

            int siteID;
            int variableID;
            int methodID;
            int qualityControlLevelID;
            int sourceID;
            int seriesID;
            long themeID = 0;
            
            object seriesIDResult = null;

            int numSavedValues = 0;

            bool seriesAlreadyExists = false;
            var beginTimeDb = DateTime.MinValue;
            var endTimeDb = DateTime.MinValue;
            var valueCountDb = 0;

            //Step 1 Begin Transaction
            using (var conn = _db.CreateConnection())
            {
                conn.Open();

                using (DbTransaction tran = conn.BeginTransaction())
                {
                    siteID = GetOrCreateSiteID(series.Site, conn);
                    variableID = GetOrCreateVariableID(series.Variable, conn);
                    methodID = GetOrCreateMethodID(series.Method, conn);
                    qualityControlLevelID = GetOrCreateQualityControlLevelID(series.QualityControlLevel, conn);
                    sourceID = GetOrCreateSourceID(series.Source, conn);

                    //****************************************************************
                    //*** Step 7 Series
                    //****************************************************************
                    using (DbCommand cmdSeries = conn.CreateCommand())
                    {
                        //To retrieve the BeginTime, EndTime and SeriesID of the existing series
                        cmdSeries.CommandText = sqlSeries;
                        cmdSeries.Parameters.Add(_db.CreateParameter(DbType.Int32, siteID));
                        cmdSeries.Parameters.Add(_db.CreateParameter(DbType.Int32, variableID));
                        cmdSeries.Parameters.Add(_db.CreateParameter(DbType.Int32, methodID));
                        cmdSeries.Parameters.Add(_db.CreateParameter(DbType.Int32, qualityControlLevelID));
                        cmdSeries.Parameters.Add(_db.CreateParameter(DbType.Int32, sourceID));

                        DbDataReader reader = cmdSeries.ExecuteReader(CommandBehavior.SingleRow);
                        if (reader.HasRows)
                        {
                            seriesIDResult = reader[0];
                            try
                            {
                                beginTimeDb = Convert.ToDateTime(reader[1]);
                                endTimeDb = Convert.ToDateTime(reader[3]);
                                valueCountDb = Convert.ToInt32(reader[5]);
                            }
                            catch { }
                            finally
                            {
                                reader.Close();
                                reader.Dispose();
                            }
                        }
                    }


                    if (seriesIDResult != null && beginTimeDb > DateTime.MinValue && endTimeDb > DateTime.MinValue)
                    {
                        //Case 1: Series Already Exists.
                        seriesAlreadyExists = true;
                        seriesID = Convert.ToInt32(seriesIDResult);

                        //If the series already exists, don't save any data values within the existing series time range
                        //to do this, we remove data values that should not be saved from the DataValueList                    
                        for (int i = series.DataValueList.Count - 1; i >= 0; i--)
                        {
                            DataValue val = series.DataValueList[i];
                            if (val.LocalDateTime >= beginTimeDb && val.LocalDateTime <= endTimeDb)
                            {
                                series.DataValueList.Remove(val);
                            }
                        }
                    }
                    else
                    {
                        //Case 2: Series does not exist.
                        using (DbCommand cmd18 = conn.CreateCommand())
                        {
                            cmd18.CommandText = sqlSaveSeries;
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, siteID));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, variableID));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, methodID));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, sourceID));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, qualityControlLevelID));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.Boolean, series.IsCategorical));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.BeginDateTime));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.EndDateTime));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.BeginDateTimeUTC));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.EndDateTimeUTC));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, series.ValueCount));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.CreationDateTime));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.Boolean, series.Subscribed));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.UpdateDateTime));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.LastCheckedDateTime));

                            seriesIDResult = cmd18.ExecuteScalar();
                            seriesID = Convert.ToInt32(seriesIDResult);
                        }
                    }

                    //**********************************************************************
                    //*** Data Value - related: Only if new data values are saved **********
                    //**********************************************************************
                    if (series.DataValueList.Count > 0)
                    {

                        //****************************************************************
                        //*** Step 8 Qualifier and Sample Lookup
                        //****************************************************************
                        Dictionary<string, Qualifier> qualifierLookup;
                        Dictionary<string, Sample> sampleLookup;
                        Dictionary<string, OffsetType> offsetLookup;
                        GetLookups(series, out qualifierLookup, out sampleLookup, out offsetLookup);
                      
                        SaveQualifiers(conn, qualifierLookup);
                        SaveSamplesAndLabMethods(conn, sampleLookup);
                        SaveOffsets(conn, offsetLookup);
                        numSavedValues = SaveDataValues(conn, seriesID, series.DataValueList);

                        //****************************************************************
                        //*** Step 14 Data Series Update                       ***********
                        //****************************************************************
                        if (seriesAlreadyExists && seriesID > 0)
                        {
                            //begin DateTime
                            DateTime beginDateTime = beginTimeDb;
                            DateTime beginDateTimeUTC = beginTimeDb;
                            if (series.BeginDateTime < beginTimeDb)
                            {
                                beginDateTime = series.BeginDateTime;
                                beginDateTimeUTC = series.BeginDateTimeUTC;
                            }

                            //end DateTime
                            DateTime endDateTime = endTimeDb;
                            DateTime endDateTimeUTC = endTimeDb;
                            if (series.EndDateTime > endTimeDb)
                            {
                                endDateTime = series.EndDateTime;
                                endDateTimeUTC = series.EndDateTimeUTC;
                            }

                            //valueCount and UpdateDateTime
                            int valueCount = valueCountDb + series.ValueCount;
                            DateTime updateDateTime = DateTime.Now;

                            using (DbCommand cmdUpdateSeries = conn.CreateCommand())
                            {
                                cmdUpdateSeries.CommandText = sqlUpdateSeries;
                                cmdUpdateSeries.Parameters.Add(_db.CreateParameter(DbType.DateTime, beginDateTime));
                                cmdUpdateSeries.Parameters.Add(_db.CreateParameter(DbType.DateTime, beginDateTimeUTC));
                                cmdUpdateSeries.Parameters.Add(_db.CreateParameter(DbType.DateTime, endDateTime));
                                cmdUpdateSeries.Parameters.Add(_db.CreateParameter(DbType.DateTime, endDateTimeUTC));
                                cmdUpdateSeries.Parameters.Add(_db.CreateParameter(DbType.Int32, valueCount));
                                cmdUpdateSeries.Parameters.Add(_db.CreateParameter(DbType.DateTime, updateDateTime));
                                cmdUpdateSeries.Parameters.Add(_db.CreateParameter(DbType.Int32, seriesID));

                                cmdUpdateSeries.ExecuteNonQuery();
                            }
                        }
                    } //End of (If DataValueList.Count > 0)

                    //****************************************************************
                    //*** Step 15 Data Theme                               ***********
                    //****************************************************************
                   // string sqlThemeSeries = "SELECT ThemeID FROM DataThemes WHERE ThemeID = ? AND SeriesID = ?";
                   // string sqlSaveTheme1 = "INSERT INTO DataThemeDescriptions(ThemeName, ThemeDescription) VALUES (?,?)" + LastRowIDSelect;
                   // string sqlSaveTheme2 = "INSERT INTO DataThemes(ThemeID,SeriesID) VALUEs (?,?)";
                   // Database changes need to happen here - Eric Hullinger
                    var themeIDResult = GetThemeID(conn, theme);
                    if (themeIDResult.HasValue)
                    {
                        themeID = themeIDResult.Value;
                    }
                    if (themeID == 0)
                    {
                        using (DbCommand cmd23 = conn.CreateCommand())
                        {
                            cmd23.CommandText = sqlSaveTheme1;
                            cmd23.Parameters.Add(_db.CreateParameter(DbType.String, theme.Name));
                            cmd23.Parameters.Add(_db.CreateParameter(DbType.String, theme.Description));
                            themeID = Convert.ToInt32(cmd23.ExecuteScalar());
                        }
                    }

                    //To save the Theme-Series combination (DataThemes DataTable)
                    object seriesThemeCombinationResult;
                    using (DbCommand cmd24 = conn.CreateCommand())
                    {
                        cmd24.CommandText = sqlThemeSeries;
                        cmd24.Parameters.Add(_db.CreateParameter(DbType.Int32, themeID));
                        cmd24.Parameters.Add(_db.CreateParameter(DbType.Int32, seriesID));
                        seriesThemeCombinationResult = cmd24.ExecuteScalar();
                    }

                    if (seriesThemeCombinationResult == null)
                    {
                        using (DbCommand cmd25 = conn.CreateCommand())
                        {
                            cmd25.CommandText = sqlSaveTheme2;
                            cmd25.Parameters.Add(_db.CreateParameter(DbType.Int32, themeID));
                            cmd25.Parameters.Add(_db.CreateParameter(DbType.Int32, seriesID));
                            cmd25.ExecuteNonQuery();
                        }
                    }
                    
                    
                    //Step 13 Commit Transaction
                    tran.Commit();
                }
                conn.Close();
            }

            series.Id = seriesID;

            return numSavedValues;
        }

        /// <summary>
        /// Saves a data series to the database. The series will be associated with the 
        /// specified theme. This method checks whether there is any existing series with 
        /// the same site, variable, method, source and QCLevel in the database. If there 
        /// is an existing series with the same properties, the existing series is deleted
        /// and it is replaced by the new series.
        /// </summary>
        /// <param name="series">The time series</param>
        /// <param name="theme">The associated theme</param>
        /// <returns>Number of DataValue saved</returns>
        private int SaveSeriesOverwrite(Series series, Theme theme)
        {
            const string sqlThemeSeries = "SELECT ThemeID FROM DataThemes WHERE ThemeID = ? AND SeriesID = ?";
            const string sqlSeries = "SELECT SeriesID, BeginDateTime, BeginDateTimeUTC, EndDateTime, EndDateTimeUTC, ValueCount FROM DataSeries WHERE SiteID = ? AND VariableID = ? AND MethodID = ? AND QualityControlLevelID = ? AND SourceID = ?";
            string sqlSaveSeries = "INSERT INTO DataSeries(SiteID, VariableID, MethodID, SourceID, QualityControlLevelID, " +
                "IsCategorical, BeginDateTime, EndDateTime, BeginDateTimeUTC, EndDateTimeUTC, ValueCount, CreationDateTime, " +
                "Subscribed, UpdateDateTime, LastCheckedDateTime) " +
                "VALUES(?, ?, ?, ?,?,?,?,?,?,?,?,?,?,?,?)" + LastRowIDSelect;
            
            string sqlSaveTheme1 = "INSERT INTO DataThemeDescriptions(ThemeName, ThemeDescription) VALUES (?,?)" + LastRowIDSelect;
            string sqlSaveTheme2 = "INSERT INTO DataThemes(ThemeID,SeriesID) VALUEs (?,?)";

            const string sqlUpdateSeries = "UPDATE DataSeries SET BeginDateTime = ?, BeginDateTimeUTC = ?, EndDateTime = ?, EndDateTimeUTC = ?, " +
                                           "ValueCount = ?, UpdateDateTime = ? WHERE SeriesID = ?";

            const string sqlDeleteValues = "DELETE FROM DataValues WHERE SeriesID = ? AND LocalDateTime >= ? AND LocalDateTime <= ?";

            int siteID;
            int variableID;
            int methodID;
            int qualityControlLevelID;
            int sourceID;
            int seriesID;
            long themeID = 0;
            
            object seriesIDResult = null;

            int numSavedValues = 0;

            bool seriesAlreadyExists;
            DateTime beginTimeDb = DateTime.MinValue;
            DateTime endTimeDb = DateTime.MinValue;
            int valueCountDb = 0;

            //Step 1 Begin Transaction
            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();

                using (DbTransaction tran = conn.BeginTransaction())
                {
                    siteID = GetOrCreateSiteID(series.Site, conn);
                    variableID = GetOrCreateVariableID(series.Variable, conn);
                    methodID = GetOrCreateMethodID(series.Method, conn);
                    qualityControlLevelID = GetOrCreateQualityControlLevelID(series.QualityControlLevel, conn);
                    sourceID = GetOrCreateSourceID(series.Source, conn);

                    //****************************************************************
                    //*** Step 7 Series
                    //****************************************************************
                    using (DbCommand cmdSeries = conn.CreateCommand())
                    {
                        //To retrieve the BeginTime, EndTime and SeriesID of the existing series
                        cmdSeries.CommandText = sqlSeries;
                        cmdSeries.Parameters.Add(_db.CreateParameter(DbType.Int32, siteID));
                        cmdSeries.Parameters.Add(_db.CreateParameter(DbType.Int32, variableID));
                        cmdSeries.Parameters.Add(_db.CreateParameter(DbType.Int32, methodID));
                        cmdSeries.Parameters.Add(_db.CreateParameter(DbType.Int32, qualityControlLevelID));
                        cmdSeries.Parameters.Add(_db.CreateParameter(DbType.Int32, sourceID));

                        DbDataReader reader = cmdSeries.ExecuteReader(CommandBehavior.SingleRow);
                        if (reader.HasRows)
                        {
                            //a series already exists:
                            seriesIDResult = reader[0];
                            try
                            {
                                beginTimeDb = Convert.ToDateTime(reader[1]);
                                endTimeDb = Convert.ToDateTime(reader[3]);
                                valueCountDb = Convert.ToInt32(reader[5]);
                            }
                            catch { }
                            finally
                            {
                                reader.Close();
                                reader.Dispose();
                            }
                        }
                    }


                    if (seriesIDResult != null && beginTimeDb > DateTime.MinValue && endTimeDb > DateTime.MinValue)
                    {
                        //Case 1: Series Already Exists.
                        seriesAlreadyExists = true;
                        seriesID = Convert.ToInt32(seriesIDResult);

                        //If the series already exists, delete any values within the existing time range from the database.
                        //also remove all items associated with the data values (qualifiers, samples..)
                        using (DbCommand cmdDeleteValues = conn.CreateCommand())
                        {
                            cmdDeleteValues.CommandText = sqlDeleteValues;
                            cmdDeleteValues.Parameters.Add(_db.CreateParameter(DbType.Int32, seriesID));
                            cmdDeleteValues.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.BeginDateTime));
                            cmdDeleteValues.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.EndDateTime));
                            var deletedCount = cmdDeleteValues.ExecuteNonQuery();
                            valueCountDb -= deletedCount; // Correct valueCount
                        }
                    }
                    else
                    {
                        //Case 2: Series does not exist.
                        seriesAlreadyExists = false;
                        using (DbCommand cmd18 = conn.CreateCommand())
                        {
                            cmd18.CommandText = sqlSaveSeries;
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, siteID));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, variableID));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, methodID));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, sourceID));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, qualityControlLevelID));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.Boolean, series.IsCategorical));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.BeginDateTime));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.EndDateTime));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.BeginDateTimeUTC));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.EndDateTimeUTC));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, series.ValueCount));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.CreationDateTime));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.Boolean, series.Subscribed));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.UpdateDateTime));
                            cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.LastCheckedDateTime));

                            seriesIDResult = cmd18.ExecuteScalar();
                            seriesID = Convert.ToInt32(seriesIDResult);
                        }
                    }

                    //**********************************************************************
                    //*** Data Value - related: Only if new data values are saved **********
                    //**********************************************************************
                    if (series.DataValueList.Count > 0)
                    {

                        //****************************************************************
                        //*** Step 8 Qualifier and Sample Lookup
                        //****************************************************************
                        Dictionary<string, Qualifier> qualifierLookup;
                        Dictionary<string, Sample> sampleLookup;
                        Dictionary<string, OffsetType> offsetLookup;
                        GetLookups(series, out qualifierLookup, out sampleLookup, out offsetLookup);
                     
                        SaveQualifiers(conn, qualifierLookup);
                        SaveSamplesAndLabMethods(conn, sampleLookup);
                        SaveOffsets(conn, offsetLookup);
                        numSavedValues = SaveDataValues(conn, seriesID, series.DataValueList);

                        //****************************************************************
                        //*** Step 14 Data Series Update                       ***********
                        //****************************************************************
                        if (seriesAlreadyExists && seriesID > 0)
                        {
                            //begin DateTime
                            DateTime beginDateTime = beginTimeDb;
                            DateTime beginDateTimeUTC = beginTimeDb;
                            if (series.BeginDateTime < beginTimeDb)
                            {
                                beginDateTime = series.BeginDateTime;
                                beginDateTimeUTC = series.BeginDateTimeUTC;
                            }

                            //end DateTime
                            DateTime endDateTime = endTimeDb;
                            DateTime endDateTimeUTC = endTimeDb;
                            if (series.EndDateTime > endTimeDb)
                            {
                                endDateTime = series.EndDateTime;
                                endDateTimeUTC = series.EndDateTimeUTC;
                            }

                            //valueCount and UpdateDateTime
                            int valueCount = valueCountDb + series.ValueCount;
                            DateTime updateDateTime = DateTime.Now;

                            using (DbCommand cmdUpdateSeries = conn.CreateCommand())
                            {
                                cmdUpdateSeries.CommandText = sqlUpdateSeries;
                                cmdUpdateSeries.Parameters.Add(_db.CreateParameter(DbType.DateTime, beginDateTime));
                                cmdUpdateSeries.Parameters.Add(_db.CreateParameter(DbType.DateTime, beginDateTimeUTC));
                                cmdUpdateSeries.Parameters.Add(_db.CreateParameter(DbType.DateTime, endDateTime));
                                cmdUpdateSeries.Parameters.Add(_db.CreateParameter(DbType.DateTime, endDateTimeUTC));
                                cmdUpdateSeries.Parameters.Add(_db.CreateParameter(DbType.Int32, valueCount));
                                cmdUpdateSeries.Parameters.Add(_db.CreateParameter(DbType.DateTime, updateDateTime));
                                cmdUpdateSeries.Parameters.Add(_db.CreateParameter(DbType.Int32, seriesID));

                                cmdUpdateSeries.ExecuteNonQuery();
                            }
                        }
                    } //End of (If DataValueList.Count > 0)

                    //****************************************************************
                    //*** Step 15 Data Theme                               ***********
                    //****************************************************************

                    var themeIDResult = GetThemeID(conn, theme);
                    if (themeIDResult.HasValue)
                    {
                        themeID = themeIDResult.Value;
                    }
                    if (themeID == 0)
                    {
                        using (DbCommand cmd23 = conn.CreateCommand())
                        {
                            cmd23.CommandText = sqlSaveTheme1;
                            cmd23.Parameters.Add(_db.CreateParameter(DbType.String, theme.Name));
                            cmd23.Parameters.Add(_db.CreateParameter(DbType.String, theme.Description));
                            themeID = Convert.ToInt32(cmd23.ExecuteScalar());
                        }
                    }

                    //To save the Theme-Series combination (DataThemes DataTable)
                    object seriesThemeCombinationResult;
                    using (DbCommand cmd24 = conn.CreateCommand())
                    {
                        cmd24.CommandText = sqlThemeSeries;
                        cmd24.Parameters.Add(_db.CreateParameter(DbType.Int32, themeID));
                        cmd24.Parameters.Add(_db.CreateParameter(DbType.Int32, seriesID));
                        seriesThemeCombinationResult = cmd24.ExecuteScalar();
                    }

                    if (seriesThemeCombinationResult == null)
                    {
                        using (DbCommand cmd25 = conn.CreateCommand())
                        {
                            cmd25.CommandText = sqlSaveTheme2;
                            cmd25.Parameters.Add(_db.CreateParameter(DbType.Int32, themeID));
                            cmd25.Parameters.Add(_db.CreateParameter(DbType.Int32, seriesID));
                            cmd25.ExecuteNonQuery();
                        }
                    }


                    //Step 13 Commit Transaction
                    tran.Commit();
                }
                conn.Close();
            }
            series.Id = seriesID;

            return numSavedValues;
        }

        /// <summary>
        /// Saves a data series to the database. The series will be associated with the 
        /// specified theme. This method does not check whether there are any existing series with 
        /// the same properties in the database. It will always create a new 'copy' of the series
        /// </summary>
        /// <param name="series">The time series</param>
        /// <param name="theme">The associated theme</param>
        /// <returns>Number of DataValue saved</returns>
        private int SaveSeriesAsCopy(Series series, Theme theme)
        {
            string sqlSaveSeries = "INSERT INTO DataSeries(SiteID, VariableID, MethodID, SourceID, QualityControlLevelID, " +
                "IsCategorical, BeginDateTime, EndDateTime, BeginDateTimeUTC, EndDateTimeUTC, ValueCount, CreationDateTime, " +
                "Subscribed, UpdateDateTime, LastCheckedDateTime) " +
                "VALUES(?, ?, ?, ?,?,?,?,?,?,?,?,?,?,?,?)" + LastRowIDSelect;
            string sqlSaveTheme1 = "INSERT INTO DataThemeDescriptions(ThemeName, ThemeDescription) VALUES (?,?)" + LastRowIDSelect;
            string sqlSaveTheme2 = "INSERT INTO DataThemes(ThemeID,SeriesID) VALUEs (?,?)";

            int siteID = 0;
            int variableID = 0;
            int methodID = 0;
            int qualityControlLevelID = 0;
            int sourceID = 0;
            int seriesID = 0;
            long themeID = 0;
            
            object seriesIDResult = null;
            
            int numSavedValues = 0;
            
            //Step 1 Begin Transaction
            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                
                using (DbTransaction tran = conn.BeginTransaction())
                {
                    siteID = GetOrCreateSiteID(series.Site, conn);
                    variableID = GetOrCreateVariableID(series.Variable, conn);
                    methodID = GetOrCreateMethodID(series.Method, conn);
                    qualityControlLevelID = GetOrCreateQualityControlLevelID(series.QualityControlLevel, conn);
                    sourceID = GetOrCreateSourceID(series.Source, conn);
                   
                    //****************************************************************
                    //*** Step 7 Series
                    //****************************************************************
                    using (DbCommand cmd18 = conn.CreateCommand())
                    {
                        cmd18.CommandText = sqlSaveSeries;
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, siteID));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, variableID));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, methodID));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, sourceID));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, qualityControlLevelID));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.Boolean, series.IsCategorical));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.BeginDateTime));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.EndDateTime));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.BeginDateTimeUTC));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.EndDateTimeUTC));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, series.ValueCount));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.CreationDateTime));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.Boolean, series.Subscribed));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.UpdateDateTime));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.LastCheckedDateTime));

                        seriesIDResult = cmd18.ExecuteScalar();
                        seriesID = Convert.ToInt32(seriesIDResult);
                    }


                    //****************************************************************
                    //*** Step 8 Qualifier and Sample Lookup
                    //****************************************************************
                    Dictionary<string, Qualifier> qualifierLookup;
                    Dictionary<string, Sample> sampleLookup;
                    Dictionary<string, OffsetType> offsetLookup;
                    GetLookups(series, out qualifierLookup, out sampleLookup, out offsetLookup);
                    
                    SaveQualifiers(conn, qualifierLookup);
                    SaveSamplesAndLabMethods(conn, sampleLookup);
                    SaveOffsets(conn, offsetLookup);
                    numSavedValues = SaveDataValues(conn, seriesID, series.DataValueList);

                    //****************************************************************
                    //*** Step 14 Data Theme                               ***********
                    //****************************************************************
                    var themeIDResult = GetThemeID(conn, theme);
                    if (themeIDResult.HasValue)
                    {
                        themeID = themeIDResult.Value;
                    }
                    if (themeID == 0)
                    {
                        using (DbCommand cmd23 = conn.CreateCommand())
                        {
                            cmd23.CommandText = sqlSaveTheme1;
                            cmd23.Parameters.Add(_db.CreateParameter(DbType.String, theme.Name));
                            cmd23.Parameters.Add(_db.CreateParameter(DbType.String, theme.Description));
                            themeID = Convert.ToInt32(cmd23.ExecuteScalar());
                        }
                    }

                    using (DbCommand cmd24 = conn.CreateCommand())
                    {
                        cmd24.CommandText = sqlSaveTheme2;
                        cmd24.Parameters.Add(_db.CreateParameter(DbType.Int32, themeID));
                        cmd24.Parameters.Add(_db.CreateParameter(DbType.Int32, seriesID));
                        cmd24.ExecuteNonQuery();
                    }

                    //Step 13 Commit Transaction
                    tran.Commit();
                }
                conn.Close();
            }
            series.Id = seriesID;

            return numSavedValues;
        }
       
        #endregion
      
        #region Private methods

        private void GetLookups(Series series, out Dictionary<string, Qualifier> qualifierLookup,
                                             out Dictionary<string, Sample> sampleLookup,
                                             out Dictionary<string, OffsetType> offsetLookup)
        {
            qualifierLookup = new Dictionary<string, Qualifier>();
            sampleLookup = new Dictionary<string, Sample>();
            offsetLookup = new Dictionary<string, OffsetType>();

            foreach (var val in series.DataValueList)
            {
                if (val.Qualifier != null)
                {
                    if (!qualifierLookup.ContainsKey(val.Qualifier.Code))
                    {
                        qualifierLookup.Add(val.Qualifier.Code, val.Qualifier);
                    }
                }

                if (val.Sample != null)
                {
                    if (!sampleLookup.ContainsKey(val.Sample.LabSampleCode))
                    {
                        sampleLookup.Add(val.Sample.LabSampleCode, val.Sample);
                    }
                }
                if (val.OffsetType != null)
                {
                    if (!offsetLookup.ContainsKey(val.OffsetType.Description))
                    {
                        offsetLookup.Add(val.OffsetType.Description, val.OffsetType);
                    }
                }
            }
        }

        private int GetOrCreateMethodID(Method method, DbConnection conn)
        {
            const string sqlMethod = "SELECT MethodID FROM Methods WHERE MethodDescription = ?";
            string sqlSaveMethod = "INSERT INTO Methods(MethodDescription, MethodLink) VALUES(?, ?)" + LastRowIDSelect;

            int methodID = 0;
            if (method != null)
            {
                using (DbCommand cmd10 = conn.CreateCommand())
                {
                    cmd10.CommandText = sqlMethod;
                    cmd10.Parameters.Add(_db.CreateParameter(DbType.String, method.Description));
                    var methodIDResult = cmd10.ExecuteScalar();
                    if (methodIDResult != null)
                    {
                        methodID = Convert.ToInt32(methodIDResult);
                    }
                }

                if (methodID == 0)
                {
                    using (DbCommand cmd11 = conn.CreateCommand())
                    {
                        cmd11.CommandText = sqlSaveMethod;
                        cmd11.Parameters.Add(_db.CreateParameter(DbType.String, method.Description));
                        cmd11.Parameters.Add(_db.CreateParameter(DbType.String, method.Link));
                        var methodIDResult = cmd11.ExecuteScalar();
                        methodID = Convert.ToInt32(methodIDResult);
                    }
                }
            }

            return methodID;
        }

        private int GetOrCreateQualityControlLevelID(QualityControlLevel qc, DbConnection conn)
        {
            const string sqlQuality = "SELECT QualityControlLevelID FROM QualityControlLevels WHERE Definition = ?";
            string sqlSaveQualityControl = "INSERT INTO QualityControlLevels(QualityControlLevelCode, Definition, Explanation) " +
               "VALUES(?,?,?)" + LastRowIDSelect;

            int qualityControlLevelID = 0;
            if (qc != null)
            {
                using (DbCommand cmd12 = conn.CreateCommand())
                {
                    cmd12.CommandText = sqlQuality;
                    cmd12.Parameters.Add(_db.CreateParameter(DbType.String, qc.Definition));
                    var qualityControlLevelIDResult = cmd12.ExecuteScalar();
                    if (qualityControlLevelIDResult != null)
                    {
                        qualityControlLevelID = Convert.ToInt32(qualityControlLevelIDResult);
                    }
                }

                if (qualityControlLevelID == 0)
                {
                    using (DbCommand cmd13 = conn.CreateCommand())
                    {
                        cmd13.CommandText = sqlSaveQualityControl;
                        cmd13.Parameters.Add(_db.CreateParameter(DbType.String, qc.Code));
                        cmd13.Parameters.Add(_db.CreateParameter(DbType.String, qc.Definition));
                        cmd13.Parameters.Add(_db.CreateParameter(DbType.String, qc.Explanation));
                        var qualityControlLevelIDResult = cmd13.ExecuteScalar();
                        qualityControlLevelID = Convert.ToInt32(qualityControlLevelIDResult);
                    }
                }
            }
            return qualityControlLevelID;
        }

        private int GetOrCreateSiteID(Site site, DbConnection conn)
        {
            const string sqlSite = "SELECT SiteID FROM Sites WHERE SiteCode = ?";
            const string sqlSpatialReference = "SELECT SpatialReferenceID FROM SpatialReferences WHERE SRSID = ? AND SRSName = ?";

            string sqlSaveSpatialReference = "INSERT INTO SpatialReferences(SRSID, SRSName) VALUES(?, ?)" + LastRowIDSelect;
            var sites = _db.GetTableSchema("Sites");

            var sqlSaveSite = (sites.Columns.Contains("Country") && sites.Columns.Contains("SiteType"))
                                  ? "INSERT INTO Sites(SiteCode, SiteName, Latitude, Longitude, LatLongDatumID, Elevation_m, VerticalDatum, " +
                                    "LocalX, LocalY, LocalProjectionID, PosAccuracy_m, State, County, Comments, Country, SiteType) " +
                                    "VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)" + LastRowIDSelect
                                  : "INSERT INTO Sites(SiteCode, SiteName, Latitude, Longitude, LatLongDatumID, Elevation_m, VerticalDatum, " +
                                    "LocalX, LocalY, LocalProjectionID, PosAccuracy_m, State, County, Comments) " +
                                    "VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)" + LastRowIDSelect;

            int siteID = 0;
            int spatialReferenceID = 0;
            int localProjectionID = 0;

            using (DbCommand cmd01 = conn.CreateCommand())
            {
                cmd01.CommandText = sqlSite;
                cmd01.Parameters.Add(_db.CreateParameter(DbType.String, site.Code));
                var siteIDResult = cmd01.ExecuteScalar();
                if (siteIDResult != null)
                {
                    siteID = Convert.ToInt32(siteIDResult);
                }
            }

            if (siteID == 0) //New Site needs to be created
            {
                using (DbCommand cmd02 = conn.CreateCommand())
                {
                    cmd02.CommandText = sqlSpatialReference;
                    cmd02.Parameters.Add(_db.CreateParameter(DbType.Int32));
                    cmd02.Parameters.Add(_db.CreateParameter(DbType.String));

                    if (site.SpatialReference != null)
                    {
                        cmd02.Parameters[0].Value = site.SpatialReference.SRSID;
                        cmd02.Parameters[1].Value = site.SpatialReference.SRSName;

                        var spatialReferenceIDResult = cmd02.ExecuteScalar();
                        if (spatialReferenceIDResult != null)
                        {
                            spatialReferenceID = Convert.ToInt32(spatialReferenceIDResult);
                        }
                    }
                    if (site.LocalProjection != null)
                    {
                        cmd02.Parameters[0].Value = site.LocalProjection.SRSID;
                        cmd02.Parameters[1].Value = site.LocalProjection.SRSName;

                        var localProjectionIDResult = cmd02.ExecuteScalar();
                        if (localProjectionIDResult != null)
                        {
                            localProjectionID = Convert.ToInt32(localProjectionIDResult);
                        }
                    }
                }

                //save spatial reference
                if (spatialReferenceID == 0 && 
                    site.SpatialReference != null)
                {
                    using (DbCommand cmd03 = conn.CreateCommand())
                    {
                        //Save the spatial reference (Lat / Long Datum)
                        cmd03.CommandText = sqlSaveSpatialReference;
                        cmd03.Parameters.Add(_db.CreateParameter(DbType.Int32, site.SpatialReference.SRSID));
                        cmd03.Parameters.Add(_db.CreateParameter(DbType.String, site.SpatialReference.SRSName));

                        var spatialReferenceIDResult = cmd03.ExecuteScalar();
                        spatialReferenceID = Convert.ToInt32(spatialReferenceIDResult);
                    }
                }

                //save local projection
                if (localProjectionID == 0 && 
                    site.LocalProjection != null)
                {
                    //save spatial reference and the local projection
                    using (DbCommand cmd03 = conn.CreateCommand())
                    {
                        //Save the spatial reference (Lat / Long Datum)
                        cmd03.CommandText = sqlSaveSpatialReference;
                        cmd03.Parameters.Add(_db.CreateParameter(DbType.Int32, site.LocalProjection.SRSID));
                        cmd03.Parameters.Add(_db.CreateParameter(DbType.String, site.LocalProjection.SRSName));

                        var localProjectionIDResult = cmd03.ExecuteScalar();
                        localProjectionID = Convert.ToInt32(localProjectionIDResult);
                    }
                }
                

                //Insert the site to the database
                using (DbCommand cmd04 = conn.CreateCommand())
                {
                    cmd04.CommandText = sqlSaveSite;
                    cmd04.Parameters.Add(_db.CreateParameter(DbType.String, site.Code));
                    cmd04.Parameters.Add(_db.CreateParameter(DbType.String, site.Name));
                    cmd04.Parameters.Add(_db.CreateParameter(DbType.Double, site.Latitude));
                    cmd04.Parameters.Add(_db.CreateParameter(DbType.Double, site.Longitude));
                    cmd04.Parameters.Add(_db.CreateParameter(DbType.Int32, spatialReferenceID));
                    cmd04.Parameters.Add(_db.CreateParameter(DbType.Double, site.Elevation_m));
                    cmd04.Parameters.Add(_db.CreateParameter(DbType.String, site.VerticalDatum));
                    cmd04.Parameters.Add(_db.CreateParameter(DbType.Double, site.LocalX));
                    cmd04.Parameters.Add(_db.CreateParameter(DbType.Double, site.LocalY));
                    cmd04.Parameters.Add(_db.CreateParameter(DbType.Int32, localProjectionID));
                    cmd04.Parameters.Add(_db.CreateParameter(DbType.Double, site.PosAccuracy_m));
                    cmd04.Parameters.Add(_db.CreateParameter(DbType.String, site.State));
                    cmd04.Parameters.Add(_db.CreateParameter(DbType.String, site.County));
                    cmd04.Parameters.Add(_db.CreateParameter(DbType.String, site.Comments));
                    cmd04.Parameters.Add(_db.CreateParameter(DbType.String, site.Country));
                    cmd04.Parameters.Add(_db.CreateParameter(DbType.String, site.SiteType));


                    var siteIDResult = cmd04.ExecuteScalar();
                    siteID = Convert.ToInt32(siteIDResult);
                }
            }
            return siteID;
        }

        private int GetOrCreateSourceID(Source source, DbConnection conn)
        {
            const string sqlSource = "SELECT SourceID FROM Sources WHERE Organization = ?";
            const string sqlISOMetadata = "SELECT MetadataID FROM ISOMetadata WHERE Title = ? AND MetadataLink = ?";

            string sqlSaveSource = "INSERT INTO Sources(Organization, SourceDescription, SourceLink, ContactName, Phone, " +
                                  "Email, Address, City, State, ZipCode, Citation, MetadataID) " +
                                  "VALUES(?,?,?,?,?,?,?,?,?,?,?,?)" + LastRowIDSelect;
            string sqlSaveISOMetadata = "INSERT INTO ISOMetadata(TopicCategory, Title, Abstract, ProfileVersion, MetadataLink) " +
                                    "VALUES(?,?,?,?,?)" + LastRowIDSelect;

            int sourceID = 0;
            int isoMetadataID = 0;
            if (source != null)
            {
                using (DbCommand cmd14 = conn.CreateCommand())
                {
                    cmd14.CommandText = sqlSource;
                    cmd14.Parameters.Add(_db.CreateParameter(DbType.String, source.Organization));
                    var sourceIDResult = cmd14.ExecuteScalar();
                    if (sourceIDResult != null)
                    {
                        sourceID = Convert.ToInt32(sourceIDResult);
                    }
                }

                if (sourceID == 0)
                {
                    ISOMetadata isoMetadata = source.ISOMetadata;

                    using (DbCommand cmd15 = conn.CreateCommand())
                    {
                        cmd15.CommandText = sqlISOMetadata;
                        cmd15.Parameters.Add(_db.CreateParameter(DbType.String, isoMetadata.Title));
                        cmd15.Parameters.Add(_db.CreateParameter(DbType.String, isoMetadata.MetadataLink));
                        var isoMetadataIDResult = cmd15.ExecuteScalar();
                        if (isoMetadataIDResult != null)
                        {
                            isoMetadataID = Convert.ToInt32(isoMetadataIDResult);
                        }
                    }

                    if (isoMetadataID == 0)
                    {
                        using (DbCommand cmd16 = conn.CreateCommand())
                        {
                            cmd16.CommandText = sqlSaveISOMetadata;
                            cmd16.Parameters.Add(_db.CreateParameter(DbType.String, isoMetadata.TopicCategory));
                            cmd16.Parameters.Add(_db.CreateParameter(DbType.String, isoMetadata.Title));
                            cmd16.Parameters.Add(_db.CreateParameter(DbType.String, isoMetadata.Abstract));
                            cmd16.Parameters.Add(_db.CreateParameter(DbType.String, isoMetadata.ProfileVersion));
                            cmd16.Parameters.Add(_db.CreateParameter(DbType.String, isoMetadata.MetadataLink));
                            var isoMetadataIDResult = cmd16.ExecuteScalar();
                            isoMetadataID = Convert.ToInt32(isoMetadataIDResult);
                        }
                    }

                    using (DbCommand cmd17 = conn.CreateCommand())
                    {
                        cmd17.CommandText = sqlSaveSource;
                        cmd17.Parameters.Add(_db.CreateParameter(DbType.String, source.Organization));
                        cmd17.Parameters.Add(_db.CreateParameter(DbType.String, source.Description));
                        cmd17.Parameters.Add(_db.CreateParameter(DbType.String, source.Link));
                        cmd17.Parameters.Add(_db.CreateParameter(DbType.String, source.ContactName));
                        cmd17.Parameters.Add(_db.CreateParameter(DbType.String, source.Phone));
                        cmd17.Parameters.Add(_db.CreateParameter(DbType.String, source.Email));
                        cmd17.Parameters.Add(_db.CreateParameter(DbType.String, source.Address));
                        cmd17.Parameters.Add(_db.CreateParameter(DbType.String, source.City));
                        cmd17.Parameters.Add(_db.CreateParameter(DbType.String, source.State));
                        cmd17.Parameters.Add(_db.CreateParameter(DbType.Int32, source.ZipCode));
                        cmd17.Parameters.Add(_db.CreateParameter(DbType.String, source.Citation));
                        cmd17.Parameters.Add(_db.CreateParameter(DbType.String, isoMetadataID));
                        var sourceIDResult = cmd17.ExecuteScalar();
                        sourceID = Convert.ToInt32(sourceIDResult);
                    }
                }
            }

            return sourceID;
        }

        private int GetOrCreateVariableID(Variable variable, DbConnection conn)
        {
            const string sqlVariable = "SELECT VariableID FROM Variables WHERE VariableCode = ? AND DataType = ?";
            string sqlSaveVariable = "INSERT INTO Variables(VariableCode, VariableName, Speciation, VariableUnitsID, SampleMedium, ValueType, " +
                "IsRegular, ISCategorical, TimeSupport, TimeUnitsID, DataType, GeneralCategory, NoDataValue) " +
                "VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)" + LastRowIDSelect;

            int variableID = 0;
            long variableUnitsID = 0;
            long timeUnitsID = 0;

            using (DbCommand cmd05 = conn.CreateCommand())
            {
                cmd05.CommandText = sqlVariable;
                cmd05.Parameters.Add(_db.CreateParameter(DbType.String, variable.Code));
                cmd05.Parameters.Add(_db.CreateParameter(DbType.String, variable.DataType));
                cmd05.Parameters[0].Value = variable.Code;
                cmd05.Parameters[1].Value = variable.DataType;
                var variableIDResult = cmd05.ExecuteScalar();
                if (variableIDResult != null)
                {
                    variableID = Convert.ToInt32(variableIDResult);
                }
            }

            if (variableID == 0) //New variable needs to be created
            {
                // Get Variable unit
                if (variable.VariableUnit != null)
                {
                    var unitID = GetUnitID(conn, variable.VariableUnit);
                    if (unitID.HasValue)
                    {
                        variableUnitsID = unitID.Value;
                    }
                }

                // Get Time Unit
                if (variable.TimeUnit != null)
                {
                    var unitID = GetUnitID(conn, variable.TimeUnit);
                    if (unitID.HasValue)
                    {
                        timeUnitsID = unitID.Value;
                    }
                }

                // Save the variable units
                if (variableUnitsID == 0 &&
                    variable.VariableUnit != null)
                {
                    SaveUnit(conn, variable.VariableUnit);
                    variableUnitsID = variable.VariableUnit.Id;
                }

                // Save the time units
                if (timeUnitsID == 0 &&
                    variable.TimeUnit != null)
                {
                    SaveUnit(conn, variable.TimeUnit);
                    timeUnitsID = variable.TimeUnit.Id;
                }

                //Insert the variable to the database
                using (DbCommand cmd09 = conn.CreateCommand())
                {
                    cmd09.CommandText = sqlSaveVariable;
                    cmd09.Parameters.Add(_db.CreateParameter(DbType.String, variable.Code));
                    cmd09.Parameters.Add(_db.CreateParameter(DbType.String, variable.Name));
                    cmd09.Parameters.Add(_db.CreateParameter(DbType.String, variable.Speciation));
                    cmd09.Parameters.Add(_db.CreateParameter(DbType.Int32, variableUnitsID));
                    cmd09.Parameters.Add(_db.CreateParameter(DbType.String, variable.SampleMedium));
                    cmd09.Parameters.Add(_db.CreateParameter(DbType.String, variable.ValueType));
                    cmd09.Parameters.Add(_db.CreateParameter(DbType.Boolean, variable.IsRegular));
                    cmd09.Parameters.Add(_db.CreateParameter(DbType.Boolean, variable.IsCategorical));
                    cmd09.Parameters.Add(_db.CreateParameter(DbType.Double, variable.TimeSupport));
                    cmd09.Parameters.Add(_db.CreateParameter(DbType.Int32, timeUnitsID));
                    cmd09.Parameters.Add(_db.CreateParameter(DbType.String, variable.DataType));
                    cmd09.Parameters.Add(_db.CreateParameter(DbType.String, variable.GeneralCategory));
                    cmd09.Parameters.Add(_db.CreateParameter(DbType.Double, variable.NoDataValue));

                    var variableIDResult = cmd09.ExecuteScalar();
                    variableID = Convert.ToInt32(variableIDResult);
                }
            }

            return variableID;
        }

        private long? GetUnitID(DbConnection conn, Unit unit)
        {
            const string sqlUnits = "SELECT UnitsID FROM Units WHERE UnitsName = ? AND UnitsAbbreviation = ?";

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sqlUnits;
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, unit.Name));
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, unit.Abbreviation));

                var result = cmd.ExecuteScalar();
                if (result != null)
                    return Convert.ToInt64(result);
                return null;
            }
        }

        private long? GetQualifierID(DbConnection conn, Qualifier qualifier)
        {
            const string sqlQualifier = "SELECT QualifierID FROM Qualifiers WHERE QualifierCode = ?";

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sqlQualifier;
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, qualifier.Code));

                var result = cmd.ExecuteScalar();
                if (result != null)
                    return Convert.ToInt64(result);
                return null;
            }
        }

        private long? GetThemeID(DbConnection conn, Theme theme)
        {
            const string sqlTheme = "SELECT ThemeID FROM DataThemeDescriptions WHERE ThemeName = ?";

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sqlTheme;
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, theme.Name));

                var result = cmd.ExecuteScalar();
                if (result != null)
                    return Convert.ToInt64(result);
                return null;
            }
        }

        private long? GetSampleID(DbConnection conn, Sample sample)
        {
            const string sqlSample = "SELECT SampleID FROM Samples WHERE SampleType = ? AND LabSampleCode = ?";

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sqlSample;
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, sample.SampleType));
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, sample.LabSampleCode));

                var result = cmd.ExecuteScalar();
                if (result != null)
                    return Convert.ToInt64(result);
                return null;
            }
        }

        private long? GetLabMethodID(DbConnection conn, LabMethod labMethod)
        {
            const string sqlLabMethod = "SELECT LabMethodID FROM LabMethods WHERE LabName = ? AND LabMethodName = ?";

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sqlLabMethod;
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, labMethod.LabName));
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, labMethod.LabMethodName));

                var result = cmd.ExecuteScalar();
                if (result != null)
                    return Convert.ToInt64(result);
                return null;
            }
        }

        private long? GetOffsetTypeID(DbConnection conn, OffsetType offsetType)
        {
            const string sqlOffsetType = "SELECT OffsetTypeID FROM OffsetTypes WHERE OffsetDescription = ?";

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sqlOffsetType;
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, offsetType.Description));

                var result = cmd.ExecuteScalar();
                if (result != null)
                    return Convert.ToInt64(result);
                return null;
            }
        }

        private void SaveLabMethod(DbConnection conn, LabMethod labMethodToSave)
        {
            var sqlSaveLabMethod = "INSERT INTO LabMethods(LabName, LabOrganization, LabMethodName, LabMethodLink, LabMethodDescription) " +
                "VALUES(?, ?, ?, ?, ?)" + LastRowIDSelect;

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sqlSaveLabMethod;
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, labMethodToSave.LabName));
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, labMethodToSave.LabOrganization));
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, labMethodToSave.LabMethodName));
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, labMethodToSave.LabMethodLink));
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, labMethodToSave.LabMethodDescription));

                var labMethodIDResult = cmd.ExecuteScalar();
                labMethodToSave.Id = Convert.ToInt64(labMethodIDResult);
            }
        }

        private void SaveSample(DbConnection conn, Sample sample)
        {
            var sqlSaveSample = "INSERT INTO Samples(SampleType, LabSampleCode, LabMethodID) VALUES (?,?, ?)" + LastRowIDSelect;

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sqlSaveSample;
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, sample.SampleType));
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, sample.LabSampleCode));
                cmd.Parameters.Add(_db.CreateParameter(DbType.Int64, sample.LabMethod.Id));

                var sampleIDResult = cmd.ExecuteScalar();
                sample.Id = Convert.ToInt64(sampleIDResult);
            }
        }

        private void SaveSamplesAndLabMethods(DbConnection conn, Dictionary<string, Sample> sampleLookup)
        {
            if (sampleLookup.Count <= 0) return;

            var unsavedSamples = new List<Sample>();
            var unsavedLabMethods = new List<LabMethod>();
            var labMethodLookup = new Dictionary<string, LabMethod>();

            foreach (var sample in sampleLookup.Values)
            {
                var id = GetSampleID(conn, sample);
                if (id.HasValue)
                {
                    sample.Id = id.Value;
                }
                else
                {
                    unsavedSamples.Add(sample);
                    var labMethodKey = sample.LabMethod.LabName + "|" + sample.LabMethod.LabMethodName;
                    if (!labMethodLookup.ContainsKey(labMethodKey))
                    {
                        labMethodLookup.Add(labMethodKey, sample.LabMethod);
                    }
                }
            }
            foreach (var labMethod in labMethodLookup.Values)
            {
                var id = GetLabMethodID(conn, labMethod);
                if (id.HasValue)
                {
                    labMethod.Id = id.Value;
                }
                else
                {
                    unsavedLabMethods.Add(labMethod);
                }
            }

            //save lab methods
            foreach (var labMethodToSave in unsavedLabMethods)
            {
                SaveLabMethod(conn, labMethodToSave);
            }

            //save samples
            foreach (var sample in unsavedSamples)
            {
                SaveSample(conn, sample);
            }
        }

        private void SaveQualifiers(DbConnection conn, Dictionary<string, Qualifier> qualifierLookup)
        {
            if (qualifierLookup.Count <= 0) return;

            var sqlSaveQualifier = "INSERT INTO Qualifiers(QualifierCode, QualifierDescription) VALUES (?,?)" + LastRowIDSelect;

            var unsavedQualifiers = new List<Qualifier>();
            foreach (var qualifier in qualifierLookup.Values)
            {
                var id = GetQualifierID(conn, qualifier);
                if (id.HasValue)
                {
                    qualifier.Id = id.Value;
                }
                else
                {
                    unsavedQualifiers.Add(qualifier);
                }
            }
            if (unsavedQualifiers.Count > 0)
            {
                using (DbCommand cmd20 = conn.CreateCommand())
                {
                    cmd20.CommandText = sqlSaveQualifier;
                    cmd20.Parameters.Add(_db.CreateParameter(DbType.String));
                    cmd20.Parameters.Add(_db.CreateParameter(DbType.String));

                    foreach (var qual2 in unsavedQualifiers)
                    {
                        cmd20.Parameters[0].Value = qual2.Code;
                        cmd20.Parameters[1].Value = qual2.Description;
                        var qualifierIDResult = cmd20.ExecuteScalar();
                        qual2.Id = Convert.ToInt64(qualifierIDResult);
                    }
                }
            }
        }

        private void SaveOffsets(DbConnection conn, Dictionary<string, OffsetType> offsetLookup)
        {
            if (offsetLookup.Count <= 0) return;

            var offsetUnitLookup = new Dictionary<string, Unit>();
            var unsavedOffsetUnits = new List<Unit>();
            var unsavedoffsets = new List<OffsetType>();

            foreach (var offset in offsetLookup.Values)
            {
                var id = GetOffsetTypeID(conn, offset);
                if (id.HasValue)
                {
                    offset.Id = id.Value;
                }
                else
                {
                    unsavedoffsets.Add(offset);
                    string offsetUnitsKey = offset.Unit.Abbreviation + "|" + offset.Unit.Name;
                    if (!offsetUnitLookup.ContainsKey(offsetUnitsKey))
                    {
                        offsetUnitLookup.Add(offsetUnitsKey, offset.Unit);
                    }
                }
            }

            //check for existing offset units
            foreach (var offsetUnit in offsetUnitLookup.Values)
            {
                var unitID = GetUnitID(conn, offsetUnit);
                if (unitID.HasValue)
                {
                    offsetUnit.Id = unitID.Value;
                }
                else
                {
                    unsavedOffsetUnits.Add(offsetUnit);
                }
            }

            //save offset units
            foreach (var unitToSave in unsavedOffsetUnits)
            {
                SaveUnit(conn, unitToSave);
            }

            //save offset types
            foreach (var offsetToSave in unsavedoffsets)
            {
                SaveOffsetType(conn, offsetToSave);
            }
        }

        private void SaveUnit(DbConnection conn, Unit unit)
        {
            var sqlSaveUnits = "INSERT INTO Units(UnitsName, UnitsType, UnitsAbbreviation) VALUES(?, ?, ?)" + LastRowIDSelect;

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sqlSaveUnits;
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, unit.Name));
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, unit.UnitsType));
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, unit.Abbreviation));

                var result = cmd.ExecuteScalar();
                unit.Id = Convert.ToInt64(result);
            }
        }

        private void SaveOffsetType(DbConnection conn, OffsetType offsetType)
        {
            var sqlSaveOffsetType = "INSERT INTO OffsetTypes(OffsetUnitsID, OffsetDescription) VALUES (?, ?)" + LastRowIDSelect;

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sqlSaveOffsetType;
                cmd.Parameters.Add(_db.CreateParameter(DbType.Int32, offsetType.Unit.Id));
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, offsetType.Description));

                var result = cmd.ExecuteScalar();
                offsetType.Id = Convert.ToInt64(result);
            }
        }

        private int SaveDataValues(DbConnection conn, int seriesID, IEnumerable<DataValue> dataValueList)
        {
            const string sqlSaveDataValue = "INSERT INTO DataValues(SeriesID, DataValue, ValueAccuracy, LocalDateTime, " +
              "UTCOffset, DateTimeUTC, OffsetValue, OffsetTypeID, CensorCode, QualifierID, SampleID, FileID) " +
              "VALUES(?,?,?,?,?,?,?,?,?,?,?,?)";

            var numSavedValues = 0;
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sqlSaveDataValue;
                cmd.Parameters.Add(_db.CreateParameter(DbType.Int32, seriesID));
                cmd.Parameters.Add(_db.CreateParameter(DbType.Double));
                cmd.Parameters.Add(_db.CreateParameter(DbType.Double));
                cmd.Parameters.Add(_db.CreateParameter(DbType.DateTime));
                cmd.Parameters.Add(_db.CreateParameter(DbType.Double));
                cmd.Parameters.Add(_db.CreateParameter(DbType.DateTime));
                cmd.Parameters.Add(_db.CreateParameter(DbType.Double));
                cmd.Parameters.Add(_db.CreateParameter(DbType.Int32));
                cmd.Parameters.Add(_db.CreateParameter(DbType.String));
                cmd.Parameters.Add(_db.CreateParameter(DbType.Int32));
                cmd.Parameters.Add(_db.CreateParameter(DbType.Int32));
                cmd.Parameters.Add(_db.CreateParameter(DbType.Int32));

                foreach (var val in dataValueList)
                {
                    cmd.Parameters[1].Value = val.Value;
                    cmd.Parameters[2].Value = null;
                    if (val.ValueAccuracy != 0)
                    {
                        cmd.Parameters[2].Value = val.ValueAccuracy;
                    }
                    cmd.Parameters[3].Value = val.LocalDateTime;
                    cmd.Parameters[4].Value = val.UTCOffset;
                    cmd.Parameters[5].Value = val.DateTimeUTC;
                    if (val.OffsetType != null)
                    {
                        cmd.Parameters[6].Value = val.OffsetValue;
                        cmd.Parameters[7].Value = val.OffsetType.Id;
                    }
                    else
                    {
                        cmd.Parameters[6].Value = null;
                        cmd.Parameters[7].Value = null;
                    }
                    cmd.Parameters[8].Value = val.CensorCode;
                    if (val.Qualifier != null)
                    {
                        cmd.Parameters[9].Value = val.Qualifier.Id;
                    }

                    if (val.Sample != null)
                    {
                        cmd.Parameters[10].Value = val.Sample.Id;
                    }

                    cmd.Parameters[11].Value = null;

                    cmd.ExecuteNonQuery();
                    numSavedValues++;
                }
            }
            return numSavedValues;
        }

        #endregion
    }
}

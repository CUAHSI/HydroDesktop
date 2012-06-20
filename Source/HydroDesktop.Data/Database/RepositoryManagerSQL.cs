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
            string sqlSample = "SELECT SampleID FROM Samples WHERE SampleType = ? AND LabSampleCode = ?";
            string sqlLabMethod = "SELECT LabMethodID FROM LabMethods WHERE LabName = ? AND LabMethodName = ?";
            string sqlOffsetType = "SELECT OffsetTypeID FROM OffsetTypes WHERE OffsetDescription = ?";
            string sqlRowID = "; SELECT LAST_INSERT_ROWID();";

            
            string sqlSaveUnits = "INSERT INTO Units(UnitsName, UnitsType, UnitsAbbreviation) VALUES(?, ?, ?)" + sqlRowID;
            string sqlSaveSeries = "INSERT INTO DataSeries(SiteID, VariableID, MethodID, SourceID, QualityControlLevelID, " +
                "IsCategorical, BeginDateTime, EndDateTime, BeginDateTimeUTC, EndDateTimeUTC, ValueCount, CreationDateTime, " +
                "Subscribed, UpdateDateTime, LastCheckedDateTime) " +
                "VALUES(?, ?, ?, ?,?,?,?,?,?,?,?,?,?,?,?)" + sqlRowID;

            string sqlSaveQualifier = "INSERT INTO Qualifiers(QualifierCode, QualifierDescription) VALUES (?,?)" + sqlRowID;

            string sqlSaveSample = "INSERT INTO Samples(SampleType, LabSampleCode, LabMethodID) VALUES (?,?, ?)" + sqlRowID;

            string sqlSaveLabMethod = "INSERT INTO LabMethods(LabName, LabOrganization, LabMethodName, LabMethodLink, LabMethodDescription) " +
                "VALUES(?, ?, ?, ?, ?)" + sqlRowID;

            string sqlSaveOffsetType = "INSERT INTO OffsetTypes(OffsetUnitsID, OffsetDescription) VALUES (?, ?)" + sqlRowID;

            string sqlSaveDataValue = "INSERT INTO DataValues(SeriesID, DataValue, ValueAccuracy, LocalDateTime, " +
                "UTCOffset, DateTimeUTC, OffsetValue, OffsetTypeID, CensorCode, QualifierID, SampleID, FileID) " +
                "VALUES(?,?,?,?,?,?,?,?,?,?,?,?)";

            string sqlSaveTheme1 = "INSERT INTO DataThemeDescriptions(ThemeName, ThemeDescription) VALUES (?,?)" + sqlRowID;
            string sqlSaveTheme2 = "INSERT INTO DataThemes(ThemeID,SeriesID) VALUEs (?,?)";

            
            int methodID = 0;
            int qualityControlLevelID = 0;
            int sourceID = 0;
            int seriesID = 0;
            long themeID = 0;
            
            object seriesIDResult = null;
            object qualifierIDResult = null;
            object sampleIDResult = null;
            object labMethodIDResult = null;
            object offsetTypeIDResult = null;
            object offsetUnitIDResult = null;

            var qualifierLookup = new Dictionary<string, Qualifier>();
            var sampleLookup = new Dictionary<string, Sample>();
            var offsetLookup = new Dictionary<string, OffsetType>();

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
            
            int numSavedValues = 0;

            //Step 1 Begin Transaction
            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();

                using (DbTransaction tran = conn.BeginTransaction())
                {
                    //****************************************************************
                    //*** Step 4 Method
                    //****************************************************************
                    methodID = GetOrCreateMethodID(series.Method, conn);

                    //****************************************************************
                    //*** Step 5 Quality Control Level
                    //****************************************************************
                    qualityControlLevelID = GetOrCreateQualityControlLevelID(series.QualityControlLevel, conn);

                    //****************************************************************
                    //*** Step 6 Source
                    //****************************************************************
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
                    GetLookups(series, out qualifierLookup, out sampleLookup, out offsetLookup);

                    //****************************************************************
                    //*** Step 9 Qualifiers
                    //****************************************************************
                    if (qualifierLookup.Count > 0)
                    {
                        var unsavedQualifiers = new List<Qualifier>();
                        foreach (Qualifier qualifier in qualifierLookup.Values)
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

                                foreach (Qualifier qual2 in unsavedQualifiers)
                                {
                                    cmd20.Parameters[0].Value = qual2.Code;
                                    cmd20.Parameters[1].Value = qual2.Description;
                                    qualifierIDResult = cmd20.ExecuteScalar();
                                    qual2.Id = Convert.ToInt32(qualifierIDResult);
                                }
                            }
                        }
                    }

                    //****************************************************************
                    //*** TODO Step 10 Samples and Lab Methods
                    //****************************************************************
                    if (sampleLookup.Count > 0)
                    {
                        Dictionary<string, LabMethod> labMethodLookup = new Dictionary<string, LabMethod>();

                        using (DbCommand cmd21 = conn.CreateCommand())
                        {
                            cmd21.CommandText = sqlSample;
                            cmd21.Parameters.Add(_db.CreateParameter(DbType.String));
                            cmd21.Parameters.Add(_db.CreateParameter(DbType.String));

                            foreach (Sample sample in sampleLookup.Values)
                            {
                                cmd21.Parameters[0].Value = sample.SampleType;
                                cmd21.Parameters[1].Value = sample.LabSampleCode;
                                sampleIDResult = cmd21.ExecuteScalar();
                                if (sampleIDResult != null)
                                {
                                    sample.Id = Convert.ToInt32(sampleIDResult);
                                }
                            }
                        }


                        List<Sample> unsavedSamples = new List<Sample>();
                        List<LabMethod> unsavedLabMethods = new List<LabMethod>();

                        foreach (Sample samp in sampleLookup.Values)
                        {
                            if (samp.Id == 0)
                            {
                                unsavedSamples.Add(samp);
                                string labMethodKey = samp.LabMethod.LabName + "|" + samp.LabMethod.LabMethodName;
                                if (!labMethodLookup.ContainsKey(labMethodKey))
                                {
                                    labMethodLookup.Add(labMethodKey, samp.LabMethod);
                                }
                            }
                        }

                        using (DbCommand cmd22 = conn.CreateCommand())
                        {
                            cmd22.CommandText = sqlLabMethod;
                            cmd22.Parameters.Add(_db.CreateParameter(DbType.String));
                            cmd22.Parameters.Add(_db.CreateParameter(DbType.String));

                            foreach (LabMethod labMethod in labMethodLookup.Values)
                            {
                                cmd22.Parameters[0].Value = labMethod.LabName;
                                cmd22.Parameters[1].Value = labMethod.LabMethodName;
                                labMethodIDResult = cmd22.ExecuteScalar();
                                if (labMethodIDResult != null)
                                {
                                    labMethod.Id = Convert.ToInt32(labMethodIDResult);
                                }
                            }
                        }

                        //check unsaved lab methods
                        foreach (LabMethod lm in labMethodLookup.Values)
                        {
                            if (lm.Id == 0)
                            {
                                unsavedLabMethods.Add(lm);
                            }
                        }

                        //save lab methods
                        if (unsavedLabMethods.Count > 0)
                        {
                            using (DbCommand cmd23 = conn.CreateCommand())
                            {
                                cmd23.CommandText = sqlSaveLabMethod;
                                cmd23.Parameters.Add(_db.CreateParameter(DbType.String));
                                cmd23.Parameters.Add(_db.CreateParameter(DbType.String));
                                cmd23.Parameters.Add(_db.CreateParameter(DbType.String));
                                cmd23.Parameters.Add(_db.CreateParameter(DbType.String));
                                cmd23.Parameters.Add(_db.CreateParameter(DbType.String));

                                foreach (LabMethod labMethodToSave in unsavedLabMethods)
                                {
                                    cmd23.Parameters[0].Value = labMethodToSave.LabName;
                                    cmd23.Parameters[1].Value = labMethodToSave.LabOrganization;
                                    cmd23.Parameters[2].Value = labMethodToSave.LabMethodName;
                                    cmd23.Parameters[3].Value = labMethodToSave.LabMethodLink;
                                    cmd23.Parameters[4].Value = labMethodToSave.LabMethodDescription;
                                    labMethodIDResult = cmd23.ExecuteScalar();
                                    labMethodToSave.Id = Convert.ToInt32(labMethodIDResult);
                                }
                            }
                        }

                        //save samples
                        if (unsavedSamples.Count > 0)
                        {
                            using (DbCommand cmd24 = conn.CreateCommand())
                            {
                                cmd24.CommandText = sqlSaveSample;
                                cmd24.Parameters.Add(_db.CreateParameter(DbType.String));
                                cmd24.Parameters.Add(_db.CreateParameter(DbType.String));
                                cmd24.Parameters.Add(_db.CreateParameter(DbType.Int32));

                                foreach (Sample samp3 in unsavedSamples)
                                {
                                    cmd24.Parameters[0].Value = samp3.SampleType;
                                    cmd24.Parameters[1].Value = samp3.LabSampleCode;
                                    cmd24.Parameters[2].Value = samp3.LabMethod.Id;
                                    sampleIDResult = cmd24.ExecuteScalar();
                                    samp3.Id = Convert.ToInt32(sampleIDResult);
                                }
                            }
                        }
                    }



                    //****************************************************************
                    //*** TODO Step 11 Vertical Offsets
                    //****************************************************************
                    if (offsetLookup.Count > 0)
                    {
                        Dictionary<string, Unit> offsetUnitLookup = new Dictionary<string, Unit>();
                        List<Unit> unsavedOffsetUnits = new List<Unit>();

                        using (DbCommand cmd25 = conn.CreateCommand())
                        {
                            cmd25.CommandText = sqlOffsetType;
                            cmd25.Parameters.Add(_db.CreateParameter(DbType.String));

                            foreach (OffsetType offset in offsetLookup.Values)
                            {
                                cmd25.Parameters[0].Value = offset.Description;
                                offsetTypeIDResult = cmd25.ExecuteScalar();
                                if (offsetTypeIDResult != null)
                                {
                                    offset.Id = Convert.ToInt32(offsetTypeIDResult);
                                }
                            }
                        }

                        //check unsaved offsets
                        List<OffsetType> unsavedoffsets = new List<OffsetType>();
                        foreach (OffsetType offset2 in offsetLookup.Values)
                        {
                            if (offset2.Id == 0)
                            {
                                unsavedoffsets.Add(offset2);
                                string offsetUnitsKey = offset2.Unit.Abbreviation + "|" + offset2.Unit.Name;
                                if (!offsetUnitLookup.ContainsKey(offsetUnitsKey))
                                {
                                    offsetUnitLookup.Add(offsetUnitsKey, offset2.Unit);
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
                        if (unsavedOffsetUnits.Count > 0)
                        {
                            using (DbCommand cmd27 = conn.CreateCommand())
                            {
                                cmd27.CommandText = sqlSaveUnits;
                                cmd27.Parameters.Add(_db.CreateParameter(DbType.String));
                                cmd27.Parameters.Add(_db.CreateParameter(DbType.String));
                                cmd27.Parameters.Add(_db.CreateParameter(DbType.String));

                                foreach (Unit unitToSave in unsavedOffsetUnits)
                                {
                                    cmd27.Parameters[0].Value = unitToSave.Name;
                                    cmd27.Parameters[1].Value = unitToSave.UnitsType;
                                    cmd27.Parameters[2].Value = unitToSave.Abbreviation;

                                    offsetUnitIDResult = cmd27.ExecuteScalar();
                                    unitToSave.Id = Convert.ToInt32(offsetUnitIDResult);
                                }
                            }
                        }

                        //save offset types
                        if (unsavedoffsets.Count > 0)
                        {
                            using (DbCommand cmd28 = conn.CreateCommand())
                            {
                                cmd28.CommandText = sqlSaveOffsetType;
                                cmd28.Parameters.Add(_db.CreateParameter(DbType.Int32));
                                cmd28.Parameters.Add(_db.CreateParameter(DbType.String));

                                foreach (OffsetType offsetToSave in unsavedoffsets)
                                {
                                    cmd28.Parameters[0].Value = offsetToSave.Unit.Id;
                                    cmd28.Parameters[1].Value = offsetToSave.Description;
                                    offsetTypeIDResult = cmd28.ExecuteScalar();
                                    offsetToSave.Id = Convert.ToInt32(offsetTypeIDResult);
                                }
                            }
                        }
                    }

                    //****************************************************************
                    //*** TODO Step 12 Data File - QueryInfo - DataService ***********
                    //****************************************************************

                    //****************************************************************
                    //*** TODO Step 13 Data Values                         ***********
                    //****************************************************************
                    using (DbCommand cmd30 = conn.CreateCommand())
                    {
                        cmd30.CommandText = sqlSaveDataValue;
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.Int32, seriesID));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.Double));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.Double));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.DateTime));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.Double));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.DateTime));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.Double));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.Int32));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.String));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.Int32));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.Int32));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.Int32));

                        foreach (DataValue val in series.DataValueList)
                        {
                            cmd30.Parameters[1].Value = val.Value;
                            cmd30.Parameters[2].Value = null;
                            if (val.ValueAccuracy != 0)
                            {
                                cmd30.Parameters[2].Value = val.ValueAccuracy;
                            }
                            cmd30.Parameters[3].Value = val.LocalDateTime;
                            cmd30.Parameters[4].Value = val.UTCOffset;
                            cmd30.Parameters[5].Value = val.DateTimeUTC;
                            if (val.OffsetType != null)
                            {
                                cmd30.Parameters[6].Value = val.OffsetValue;
                                cmd30.Parameters[7].Value = val.OffsetType.Id;
                            }
                            else
                            {
                                cmd30.Parameters[6].Value = null;
                                cmd30.Parameters[7].Value = null;
                            }
                            cmd30.Parameters[8].Value = val.CensorCode;
                            if (val.Qualifier != null)
                            {
                                cmd30.Parameters[9].Value = val.Qualifier.Id;
                            }

                            if (val.Sample != null)
                            {
                                cmd30.Parameters[10].Value = val.Sample.Id;
                            }

                            cmd30.Parameters[11].Value = null; //TODO Check Data File

                            cmd30.ExecuteNonQuery();
                            numSavedValues++;
                        }
                    }

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
            if (overwrite == OverwriteOptions.Append || overwrite == OverwriteOptions.Fill)
            {
                return SaveSeriesAppend(seriesToSave, theme);
            }
            if (overwrite == OverwriteOptions.Copy)
            {
                return SaveSeriesAsCopy(seriesToSave, theme);
            }
            if (overwrite == OverwriteOptions.Overwrite)
            {
                return SaveSeriesOverwrite(seriesToSave, theme);
            }
            //default option is 'append'...
            return SaveSeriesAppend(seriesToSave, theme);
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
            string sqlSample = "SELECT SampleID FROM Samples WHERE SampleType = ? AND LabSampleCode = ?";
            string sqlLabMethod = "SELECT LabMethodID FROM LabMethods WHERE LabName = ? AND LabMethodName = ?";
            string sqlOffsetType = "SELECT OffsetTypeID FROM OffsetTypes WHERE OffsetDescription = ?";
            string sqlThemeSeries = "SELECT ThemeID FROM DataThemes WHERE ThemeID = ? AND SeriesID = ?";
            string sqlRowID = "; SELECT LAST_INSERT_ROWID();";
            string sqlSeries = "SELECT SeriesID, BeginDateTime, BeginDateTimeUTC, EndDateTime, EndDateTimeUTC, ValueCount FROM DataSeries WHERE SiteID = ? AND VariableID = ? AND MethodID = ? AND QualityControlLevelID = ? AND SourceID = ?";

            string sqlSaveUnits = "INSERT INTO Units(UnitsName, UnitsType, UnitsAbbreviation) VALUES(?, ?, ?)" + sqlRowID;
            string sqlSaveSeries = "INSERT INTO DataSeries(SiteID, VariableID, MethodID, SourceID, QualityControlLevelID, " +
                "IsCategorical, BeginDateTime, EndDateTime, BeginDateTimeUTC, EndDateTimeUTC, ValueCount, CreationDateTime, " +
                "Subscribed, UpdateDateTime, LastCheckedDateTime) " +
                "VALUES(?, ?, ?, ?,?,?,?,?,?,?,?,?,?,?,?)" + sqlRowID;

            string sqlSaveQualifier = "INSERT INTO Qualifiers(QualifierCode, QualifierDescription) VALUES (?,?)" + sqlRowID;

            string sqlSaveSample = "INSERT INTO Samples(SampleType, LabSampleCode, LabMethodID) VALUES (?,?, ?)" + sqlRowID;

            string sqlSaveLabMethod = "INSERT INTO LabMethods(LabName, LabOrganization, LabMethodName, LabMethodLink, LabMethodDescription) " +
                "VALUES(?, ?, ?, ?, ?)" + sqlRowID;

            string sqlSaveOffsetType = "INSERT INTO OffsetTypes(OffsetUnitsID, OffsetDescription) VALUES (?, ?)" + sqlRowID;

            string sqlSaveDataValue = "INSERT INTO DataValues(SeriesID, DataValue, ValueAccuracy, LocalDateTime, " +
                "UTCOffset, DateTimeUTC, OffsetValue, OffsetTypeID, CensorCode, QualifierID, SampleID, FileID) " +
                "VALUES(?,?,?,?,?,?,?,?,?,?,?,?)";

            string sqlSaveTheme1 = "INSERT INTO DataThemeDescriptions(ThemeName, ThemeDescription) VALUES (?,?)" + sqlRowID;
            string sqlSaveTheme2 = "INSERT INTO DataThemes(ThemeID,SeriesID) VALUEs (?,?)";

            string sqlUpdateSeries = "UPDATE DataSeries SET BeginDateTime = ?, BeginDateTimeUTC = ?, EndDateTime = ?, EndDateTimeUTC = ?, " +
                "ValueCount = ?, UpdateDateTime = ? WHERE SeriesID = ?";

            int siteID = 0;
            int variableID = 0;
            int methodID = 0;
            int qualityControlLevelID = 0;
            int sourceID = 0;
            int seriesID = 0;
            long themeID = 0;
            
            object seriesIDResult = null;
            object qualifierIDResult = null;
            object sampleIDResult = null;
            object labMethodIDResult = null;
            object offsetTypeIDResult = null;
            object offsetUnitIDResult = null;

            var qualifierLookup = new Dictionary<string, Qualifier>();
            var sampleLookup = new Dictionary<string, Sample>();
            var offsetLookup = new Dictionary<string, OffsetType>();

            int numSavedValues = 0;

            bool seriesAlreadyExists = false;
            DateTime beginTimeDb = DateTime.MinValue;
            DateTime beginTimeUtcDb = beginTimeDb;
            DateTime endTimeDb = DateTime.MinValue;
            DateTime endTimeUtcDb = endTimeDb;
            int valueCountDb = 0;

            //Step 1 Begin Transaction
            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();

                using (DbTransaction tran = conn.BeginTransaction())
                {
                    //****************************************************************
                    //*** Step 2 Site
                    //****************************************************************
                    siteID = GetOrCreateSiteID(series.Site, conn);

                    //****************************************************************
                    //*** Step 3 Variable
                    //****************************************************************
                    variableID = GetOrCreateVariableID(series.Variable, conn);

                    //****************************************************************
                    //*** Step 4 Method
                    //****************************************************************
                    methodID = GetOrCreateMethodID(series.Method, conn);

                    //****************************************************************
                    //*** Step 5 Quality Control Level
                    //****************************************************************
                    qualityControlLevelID = GetOrCreateQualityControlLevelID(series.QualityControlLevel, conn);

                    //****************************************************************
                    //*** Step 6 Source
                    //****************************************************************
                    sourceID = GetOrCreateSourceID(series.Source, conn);

                    //****************************************************************
                    //*** Step 7 Series
                    //****************************************************************
                    seriesIDResult = null;
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
                                beginTimeUtcDb = Convert.ToDateTime(reader[2]);
                                endTimeDb = Convert.ToDateTime(reader[3]);
                                endTimeUtcDb = Convert.ToDateTime(reader[4]);
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
                        GetLookups(series, out qualifierLookup, out sampleLookup, out offsetLookup);

                        //****************************************************************
                        //*** Step 9 Qualifiers
                        //****************************************************************
                        if (qualifierLookup.Count > 0)
                        {
                            var unsavedQualifiers = new List<Qualifier>();
                            foreach (var qualifier in qualifierLookup.Values)
                            {
                                var id = GetQualifierID(conn, qualifier);
                                if (id.HasValue)
                                {
                                    qualifier.Id = id.Value;
                                }else
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

                                    foreach (Qualifier qual2 in unsavedQualifiers)
                                    {
                                        cmd20.Parameters[0].Value = qual2.Code;
                                        cmd20.Parameters[1].Value = qual2.Description;
                                        qualifierIDResult = cmd20.ExecuteScalar();
                                        qual2.Id = Convert.ToInt32(qualifierIDResult);
                                    }
                                }
                            }
                        }

                        //****************************************************************
                        //*** TODO Step 10 Samples and Lab Methods
                        //****************************************************************
                        if (sampleLookup.Count > 0)
                        {
                            Dictionary<string, LabMethod> labMethodLookup = new Dictionary<string, LabMethod>();

                            using (DbCommand cmd21 = conn.CreateCommand())
                            {
                                cmd21.CommandText = sqlSample;
                                cmd21.Parameters.Add(_db.CreateParameter(DbType.String));
                                cmd21.Parameters.Add(_db.CreateParameter(DbType.String));

                                foreach (Sample sample in sampleLookup.Values)
                                {
                                    cmd21.Parameters[0].Value = sample.SampleType;
                                    cmd21.Parameters[1].Value = sample.LabSampleCode;
                                    sampleIDResult = cmd21.ExecuteScalar();
                                    if (sampleIDResult != null)
                                    {
                                        sample.Id = Convert.ToInt32(sampleIDResult);
                                    }
                                }
                            }


                            List<Sample> unsavedSamples = new List<Sample>();
                            List<LabMethod> unsavedLabMethods = new List<LabMethod>();

                            foreach (Sample samp in sampleLookup.Values)
                            {
                                if (samp.Id == 0)
                                {
                                    unsavedSamples.Add(samp);
                                    string labMethodKey = samp.LabMethod.LabName + "|" + samp.LabMethod.LabMethodName;
                                    if (!labMethodLookup.ContainsKey(labMethodKey))
                                    {
                                        labMethodLookup.Add(labMethodKey, samp.LabMethod);
                                    }
                                }
                            }

                            using (DbCommand cmd22 = conn.CreateCommand())
                            {
                                cmd22.CommandText = sqlLabMethod;
                                cmd22.Parameters.Add(_db.CreateParameter(DbType.String));
                                cmd22.Parameters.Add(_db.CreateParameter(DbType.String));

                                foreach (LabMethod labMethod in labMethodLookup.Values)
                                {
                                    cmd22.Parameters[0].Value = labMethod.LabName;
                                    cmd22.Parameters[1].Value = labMethod.LabMethodName;
                                    labMethodIDResult = cmd22.ExecuteScalar();
                                    if (labMethodIDResult != null)
                                    {
                                        labMethod.Id = Convert.ToInt32(labMethodIDResult);
                                    }
                                }
                            }

                            //check unsaved lab methods
                            foreach (LabMethod lm in labMethodLookup.Values)
                            {
                                if (lm.Id == 0)
                                {
                                    unsavedLabMethods.Add(lm);
                                }
                            }

                            //save lab methods
                            if (unsavedLabMethods.Count > 0)
                            {
                                using (DbCommand cmd23 = conn.CreateCommand())
                                {
                                    cmd23.CommandText = sqlSaveLabMethod;
                                    cmd23.Parameters.Add(_db.CreateParameter(DbType.String));
                                    cmd23.Parameters.Add(_db.CreateParameter(DbType.String));
                                    cmd23.Parameters.Add(_db.CreateParameter(DbType.String));
                                    cmd23.Parameters.Add(_db.CreateParameter(DbType.String));
                                    cmd23.Parameters.Add(_db.CreateParameter(DbType.String));

                                    foreach (LabMethod labMethodToSave in unsavedLabMethods)
                                    {
                                        cmd23.Parameters[0].Value = labMethodToSave.LabName;
                                        cmd23.Parameters[1].Value = labMethodToSave.LabOrganization;
                                        cmd23.Parameters[2].Value = labMethodToSave.LabMethodName;
                                        cmd23.Parameters[3].Value = labMethodToSave.LabMethodLink;
                                        cmd23.Parameters[4].Value = labMethodToSave.LabMethodDescription;
                                        labMethodIDResult = cmd23.ExecuteScalar();
                                        labMethodToSave.Id = Convert.ToInt32(labMethodIDResult);
                                    }
                                }
                            }

                            //save samples
                            if (unsavedSamples.Count > 0)
                            {
                                using (DbCommand cmd24 = conn.CreateCommand())
                                {
                                    cmd24.CommandText = sqlSaveSample;
                                    cmd24.Parameters.Add(_db.CreateParameter(DbType.String));
                                    cmd24.Parameters.Add(_db.CreateParameter(DbType.String));
                                    cmd24.Parameters.Add(_db.CreateParameter(DbType.Int32));

                                    foreach (Sample samp3 in unsavedSamples)
                                    {
                                        cmd24.Parameters[0].Value = samp3.SampleType;
                                        cmd24.Parameters[1].Value = samp3.LabSampleCode;
                                        cmd24.Parameters[2].Value = samp3.LabMethod.Id;
                                        sampleIDResult = cmd24.ExecuteScalar();
                                        samp3.Id = Convert.ToInt32(sampleIDResult);
                                    }
                                }
                            }
                        }



                        //****************************************************************
                        //*** TODO Step 11 Vertical Offsets (NEEDS TESTING DATA - DCEW)
                        //****************************************************************
                        if (offsetLookup.Count > 0)
                        {
                            Dictionary<string, Unit> offsetUnitLookup = new Dictionary<string, Unit>();
                            List<Unit> unsavedOffsetUnits = new List<Unit>();

                            using (DbCommand cmd25 = conn.CreateCommand())
                            {
                                cmd25.CommandText = sqlOffsetType;
                                cmd25.Parameters.Add(_db.CreateParameter(DbType.String));

                                foreach (OffsetType offset in offsetLookup.Values)
                                {
                                    cmd25.Parameters[0].Value = offset.Description;
                                    offsetTypeIDResult = cmd25.ExecuteScalar();
                                    if (offsetTypeIDResult != null)
                                    {
                                        offset.Id = Convert.ToInt32(offsetTypeIDResult);
                                    }
                                }
                            }

                            //check unsaved offsets
                            List<OffsetType> unsavedoffsets = new List<OffsetType>();
                            foreach (OffsetType offset2 in offsetLookup.Values)
                            {
                                if (offset2.Id == 0)
                                {
                                    unsavedoffsets.Add(offset2);
                                    string offsetUnitsKey = offset2.Unit.Abbreviation + "|" + offset2.Unit.Name;
                                    if (!offsetUnitLookup.ContainsKey(offsetUnitsKey))
                                    {
                                        offsetUnitLookup.Add(offsetUnitsKey, offset2.Unit);
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
                            if (unsavedOffsetUnits.Count > 0)
                            {
                                using (DbCommand cmd27 = conn.CreateCommand())
                                {
                                    cmd27.CommandText = sqlSaveUnits;
                                    cmd27.Parameters.Add(_db.CreateParameter(DbType.String));
                                    cmd27.Parameters.Add(_db.CreateParameter(DbType.String));
                                    cmd27.Parameters.Add(_db.CreateParameter(DbType.String));

                                    foreach (Unit unitToSave in unsavedOffsetUnits)
                                    {
                                        cmd27.Parameters[0].Value = unitToSave.Name;
                                        cmd27.Parameters[1].Value = unitToSave.UnitsType;
                                        cmd27.Parameters[2].Value = unitToSave.Abbreviation;

                                        offsetUnitIDResult = cmd27.ExecuteScalar();
                                        unitToSave.Id = Convert.ToInt32(offsetUnitIDResult);
                                    }
                                }
                            }

                            //save offset types
                            if (unsavedoffsets.Count > 0)
                            {
                                using (DbCommand cmd28 = conn.CreateCommand())
                                {
                                    cmd28.CommandText = sqlSaveOffsetType;
                                    cmd28.Parameters.Add(_db.CreateParameter(DbType.Int32));
                                    cmd28.Parameters.Add(_db.CreateParameter(DbType.String));

                                    foreach (OffsetType offsetToSave in unsavedoffsets)
                                    {
                                        cmd28.Parameters[0].Value = offsetToSave.Unit.Id;
                                        cmd28.Parameters[1].Value = offsetToSave.Description;
                                        offsetTypeIDResult = cmd28.ExecuteScalar();
                                        offsetToSave.Id = Convert.ToInt32(offsetTypeIDResult);
                                    }
                                }
                            }
                        }

                        //****************************************************************
                        //*** TODO Step 12 Data File - QueryInfo - DataService ***********
                        //****************************************************************

                        //****************************************************************
                        //*** TODO Step 13 Data Values related information     ***********
                        //****************************************************************

                        using (DbCommand cmd30 = conn.CreateCommand())
                        {
                            cmd30.CommandText = sqlSaveDataValue;
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.Int32, seriesID));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.Double));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.Double));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.DateTime));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.Double));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.DateTime));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.Double));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.Int32));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.String));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.Int32));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.Int32));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.Int32));

                            foreach (DataValue val in series.DataValueList)
                            {
                                cmd30.Parameters[1].Value = val.Value;
                                cmd30.Parameters[2].Value = null;
                                if (val.ValueAccuracy != 0)
                                {
                                    cmd30.Parameters[2].Value = val.ValueAccuracy;
                                }
                                cmd30.Parameters[3].Value = val.LocalDateTime;
                                cmd30.Parameters[4].Value = val.UTCOffset;
                                cmd30.Parameters[5].Value = val.DateTimeUTC;
                                if (val.OffsetType != null)
                                {
                                    cmd30.Parameters[6].Value = val.OffsetValue;
                                    cmd30.Parameters[7].Value = val.OffsetType.Id;
                                }
                                else
                                {
                                    cmd30.Parameters[6].Value = null;
                                    cmd30.Parameters[7].Value = null;
                                }
                                cmd30.Parameters[8].Value = val.CensorCode;
                                if (val.Qualifier != null)
                                {
                                    cmd30.Parameters[9].Value = val.Qualifier.Id;
                                }

                                if (val.Sample != null)
                                {
                                    cmd30.Parameters[10].Value = val.Sample.Id;
                                }

                                cmd30.Parameters[11].Value = null; //TODO Check Data File

                                cmd30.ExecuteNonQuery();
                                numSavedValues++;
                            }
                        }

                        //****************************************************************
                        //*** Step 14 Data Series Update                       ***********
                        //****************************************************************
                        if (seriesAlreadyExists == true && seriesID > 0)
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
                    object seriesThemeCombinationResult = null;
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
            string sqlSample = "SELECT SampleID FROM Samples WHERE SampleType = ? AND LabSampleCode = ?";
            string sqlLabMethod = "SELECT LabMethodID FROM LabMethods WHERE LabName = ? AND LabMethodName = ?";
            string sqlOffsetType = "SELECT OffsetTypeID FROM OffsetTypes WHERE OffsetDescription = ?";
            string sqlThemeSeries = "SELECT ThemeID FROM DataThemes WHERE ThemeID = ? AND SeriesID = ?";
            string sqlRowID = "; SELECT LAST_INSERT_ROWID();";
            string sqlSeries = "SELECT SeriesID, BeginDateTime, BeginDateTimeUTC, EndDateTime, EndDateTimeUTC, ValueCount FROM DataSeries WHERE SiteID = ? AND VariableID = ? AND MethodID = ? AND QualityControlLevelID = ? AND SourceID = ?";

            string sqlSaveUnits = "INSERT INTO Units(UnitsName, UnitsType, UnitsAbbreviation) VALUES(?, ?, ?)" + sqlRowID;
            string sqlSaveSeries = "INSERT INTO DataSeries(SiteID, VariableID, MethodID, SourceID, QualityControlLevelID, " +
                "IsCategorical, BeginDateTime, EndDateTime, BeginDateTimeUTC, EndDateTimeUTC, ValueCount, CreationDateTime, " +
                "Subscribed, UpdateDateTime, LastCheckedDateTime) " +
                "VALUES(?, ?, ?, ?,?,?,?,?,?,?,?,?,?,?,?)" + sqlRowID;

            string sqlSaveQualifier = "INSERT INTO Qualifiers(QualifierCode, QualifierDescription) VALUES (?,?)" + sqlRowID;

            string sqlSaveSample = "INSERT INTO Samples(SampleType, LabSampleCode, LabMethodID) VALUES (?,?, ?)" + sqlRowID;

            string sqlSaveLabMethod = "INSERT INTO LabMethods(LabName, LabOrganization, LabMethodName, LabMethodLink, LabMethodDescription) " +
                "VALUES(?, ?, ?, ?, ?)" + sqlRowID;

            string sqlSaveOffsetType = "INSERT INTO OffsetTypes(OffsetUnitsID, OffsetDescription) VALUES (?, ?)" + sqlRowID;

            string sqlSaveDataValue = "INSERT INTO DataValues(SeriesID, DataValue, ValueAccuracy, LocalDateTime, " +
                "UTCOffset, DateTimeUTC, OffsetValue, OffsetTypeID, CensorCode, QualifierID, SampleID, FileID) " +
                "VALUES(?,?,?,?,?,?,?,?,?,?,?,?)";

            string sqlSaveTheme1 = "INSERT INTO DataThemeDescriptions(ThemeName, ThemeDescription) VALUES (?,?)" + sqlRowID;
            string sqlSaveTheme2 = "INSERT INTO DataThemes(ThemeID,SeriesID) VALUEs (?,?)";

            string sqlUpdateSeries = "UPDATE DataSeries SET BeginDateTime = ?, BeginDateTimeUTC = ?, EndDateTime = ?, EndDateTimeUTC = ?, " +
                "ValueCount = ?, UpdateDateTime = ? WHERE SeriesID = ?";

            string sqlDeleteValues = "DELETE FROM DataValues WHERE SeriesID = ? AND LocalDateTime >= ? AND LocalDateTime <= ?";

            int siteID = 0;
            int variableID = 0;
            int methodID = 0;
            int qualityControlLevelID = 0;
            int sourceID = 0;
            int seriesID = 0;
            long themeID = 0;
            
            object seriesIDResult = null;
            object qualifierIDResult = null;
            object sampleIDResult = null;
            object labMethodIDResult = null;
            object offsetTypeIDResult = null;
            object offsetUnitIDResult = null;

            var qualifierLookup = new Dictionary<string, Qualifier>();
            var sampleLookup = new Dictionary<string, Sample>();
            var offsetLookup = new Dictionary<string, OffsetType>();

            int numSavedValues = 0;

            bool seriesAlreadyExists = false;
            DateTime beginTimeDb = DateTime.MinValue;
            DateTime beginTimeUtcDb = beginTimeDb;
            DateTime endTimeDb = DateTime.MinValue;
            DateTime endTimeUtcDb = endTimeDb;
            int valueCountDb = 0;

            //Step 1 Begin Transaction
            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();

                using (DbTransaction tran = conn.BeginTransaction())
                {
                    //****************************************************************
                    //*** Step 2 Site
                    //****************************************************************
                    siteID = GetOrCreateSiteID(series.Site, conn);

                    //****************************************************************
                    //*** Step 3 Variable
                    //****************************************************************
                    variableID = GetOrCreateVariableID(series.Variable, conn);

                    //****************************************************************
                    //*** Step 4 Method
                    //****************************************************************
                    methodID = GetOrCreateMethodID(series.Method, conn);

                    //****************************************************************
                    //*** Step 5 Quality Control Level
                    //****************************************************************
                    qualityControlLevelID = GetOrCreateQualityControlLevelID(series.QualityControlLevel, conn);

                    //****************************************************************
                    //*** Step 6 Source
                    //****************************************************************
                    sourceID = GetOrCreateSourceID(series.Source, conn);

                    //****************************************************************
                    //*** Step 7 Series
                    //****************************************************************
                    seriesIDResult = null;
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
                                beginTimeUtcDb = Convert.ToDateTime(reader[2]);
                                endTimeDb = Convert.ToDateTime(reader[3]);
                                endTimeUtcDb = Convert.ToDateTime(reader[4]);
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
                        GetLookups(series, out qualifierLookup, out sampleLookup, out offsetLookup);

                        //****************************************************************
                        //*** Step 9 Qualifiers
                        //****************************************************************
                        if (qualifierLookup.Count > 0)
                        {
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

                                    foreach (Qualifier qual2 in unsavedQualifiers)
                                    {
                                        cmd20.Parameters[0].Value = qual2.Code;
                                        cmd20.Parameters[1].Value = qual2.Description;
                                        qualifierIDResult = cmd20.ExecuteScalar();
                                        qual2.Id = Convert.ToInt32(qualifierIDResult);
                                    }
                                }
                            }
                        }

                        //****************************************************************
                        //*** TODO Step 10 Samples and Lab Methods
                        //****************************************************************
                        if (sampleLookup.Count > 0)
                        {
                            Dictionary<string, LabMethod> labMethodLookup = new Dictionary<string, LabMethod>();

                            using (DbCommand cmd21 = conn.CreateCommand())
                            {
                                cmd21.CommandText = sqlSample;
                                cmd21.Parameters.Add(_db.CreateParameter(DbType.String));
                                cmd21.Parameters.Add(_db.CreateParameter(DbType.String));

                                foreach (Sample sample in sampleLookup.Values)
                                {
                                    cmd21.Parameters[0].Value = sample.SampleType;
                                    cmd21.Parameters[1].Value = sample.LabSampleCode;
                                    sampleIDResult = cmd21.ExecuteScalar();
                                    if (sampleIDResult != null)
                                    {
                                        sample.Id = Convert.ToInt32(sampleIDResult);
                                    }
                                }
                            }


                            List<Sample> unsavedSamples = new List<Sample>();
                            List<LabMethod> unsavedLabMethods = new List<LabMethod>();

                            foreach (Sample samp in sampleLookup.Values)
                            {
                                if (samp.Id == 0)
                                {
                                    unsavedSamples.Add(samp);
                                    string labMethodKey = samp.LabMethod.LabName + "|" + samp.LabMethod.LabMethodName;
                                    if (!labMethodLookup.ContainsKey(labMethodKey))
                                    {
                                        labMethodLookup.Add(labMethodKey, samp.LabMethod);
                                    }
                                }
                            }

                            using (DbCommand cmd22 = conn.CreateCommand())
                            {
                                cmd22.CommandText = sqlLabMethod;
                                cmd22.Parameters.Add(_db.CreateParameter(DbType.String));
                                cmd22.Parameters.Add(_db.CreateParameter(DbType.String));

                                foreach (LabMethod labMethod in labMethodLookup.Values)
                                {
                                    cmd22.Parameters[0].Value = labMethod.LabName;
                                    cmd22.Parameters[1].Value = labMethod.LabMethodName;
                                    labMethodIDResult = cmd22.ExecuteScalar();
                                    if (labMethodIDResult != null)
                                    {
                                        labMethod.Id = Convert.ToInt32(labMethodIDResult);
                                    }
                                }
                            }

                            //check unsaved lab methods
                            foreach (LabMethod lm in labMethodLookup.Values)
                            {
                                if (lm.Id == 0)
                                {
                                    unsavedLabMethods.Add(lm);
                                }
                            }

                            //save lab methods
                            if (unsavedLabMethods.Count > 0)
                            {
                                using (DbCommand cmd23 = conn.CreateCommand())
                                {
                                    cmd23.CommandText = sqlSaveLabMethod;
                                    cmd23.Parameters.Add(_db.CreateParameter(DbType.String));
                                    cmd23.Parameters.Add(_db.CreateParameter(DbType.String));
                                    cmd23.Parameters.Add(_db.CreateParameter(DbType.String));
                                    cmd23.Parameters.Add(_db.CreateParameter(DbType.String));
                                    cmd23.Parameters.Add(_db.CreateParameter(DbType.String));

                                    foreach (LabMethod labMethodToSave in unsavedLabMethods)
                                    {
                                        cmd23.Parameters[0].Value = labMethodToSave.LabName;
                                        cmd23.Parameters[1].Value = labMethodToSave.LabOrganization;
                                        cmd23.Parameters[2].Value = labMethodToSave.LabMethodName;
                                        cmd23.Parameters[3].Value = labMethodToSave.LabMethodLink;
                                        cmd23.Parameters[4].Value = labMethodToSave.LabMethodDescription;
                                        labMethodIDResult = cmd23.ExecuteScalar();
                                        labMethodToSave.Id = Convert.ToInt32(labMethodIDResult);
                                    }
                                }
                            }

                            //save samples
                            if (unsavedSamples.Count > 0)
                            {
                                using (DbCommand cmd24 = conn.CreateCommand())
                                {
                                    cmd24.CommandText = sqlSaveSample;
                                    cmd24.Parameters.Add(_db.CreateParameter(DbType.String));
                                    cmd24.Parameters.Add(_db.CreateParameter(DbType.String));
                                    cmd24.Parameters.Add(_db.CreateParameter(DbType.Int32));

                                    foreach (Sample samp3 in unsavedSamples)
                                    {
                                        cmd24.Parameters[0].Value = samp3.SampleType;
                                        cmd24.Parameters[1].Value = samp3.LabSampleCode;
                                        cmd24.Parameters[2].Value = samp3.LabMethod.Id;
                                        sampleIDResult = cmd24.ExecuteScalar();
                                        samp3.Id = Convert.ToInt32(sampleIDResult);
                                    }
                                }
                            }
                        }



                        //****************************************************************
                        //*** TODO Step 11 Vertical Offsets (NEEDS TESTING DATA - DCEW)
                        //****************************************************************
                        if (offsetLookup.Count > 0)
                        {
                            Dictionary<string, Unit> offsetUnitLookup = new Dictionary<string, Unit>();
                            List<Unit> unsavedOffsetUnits = new List<Unit>();

                            using (DbCommand cmd25 = conn.CreateCommand())
                            {
                                cmd25.CommandText = sqlOffsetType;
                                cmd25.Parameters.Add(_db.CreateParameter(DbType.String));

                                foreach (OffsetType offset in offsetLookup.Values)
                                {
                                    cmd25.Parameters[0].Value = offset.Description;
                                    offsetTypeIDResult = cmd25.ExecuteScalar();
                                    if (offsetTypeIDResult != null)
                                    {
                                        offset.Id = Convert.ToInt32(offsetTypeIDResult);
                                    }
                                }
                            }

                            //check unsaved offsets
                            List<OffsetType> unsavedoffsets = new List<OffsetType>();
                            foreach (OffsetType offset2 in offsetLookup.Values)
                            {
                                if (offset2.Id == 0)
                                {
                                    unsavedoffsets.Add(offset2);
                                    string offsetUnitsKey = offset2.Unit.Abbreviation + "|" + offset2.Unit.Name;
                                    if (!offsetUnitLookup.ContainsKey(offsetUnitsKey))
                                    {
                                        offsetUnitLookup.Add(offsetUnitsKey, offset2.Unit);
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
                            if (unsavedOffsetUnits.Count > 0)
                            {
                                using (DbCommand cmd27 = conn.CreateCommand())
                                {
                                    cmd27.CommandText = sqlSaveUnits;
                                    cmd27.Parameters.Add(_db.CreateParameter(DbType.String));
                                    cmd27.Parameters.Add(_db.CreateParameter(DbType.String));
                                    cmd27.Parameters.Add(_db.CreateParameter(DbType.String));

                                    foreach (Unit unitToSave in unsavedOffsetUnits)
                                    {
                                        cmd27.Parameters[0].Value = unitToSave.Name;
                                        cmd27.Parameters[1].Value = unitToSave.UnitsType;
                                        cmd27.Parameters[2].Value = unitToSave.Abbreviation;

                                        offsetUnitIDResult = cmd27.ExecuteScalar();
                                        unitToSave.Id = Convert.ToInt32(offsetUnitIDResult);
                                    }
                                }
                            }

                            //save offset types
                            if (unsavedoffsets.Count > 0)
                            {
                                using (DbCommand cmd28 = conn.CreateCommand())
                                {
                                    cmd28.CommandText = sqlSaveOffsetType;
                                    cmd28.Parameters.Add(_db.CreateParameter(DbType.Int32));
                                    cmd28.Parameters.Add(_db.CreateParameter(DbType.String));

                                    foreach (OffsetType offsetToSave in unsavedoffsets)
                                    {
                                        cmd28.Parameters[0].Value = offsetToSave.Unit.Id;
                                        cmd28.Parameters[1].Value = offsetToSave.Description;
                                        offsetTypeIDResult = cmd28.ExecuteScalar();
                                        offsetToSave.Id = Convert.ToInt32(offsetTypeIDResult);
                                    }
                                }
                            }
                        }

                        //****************************************************************
                        //*** TODO Step 12 Data File - QueryInfo - DataService ***********
                        //****************************************************************

                        //****************************************************************
                        //*** TODO Step 13 Data Values related information     ***********
                        //****************************************************************

                        using (DbCommand cmd30 = conn.CreateCommand())
                        {
                            cmd30.CommandText = sqlSaveDataValue;
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.Int32, seriesID));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.Double));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.Double));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.DateTime));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.Double));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.DateTime));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.Double));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.Int32));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.String));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.Int32));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.Int32));
                            cmd30.Parameters.Add(_db.CreateParameter(DbType.Int32));

                            foreach (DataValue val in series.DataValueList)
                            {
                                cmd30.Parameters[1].Value = val.Value;
                                cmd30.Parameters[2].Value = null;
                                if (val.ValueAccuracy != 0)
                                {
                                    cmd30.Parameters[2].Value = val.ValueAccuracy;
                                }
                                cmd30.Parameters[3].Value = val.LocalDateTime;
                                cmd30.Parameters[4].Value = val.UTCOffset;
                                cmd30.Parameters[5].Value = val.DateTimeUTC;
                                if (val.OffsetType != null)
                                {
                                    cmd30.Parameters[6].Value = val.OffsetValue;
                                    cmd30.Parameters[7].Value = val.OffsetType.Id;
                                }
                                else
                                {
                                    cmd30.Parameters[6].Value = null;
                                    cmd30.Parameters[7].Value = null;
                                }
                                cmd30.Parameters[8].Value = val.CensorCode;
                                if (val.Qualifier != null)
                                {
                                    cmd30.Parameters[9].Value = val.Qualifier.Id;
                                }

                                if (val.Sample != null)
                                {
                                    cmd30.Parameters[10].Value = val.Sample.Id;
                                }

                                cmd30.Parameters[11].Value = null; //TODO Check Data File

                                cmd30.ExecuteNonQuery();
                                numSavedValues++;
                            }
                        }

                        //****************************************************************
                        //*** Step 14 Data Series Update                       ***********
                        //****************************************************************
                        if (seriesAlreadyExists == true && seriesID > 0)
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
                    object seriesThemeCombinationResult = null;
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
        public int SaveSeriesAsCopy(Series series, Theme theme)
        {
            string sqlSample = "SELECT SampleID FROM Samples WHERE SampleType = ? AND LabSampleCode = ?";
            string sqlLabMethod = "SELECT LabMethodID FROM LabMethods WHERE LabName = ? AND LabMethodName = ?";
            string sqlOffsetType = "SELECT OffsetTypeID FROM OffsetTypes WHERE OffsetDescription = ?";
            string sqlRowID = "; SELECT LAST_INSERT_ROWID();";
            
            string sqlSaveUnits = "INSERT INTO Units(UnitsName, UnitsType, UnitsAbbreviation) VALUES(?, ?, ?)" + sqlRowID;
            string sqlSaveSeries = "INSERT INTO DataSeries(SiteID, VariableID, MethodID, SourceID, QualityControlLevelID, " +
                "IsCategorical, BeginDateTime, EndDateTime, BeginDateTimeUTC, EndDateTimeUTC, ValueCount, CreationDateTime, " +
                "Subscribed, UpdateDateTime, LastCheckedDateTime) " +
                "VALUES(?, ?, ?, ?,?,?,?,?,?,?,?,?,?,?,?)" + sqlRowID;

            string sqlSaveQualifier = "INSERT INTO Qualifiers(QualifierCode, QualifierDescription) VALUES (?,?)" + sqlRowID;

            string sqlSaveSample = "INSERT INTO Samples(SampleType, LabSampleCode, LabMethodID) VALUES (?,?, ?)" + sqlRowID;

            string sqlSaveLabMethod = "INSERT INTO LabMethods(LabName, LabOrganization, LabMethodName, LabMethodLink, LabMethodDescription) " +
                "VALUES(?, ?, ?, ?, ?)" + sqlRowID;

            string sqlSaveOffsetType = "INSERT INTO OffsetTypes(OffsetUnitsID, OffsetDescription) VALUES (?, ?)" + sqlRowID;

            string sqlSaveDataValue = "INSERT INTO DataValues(SeriesID, DataValue, ValueAccuracy, LocalDateTime, " +
                "UTCOffset, DateTimeUTC, OffsetValue, OffsetTypeID, CensorCode, QualifierID, SampleID, FileID) " +
                "VALUES(?,?,?,?,?,?,?,?,?,?,?,?)";

            string sqlSaveTheme1 = "INSERT INTO DataThemeDescriptions(ThemeName, ThemeDescription) VALUES (?,?)" + sqlRowID;
            string sqlSaveTheme2 = "INSERT INTO DataThemes(ThemeID,SeriesID) VALUEs (?,?)";

            int siteID = 0;
            int variableID = 0;
            int methodID = 0;
            int qualityControlLevelID = 0;
            int sourceID = 0;
            int seriesID = 0;
            long themeID = 0;
            
            object seriesIDResult = null;
            object qualifierIDResult = null;
            object sampleIDResult = null;
            object labMethodIDResult = null;
            object offsetTypeIDResult = null;
            object offsetUnitIDResult = null;

            var qualifierLookup = new Dictionary<string, Qualifier>();
            var sampleLookup = new Dictionary<string, Sample>();
            var offsetLookup = new Dictionary<string, OffsetType>();

            int numSavedValues = 0;
            
            //Step 1 Begin Transaction
            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                
                using (DbTransaction tran = conn.BeginTransaction())
                {
                    //****************************************************************
                    //*** Step 2 Site
                    //****************************************************************
                    siteID = GetOrCreateSiteID(series.Site, conn);
                   
                    //****************************************************************
                    //*** Step 3 Variable
                    //****************************************************************
                    variableID = GetOrCreateVariableID(series.Variable, conn);

                    //****************************************************************
                    //*** Step 4 Method
                    //****************************************************************
                    methodID = GetOrCreateMethodID(series.Method, conn);

                    //****************************************************************
                    //*** Step 5 Quality Control Level
                    //****************************************************************
                    qualityControlLevelID = GetOrCreateQualityControlLevelID(series.QualityControlLevel, conn);
                   
                    //****************************************************************
                    //*** Step 6 Source
                    //****************************************************************
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
                    GetLookups(series, out qualifierLookup, out sampleLookup, out offsetLookup);

                    //****************************************************************
                    //*** Step 9 Qualifiers
                    //****************************************************************
                    if (qualifierLookup.Count > 0)
                    {
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

                                foreach (Qualifier qual2 in unsavedQualifiers)
                                {
                                    cmd20.Parameters[0].Value = qual2.Code;
                                    cmd20.Parameters[1].Value = qual2.Description;
                                    qualifierIDResult = cmd20.ExecuteScalar();
                                    qual2.Id = Convert.ToInt32(qualifierIDResult);
                                }
                            }
                        }
                    }

                    //****************************************************************
                    //*** TODO Step 10 Samples and Lab Methods
                    //****************************************************************
                    if (sampleLookup.Count > 0)
                    {
                        Dictionary<string, LabMethod> labMethodLookup = new Dictionary<string, LabMethod>();

                        using (DbCommand cmd21 = conn.CreateCommand())
                        {
                            cmd21.CommandText = sqlSample;
                            cmd21.Parameters.Add(_db.CreateParameter(DbType.String));
                            cmd21.Parameters.Add(_db.CreateParameter(DbType.String));

                            foreach (Sample sample in sampleLookup.Values)
                            {
                                cmd21.Parameters[0].Value = sample.SampleType;
                                cmd21.Parameters[1].Value = sample.LabSampleCode;
                                sampleIDResult = cmd21.ExecuteScalar();
                                if (sampleIDResult != null)
                                {
                                    sample.Id = Convert.ToInt32(sampleIDResult);
                                }
                            }
                        }


                        List<Sample> unsavedSamples = new List<Sample>();
                        List<LabMethod> unsavedLabMethods = new List<LabMethod>();
                        
                        foreach (Sample samp in sampleLookup.Values)
                        {
                            if (samp.Id == 0)
                            {
                                unsavedSamples.Add(samp);
                                string labMethodKey = samp.LabMethod.LabName + "|" + samp.LabMethod.LabMethodName;
                                if (! labMethodLookup.ContainsKey(labMethodKey))
                                {
                                    labMethodLookup.Add(labMethodKey, samp.LabMethod);
                                }
                            }
                        }

                        using (DbCommand cmd22 = conn.CreateCommand())
                        {
                            cmd22.CommandText = sqlLabMethod;
                            cmd22.Parameters.Add(_db.CreateParameter(DbType.String));
                            cmd22.Parameters.Add(_db.CreateParameter(DbType.String));

                            foreach (LabMethod labMethod in labMethodLookup.Values)
                            {
                                cmd22.Parameters[0].Value = labMethod.LabName;
                                cmd22.Parameters[1].Value = labMethod.LabMethodName;
                                labMethodIDResult = cmd22.ExecuteScalar();
                                if (labMethodIDResult != null)
                                {
                                    labMethod.Id = Convert.ToInt32(labMethodIDResult);
                                }
                            }
                        }

                        //check unsaved lab methods
                        foreach (LabMethod lm in labMethodLookup.Values)
                        {
                            if (lm.Id == 0)
                            {
                                unsavedLabMethods.Add(lm);
                            }
                        }

                        //save lab methods
                        if (unsavedLabMethods.Count > 0)
                        {
                            using (DbCommand cmd23 = conn.CreateCommand())
                            {
                                cmd23.CommandText = sqlSaveLabMethod;
                                cmd23.Parameters.Add(_db.CreateParameter(DbType.String));
                                cmd23.Parameters.Add(_db.CreateParameter(DbType.String));
                                cmd23.Parameters.Add(_db.CreateParameter(DbType.String));
                                cmd23.Parameters.Add(_db.CreateParameter(DbType.String));
                                cmd23.Parameters.Add(_db.CreateParameter(DbType.String));
                            
                                foreach (LabMethod labMethodToSave in unsavedLabMethods)
                                {
                                    cmd23.Parameters[0].Value = labMethodToSave.LabName;
                                    cmd23.Parameters[1].Value = labMethodToSave.LabOrganization;
                                    cmd23.Parameters[2].Value = labMethodToSave.LabMethodName;
                                    cmd23.Parameters[3].Value = labMethodToSave.LabMethodLink;
                                    cmd23.Parameters[4].Value = labMethodToSave.LabMethodDescription;
                                    labMethodIDResult = cmd23.ExecuteScalar();
                                    labMethodToSave.Id = Convert.ToInt32(labMethodIDResult);
                                }
                            }
                        }

                        //save samples
                        if (unsavedSamples.Count > 0)
                        {
                            using (DbCommand cmd24 = conn.CreateCommand())
                            {
                                cmd24.CommandText = sqlSaveSample;
                                cmd24.Parameters.Add(_db.CreateParameter(DbType.String));
                                cmd24.Parameters.Add(_db.CreateParameter(DbType.String));
                                cmd24.Parameters.Add(_db.CreateParameter(DbType.Int32));

                                foreach (Sample samp3 in unsavedSamples)
                                {
                                    cmd24.Parameters[0].Value = samp3.SampleType;
                                    cmd24.Parameters[1].Value = samp3.LabSampleCode;
                                    cmd24.Parameters[2].Value = samp3.LabMethod.Id;
                                    sampleIDResult = cmd24.ExecuteScalar();
                                    samp3.Id = Convert.ToInt32(sampleIDResult);
                                }
                            }
                        }
                    }



                    //****************************************************************
                    //*** TODO Step 11 Vertical Offsets (NEEDS TESTING DATA - DCEW)
                    //****************************************************************
                    if (offsetLookup.Count > 0)
                    {
                        Dictionary<string, Unit> offsetUnitLookup = new Dictionary<string, Unit>();
                        List<Unit> unsavedOffsetUnits = new List<Unit>();
                        
                        using (DbCommand cmd25 = conn.CreateCommand())
                        {
                            cmd25.CommandText = sqlOffsetType;
                            cmd25.Parameters.Add(_db.CreateParameter(DbType.String));

                            foreach (OffsetType offset in offsetLookup.Values)
                            {
                                cmd25.Parameters[0].Value = offset.Description;
                                offsetTypeIDResult = cmd25.ExecuteScalar();
                                if (offsetTypeIDResult != null)
                                {
                                    offset.Id = Convert.ToInt32(offsetTypeIDResult);
                                }
                            }
                        }

                        //check unsaved offsets
                        List<OffsetType> unsavedoffsets = new List<OffsetType>();
                        foreach (OffsetType offset2 in offsetLookup.Values)
                        {
                            if (offset2.Id == 0)
                            {
                                unsavedoffsets.Add(offset2);
                                string offsetUnitsKey =  offset2.Unit.Abbreviation + "|" + offset2.Unit.Name;
                                if (!offsetUnitLookup.ContainsKey(offsetUnitsKey))
                                {
                                    offsetUnitLookup.Add(offsetUnitsKey, offset2.Unit);
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
                            }else
                            {
                                unsavedOffsetUnits.Add(offsetUnit);
                            }
                        }
                      

                        //save offset units
                        if (unsavedOffsetUnits.Count > 0)
                        {
                            using (DbCommand cmd27 = conn.CreateCommand())
                            {
                                cmd27.CommandText = sqlSaveUnits;
                                cmd27.Parameters.Add(_db.CreateParameter(DbType.String));
                                cmd27.Parameters.Add(_db.CreateParameter(DbType.String));
                                cmd27.Parameters.Add(_db.CreateParameter(DbType.String));

                                foreach (Unit unitToSave in unsavedOffsetUnits)
                                {
                                    cmd27.Parameters[0].Value = unitToSave.Name;
                                    cmd27.Parameters[1].Value = unitToSave.UnitsType;
                                    cmd27.Parameters[2].Value = unitToSave.Abbreviation;
                                    
                                    offsetUnitIDResult = cmd27.ExecuteScalar();
                                    unitToSave.Id = Convert.ToInt32(offsetUnitIDResult);
                                }
                            }
                        }

                        //save offset types
                        if (unsavedoffsets.Count > 0)
                        {
                            using (DbCommand cmd28 = conn.CreateCommand())
                            {
                                cmd28.CommandText = sqlSaveOffsetType;
                                cmd28.Parameters.Add(_db.CreateParameter(DbType.Int32));
                                cmd28.Parameters.Add(_db.CreateParameter(DbType.String));

                                foreach (OffsetType offsetToSave in unsavedoffsets)
                                {
                                    cmd28.Parameters[0].Value = offsetToSave.Unit.Id;
                                    cmd28.Parameters[1].Value = offsetToSave.Description;
                                    offsetTypeIDResult = cmd28.ExecuteScalar();
                                    offsetToSave.Id = Convert.ToInt32(offsetTypeIDResult);
                                }
                            }
                        }
                    }

                    //****************************************************************
                    //*** TODO Step 12 Data File - QueryInfo - DataService ***********
                    //****************************************************************

                    //****************************************************************
                    //*** TODO Step 13 Data Values                         ***********
                    //****************************************************************
                    using (DbCommand cmd30 = conn.CreateCommand())
                    {
                        cmd30.CommandText = sqlSaveDataValue;
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.Int32, seriesID));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.Double));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.Double));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.DateTime));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.Double));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.DateTime));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.Double));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.Int32));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.String));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.Int32));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.Int32));
                        cmd30.Parameters.Add(_db.CreateParameter(DbType.Int32));

                        foreach (DataValue val in series.DataValueList)
                        {
                            cmd30.Parameters[1].Value = val.Value;
                            cmd30.Parameters[2].Value = null;
                            if (val.ValueAccuracy != 0)
                            {
                                cmd30.Parameters[2].Value = val.ValueAccuracy;
                            }
                            cmd30.Parameters[3].Value = val.LocalDateTime;
                            cmd30.Parameters[4].Value = val.UTCOffset;
                            cmd30.Parameters[5].Value = val.DateTimeUTC;
                            if (val.OffsetType != null)
                            {
                                cmd30.Parameters[6].Value = val.OffsetValue;
                                cmd30.Parameters[7].Value = val.OffsetType.Id;
                            }
                            else
                            {
                                cmd30.Parameters[6].Value = null;
                                cmd30.Parameters[7].Value = null;
                            }
                            cmd30.Parameters[8].Value = val.CensorCode;
                            if (val.Qualifier != null)
                            {
                                cmd30.Parameters[9].Value = val.Qualifier.Id;
                            }

                            if (val.Sample != null)
                            {
                                cmd30.Parameters[10].Value = val.Sample.Id;
                            }
                            
                            cmd30.Parameters[11].Value = null; //TODO Check Data File

                            cmd30.ExecuteNonQuery();
                            numSavedValues++;
                        }
                    }

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
            string sqlSaveUnits = "INSERT INTO Units(UnitsName, UnitsType, UnitsAbbreviation) VALUES(?, ?, ?)" + LastRowIDSelect;
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
                    using (DbCommand cmd07 = conn.CreateCommand())
                    {
                        cmd07.CommandText = sqlSaveUnits;
                        cmd07.Parameters.Add(_db.CreateParameter(DbType.String, variable.VariableUnit.Name));
                        cmd07.Parameters.Add(_db.CreateParameter(DbType.String, variable.VariableUnit.UnitsType));
                        cmd07.Parameters.Add(_db.CreateParameter(DbType.String, variable.VariableUnit.Abbreviation));
                        var variableUnitsIDResult = cmd07.ExecuteScalar();
                        variableUnitsID = Convert.ToInt32(variableUnitsIDResult);
                    }
                }

                // Save the time units
                if (timeUnitsID == 0 &&
                    variable.TimeUnit != null)
                {
                    using (DbCommand cmd08 = conn.CreateCommand())
                    {
                        cmd08.CommandText = sqlSaveUnits;
                        cmd08.Parameters.Add(_db.CreateParameter(DbType.String, variable.TimeUnit.Name));
                        cmd08.Parameters.Add(_db.CreateParameter(DbType.String, variable.TimeUnit.UnitsType));
                        cmd08.Parameters.Add(_db.CreateParameter(DbType.String, variable.TimeUnit.Abbreviation));
                        var timeUnitsIDResult = cmd08.ExecuteScalar();
                        timeUnitsID = Convert.ToInt32(timeUnitsIDResult);
                    }
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
            const string sqlUnits = "SELECT UnitsID FROM Units WHERE UnitsName = ? AND UnitsType = ? AND UnitsAbbreviation = ?";

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sqlUnits;
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, unit.Name));
                cmd.Parameters.Add(_db.CreateParameter(DbType.String, unit.UnitsType));
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

        #endregion

    }
}

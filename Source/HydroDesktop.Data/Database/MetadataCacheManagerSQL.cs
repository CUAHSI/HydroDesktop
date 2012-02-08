using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace HydroDesktop.Database
{
	/// <summary>
	/// This class is responsible for communication with the 'Metadata Cache' database
    /// This is an alternative implementation internally using SQL queries instead of NHibernate.
    /// This is to compare saving speed.
	/// </summary>
	public class MetadataCacheManagerSQL
	{
		#region Variables

        //helper class which communicates with the database
        private DbOperations _db;

		// lookup caches used by the SaveSeries method
		private Dictionary<string, Site> _siteCache = new Dictionary<string, Site> ();
		private Dictionary<string, Variable> _variableCache = new Dictionary<string, Variable> ();
		private Dictionary<string, Method> _methodCache = new Dictionary<string, Method> ();
		private Dictionary<string, QualityControlLevel> _qualControlCache = new Dictionary<string, QualityControlLevel> ();
		private Dictionary<string, Source> _sourcesCache = new Dictionary<string, Source> ();

		#endregion

		#region Constructor
		/// <summary>
		/// Creates a new instance of the manager given a connection string
		/// </summary>
		/// <param name="dbType">The type of the database (SQLite, SQLServer, ...)</param>
		/// <param name="connectionString">The connection string</param>
		public MetadataCacheManagerSQL ( DatabaseTypes dbType, string connectionString )
		{
            _db = new DbOperations(connectionString, dbType);
		}
        /// <summary>
        /// Creates a new instance of the manager using a DbOperations object
        /// </summary>
        /// <param name="db">The dbOperations object</param>
        public MetadataCacheManagerSQL(DbOperations db)
        {
            _db = db;
        }
		#endregion

		private bool NumberIsBetween ( double numberToCheck, double bounds1, double bounds2, bool inclusiveAtBounds )
		{
			double lowerBound, upperBound;

			if ( bounds1 > bounds2 )
			{
				lowerBound = bounds2;
				upperBound = bounds1;
			}
			else
			{
				lowerBound = bounds1;
				upperBound = bounds2;
			}

			if ( inclusiveAtBounds == true )
			{
				return (numberToCheck >= lowerBound && numberToCheck <= upperBound);
			}
			else
			{
				return (numberToCheck > lowerBound && numberToCheck < upperBound);
			}
		}

		private bool EnvelopesIntersect ( double env1xMin, double env1xMax, double env1yMin, double env1yMax, double env2xMin, double env2xMax, double env2yMin, double env2yMax )
		{
			return (((NumberIsBetween ( env1xMin, env2xMin, env2xMax, true ) || NumberIsBetween ( env1xMax, env2xMin, env2xMax, true )) && (NumberIsBetween ( env1yMin, env2yMin, env2yMax, true ) || NumberIsBetween ( env1yMax, env2yMin, env2yMax, true ))) ||
					((NumberIsBetween ( env2xMin, env1xMin, env1xMax, true ) || NumberIsBetween ( env2xMax, env1xMin, env1xMax, true )) && (NumberIsBetween ( env2yMin, env1yMin, env1yMax, true ) || NumberIsBetween ( env2yMax, env1yMin, env1yMax, true ))));
		}

		private bool PointIntersectsEnvelope ( double pointX, double pointY, double envXMin, double envXMax, double envYMin, double envYMax )
		{
			return (NumberIsBetween ( pointX, envXMin, envXMax, true ) && NumberIsBetween ( pointY, envYMin, envYMax, true ));
		}

		private bool DateRangesOverlap ( DateTime startDate1, DateTime endDate1, DateTime startDate2, DateTime endDate2 )
		{
			return (((startDate1 >= startDate2) && (startDate1 <= endDate2)) ||
					((endDate1 >= startDate2) && (endDate1 <= endDate2)) ||
					((startDate2 >= startDate1) && (startDate2 <= endDate1)) ||
					((endDate2 >= startDate1) && (endDate2 <= endDate1)));
		}

        

		#region Public Methods



		/// <summary>
		/// Get all data services saved in the metadata cache database
		/// </summary>
		public IList<DataServiceInfo> GetAllServices ()
		{
			string sql = "SELECT * FROM DataServices";

            System.Data.DataTable tbl = _db.LoadTable("services", sql);

            IList<DataServiceInfo> services = null;

			services = new List<DataServiceInfo> ();
			if ( tbl.Rows.Count > 0 )
            {
                foreach(System.Data.DataRow row in tbl.Rows)
                {
                    services.Add(ServiceFromDataRow(row));
                }
            }
            return services;
		}

        /// <summary>
        /// Get data service by serviceUrl
        /// </summary>
        /// <param name="serviceURL">ServiceUrl</param>
        /// <returns>Data service or null (if not found)</returns>
        public DataServiceInfo GetServiceByServiceUrl(string serviceURL)
        {
            var sql = string.Format("SELECT * FROM DataServices where ServiceID = '{0}'", serviceURL);
            var tbl = _db.LoadTable("services", sql);
            return tbl.Rows.Count == 1 ? ServiceFromDataRow(tbl.Rows[0]) : null;
        }

	    #endregion
        /// <summary>
        /// Gets all sites in box (not implemented)
        /// </summary>
        /// <param name="xMin">minimum x (longitude)</param>
        /// <param name="xMax">maximum x (lognitude)</param>
        /// <param name="yMin">minimum y (latitude)</param>
        /// <param name="yMax">maximum y (latitude)</param>
		public void GetSitesInBox ( double xMin, double xMax, double yMin, double yMax )
		{
			throw new System.NotImplementedException ();
		}
        /// <summary>
        /// Gets a list of all services within the bounding box
        /// </summary>
        /// <param name="xMin">minimum x (longitude)</param>
        /// <param name="xMax">maximum x (lognitude)</param>
        /// <param name="yMin">minimum y (latitude)</param>
        /// <param name="yMax">maximum y (latitude)</param>
        /// <returns>the list of serviceInfo objects matching the criteria</returns>
		public IList<DataServiceInfo> GetServicesInBox ( double xMin, double xMax, double yMin, double yMax )
		{
			//IList<DataServiceInfo> services = null;

            string sql = "SELECT * FROM DataServicesCache WHERE " +
                String.Format("EastLongitude BETWEEN {0} AND {1}", xMin, xMax) +
                String.Format("OR WestLongitude BETWEEN {0} AND {1}", xMin, xMax) +
                String.Format("OR NorthLatitude BETWEEN {0} AND {1}", yMin, yMax) +
                String.Format("OR SouthLatitude BETWEEN {0} AND {1}", yMin, yMax);

            DataTable tbl = _db.LoadTable(sql);

            IList<DataServiceInfo> services = null;

            if (tbl.Rows.Count > 0)
            {
                services = new List<DataServiceInfo>();
                foreach (System.Data.DataRow row in tbl.Rows)
                {
                    services.Add(ServiceFromDataRow(row));
                }
            }
            return services;
		}

        private string DetailedSeriesSQLQuery()
        {
            string sql = "SELECT SeriesID, " +
                "SiteName, SiteCode, Latitude, Longitude, " +
                "VariableName, VariableCode, DataType, ValueType, Speciation, SampleMedium, " +
                "TimeSupport, GeneralCategory, " +
                "TimeUnitsName, " +
                "BeginDateTime, EndDateTime, DataSeriesCache.ValueCount, ServiceTitle, ServiceEndpointURL " +
                "FROM DataSeriesCache " +
                "LEFT JOIN SitesCache ON DataSeriesCache.SiteID = SitesCache.SiteID " +
                "LEFT JOIN VariablesCache ON DataSeriesCache.VariableID = VariablesCache.VariableID " + 
                "LEFT JOIN DataServices ON DataSeriesCache.ServiceID = DataServices.ServiceID";
            return sql;
        }

        private DataServiceInfo ServiceFromDataRow(System.Data.DataRow row)
        {
            DataServiceInfo dsi = new DataServiceInfo();
            dsi.Id = DataReader.ReadInteger(row["ServiceID"]);
            dsi.ServiceCode = DataReader.ReadString(row["ServiceCode"]);
            dsi.ServiceName = DataReader.ReadString(row["ServiceName"]);
            dsi.ServiceType = DataReader.ReadString(row["ServiceType"]);
            dsi.Version = DataReader.ReadDouble(row["ServiceVersion"]);
            dsi.Protocol = DataReader.ReadString(row["ServiceProtocol"]);
            dsi.EndpointURL = DataReader.ReadString(row["ServiceEndpointURL"]);
            dsi.DescriptionURL = DataReader.ReadString(row["ServiceDescriptionURL"]);
            dsi.NorthLatitude = DataReader.ReadDouble(row["NorthLatitude"]);
            dsi.SouthLatitude = DataReader.ReadDouble(row["SouthLatitude"]);
            dsi.EastLongitude = DataReader.ReadDouble(row["EastLongitude"]);
            dsi.WestLongitude = DataReader.ReadDouble(row["WestLongitude"]);
            dsi.Abstract = DataReader.ReadString(row["Abstract"]);
            dsi.ContactEmail = DataReader.ReadString(row["ContactEmail"]);
            dsi.ContactName = DataReader.ReadString(row["ContactName"]);
            dsi.Citation = DataReader.ReadString(row["Citation"]);
            dsi.IsHarvested = DataReader.ReadBoolean(row["IsHarvested"]);
            dsi.HarveDateTime = DataReader.ReadDateTime(row["HarveDateTime"]);
            dsi.ServiceTitle = DataReader.ReadString(row["ServiceTitle"]);
            return dsi;
        }

        private Variable VariableFromDataRow(DataRow row)
        {
            Variable v = new Variable();
            v.Name = Convert.ToString(row["VariableName"]);
            v.Code = Convert.ToString(row["VariableCode"]);
            v.DataType = Convert.ToString(row["DataType"]);
            v.ValueType = Convert.ToString(row["ValueType"]);
            v.Speciation = Convert.ToString(row["Speciation"]);
            v.SampleMedium = Convert.ToString(row["SampleMedium"]);
            v.TimeSupport = Convert.ToDouble(row["TimeSupport"]);
            v.GeneralCategory = Convert.ToString(row["GeneralCategory"]);
            v.VariableUnit = Unit.Unknown;
            v.VariableUnit.Name = Convert.ToString(row["VariableUnitsName"]);
            v.VariableUnit.Abbreviation = Convert.ToString(row["VariableUnitsAbbreviation"]);
            v.TimeUnit = Unit.UnknownTimeUnit;
            v.TimeUnit.Name = Convert.ToString(row["TimeUnitsName"]);
            v.IsRegular = Convert.ToBoolean(row["IsRegular"]);
            v.NoDataValue = Convert.ToDouble(row["NoDataValue"]);
            v.Id = Convert.ToInt32(row["VariableID"]);
            return v;
        }

        private SeriesMetadata SeriesFromDataRow(DataRow row)
        {
            Site site = new Site();
            site.Name = Convert.ToString(row["SiteName"]);
            site.Code = Convert.ToString(row["SiteCode"]);
            site.Latitude = Convert.ToDouble(row["Latitude"]);
            site.Longitude = Convert.ToDouble(row["Longitude"]);

            Variable v = new Variable();
            v.Name = Convert.ToString(row["VariableName"]);
            v.Code = Convert.ToString(row["VariableCode"]);
            v.DataType = Convert.ToString(row["DataType"]);
            v.ValueType = Convert.ToString(row["ValueType"]);
            v.Speciation = Convert.ToString(row["Speciation"]);
            v.SampleMedium = Convert.ToString(row["SampleMedium"]);
            v.TimeSupport = Convert.ToDouble(row["TimeSupport"]);
            v.GeneralCategory = Convert.ToString(row["GeneralCategory"]);
            v.VariableUnit = Unit.Unknown;
            v.VariableUnit.Name = Convert.ToString(row["VariableUnitsName"]);
            v.TimeUnit = Unit.UnknownTimeUnit;
            v.TimeUnit.Name = Convert.ToString(row["TimeUnitsName"]);

            Method m = Method.Unknown;
            //m.Description = Convert.ToString(row["MethodDescription"]);

            Source src = Source.Unknown;
            //src.Description = Convert.ToString(row["SourceDescription"]);
            src.Organization = Convert.ToString(row["Organization"]);
            src.Citation = Convert.ToString(row["Citation"]);

            QualityControlLevel qc = QualityControlLevel.Unknown;
            //qc.Code = Convert.ToString(row["QualityControlLevelCode"]);
            //qc.Definition = Convert.ToString(row["QualityControlLevelDefinition"]);

            SeriesMetadata newSeries = new SeriesMetadata(site, v, m, qc, src);
            newSeries.BeginDateTime = Convert.ToDateTime(row["BeginDateTime"]);
            newSeries.EndDateTime = Convert.ToDateTime(row["EndDateTime"]);
            newSeries.BeginDateTimeUTC = Convert.ToDateTime(row["BeginDateTimeUTC"]);
            newSeries.EndDateTimeUTC = Convert.ToDateTime(row["EndDateTimeUTC"]);
            newSeries.ValueCount = Convert.ToInt32(row["ValueCount"]);

            DataServiceInfo servInfo = new DataServiceInfo();
            servInfo.EndpointURL = Convert.ToString(row["ServiceEndpointURL"]);
            //servInfo.ServiceCode = Convert.ToString(row["ServiceCode"]);
            newSeries.DataService = servInfo;

            return newSeries;
        }

        /// <summary>
        /// Converts DataRow into SeriesDataCart
        /// </summary>
        /// <param name="row">DataRow to convert</param>
        /// <returns>SeriesDataCart</returns>
        public SeriesDataCart SeriesDataCartFromRow(DataRow row)
        {
            var result = new SeriesDataCart();
            result.SiteName = Convert.ToString(row["SiteName"]);
            result.SiteCode = Convert.ToString(row["SiteCode"]);
            result.Latitude = Convert.ToDouble(row["Latitude"], CultureInfo.InvariantCulture);
            result.Longitude = Convert.ToDouble(row["Longitude"], CultureInfo.InvariantCulture);

            //Variable v = new Variable();
            result.VariableName = Convert.ToString(row["VariableName"]);
            result.VariableCode = Convert.ToString(row["VariableCode"]);
            result.DataType = Convert.ToString(row["DataType"]);
            result.ValueType = Convert.ToString(row["ValueType"]);
            
            result.SampleMedium = Convert.ToString(row["SampleMedium"]);
            result.TimeSupport = Convert.ToDouble(row["TimeSupport"], CultureInfo.InvariantCulture);
            result.GeneralCategory = Convert.ToString(row["GeneralCategory"]);
            result.TimeUnit = Convert.ToString(row["TimeUnitsName"]);

            result.BeginDate = Convert.ToDateTime(row["BeginDateTime"], CultureInfo.InvariantCulture);
            result.EndDate = Convert.ToDateTime(row["EndDateTime"], CultureInfo.InvariantCulture);
            result.ValueCount = Convert.ToInt32(row["ValueCount"]);

            result.ServURL = Convert.ToString(row["ServiceEndpointURL"]);
            result.ServCode = Convert.ToString(row["ServiceTitle"]);

            return result;
        }
        /// <summary>
        /// Gets a list of all data series within the bounding box
        /// </summary>
        /// <param name="xMin">minimum X (longitude)</param>
        /// <param name="xMax">maximum X (longitude)</param>
        /// <param name="yMin">minimum Y (latitude)</param>
        /// <param name="yMax">maximum Y (latitude)</param>
        /// <returns>the list of data series metadata matching the search criteria</returns>
		public IList<SeriesDataCart> GetSeriesListInBox ( double xMin, double xMax, double yMin, double yMax )
		{
            string sql1 = DetailedSeriesSQLQuery();
            string sqlWhere = " WHERE Latitude > @minlat AND Latitude <= @maxlat AND Longitude > @minlon AND Longitude <= @maxlon";
            string sql = sql1 + sqlWhere;

            DbCommand cmd = _db.CreateCommand(sql);
            //lat, lon parameters
            _db.AddParameter(cmd, "@minlat", DbType.Double);
            _db.AddParameter(cmd, "@maxlat", DbType.Double);
            _db.AddParameter(cmd, "@minlon", DbType.Double);
            _db.AddParameter(cmd, "@maxlon", DbType.Double);

            cmd.Parameters[0].Value = yMin;
            cmd.Parameters[1].Value = yMax;
            cmd.Parameters[2].Value = xMin;
            cmd.Parameters[3].Value = xMax;

            DataTable seriesTable = _db.LoadTable("seriesTable", cmd);
            
            //DataTable seriesTable = _db.LoadTable("seriesTable", sql);

            IList<SeriesDataCart> lst = new List<SeriesDataCart>();
            foreach (DataRow row in seriesTable.Rows)
            {
                SeriesDataCart newSeries = SeriesDataCartFromRow(row);
                lst.Add(newSeries);
            }

            return lst;
		}

        /// <summary>
        /// Gets a data table of all data series within the bounding box
        /// </summary>
        /// <param name="xMin">minimum X (longitude)</param>
        /// <param name="xMax">maximum X (longitude)</param>
        /// <param name="yMin">minimum Y (latitude)</param>
        /// <param name="yMax">maximum Y (latitude)</param>
        /// <param name="conceptCodes">array of Concept keywords</param>
        /// <param name="startDate">start date</param>
        /// <param name="endDate">end date</param>
        /// <param name="networkIDs">larray of service codes</param>
        /// <returns>the list of data series metadata matching the search criteria</returns>
		public DataTable GetSeriesDataTableInBox ( double xMin, double xMax, double yMin, double yMax, string[] conceptCodes, DateTime startDate, DateTime endDate, int[] networkIDs )
		{
            string sql1 = DetailedSeriesSQLQuery();
            string sqlWhere1 = " WHERE Latitude > @minlat AND Latitude < @maxlat AND Longitude > @minlon AND Longitude < @maxlon";
            string sqlWhere2 = "";

            //concept keywords | variable names
            if (conceptCodes == null)
            {
                sqlWhere2 = "";
            }
            else if (conceptCodes.Length == 0)
            {
                sqlWhere2 = "";
            }
            else if (string.IsNullOrEmpty(conceptCodes[0]))
            {
                sqlWhere2 = "";
            }
            else if (conceptCodes.Length == 1)
            {
                sqlWhere2 = " AND VariableName = '" + conceptCodes[0] + "'";
            }
            else if (conceptCodes.Length > 1)
            {
                sqlWhere2 = " AND VariableName IN (";
                foreach (string keyword in conceptCodes)
                {
                    sqlWhere2 += "'" + keyword + "',";
                }
                if (sqlWhere2.EndsWith(","))
                {
                    sqlWhere2 = sqlWhere2.Substring(0, sqlWhere2.Length - 1);
                }
                sqlWhere2 += ")";
            }

            //date and time
            string sqlWhere3 = " AND ( (BeginDateTime < @p1 AND EndDateTime > @p2) OR (BeginDateTime > @p1 AND BeginDateTime <= @p2) OR (EndDateTime > @p1 AND EndDateTime <= @p2) )";
            
            //network IDs
            string sqlWhere4 = "";

            if (networkIDs != null)
            {
                if (networkIDs.Length == 1)
                {
                    sqlWhere4 = " AND DataSeriesCache.ServiceID = " + networkIDs[0];
                }
                else if (networkIDs.Length > 1)
                {
                    sqlWhere4 = " AND DataSeriesCache.ServiceID IN (";
                    foreach (int servID in networkIDs)
                    {
                        sqlWhere4 += servID.ToString() + ",";
                    }
                    if (sqlWhere4.EndsWith(","))
                    {
                        sqlWhere4 = sqlWhere4.Substring(0, sqlWhere4.Length - 1);
                    }
                    sqlWhere4 += ")";
                }
            }

            string sql = sql1 + sqlWhere1 + sqlWhere2 + sqlWhere3 + sqlWhere4;
            DbCommand cmd = _db.CreateCommand(sql);
            //lat, lon parameters
            _db.AddParameter(cmd, "@minlat", DbType.Double);
            _db.AddParameter(cmd, "@maxlat", DbType.Double);
            _db.AddParameter(cmd, "@minlon", DbType.Double);
            _db.AddParameter(cmd, "@maxlon", DbType.Double);
            
            _db.AddParameter(cmd, "@p1", DbType.DateTime);
            _db.AddParameter(cmd, "@p2", DbType.DateTime);
            cmd.Parameters[0].Value = yMin;
            cmd.Parameters[1].Value = yMax;
            cmd.Parameters[2].Value = xMin;
            cmd.Parameters[3].Value = xMax;
            cmd.Parameters[4].Value = startDate;
            cmd.Parameters[5].Value = endDate;
            
            var seriesTable = _db.LoadTable("seriesTable", cmd);
            return seriesTable;
		}

        /// <summary>
        /// Gets a list of all data series within the bounding box
        /// </summary>
        /// <param name="xMin">minimum X (longitude)</param>
        /// <param name="xMax">maximum X (longitude)</param>
        /// <param name="yMin">minimum Y (latitude)</param>
        /// <param name="yMax">maximum Y (latitude)</param>
        /// <param name="conceptCodes">array of Concept keywords</param>
        /// <param name="startDate">start date</param>
        /// <param name="endDate">end date</param>
        /// <param name="networkIDs">larray of service codes</param>
        /// <returns>the list of data series metadata matching the search criteria</returns>
        public IList<SeriesDataCart> GetSeriesListInBox(double xMin, double xMax, double yMin, double yMax, string[] conceptCodes, DateTime startDate, DateTime endDate, int[] networkIDs)
        {
            var dt = GetSeriesDataTableInBox(xMin, xMax, yMin, yMax, conceptCodes, startDate, endDate, networkIDs);
            return (from DataRow row in dt.Rows select SeriesDataCartFromRow(row)).ToList();
        }

	    /// <summary>
		/// Gets all variables that are currently stored in the metadata cache database
		/// </summary>
		public IList<Variable> GetVariables()
		{
            string sql = "SELECT * FROM VariablesCache";

            DataTable tbl = _db.LoadTable(sql);
            List<Variable> variables = new List<Variable>();
            foreach (DataRow row in tbl.Rows)
            {
                Variable v = VariableFromDataRow(row);
                variables.Add(v);
            }
            return variables;
		}

        /// <summary>
        /// Gets all variables that are currently stored in the metadata cache database
        /// </summary>
        public IList<Variable> GetVariablesByService(int serviceID)
        {
            string sql = "SELECT * FROM VariablesCache WHERE serviceID=" + serviceID;

            DataTable tbl = _db.LoadTable(sql);
            List<Variable> variables = new List<Variable>();
            foreach (DataRow row in tbl.Rows)
            {
                Variable v = VariableFromDataRow(row);
                variables.Add(v);
            }
            return variables;
        }

        /// <summary>
        /// Gets the names of all variables accessible by the specific web service
        /// </summary>
        /// <returns></returns>
        public IList<string> GetVariableNamesByService(int serviceID)
        {
            string sql = "SELECT DISTINCT VariableName FROM VariablesCache WHERE ServiceID = " + serviceID;
            DataTable tbl = _db.LoadTable(sql);
            List<string> variableNames = new List<string>();
            foreach (DataRow row in tbl.Rows)
            {
                variableNames.Add(row["VariableName"].ToString());
            }
            return variableNames;
        }

        /// <summary>
        /// Gets the names of all variables that are currently stored in the metadata cache DB
        /// </summary>
        /// <returns></returns>
        public IList<string> GetVariableNames()
        {
            string sql = "SELECT DISTINCT VariableName FROM VariablesCache";
            DataTable tbl = _db.LoadTable(sql);
            List<string> variableNames = new List<string>();
            foreach (DataRow row in tbl.Rows)
            {
                variableNames.Add(row["VariableName"].ToString());
            }
            return variableNames;
        }

		/// <summary>
		/// Saves a new data service object to the database. If an entry with the same
		/// web service URL already exists in the database, update it.
		/// </summary>
		/// <param name="service">the ServiceInfo object to be saved to the DB</param>
		public void SaveDataService ( DataServiceInfo service )
		{
            string sqlInsert =
                "INSERT INTO DataServices(" +
                "ServiceCode, ServiceName, ServiceType, ServiceVersion, ServiceProtocol, " +
                "ServiceEndpointURL, ServiceDescriptionURL, NorthLatitude, SouthLatitude, EastLongitude, WestLongitude, " +
                "Abstract, ContactName, ContactEmail, Citation, IsHarvested, HarveDateTime, ServiceTitle) " +
                "VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            
            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();

                using (DbTransaction tran = conn.BeginTransaction())
                {
                    using (DbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sqlInsert;
                        cmd.Parameters.Add(_db.CreateParameter(DbType.String, service.ServiceCode));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.String, service.ServiceName));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.String, service.ServiceType));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.Double, service.Version));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.String, service.Protocol));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.String, service.EndpointURL));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.String, service.DescriptionURL));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.Double, service.NorthLatitude));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.Double, service.SouthLatitude));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.Double, service.EastLongitude));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.Double, service.WestLongitude));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.String, service.Abstract));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.String, service.ContactName));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.String, service.ContactEmail));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.String, service.Citation));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.Boolean, service.IsHarvested));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.DateTime, service.HarveDateTime));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.String, service.ServiceTitle));

                        cmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
            }
		}

		/// <summary>
		/// Deletes all entries in the metadata cache database that were
		/// added by the data service
		/// </summary>
        /// <param name="service">The serviceInfo object to be deleted</param>
		/// <param name="deleteService">Set to true if the record in the DataServices
		/// table should also be deleted. Set to false if the record in the DataServices
		/// table should be kept</param>
		/// <returns>The total number of records deleted</returns>
		public int DeleteRecordsForService ( DataServiceInfo service, bool deleteService )
		{
            string serviceID = service.Id.ToString();

            string sqlDelete = "DELETE FROM DataSeriesCache WHERE ServiceID = " + serviceID + "; " +
                "DELETE FROM SitesCache WHERE ServiceID = " + serviceID + "; " +
                "DELETE FROM VariablesCache WHERE ServiceID = " + serviceID + "; " +
                "DELETE FROM SourcesCache WHERE ServiceID = " + serviceID + "; " +
                "DELETE FROM MethodsCache WHERE ServiceID = " + serviceID + "; " +
                "DELETE FROM QualityControlLevelsCache WHERE ServiceID = " + serviceID + ";" + 
				"DELETE FROM ISOMetadataCache WHERE ServiceID = " + serviceID + ";";

			if ( deleteService == true )
			{
				sqlDelete += "DELETE FROM DataServices WHERE ServiceID = " + serviceID + ";";
			}

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();

                using (DbTransaction tran = conn.BeginTransaction())
                {
                    using (DbCommand cmd01 = conn.CreateCommand())
                    {
                        cmd01.CommandText = sqlDelete;
                        cmd01.ExecuteNonQuery();
                    }
                    tran.Commit();
                }

            }
            return 0;
		}


        /// <summary>
        /// Deletes a series given it's ID. The series is only deleted when it belongs to one theme.
        /// </summary>
        /// <param name="seriesID">The database ID of the series</param>
        /// <returns>true if series was deleted, false otherwise</returns>
        public bool DeleteSeries(int seriesID)
        {
            int siteID = 0;
            int variableID = 0;
            int sourceID = 0;
            int qualityID = 0;
            int methodID = 0;


            string sqlSeries = "SELECT SiteID, VariableID, MethodID, SourceID, QualityControlLevelID " +
                "FROM DataSeriesCache WHERE SeriesID = " + seriesID;

            DataTable seriesTable = _db.LoadTable("seriesTable", sqlSeries);
            
            if (seriesTable.Rows.Count == 0) return false;

            DataRow seriesRow = seriesTable.Rows[0];
            siteID = Convert.ToInt32(seriesRow["SiteID"]);
            variableID = Convert.ToInt32(seriesRow["VariableID"]);
            methodID = Convert.ToInt32(seriesRow["MethodID"]);
            sourceID = Convert.ToInt32(seriesRow["SourceID"]);
            qualityID = Convert.ToInt32(seriesRow["QualityControlLevelID"]);

            //SQL Queries
            string sqlSite = "SELECT SiteID from DataSeriesCache where SiteID = " + siteID;
            string sqlVariable = "SELECT VariableID from DataSeriesCache where VariableID = " + variableID;
            string sqlSource = "SELECT SourceID from DataSeriesCache where SourceID = " + sourceID;
            string sqlMethod = "SELECT MethodID from DataSeriesCache where MethodID = " + methodID;
            string sqlQuality = "SELECT QualityControlLevelID from DataSeriesCache where QualityControlLevelID = " + qualityID;


            //SQL Delete Commands
            string sqlDeleteSeries = "DELETE FROM DataSeriesCache WHERE SeriesID = " + seriesID;

            string sqlDeleteSite = "DELETE FROM SitesCache WHERE SiteID = " + siteID;
            string sqlDeleteVariable = "DELETE FROM VariablesCache WHERE VariableID = " + variableID;
            string sqlDeleteMethod = "DELETE FROM MethodsCache WHERE MethodID = " + methodID;
            string sqlDeleteSource = "DELETE FROM SourcesCache WHERE SourceID = " + sourceID;
            string sqlDeleteQuality = "DELETE FROM QualityControlLevelsCache WHERE QualityControlLevelID = " + qualityID;

            DataTable tblSite = new DataTable();
            DataTable tblVariable = new DataTable();
            DataTable tblSource = new DataTable();
            DataTable tblMethod = new DataTable();
            DataTable tblQuality = new DataTable();

            //Begin Transaction
            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();

                using (DbTransaction tran = conn.BeginTransaction())
                {
                    // get site IDs
                    using (DbCommand cmd01 = conn.CreateCommand())
                    {
                        cmd01.CommandText = sqlSite;
                        tblSite = _db.LoadTable("t1", cmd01);
                    }

                    // get variable IDs
                    using (DbCommand cmd02 = conn.CreateCommand())
                    {
                        cmd02.CommandText = sqlVariable;
                        tblVariable = _db.LoadTable("t2", cmd02);
                    }

                    // get source IDs
                    using (DbCommand cmd03 = conn.CreateCommand())
                    {
                        cmd03.CommandText = sqlSource;
                        tblSource = _db.LoadTable("t3", cmd03);
                    }

                    // get method IDs
                    using (DbCommand cmd04 = conn.CreateCommand())
                    {
                        cmd04.CommandText = sqlMethod;
                        tblMethod = _db.LoadTable("t4", cmd04);
                    }

                    // get qualityControl IDs
                    using (DbCommand cmd05 = conn.CreateCommand())
                    {
                        cmd05.CommandText = sqlQuality;
                        tblQuality = _db.LoadTable("t5", cmd05);
                    }

                    //delete the site
                    if (tblSite.Rows.Count == 1)
                    {
                        using (DbCommand cmdDeleteSite = conn.CreateCommand())
                        {
                            cmdDeleteSite.CommandText = sqlDeleteSite;
                            cmdDeleteSite.ExecuteNonQuery();
                        }
                    }

                    //delete the variable
                    if (tblVariable.Rows.Count == 1)
                    {
                        using (DbCommand cmdDeleteVariable = conn.CreateCommand())
                        {
                            cmdDeleteVariable.CommandText = sqlDeleteVariable;
                            cmdDeleteVariable.ExecuteNonQuery();
                        }
                    }

                    //delete the method
                    if (tblMethod.Rows.Count == 1)
                    {
                        using (DbCommand cmdDeleteMethod = conn.CreateCommand())
                        {
                            cmdDeleteMethod.CommandText = sqlDeleteMethod;
                            cmdDeleteMethod.ExecuteNonQuery();
                        }
                    }

                    //delete the source
                    if (tblSource.Rows.Count == 1)
                    {
                        using (DbCommand cmdDeleteSource = conn.CreateCommand())
                        {
                            cmdDeleteSource.CommandText = sqlDeleteSource;
                            cmdDeleteSource.ExecuteNonQuery();
                        }
                    }

                    //delete the quality control level
                    if (tblQuality.Rows.Count == 1)
                    {
                        using (DbCommand cmdDeleteQuality = conn.CreateCommand())
                        {
                            cmdDeleteQuality.CommandText = sqlDeleteQuality;
                            cmdDeleteQuality.ExecuteNonQuery();
                        }
                    }  

                    //finally delete the series
                    using (DbCommand cmdDeleteSeries = conn.CreateCommand())
                    {
                        cmdDeleteSeries.CommandText = sqlDeleteSeries;
                        cmdDeleteSeries.ExecuteNonQuery();
                    }

                    //commit transaction
                    tran.Commit();
                }
            }
            return true;
        }



		/// <summary>
		/// Check if the series with the same site, variable, method,
		/// source, quality control level and data service already
		/// exists in the database.
		/// </summary>
		/// <param name="seriesToCheck">the series to be checked</param>
		/// <returns>The series from the db, or NULL if it doesn't exist</returns>
        /// <remarks>Not implemented</remarks>
		private SeriesMetadata CheckIfSeriesExists ( SeriesMetadata seriesToCheck )
		{
            throw new NotImplementedException();
		}

		/// <summary>
		/// Saves the series metadata to the metadata cache database.
		/// This method also automatically saves the site, variable,
		/// method, source and quality control level of the series.
		/// </summary>
		/// <param name="series">The series to be saved</param>
		/// <param name="dataService">The web service containing the series</param>
		public void SaveSeries ( SeriesMetadata series, DataServiceInfo dataService )
		{
            string sqlSite = "SELECT SiteID FROM SitesCache WHERE SiteCode = ?";
            string sqlVariable = "SELECT VariableID FROM VariablesCache WHERE VariableCode = ?";            
            string sqlMethod = "SELECT MethodID FROM MethodsCache WHERE MethodDescription = ?";
            string sqlSource = "SELECT SourceID FROM SourcesCache WHERE Organization = ?";          
            string sqlQuality = "SELECT QualityControlLevelID FROM QualityControlLevelsCache WHERE Definition = ?";           
            string sqlRowID = "; SELECT LAST_INSERT_ROWID();";
            
            string sqlSaveSite = "INSERT INTO SitesCache(SiteCode, SiteName, Latitude, Longitude, LatLongDatumSRSID, LatLongDatumName, " +
                "Elevation_m, VerticalDatum, LocalX, LocalY, LocalProjectionSRSID, LocalProjectionName, " +
                "PosAccuracy_m, State, County, Comments, ServiceID) " +
                "VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)" + sqlRowID;

            string sqlSaveVariable = "INSERT INTO variablesCache(VariableCode, VariableName, Speciation, " +
                "SampleMedium, ValueType, DataType, GeneralCategory, NoDataValue, VariableUnitsName, VariableUnitsType, VariableUnitsAbbreviation, " +
                "IsRegular, TimeSupport, TimeUnitsName, TimeUnitsType, TimeUnitsAbbreviation, ServiceID) " +
                "VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)" + sqlRowID;

            string sqlSaveMethod = "INSERT INTO MethodsCache(OriginMethodID, MethodDescription, MethodLink, ServiceID) VALUES(?, ?, ?, ?)" + sqlRowID;

            string sqlSaveQualityControl = "INSERT INTO QualityControlLevelsCache(OriginQualityControlLevelID, QualityControlLevelCode, Definition, Explanation) " +
                "VALUES(?,?,?,?)" + sqlRowID;

            string sqlSaveSource = "INSERT INTO SourcesCache(OriginSourceID, Organization, SourceDescription, SourceLink, ContactName, Phone, " +
                                   "Email, Address, City, State, ZipCode, Citation, MetadataID) " +
                                   "VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)" + sqlRowID;

            string sqlSaveISOMetadata = "INSERT INTO ISOMetadataCache(TopicCategory, Title, Abstract, ProfileVersion, MetadataLink) " +
                                    "VALUES(?, ?, ?, ?, ?)" + sqlRowID;

            string sqlSaveSeries = "INSERT INTO DataSeriesCache(SiteID, VariableID, MethodID, SourceID, QualityControlLevelID, " +
                "BeginDateTime, EndDateTime, BeginDateTimeUTC, EndDateTimeUTC, ValueCount, ServiceID) " +
                "VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)" + sqlRowID;

            int siteID = 0;
            int variableID = 0;
            
            int methodID = 0;
            int qualityControlLevelID = 0;
            int sourceID = 0;
            int isoMetadataID = 0;
            int seriesID = 0;
            

            object siteIDResult = null;
           
            object variableIDResult = null;
            
            object methodIDResult = null;
            object qualityControlLevelIDResult = null;
            object sourceIDResult = null;
            
            object seriesIDResult = null;
            
            //check the ServiceID (must be already set)
            if (dataService.Id <= 0)
            {
                throw new ArgumentException("The DataServiceID must be set.");
            }


            //Step 1 Begin Transaction
            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();

                using (DbTransaction tran = conn.BeginTransaction())
                {
                    //****************************************************************
                    //*** Step 2 Site
                    //****************************************************************
                    using (DbCommand cmd01 = conn.CreateCommand())
                    {
                        cmd01.CommandText = sqlSite;
                        cmd01.Parameters.Add(_db.CreateParameter(DbType.String, series.Site.Code));
                        siteIDResult = cmd01.ExecuteScalar();
                        if (siteIDResult != null)
                        {
                            siteID = Convert.ToInt32(siteIDResult);
                        }
                    }

                    if (siteID == 0) //New Site needs to be created
                    {
                        //Insert the site to the database
                        using (DbCommand cmd04 = conn.CreateCommand())
                        {
                            Site site = series.Site;

                            cmd04.CommandText = sqlSaveSite;
                            cmd04.Parameters.Add(_db.CreateParameter(DbType.String, site.Code));
                            cmd04.Parameters.Add(_db.CreateParameter(DbType.String, site.Name));
                            cmd04.Parameters.Add(_db.CreateParameter(DbType.Double, site.Latitude));
                            cmd04.Parameters.Add(_db.CreateParameter(DbType.Double, site.Longitude));
                            cmd04.Parameters.Add(_db.CreateParameter(DbType.Int32, site.SpatialReference.SRSID));
                            cmd04.Parameters.Add(_db.CreateParameter(DbType.String, site.SpatialReference.SRSName));
                            cmd04.Parameters.Add(_db.CreateParameter(DbType.Double, site.Elevation_m));
                            cmd04.Parameters.Add(_db.CreateParameter(DbType.String, site.VerticalDatum));
                            cmd04.Parameters.Add(_db.CreateParameter(DbType.Double, site.LocalX));
                            cmd04.Parameters.Add(_db.CreateParameter(DbType.Double, site.LocalY));
                            if (site.LocalProjection != null)
                            {
                                cmd04.Parameters.Add(_db.CreateParameter(DbType.Int32, site.LocalProjection.SRSID));
                                cmd04.Parameters.Add(_db.CreateParameter(DbType.String, site.LocalProjection.SRSName));
                            }
                            else
                            {
                                cmd04.Parameters.Add(_db.CreateParameter(DbType.Int32, 0));
                                cmd04.Parameters.Add(_db.CreateParameter(DbType.String, "unknown"));
                            }
                            cmd04.Parameters.Add(_db.CreateParameter(DbType.Double, site.PosAccuracy_m));
                            cmd04.Parameters.Add(_db.CreateParameter(DbType.String, site.State));
                            cmd04.Parameters.Add(_db.CreateParameter(DbType.String, site.County));
                            cmd04.Parameters.Add(_db.CreateParameter(DbType.String, site.Comments));
                            cmd04.Parameters.Add(_db.CreateParameter(DbType.Int32, dataService.Id));

                            siteIDResult = cmd04.ExecuteScalar();
                            siteID = Convert.ToInt32(siteIDResult);
                        }
                    }

                    //****************************************************************
                    //*** Step 3 Variable
                    //****************************************************************
                    Variable variable = series.Variable;

                    using (DbCommand cmd05 = conn.CreateCommand())
                    {
                        cmd05.CommandText = sqlVariable;
                        cmd05.Parameters.Add(_db.CreateParameter(DbType.String, variable.Code));
                        cmd05.Parameters[0].Value = variable.Code;
                        variableIDResult = cmd05.ExecuteScalar();
                        if (variableIDResult != null)
                        {
                            variableID = Convert.ToInt32(variableIDResult);
                        }
                    }

                    if (variableID == 0) //New variable needs to be created
                    {
                        //Insert the variable to the database
                        using (DbCommand cmd09 = conn.CreateCommand())
                        {
                            cmd09.CommandText = sqlSaveVariable;
                            cmd09.Parameters.Add(_db.CreateParameter(DbType.String, variable.Code));
                            cmd09.Parameters.Add(_db.CreateParameter(DbType.String, variable.Name));
                            cmd09.Parameters.Add(_db.CreateParameter(DbType.String, variable.Speciation));
                            cmd09.Parameters.Add(_db.CreateParameter(DbType.String, variable.SampleMedium));
                            cmd09.Parameters.Add(_db.CreateParameter(DbType.String, variable.ValueType));
                            cmd09.Parameters.Add(_db.CreateParameter(DbType.String, variable.DataType));
                            cmd09.Parameters.Add(_db.CreateParameter(DbType.String, variable.GeneralCategory));
                            cmd09.Parameters.Add(_db.CreateParameter(DbType.Double, variable.NoDataValue));
                            cmd09.Parameters.Add(_db.CreateParameter(DbType.String, variable.VariableUnit.Name));
                            cmd09.Parameters.Add(_db.CreateParameter(DbType.String, variable.VariableUnit.UnitsType));
                            cmd09.Parameters.Add(_db.CreateParameter(DbType.String, variable.VariableUnit.Abbreviation));
                            cmd09.Parameters.Add(_db.CreateParameter(DbType.Boolean, variable.IsRegular));
                            cmd09.Parameters.Add(_db.CreateParameter(DbType.Double, variable.TimeSupport));
                            cmd09.Parameters.Add(_db.CreateParameter(DbType.String, variable.TimeUnit.Name));
                            cmd09.Parameters.Add(_db.CreateParameter(DbType.String, variable.TimeUnit.UnitsType));
                            cmd09.Parameters.Add(_db.CreateParameter(DbType.String, variable.TimeUnit.Abbreviation));

                            cmd09.Parameters.Add(_db.CreateParameter(DbType.Int32, dataService.Id));

                            variableIDResult = cmd09.ExecuteScalar();
                            variableID = Convert.ToInt32(variableIDResult);
                        }
                    }

                    //****************************************************************
                    //*** Step 4 Method
                    //****************************************************************
                    Method method = series.Method;

                    using (DbCommand cmd10 = conn.CreateCommand())
                    {
                        cmd10.CommandText = sqlMethod;
                        cmd10.Parameters.Add(_db.CreateParameter(DbType.String, method.Description));
                        methodIDResult = cmd10.ExecuteScalar();
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
                            cmd11.Parameters.Add(_db.CreateParameter(DbType.Int32, method.Code));
                            cmd11.Parameters.Add(_db.CreateParameter(DbType.String, method.Description));
                            cmd11.Parameters.Add(_db.CreateParameter(DbType.String, method.Link));
                            cmd11.Parameters.Add(_db.CreateParameter(DbType.Int32, dataService.Id));
                            methodIDResult = cmd11.ExecuteScalar();
                            methodID = Convert.ToInt32(methodIDResult);
                        }
                    }

                    //****************************************************************
                    //*** Step 5 Quality Control Level
                    //****************************************************************
                    QualityControlLevel qc = series.QualityControlLevel;

                    using (DbCommand cmd12 = conn.CreateCommand())
                    {
                        cmd12.CommandText = sqlQuality;
                        cmd12.Parameters.Add(_db.CreateParameter(DbType.String, qc.Definition));
                        qualityControlLevelIDResult = cmd12.ExecuteScalar();
                        if (qualityControlLevelIDResult != null)
                        {
                            qualityControlLevelID = Convert.ToInt32(qualityControlLevelIDResult);
                        }
                    }

                    if (qualityControlLevelID == 0)
                    {
                        //to set the code
                        int qcCode = 0;
                        int.TryParse(qc.Code, out qcCode);
                        
                        using (DbCommand cmd13 = conn.CreateCommand())
                        {
                            cmd13.CommandText = sqlSaveQualityControl;
                            cmd13.Parameters.Add(_db.CreateParameter(DbType.Int32, qcCode));
                            cmd13.Parameters.Add(_db.CreateParameter(DbType.String, qc.Code));
                            cmd13.Parameters.Add(_db.CreateParameter(DbType.String, qc.Definition));
                            cmd13.Parameters.Add(_db.CreateParameter(DbType.String, qc.Explanation));
                            cmd13.Parameters.Add(_db.CreateParameter(DbType.Int32, dataService.Id));
                            qualityControlLevelIDResult = cmd13.ExecuteScalar();
                            qualityControlLevelID = Convert.ToInt32(qualityControlLevelIDResult);
                        }
                    }

                    //****************************************************************
                    //*** Step 6 Source
                    //****************************************************************
                    Source source = series.Source;

                    using (DbCommand cmd14 = conn.CreateCommand())
                    {
                        cmd14.CommandText = sqlSource;
                        cmd14.Parameters.Add(_db.CreateParameter(DbType.String, source.Organization));
                        sourceIDResult = cmd14.ExecuteScalar();
                        if (sourceIDResult != null)
                        {
                            sourceID = Convert.ToInt32(sourceIDResult);
                        }
                    }

                    if (sourceID == 0)
                    {
                        using (DbCommand cmd17 = conn.CreateCommand())
                        {
                            cmd17.CommandText = sqlSaveSource;
                            cmd17.Parameters.Add(_db.CreateParameter(DbType.Int32, source.OriginId));
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
                            cmd17.Parameters.Add(_db.CreateParameter(DbType.Int32, dataService.Id));
                            sourceIDResult = cmd17.ExecuteScalar();
                            sourceID = Convert.ToInt32(sourceIDResult);
                        }
                    }

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
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.BeginDateTime));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.EndDateTime));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.BeginDateTimeUTC));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.DateTime, series.EndDateTimeUTC));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, series.ValueCount));
                        cmd18.Parameters.Add(_db.CreateParameter(DbType.Int32, dataService.Id));
                        
                        seriesIDResult = cmd18.ExecuteScalar();
                        seriesID = Convert.ToInt32(seriesIDResult);
                    }

                    //Commit Transaction
                    tran.Commit();
                }
                conn.Close();
            }
		}

        /// <summary>
        /// updates the data row corresponding to the serviceInfo object
        /// The following parameters are updated:
        /// IsHarvested
        /// HarveDateTime
        /// ServiceName
        /// ServiceVersion
        /// ServiceType
        /// ServiceProtocol
        /// EastLongitude
        /// WestLongitude
        /// EastLatitude
        /// WestLatitude
        /// </summary>
        /// <param name="serviceInfo">the corresponding ServiceInfo</param>
        public void UpdateDataRow(DataServiceInfo serviceInfo)
        {
            string sql = "UPDATE DataServices SET " +
                "IsHarvested=?,HarveDateTime=?,ServiceName=?,ServiceVersion=?,ServiceType=?,ServiceProtocol=?," +
                "EastLongitude=?,WestLongitude=?,NorthLatitude=?,SouthLatitude=? WHERE ServiceID = ?";

            using (DbConnection conn = _db.CreateConnection())
            {
                conn.Open();

                using (DbTransaction tran = conn.BeginTransaction())
                {
                    using (DbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.Parameters.Add(_db.CreateParameter(DbType.Boolean, serviceInfo.IsHarvested));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.DateTime, serviceInfo.HarveDateTime));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.String, serviceInfo.ServiceName));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.String, serviceInfo.Version));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.String, serviceInfo.ServiceType));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.String, serviceInfo.Protocol));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.Double, serviceInfo.EastLongitude));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.Double, serviceInfo.WestLongitude));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.Double, serviceInfo.NorthLatitude));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.Double, serviceInfo.SouthLatitude));
                        cmd.Parameters.Add(_db.CreateParameter(DbType.Int32, serviceInfo.Id));

                        cmd.ExecuteNonQuery();
                    }
                                        
                    tran.Commit();
                }
            }
        }
	}
}

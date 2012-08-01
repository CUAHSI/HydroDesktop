using System;
using System.Collections.Generic;
using System.Data;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.DAL;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Repository for <see cref="Site"/>
    /// </summary>
    class SitesRepository : BaseRepository<Site>, ISitesRepository
    {
        #region Fields

        private readonly ISpatialReferenceRepository _spatialReferenceRepository;

        #endregion

        #region Constructors

        public SitesRepository(DatabaseTypes dbType, string connectionString) : base(dbType, connectionString)
        {
            _spatialReferenceRepository = new SpatialReferenceRepository(dbType, connectionString);
        }

        public SitesRepository(IHydroDbOperations db) : base(db)
        {
            _spatialReferenceRepository = new SpatialReferenceRepository(db);
        }

        #endregion

        protected override string TableName
        {
            get { return "Sites"; }
        }

        protected override Site DataRowToEntity(DataRow row)
        {
            var site = new Site
                           {
                               Id = Convert.ToInt64(row["SiteID"]),
                               Code = Convert.ToString(row["SiteCode"]),
                               Name = Convert.ToString(row["SiteName"]),
                               Latitude = Convert.ToDouble(row["Latitude"]),
                               Longitude = Convert.ToDouble(row["Longitude"]),
                               Elevation_m = Convert.ToDouble(row["Elevation_m"]),
                               Comments = Convert.ToString(row["Comments"]),
                               County = Convert.ToString(row["County"]),
                               State = Convert.ToString(row["State"]),
                               PosAccuracy_m = Convert.ToDouble(row["PosAccuracy_m"]),
                               LocalX = Convert.ToDouble(row["LocalX"]),
                               LocalY = Convert.ToDouble(row["LocalY"]),
                               VerticalDatum = Convert.ToString(row["VerticalDatum"]),
                               LocalProjection = _spatialReferenceRepository.GetByKey(row["LocalProjectionID"]),
                               SpatialReference = _spatialReferenceRepository.GetByKey(row["LatLongDatumID"]),
                               Country = Convert.ToString(row.GetDataOrNull("Country")),
                               SiteType = Convert.ToString(row.GetDataOrNull("SiteType")),
                           };
            return site;
        }

        protected override string PrimaryKeyName
        {
            get { return "SiteID"; }
        }

        public bool Exists(Site site)
        {
            if (site == null) return false;

            const string query = "select count(*) from {0} where SiteID = {1} and SiteCode = '{2}'";
            var result  = DbOperations.ExecuteSingleOutput(string.Format(query, TableName, site.Id, site.Code));
            return Convert.ToInt32(result) > 0;
        }

        public IList<Site> GetSitesWithBothVariables(Variable variable1, Variable variable2)
        {
            if (variable1.Id <= 0) throw new ArgumentException("variable1 must have a valid ID");
            if (variable2.Id <= 0) throw new ArgumentException("variable2 must have a valid ID");

            string sqlQuery = String.Format("select s1.SeriesID as 'SeriesID1', s2.SeriesID as 'SeriesID2', " +
                "site.SiteID, site.SiteName, site.SiteCode, site.Latitude, site.Longitude " +
                "FROM DataSeries s1 INNER JOIN DataSeries s2 ON s1.SiteID = s2.SiteID " +
                "INNER JOIN Sites site ON s1.SiteID = site.SiteID " +
                "WHERE s1.VariableID = {0} AND s2.VariableID = {1}", variable1.Id, variable2.Id);

            DataTable tbl = DbOperations.LoadTable(sqlQuery);
            List<Site> siteList = new List<Site>();

            foreach (DataRow r in tbl.Rows)
            {
                Site s = new Site();
                s.Id = (long)r["SiteID"];
                s.Code = (string)r["SiteCode"];
                s.Latitude = (double)r["Latitude"];
                s.Longitude = (double)r["Longitude"];
                s.Name = (string)r["SiteName"];

                Series s1 = new Series(s, variable1, Method.Unknown, QualityControlLevel.Unknown, Source.Unknown);
                s1.Id = (long)r["SeriesID1"];
                s.AddDataSeries(s1);

                Series s2 = new Series(s, variable2, Method.Unknown, QualityControlLevel.Unknown, Source.Unknown);
                s2.Id = (long)r["SeriesID2"];
                s.AddDataSeries(s2);

                siteList.Add(s);
            }
            return siteList;
        }
    }
}
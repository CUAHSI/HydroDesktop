using System;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Repository for <see cref="Site"/>
    /// </summary>
    class SitesRepository : BaseRepository, ISitesRepository
    {
        #region Constructors

        public SitesRepository(DatabaseTypes dbType, string connectionString) : base(dbType, connectionString)
        {
        }

        public SitesRepository(IHydroDbOperations db) : base(db)
        {
        }

        #endregion

        public override string TableName
        {
            get { return "Sites"; }
        }

        public Site[] GetAll()
        {
            var table = DbOperations.LoadTable(TableName, "Select * from Sites");
            var result = new Site[table.Rows.Count];
            for(int i = 0; i<table.Rows.Count; i++)
            {
                var row = table.Rows[i];
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
                                   // todo: load other fields
                               };

                result[i] = site;
            }

            return result;
        }

        public bool Exists(Site site)
        {
            if (site == null) return false;

            const string query = "select count(*) from {0} where SiteID = {1} and SiteCode = '{2}'";
            var result  = DbOperations.ExecuteSingleOutput(string.Format(query, TableName, site.Id, site.Code));
            return Convert.ToInt32(result) > 0;
        }
    }
}
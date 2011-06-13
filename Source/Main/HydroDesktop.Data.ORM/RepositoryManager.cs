using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using HydroDesktop.ObjectModel;
using HydroDesktop.Database.Map;
using HydroDesktop.Database.DataManagers;
using HydroDesktop.Database.DataManagers.DataRepository;
using NHibernate.Cache;

namespace HydroDesktop.Database
{
    public class RepositoryManager
    {
        #region Variables

        // The helper class wraps the NHibernate Session Factory object. This should
        // only be instantiated once for each database when the
        // application starts.
        private static NHibernateHelper _hibernate;

        private static ISessionFactory _factory;

        public static readonly RepositoryManager Instance = new RepositoryManager();
        
        #endregion

        #region Constructor
         public RepositoryManager()
         {
             if (_factory == null)
             {
                _factory= FluentlyConfigure( DatabaseTypes.SQLite,Config.DefaultActualDataConnection());

                _hibernate = new NHibernateHelper(_factory);
             }
             InitializeDAO();
         }
        /// <summary>
        /// Creates a new instance of the manager given a connection string
        /// </summary>
        /// <param name="dbType">The type of the database (SQLite, SQLServer, ...)</param>
        /// <param name="connectionString">The connection string</param>
        public RepositoryManager(DatabaseTypes dbType, string connectionString)
        {
            _factory = FluentlyConfigure(dbType, connectionString);
            _hibernate = new NHibernateHelper(_factory);
            InitializeDAO();
        }

        private ISessionFactory FluentlyConfigure(DatabaseTypes dbType, string connectionString)
        {
            var cfg = Fluently.Configure();
            
            switch(dbType)
            {
                case DatabaseTypes.SQLite:
                    cfg.Database
                        //    (SQLiteConfiguration.Standard.ConnectionString(connectionString).ShowSql()); // showSql does not appear to effect performance
                        //  (SQLiteConfiguration.Standard.ConnectionString(connectionString)); // showSql does not appear to effect performance
                        (SQLiteConfiguration.Standard.ConnectionString(connectionString)
                             .Cache(c => c.UseQueryCache().ProviderClass<NHibernate.Caches.SysCache2.SysCacheProvider>()).ShowSql());
                    break;
                case DatabaseTypes.SQLServer:
                    cfg.Database
                        (MsSqlConfiguration.MsSql2005.ConnectionString(connectionString).ShowSql());
                    break;
            }
           
            cfg.Mappings(m => m.FluentMappings.Add<DataValueMap>());
            cfg.Mappings(m => m.FluentMappings.Add<DataFileMap>());
            cfg.Mappings(m => m.FluentMappings.Add<DataServiceMap>());
            cfg.Mappings(m => m.FluentMappings.Add<ISOMetadataMap>());
            cfg.Mappings(m => m.FluentMappings.Add<LabMethodMap>());
            cfg.Mappings(m => m.FluentMappings.Add<MethodMap>());
            cfg.Mappings(m => m.FluentMappings.Add<OffsetTypeMap>());
            cfg.Mappings(m => m.FluentMappings.Add<QualifierMap>());
            cfg.Mappings(m => m.FluentMappings.Add<QualityControlLevelMap>());
            cfg.Mappings(m => m.FluentMappings.Add<QueryInfoMap>());
            cfg.Mappings(m => m.FluentMappings.Add<SampleMap>());
            cfg.Mappings(m => m.FluentMappings.Add<SeriesMap>());
            cfg.Mappings(m => m.FluentMappings.Add<SiteMap>());
            cfg.Mappings(m => m.FluentMappings.Add<SourceMap>());
            cfg.Mappings(m => m.FluentMappings.Add<SpatialReferenceMap>());
            cfg.Mappings(m => m.FluentMappings.Add<ThemeMap>());
            cfg.Mappings(m => m.FluentMappings.Add<UnitMap>());
            cfg.Mappings(m => m.FluentMappings.Add<VariableMap>());
            
            ISessionFactory factory = cfg.BuildSessionFactory();
           
           return factory;

            
        }

        private void InitializeDAO()
        {
            this.DataServiceDAO = new DataServiceDAO(_hibernate);
            this.ISOMetadataDAO = new ISOMetadataDAO(_hibernate);
            this.MethodDAO = new MethodDAO(_hibernate);
            this.QualifierDAO = new QualifierDAO(_hibernate);
            this.QualityControlLevelDAO = new QualityControlLevelDAO(_hibernate);
            this.SeriesDAO = new SeriesDAO(_hibernate);
            this.SiteDAO = new SiteDAO(_hibernate);
            this.SourceDAO = new SourceDAO(_hibernate);
            this.SpatialReferenceDAO = new SpatialReferenceDAO(_hibernate);
            this.ThemeDAO = new ThemeDAO(_hibernate);
            this.UnitDAO = new UnitDAO(_hibernate);
            this.VariableDAO = new VariableDAO(_hibernate);
        }

        #endregion

        #region Properties

        public DataServiceDAO DataServiceDAO { get; protected set; }

        public ISOMetadataDAO ISOMetadataDAO { get; protected set; }

        public MethodDAO MethodDAO { get; protected set; }

        public QualifierDAO QualifierDAO { get; protected set; }

        public QualityControlLevelDAO QualityControlLevelDAO { get; protected set; }

        public SeriesDAO SeriesDAO { get; protected set; }

        public SiteDAO SiteDAO { get; protected set; }

        public SourceDAO SourceDAO { get; protected set; }

        public SpatialReferenceDAO SpatialReferenceDAO { get; protected set; }

        public ThemeDAO ThemeDAO { get; protected set; }

        public UnitDAO UnitDAO { get; protected set; }

        public VariableDAO VariableDAO { get; protected set; }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Begin a transaction
        /// </summary>
        public void BeginTransaction()
        {
            _hibernate.BeginTransaction();
        }

        /// <summary>
        /// Commit a transaction
        /// </summary>
        public void Commit()
        {
            _hibernate.CommitTransaction();
        }

        /// <summary>
        /// Roll back a transaction
        /// </summary>
        public void Rollback()
        {
            _hibernate.Rollback();
        }

        /// <summary>
        /// Adds an existing series to an existing theme
        /// </summary>
        /// <param name="series"></param>
        /// <param name="theme"></param>
        public void AddSeriesToTheme(Series series, Theme theme)
        {
            using (ISession session = _factory.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    if (series.Id > 0 && theme.Id > 0)
                    {
                        Theme foundTheme = session.Get<Theme>(theme.Id);
                        Series foundSeries = session.Get<Series>(series.Id);
                        foundTheme.AddSeries(foundSeries);
                        session.Save(foundSeries);
                    }
                    tx.Commit();
                }
            }
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
            return SaveSeriesFast(seriesToSave, theme, OverwriteOptions.Copy);
        }

        /// <summary>
        /// Saves a data series to the database. The series will be associated with the 
        /// specified theme. Depending on the OverwriteOptions, this will check if the series
        /// already exists in the database and overwrite data values in the database if required.
        /// This method internally uses the IStatelessSession to improve saving speed.
        /// </summary>
        /// <param name="seriesToSave">The data series to be saved. This should contain
        /// information about site, variable, method, source and quality control level.</param>
        /// <param name="theme">The theme where this series should belong to</param>
        /// <param name="overwrite">The overwrite options. Set this to 'Copy' if 
        /// a new series should be created in the database. For options other than 'Copy',
        /// some of the existing data values in the database may be overwritten.</param>
        /// <returns>The number of data values saved</returns>
        private int SaveSeriesFast(Series series, Theme theme, OverwriteOptions overwrite)
        {
            Site newSite = null;
            Variable newVariable = null;
            Theme newTheme = null;
            Source newSource = null;
            QualityControlLevel newQualityControlLevel = null;
            Method newMethod = null;
            Series newSeries = null;
            int numSavedValues = 0;
            
            using (ITransaction tr2 = _hibernate.BeginTransaction())
            {
                ISession sess = _hibernate.GetCurrentSession();

                //(1) Check and save spatial references
                SpatialReference newSpatialRef = SpatialReferenceDAO.ExistsOrPersist(series.Site.SpatialReference);

                if (series.Site.LocalProjection != null)
                {
                    SpatialReference newLocalProjection = SpatialReferenceDAO.ExistsOrPersist(series.Site.LocalProjection);
                    series.Site.LocalProjection = newLocalProjection;
                }

                //(2) Check and load or save the site and its spatial references
                series.Site.SpatialReference = newSpatialRef;
                newSite = SiteDAO.ExistsOrPersist(series.Site);

                //(3) Check and load or save time units and variable units
                Unit newTimeUnit = UnitDAO.ExistsOrPersist(series.Variable.TimeUnit);
                Unit newVariableUnit = UnitDAO.ExistsOrPersist(series.Variable.VariableUnit);

                //(4) Check and load or save the variable
                series.Variable.TimeUnit = newTimeUnit;
                series.Variable.VariableUnit = newVariableUnit;
                newVariable = VariableDAO.ExistsOrPersist(series.Variable);

                //(5) Check and load or save the method
                newMethod = MethodDAO.ExistsOrPersist(series.Method);

                //(6) Check and load or save the quality control level
                newQualityControlLevel =
                    QualityControlLevelDAO.ExistsOrPersist(series.QualityControlLevel);

                //(7) Check and load or save the ISOMetadata
                ISOMetadata newISOMetadata =
                    ISOMetadataDAO.ExistsOrPersist(series.Source.ISOMetadata);

                //(8) Check and load or save the DataService
                //DataServiceInfo newDataService =
                //    DataServiceDAO.ExistsOrPersist(series.Source.DataService);

                //(9) Check and load or save the Source
                series.Source.ISOMetadata = newISOMetadata;
                //series.Source.DataService = newDataService;
                newSource = SourceDAO.ExistsOrPersist(series.Source);           
                
                //(10) Create a new series to be saved
                newSeries = new Series(newSite, newVariable, newMethod, newQualityControlLevel, newSource);
                newSeries.CreationDateTime = DateTime.Now;
                newSeries.IsCategorical = series.IsCategorical;
                newSeries.LastCheckedDateTime = DateTime.Now;
                newSeries.Subscribed = false;
                newSeries.UpdateDateTime = DateTime.Now;
                newSeries.BeginDateTime = series.BeginDateTime;
                newSeries.BeginDateTimeUTC = series.BeginDateTimeUTC;
                newSeries.EndDateTimeUTC = series.EndDateTimeUTC;
                newSeries.EndDateTime = series.EndDateTime;
                sess.Save(newSeries);

                //(11) Check and load or save theme
                newTheme = null;
                if (theme != null){
                newTheme = ThemeDAO.ExistsOrPersist(theme);
}
                //(12) Persist all data values in the series
                foreach (DataValue val in series.DataValueList)
                {
                    DataValue newVal = newSeries.AddDataValue(val.LocalDateTime, val.Value, val.UTCOffset);
                    sess.Save(newVal);
                    numSavedValues++;
                }

                //(13) Finally, commit the transaction
                tr2.Commit();
            }
 
            //also add the series to the theme
            if (newSeries != null && newTheme != null)
            {
                AddSeriesToTheme(newSeries, newTheme);
            }
            return numSavedValues;
        }

        /// <summary>
        /// New method for saving a series to the database
        /// </summary>
        /// <param name="series"></param>
        /// <param name="theme"></param>
        /// <returns></returns>
        private void SaveSeriesAsCopy(Series series, Theme theme)
        {
            //the whole saving process is done from within a transaction
            using (NHibernate.ITransaction tx = _hibernate.BeginTransaction())
            {
                //(0) Check and load or save theme
                Theme newTheme = null;
                if (theme != null )
                {
                    newTheme = ThemeDAO.ExistsOrPersist(theme);
                }
                
                //(1) Check and save spatial references
                SpatialReference newSpatialRef = SpatialReferenceDAO.ExistsOrPersist(series.Site.SpatialReference);

                if (series.Site.LocalProjection != null)
                {
                    SpatialReference newLocalProjection = SpatialReferenceDAO.ExistsOrPersist(series.Site.LocalProjection);
                    series.Site.LocalProjection = newLocalProjection;
                }

                //(2) Check and load or save the site and its spatial references
                series.Site.SpatialReference = newSpatialRef;              
                Site newSite = SiteDAO.ExistsOrPersist(series.Site);
                
                //(3) Check and load or save time units and variable units
                Unit newTimeUnit = UnitDAO.ExistsOrPersist(series.Variable.TimeUnit);
                Unit newVariableUnit = UnitDAO.ExistsOrPersist(series.Variable.VariableUnit);

                //(4) Check and load or save the variable
                series.Variable.TimeUnit = newTimeUnit;
                series.Variable.VariableUnit = newVariableUnit;
                Variable newVariable = VariableDAO.ExistsOrPersist(series.Variable);

                //(5) Check and load or save the method
                Method newMethod = MethodDAO.ExistsOrPersist(series.Method);

                //(6) Check and load or save the quality control level
                QualityControlLevel newQualityControlLevel = 
                    QualityControlLevelDAO.ExistsOrPersist(series.QualityControlLevel);

                //(7) Check and load or save the ISOMetadata
                ISOMetadata newISOMetadata =
                    ISOMetadataDAO.ExistsOrPersist(series.Source.ISOMetadata);

                //(8) Check and load or save the DataService
                //DataServiceInfo newDataService =
                //    DataServiceDAO.ExistsOrPersist(series.Source.DataService);

                //(9) Check and load or save the Source
                series.Source.ISOMetadata = newISOMetadata;
                //series.Source.DataService = newDataService;
                Source newSource = SourceDAO.ExistsOrPersist(series.Source);

                //(10) Create a new series to be saved
                Series newSeries = new Series(newSite, newVariable, newMethod, newQualityControlLevel, newSource);
                newSeries.CreationDateTime = DateTime.Now;
                newSeries.IsCategorical = series.IsCategorical;
                newSeries.LastCheckedDateTime = DateTime.Now;
                newSeries.Subscribed = false;
                newSeries.UpdateDateTime = DateTime.Now;
                newSeries.BeginDateTime = series.BeginDateTime;
                newSeries.BeginDateTimeUTC = series.BeginDateTimeUTC;
                newSeries.EndDateTimeUTC = series.EndDateTimeUTC;
                newSeries.EndDateTime = series.EndDateTime;

                foreach (DataValue val in series.DataValueList)
                {
                    newSeries.AddDataValue(val.LocalDateTime, val.Value, val.UTCOffset);
                }

                //(11) Assign the theme
                if (newTheme != null )
                {
                     newTheme.AddSeries(newSeries);
                }
               

                //(11) Save the series to database
                _hibernate.Save(newSeries);

                //Commit transaction
                _hibernate.CommitTransaction(tx);
            }
        }

        
        /// <summary>
        /// Saves a theme to the database including the association
        /// between any of its series.
        /// </summary>
        /// <param name="themeToSave">The theme to be saved</param>
        public void SaveTheme(Theme themeToSave)
        {
            using (ISession session = _factory.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    Theme newTheme = null;
                    
                    IQuery qry = session.CreateQuery("from Themes where Name = :p1 and Description = :p2");
                    qry.SetString("p1", themeToSave.Name);
                    qry.SetString("p2", themeToSave.Description);
                    IList<Theme> found = qry.List<Theme>();
                    if (found != null)
                    {
                        newTheme = found[0];
                        session.SaveOrUpdate(newTheme);
                    }
                    else
                    {
                        session.Save(themeToSave);
                    }
                    
                    tx.Commit();
                }
            }
        }

        /// <summary>
        /// Gets all themes from the database ordered by the theme name
        /// </summary>
        /// <returns>The list of all themes</returns>
        public IList<Theme> GetAllThemes()
        {
            IList<Theme> themes = null;
            using (ITransaction tx = _hibernate.BeginTransaction())
            {
                themes = ThemeDAO.ListAll("Name");
                tx.Commit();
            }
            return themes;
        }

        #endregion
    }
}

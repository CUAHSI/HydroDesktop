using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using FluentNHibernate;
using FluentNHibernate.Testing;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cache;
using NUnit.Framework;
using HydroDesktop.Database;
using HydroDesktop.ObjectModel;
using HydroDesktop.Database.Map;

namespace HydroDesktop.Database.Tests.MappingTests.DataRepository
{
    public class FixtureBase
    {
        protected SessionSource SessionSource { get; set; }
        protected ISession Session { get; private set; }
       
        //public void SetupContext()
        //{
            
        //    var cfg = Fluently.Configure()
        //        .Database(SQLiteConfiguration.Standard.InMemory);
        //    SessionSource = new SessionSource(cfg.BuildConfiguration()
        //                                         .Properties, new TestModel());
        //    Session = SessionSource.CreateSession();
        //    SessionSource.BuildSchema(Session);
        //}
 
        ////[SetUp]             
        //public void SetupContextMsSql()
        //{

        //    var cfg = Fluently.Configure()
        //        .Database(MsSqlConfiguration.MsSql2008.ConnectionString(
        //                      c => c.Server("localhost")
        //                               .Database("OdmNFluent")
        //                               .TrustedConnection()
        //                      )
        //                     // .DefaultSchema("OdCore")
        //                      ).Mappings(
        //      m =>
        //        m.FluentMappings.AddFromAssemblyOf<SiteMap>()
        //        );
        //    SessionSource = new SessionSource(cfg.BuildConfiguration()
        //                                         .Properties, new TestModel());
        //    Session = SessionSource.CreateSession();
        //    SessionSource.BuildSchema(Session);
        //}

        [SetUp]
        public void SetupContextSQLite()
        {
            var cfg = Fluently.Configure();
            //string dbDirectory = TestConfig.DefaultDatabaseDirectory;
            //string dbFileName = System.IO.Path.Combine(dbDirectory, "DataRepository.sqlite");
            string constring = TestConfig.DefaultActualDataConnection();
            var sqldbConfig = SQLiteConfiguration.Standard.ConnectionString(constring); //SQLiteConfiguration.Standard. UsingFile(dbFileName)
            cfg.Database(sqldbConfig
                             .Cache(c => c.UseQueryCache().ProviderClass<HashtableCacheProvider>().UseMinimalPuts())
                             .ShowSql());

            //NHibernate.Cfg.Configuration cfg2 = cfg.BuildConfiguration();
            //ISessionFactory factory = cfg.BuildSessionFactory();

            SessionSource = new SessionSource(cfg.BuildConfiguration()
                                                  .Properties, new TestModel());
            Session = SessionSource.CreateSession();
        }

        [TearDown]
        public void TearDownContext()
        {
            Session.Close();
            Session.Dispose();
        }
    }
}
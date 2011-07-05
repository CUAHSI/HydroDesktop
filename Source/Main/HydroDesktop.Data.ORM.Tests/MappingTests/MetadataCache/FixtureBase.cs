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

namespace HydroDesktop.Database.Tests.MappingTests.MetadataCache
{
    public class FixtureBase
    {
        protected SessionSource SessionSource { get; set; }
        protected ISession Session { get; private set; }
       
        [SetUp]
        public void SetupContextSQLite()
        {
            var cfg = Fluently.Configure();
            //string dbDirectory = TestConfig.DefaultDatabaseDirectory;
            //string dbFileName = System.IO.Path.Combine(dbDirectory, "MetadataCache.sqlite");
            var config = SQLiteConfiguration.Standard.ConnectionString(TestConfig.DefaultLocalCacheConnection());
            cfg.Database(config  //SQLiteConfiguration.Standard.UsingFile(dbFileName)
                .Cache(c => c.UseQueryCache().ProviderClass<HashtableCacheProvider>().UseMinimalPuts())
                .ShowSql());
            //cfg.Mappings(m => m.FluentMappings.Add(typeof(DataServiceMap)));

            //NHibernate.Cfg.Configuration cfg2 = cfg.BuildConfiguration();
            //ISessionFactory factory = cfg.BuildSessionFactory();

            SessionSource = new SessionSource(cfg.BuildConfiguration()
                                                  .Properties, 
                                              new TestModel2());
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
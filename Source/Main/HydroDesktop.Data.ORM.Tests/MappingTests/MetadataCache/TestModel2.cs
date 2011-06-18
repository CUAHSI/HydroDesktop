using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using HydroDesktop.Database.Map.MetadataCache;

namespace HydroDesktop.Database.Tests.MappingTests.MetadataCache
{
    public class TestModel2 : PersistenceModel
    {
        public TestModel2()
        {
       
            // add mappings
            Add(typeof(HydroDesktop.Database.Map.MetadataCache.DataServiceMap));
            Add(typeof(HydroDesktop.Database.Map.MetadataCache.MethodMap));
            Add(typeof(HydroDesktop.Database.Map.MetadataCache.SiteMap));
            Add(typeof(HydroDesktop.Database.Map.MetadataCache.VariableMap));
            Add(typeof(HydroDesktop.Database.Map.MetadataCache.QualityControlLevelMap));
            Add(typeof(HydroDesktop.Database.Map.MetadataCache.SourceMap));
            Add(typeof(HydroDesktop.Database.Map.MetadataCache.ISOMetadataMap));
            Add(typeof(HydroDesktop.Database.Map.MetadataCache.SeriesMap));
            //AddMappingsFromAssembly(typeof (DataServiceInfo).Assembly);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using HydroDesktop.Database.Map;

namespace HydroDesktop.Database.Tests.MappingTests.DataRepository
{
    public class TestModel : PersistenceModel
    {
        public TestModel()
        {
            // add mappings
            Add(typeof(DataValueMap));
            Add(typeof(DataFileMap));
            Add(typeof(DataServiceMap));
            Add(typeof(ISOMetadataMap));
            Add(typeof(LabMethodMap));
            Add(typeof(MethodMap));
            Add(typeof(OffsetTypeMap));
            Add(typeof(QualifierMap));
            Add(typeof(QualityControlLevelMap));
            Add(typeof(QueryInfoMap));
            Add(typeof(SampleMap));
            Add(typeof(SeriesMap));
            Add(typeof(SiteMap));
            Add(typeof(SourceMap));
            Add(typeof(SpatialReferenceMap));
            Add(typeof(ThemeMap));
            Add(typeof(UnitMap));
            Add(typeof(VariableMap));
        }
    }
}
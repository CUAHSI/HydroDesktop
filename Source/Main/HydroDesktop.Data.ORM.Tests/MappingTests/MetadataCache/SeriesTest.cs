using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Testing;
using NUnit.Framework;
using HydroDesktop.ObjectModel;
using HydroDesktop.Database.Map.MetadataCache;

namespace HydroDesktop.Database.Tests.MappingTests.MetadataCache
{
    [TestFixture]
    public class SeriesTest : FixtureBase
    {
        static Random random = new Random();

        static DateTime sqlMinDateTime = new DateTime(1753,1,1);
        DataServiceInfo service ;
        Source source ;
        Method method;
        Site site;
        Variable variable ;
        QualityControlLevel qc ;

        [SetUp]
        public void Setup()
        {
            int id = random.Next(); 
            
            service = DataServiceTest.CreateDataService();
             source = SourcesTest.CreateSource(service, null);
             method = MethodsTest.CreateMethod(service);
             site = SitesTest.CreateSite(service, id);
            variable = VariableTest.CreateVariable(service, id);
             qc = QualityControlLevelsTest.CreateQualityControlLevel(service);

            Session.Save(service);
            Session.Save(source.ISOMetadata);
            Session.Save(source);
            Session.Save(method);
            Session.Save(site);
            Session.Save(variable.TimeUnit);
            Session.Save(variable.VariableUnit);
            Session.Save(variable);
            Session.Save(qc);

        }
        [Test]
        [Ignore("No perister for Unit")]
        public void CanMapSeries()
        {
            int id = random.Next();

    
            new PersistenceSpecification<SeriesCache>(Session)
                .CheckProperty(p => p.BeginDateTime, sqlMinDateTime)
                .CheckProperty(p => p.EndDateTime, DateTime.Now.Date)
                .CheckProperty(p => p.BeginDateTimeUTC, sqlMinDateTime.AddHours(8))
                .CheckProperty(p => p.EndDateTimeUTC, DateTime.Now.Date.AddHours(8))
                .CheckProperty(p => p.ValueCount, 365)
                .CheckReference(p => p.Site, site)
                .CheckReference(p => p.Variable, variable)
                .CheckReference(p => p.Method, method)
                .CheckReference(p => p.Source, source)
                .CheckReference(p => p.QualityControlLevel, qc)
                .CheckReference(p => p.DataService, service)
                .VerifyTheMappings();
        }

        public static Series CreateSeries(int identifier)
        {
            Series series = new Series();

            series.Site = SitesTest.CreateSite(identifier);
            series.Variable = VariableTest.CreateVariable(identifier);

            series.ValueCount = identifier;
            series.BeginDateTime = sqlMinDateTime;
            series.BeginDateTimeUTC = series.BeginDateTimeUTC.AddHours(8);
            series.EndDateTime = new DateTime(2009, 12, 12, 12, 12, 0);
            series.EndDateTimeUTC = series.EndDateTime.AddHours(8);
            series.IsCategorical = false;

            series.Source = Source.Unknown;
            series.Method = Method.Unknown;
            series.QualityControlLevel = QualityControlLevel.Unknown;

            return series;
        }
    }
}
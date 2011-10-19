using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Testing;
using NUnit.Framework;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Tests.MappingTests.DataRepository
{
    [TestFixture]
    [Category("Mapping.Data")]
    public class SeriesTest : FixtureBase
    {
        static Random random = new Random();

        static DateTime sqlMinDateTime = new DateTime(1753,1,1);
        private Site site ;

        private Variable variable;
        private int id;

        [SetUp]
        public void Setup()
        {
            id = random.Next();
            site = DataSeriesTest.CreateSite(id);
            DataSeriesTest.SaveSite(Session, site);

            variable = DataSeriesTest.CreateVariable(id);
            DataSeriesTest.SaveVariable(Session, variable);

        }

  

        [Test]
        public void CanMapSeries()
        {
           
            IList<Theme> themeList = new List<Theme>();
            themeList.Add(ThemesTest.CreateTheme());
            

            new PersistenceSpecification<Series>(Session)
                .CheckProperty(p => p.BeginDateTime, sqlMinDateTime)
                .CheckProperty(p => p.EndDateTime, DateTime.Now.Date)
                .CheckProperty(p => p.BeginDateTimeUTC, sqlMinDateTime.AddHours(8))
                .CheckProperty(p => p.EndDateTimeUTC, DateTime.Now.Date.AddHours(8))
                .CheckProperty(p => p.IsCategorical, false)
                .CheckProperty(p => p.Subscribed, false)
                .CheckProperty(p => p.CreationDateTime, DateTime.Now.Date)
                .CheckProperty(p => p.UpdateDateTime, DateTime.Now.Date)
                .CheckProperty(p => p.LastCheckedDateTime, DateTime.Now.Date)
                .CheckReference(p => p.Site, site)
                .CheckReference(p => p.Variable, variable)
                .CheckReference(p => p.Method, Method.Unknown)
                .CheckReference(p => p.Source, Source.Unknown)
                .CheckReference(p => p.QualityControlLevel, QualityControlLevel.Unknown)
                .VerifyTheMappings();
        }

        #region helpers 
 

        public static Series CreateSeries(int identifier)
        {
            Series series = new Series();

            series.Site = DataSeriesTest.CreateSite(identifier);
            series.Variable = DataSeriesTest.CreateVariable(identifier);

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

        public static Series CreateSeriesWithValues(int identifier)
        {
            Series randomSeries = CreateSeries(identifier);
            for (int i = 0; i < 1000; i++)
            {
                randomSeries.AddDataValue(DateTime.Now.Date.AddDays(i), 1000.0 * random.NextDouble());
            }
            return randomSeries;
        }
#endregion
        //[Test]
        //public void SeriesRecordValidation()
        //{
        //    int id = random.Next();
        //    DateTimeOffset endDateTime = new DateTimeOffset(2009, 12, 12, 12, 12, 12,
        //       new TimeSpan(12, 0, 0));

        //    SeriesRecord s = new SeriesRecord();

        //    Assert.IsFalse(s.IsValid);

        //    s.Site = CreateSite(id);
        //    s.Variable = CreateVariable(id);

        //    Assert.IsFalse(s.IsValid);

        //    s.Sources.Add( GetSource(id)) ;
        //    s.Network = new DataSourceCode
        //                    {
        //                        PrefixCode = "OdmTest"
        //                    };

        //    Assert.IsTrue(s.IsValid);

        //    s.BeginDateTime = sqlMinDateTime;
        //    s.EndDateTime = endDateTime.UtcDateTime;
        //    s.OffsetFromUtc = endDateTime.Offset;

        //    s.ValueCount = 2;
        //    s.GlobalIdentifier = new System.Guid();

        //    Assert.IsTrue(s.IsValid);
        //}

        //[Test]
        //public void SeriesRecordOdmValidation()
        //{
        //    int id = random.Next();
        //    DateTimeOffset endDateTime = new DateTimeOffset(2009, 12, 12, 12, 12, 12,
        //       new TimeSpan(12, 0, 0));

        //    SeriesOdmData s = new SeriesOdmData();

        //    Assert.IsFalse(s.IsValid);

        //    s.Site = CreateSite(id);
        //    s.Variable = CreateVariable(id);

        //    Assert.IsFalse(s.IsValid);

        //    s.BeginDateTime = DateTime.MinValue;
        //    s.EndDateTime = endDateTime.UtcDateTime;
        //    s.OffsetFromUtc = endDateTime.Offset;

        //     Source source = GetSource(id);
        //     s.Sources.Add(source);

        //    s.Network = new DataSourceCode
        //    {
        //        PrefixCode = "OdmTest"
        //    };

        //    s.ValueCount = 2;
        //    s.GlobalIdentifier = new System.Guid();

        //    Assert.IsFalse(s.IsValid);


        //    SaveTheme method = new SaveTheme { Code = "Meth001", Description = "SaveTheme 001" };

        //    QualityControlLevel qcLevel = new QualityControlLevel
        //   {
        //       Code = "QC001",
        //       Name = "QC Level 1",
        //       Description = "Detailed QC LEvel 1"
        //   };

        //    CensorCodeCv censored = new CensorCodeCv
        //    {
        //        Name = "Test",
        //        NotCensored = true
        //    };

        //     s.Methods.Add(method);
        //    s.QualityControlLevels.Add(qcLevel);

        //    s.Scale = TimeScaleTest.CreateTimeScale(id);

        //    Assert.IsTrue(s.IsValid);
        //    // Need to validate that is it is a valid DataValueCore, also. 
        //    //  to validate that the Validations Rules tree is working

        //    Source source2 = new Source();
        //    source2.Organization = "test" + id;
        //    source2.Description = "source descrition" + id;
        //    s.Sources.Add(source2);
        //    Assert.IsFalse(s.IsValid);
        //}

        //private Source GetSource(int identifier)
        //{
        //    Source source = new Source();
        //    source.Organization = "test" + identifier;
        //    source.Description = "source descrition" + identifier;
        //    return source;
        //}

        //[Test]
        //public void CanMapSeriesRecord()
        //{

        //    int id = random.Next();

        //    //  DateTimeOffset endDateTime = DateTimeOffset.Now; // Equals fails...
        //    DateTimeOffset endDateTime = new DateTimeOffset(2009, 12, 12, 12, 12, 12,
        //        new TimeSpan(12, 0, 0));

        //    new PersistenceSpecification<SeriesRecord>(Session)
        //        .CheckReference(s => s.Site, CreateSite(id))
        //        .CheckReference(s => s.Variable, CreateVariable(id))
        //        .CheckProperty(s => s.BeginDateTime, sqlMinDateTime)
        //       .CheckProperty(s => s.EndDateTime, endDateTime.UtcDateTime)
        //       .CheckProperty(s => s.OffsetFromUtc, endDateTime.Offset)
        //       .CheckProperty(s => s.ValueCount, 2)
        //        .CheckProperty(s => s.GlobalIdentifier, new System.Guid())
        //        .VerifyTheMappings();

        //}

        //[Test]
        //public void CanMapSeriesRecord1()
        //{

        //    int id = random.Next();

        //    //  DateTimeOffset endDateTime = DateTimeOffset.Now; // Equals fails...
        //    DateTimeOffset endDateTime = new DateTimeOffset(2009, 12, 12, 12, 12, 12,
        //        new TimeSpan(12, 0, 0));
        //    Site site = CreateSite(id);
        //    Variable variable = CreateVariable(id);
        //    TimeScale timeScale = CreateTimeScale(id);
        //    Session.Save(site);
        //    Session.Save(variable);
        //    Session.Save(timeScale);

        //    new PersistenceSpecification<SeriesRecord>(Session)
        //        .CheckReference(s => s.Site, site)
        //        .CheckReference(s => s.Variable, variable)
        //        .CheckProperty(s => s.BeginDateTime, sqlMinDateTime)
        //       .CheckProperty(s => s.EndDateTime, endDateTime.UtcDateTime)
        //       .CheckProperty(s => s.OffsetFromUtc, endDateTime.Offset)
        //        .CheckProperty(s => s.ValueCount, 2)
        //        .CheckReference(s => s.Scale, timeScale)
        //        .VerifyTheMappings();

        //}

        //[Test]
        //public void CanCreateManySeriesRecords()
        //{

        //    List<int> identifiers = new List<int>(3);
        //    identifiers.Add(random.Next());
        //    identifiers.Add(random.Next());
        //    identifiers.Add(random.Next());

        //    List<SeriesRecord> seriesRecords = new List<SeriesRecord>(3);

        //    foreach (var i in identifiers)
        //    {
        //        SeriesRecord series = CreateSeriesRecord(i);
        //        seriesRecords.Add(series);


        //        Session.Save(series.Site);
        //        Session.Save(series.Variable);
        //        Session.Save(series.Scale);

        //        Session.Save(series);
        //    }

        //    var fromDb = Session.CreateCriteria(typeof(SeriesRecord)).List();
        //    Assert.AreEqual(3, fromDb.Count, "count should be 3");

        //}

        //[Test]
        //public void CanCreateMixedSeriesRecords()
        //{

        //    List<int> identifiers = new List<int>(3);
        //    identifiers.Add(random.Next());
        //    identifiers.Add(random.Next());
        //    identifiers.Add(random.Next());

        //    Source source = new Source();
        //    source.Organization = "a";
        //    source.Description = " A source named a";
        //    Session.Save(source);

        //    List<SeriesRecord> seriesRecords = new List<SeriesRecord>(3);

        //    foreach (var i in identifiers)
        //    {
        //        SeriesRecord series = CreateSeriesRecord(i);
        //        seriesRecords.Add(series);


        //        Session.Save(series.Site);
        //        Session.Save(series.Variable);
        //        Session.Save(series.Scale);

        //        Session.Save(series);
        //    }

        //    var fromDb = Session.CreateCriteria(typeof(SeriesRecord)).List();
        //    Assert.AreEqual(3, fromDb.Count, "count should be 3");

        //    List<int> identifiers2 = new List<int>(3);
        //    identifiers2.Add(random.Next());
        //    identifiers2.Add(random.Next());
        //    identifiers2.Add(random.Next());

        //    List<SeriesRecord> seriesRecords2 = new List<SeriesRecord>(3);

        //    foreach (var i in identifiers2)
        //    {
        //        SeriesRecord series = CreateSeriesRecord(i);
        //        SeriesOdmData seriesOdmData = new SeriesOdmData(series);
        //        List<DataValueOdm> dataValues = new List<DataValueOdm>();
        //        seriesOdmData.DataValues = dataValues;
        //        for (int dvi = 5; dvi > 0; dvi--)
        //        {
        //            DateTime date = DateTime.Now.AddDays(-dvi);
        //            DateTimeOffset obsDateTime = new DateTimeOffset(date);
        //            dataValues.Add(DataValuesTest.CreateDataValueOdm(
        //                               random.Next(), seriesOdmData, obsDateTime, source
        //                               )
        //                               );

        //        }
        //        seriesRecords2.Add(seriesOdmData);

        //        // these have to be exist
        //        Session.Save(seriesOdmData.Site);
        //        Session.Save(seriesOdmData.Variable);
        //        Session.Save(seriesOdmData.Scale);

        //        // Then we can save the series, and data
        //        Session.Save(seriesOdmData);
        //    }
        //    fromDb = Session.CreateCriteria(typeof(SeriesOdmData)).List();
        //    Assert.AreEqual(3, fromDb.Count, "count should be 3");

        //    fromDb = Session.CreateCriteria(typeof(SeriesRecord)).List();
        //    Assert.AreEqual(6, fromDb.Count, "count should be 6");
        //}



        //public static Site CreateSite(int identifier)
        //{
        //    return SitesTest.CreateSite(identifier);
        //}

        //public static Variable CreateVariable(int identifier)
        //{
        //    Variable variable = VariableTest.CreateVariable(identifier);
        //    return variable;
        //}

        //public static TimeScale CreateTimeScale(int identifier)
        //{
        //    return TimeScaleTest.CreateTimeScale(identifier);
        //}
    }
}
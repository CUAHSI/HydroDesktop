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
    public class DataValuesTest : FixtureBase
    {
        static Random random = new Random();

        [Test]
        public void CanMapDataValueSimple()
        {
            int id = random.Next(1000);
            Series series = SeriesTest.CreateSeries(id);
            
            using (NHibernate.ITransaction tx = Session.BeginTransaction())
            {
                DataFile file = Session.Get<DataFile>(1L);

                Session.SaveOrUpdate(series.Site.SpatialReference);
                Session.SaveOrUpdate(series.Site);
                Session.SaveOrUpdate(series.Variable.TimeUnit);
                Session.SaveOrUpdate(series.Variable.VariableUnit);
                Session.SaveOrUpdate(series.Variable);
                Session.SaveOrUpdate(series.Method);
                Session.SaveOrUpdate(series.QualityControlLevel);
                Session.SaveOrUpdate(series.Source);
                foreach (DataValue val in series.DataValueList)
                {
                    if (val.Sample != null)
                    {
                        Session.SaveOrUpdate(val.Sample.LabMethod);
                    }
                    if (val.Qualifier != null)
                    {
                        Session.SaveOrUpdate(val.Qualifier);
                    }
                    if (val.OffsetType != null)
                    {
                        Session.SaveOrUpdate(val.OffsetType);
                    }
                }
                Session.Save(series);

                tx.Commit();
            }

            Assert.AreNotEqual(series.Id, 0, "the series should be saved and id should not be null.");
        }

        public static DataValue CreateDataValue(int id, Series series, DateTime obsTime)
        {
            DataValue val = series.AddDataValue();
            val.CensorCode = "nc";
            val.LocalDateTime = obsTime;
            val.UTCOffset = -8.0;
            val.DateTimeUTC = obsTime.AddHours(-8);
            val.Value = Convert.ToDouble(id);
            val.ValueAccuracy = 0.0;
            return val;
        }
        
        
        // [Test]
        // public void checkCoreDataValueValidation()
        // {
        //     // Do not need to save to test validation

        //     int id = random.Next();
            
        //     SeriesRecord series = SeriesRecordTest.CreateSeriesRecord(id);
        //     SeriesCoreData seriesCoreData = new SeriesCoreData(series);
        //     DateTimeOffset obsTimeOffset = new DateTimeOffset(2009, 12, 12, 12, 12, 12,
        //                  new TimeSpan(12, 0, 0));
        //     DateTime obsDateTime = obsTimeOffset.UtcDateTime;
        //     TimeSpan offsetUtc = obsTimeOffset.Offset;

        //     DataValueCore coreValue = new DataValueCore();
            
        //     coreValue.Series = seriesCoreData;
        //     coreValue.Value = id;
        //     coreValue.DateTimeUtc = DateTime.Now;
        //     coreValue.OffsetFromUtc = offsetUtc;
        //     Assert.IsTrue(coreValue.IsValid);

        // }

        // [Test]
        // public void checkOdmDataValueValidation()
        // {
        //     // Do not need to save to test validatio
        //     int id = random.Next();

        //     SeriesRecord series = SeriesRecordTest.CreateSeriesRecord(id);
        //     SeriesOdmData seriesCoreData = new SeriesOdmData(series);
        //     DateTimeOffset obsTimeOffset = new DateTimeOffset(2009, 12, 12, 12, 12, 12,
        //                  new TimeSpan(12, 0, 0));
        //     DataValueOdm valueOdm = new DataValueOdm();

        //     valueOdm.Series = seriesCoreData;
        //     valueOdm.Value = id;
        //     valueOdm.DateTimeUtc = DateTime.Now;
        //     valueOdm.OffsetFromUtc = obsTimeOffset.Offset;

        //     Assert.IsFalse(valueOdm.IsValid);

        //     Source source = CreateSource(id);

        //     SaveTheme method = new SaveTheme { Code = "Meth001", Description = "SaveTheme 001" };
        //     QualityControlLevel qcLevel = new QualityControlLevel
        //     {
        //         Code = "QC001",
        //         Name = "QC Level 1",
        //         Description = "Detailed QC LEvel 1"
        //     };
        //     CensorCodeCv censored = new CensorCodeCv
        //     {
        //         Name = "Test",
        //         NotCensored = true
        //     };
            
        //     valueOdm.Source = source;
        //     valueOdm.SaveTheme = method;
        //     valueOdm.QualityControlLevel = qcLevel;
        //     valueOdm.CensorCode = censored;

        //     Assert.IsTrue(valueOdm.IsValid );
        // }
        // [Test]
        //public void CanMapCoreDataValueUsingSeriesCoreData()
        //{
        //    int id = random.Next();

        //    SeriesRecord series =  SeriesRecordTest.CreateSeriesRecord(id);
        //    SeriesCoreData seriesCoreData = new SeriesCoreData(series);

        //      Session.Save(seriesCoreData.Site);
        //      Session.Save(seriesCoreData.Variable);
        //      Session.Save(seriesCoreData.Scale);

        //      Session.Save(seriesCoreData);

        //     Source source = CreateSource(id);
        //     Session.Save(source);

        //    DateTimeOffset obsTimeOffset = new DateTimeOffset(2009, 12, 12, 12, 12, 12,
        //       new TimeSpan(12, 0, 0));

        //    new PersistenceSpecification<DataValueCore>(Session)
        //        .CheckReference(dv => dv.Series, seriesCoreData)
        //        .CheckProperty(dv => dv.DateTimeUtc, obsTimeOffset.UtcDateTime)
        //         .CheckProperty(dv => dv.OffsetFromUtc, obsTimeOffset.Offset)
        //        .CheckProperty(dv => dv.Value, Convert.ToDouble(id))  
        //        .VerifyTheMappings();
        //    /* Note can't compare int to double
        //     * use Convert.ToDouble() 
        //     * */

        //}

        // [Test]
        // public void CanMapDataValueOdmUsingSeriesOdmData()
        // {
        //     int id = random.Next();

        //     SeriesRecord series = SeriesRecordTest.CreateSeriesRecord(id);
        //     SeriesOdmData seriesOdmData = new SeriesOdmData(series);

        //     Session.Save(seriesOdmData.Site);
        //     Session.Save(seriesOdmData.Variable);
        //     Session.Save(seriesOdmData.Scale);

        //     Session.Save(seriesOdmData);

        //     Source source = CreateSource(id);
        //     Session.Save(source);

        //     DateTimeOffset obsTimeOffset = new DateTimeOffset(2009, 12, 12, 12, 12, 12,
        //        new TimeSpan(12, 0, 0));

        //     //new PersistenceSpecification<DataValueOdm>(Session)
        //     //    .CheckReference(dv => dv.Series, seriesOdmData)
        //     //    .CheckProperty(dv => dv.DateTime, obsTimeOffset)
        //     //    .CheckProperty(dv => dv.Value, Convert.ToDouble(id))
        //     //    .CheckReference(dv => dv.Source, source)
        //     //    .VerifyTheMappings();
        //     /* Note can't compare int to double
        //      * use Convert.ToDouble() 
        //      * */
        //     DataValueOdm dv = CreateDataValueOdm(id, seriesOdmData, obsTimeOffset, source);

          
        //     Assert.IsFalse(dv.IsValid,"Data Value should not be valid by ODM Rules.  ODM Specification are not being enforced");

        //     Assert.GreaterOrEqual(dv.GetRuleViolations().Count(),3, "Rules for ODM Specification are not being enforced");
           
        //     SaveTheme method = new SaveTheme{Code = "Meth001", Description = "SaveTheme 001"};
        //     QualityControlLevel qcLevel = new QualityControlLevel
        //                                       {
        //                                           Code = "QC001",
        //                                           Name = "QC Level 1",
        //                                           Description = "Detailed QC LEvel 1"
        //                                       };
        //     CensorCodeCv censored = new CensorCodeCv
        //                                 {
        //                                     Name = "Test",
        //                                     NotCensored = true
        //                                 };
        //     dv.SaveTheme = method;
        //     dv.QualityControlLevel = qcLevel;
        //     dv.CensorCode = censored;

        //     Session.Save(dv);
        //     var fromDb = Session.Get<DataValueOdm>(dv.Id);

        //     Assert.AreEqual(fromDb.DateTimeUtc, obsTimeOffset.UtcDateTime, "DateTimes are not equal");
        //     Assert.AreEqual(fromDb.OffsetFromUtc, obsTimeOffset.Offset, "DateTimes are not equal");
        //     Assert.AreEqual(seriesOdmData, fromDb.Series, "Series Differ");
        //     Assert.AreEqual(fromDb.Value, Convert.ToDouble(id), "Value Differs");
        // }

        //public static DataValueOdm CreateDataValueOdm(int id, SeriesOdmData seriesOdmData, DateTimeOffset obsTimeOffset, Source source)
        //{
        //    return new DataValueOdm
        //               {
        //                   DateTimeUtc = obsTimeOffset.UtcDateTime,
        //                   OffsetFromUtc = obsTimeOffset.Offset,
        //                   Series = seriesOdmData,
        //                   Value = Convert.ToDouble(id),
        //                   Source = source
        //               };
        //}

        //public static DataValueOdm CreateValidDataValueOdm(int id, SeriesOdmData seriesOdmData, DateTimeOffset obsTimeOffset, Source source)
        //{
        //    return new DataValueOdm
        //    {
        //        DateTimeUtc = obsTimeOffset.UtcDateTime,
        //        OffsetFromUtc = obsTimeOffset.Offset,
        //        Series = seriesOdmData,
        //        Value = Convert.ToDouble(id),
        //        Source = source
        //    };
        //}

        
        
        //public static DataValueCore CreateDataValueCoreValue(int id, SeriesCoreData seriesInsturmentData, DateTimeOffset obsTimeOffset)
        //{
        //    return new DataValueCore
        //               {
        //                   DateTimeUtc = obsTimeOffset.UtcDateTime,
        //                   OffsetFromUtc = obsTimeOffset.Offset,
        //                   Series = seriesInsturmentData,
        //                   Value = Convert.ToDouble(id)
        //               };
        //}

        //static Source CreateSource(int identifier)
        //{
        //    Source source = new Source();
        //    source.Organization = "test" + identifier;
        //    source.Description = "source descrition" + identifier;

        //    return source;
        //}
    }
}
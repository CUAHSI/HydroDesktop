using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FluentNHibernate.Testing;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Tests.MappingTests.DataRepository
{
    [TestFixture]
    [Category("Mapping.Data")]
    public class ThemesTest : FixtureBase
    {
        Random random = new Random();

        [Test]
        public void CanMapTheme()
        {
            long identifier = random.Next();
            new PersistenceSpecification<Theme>(Session)
                .CheckProperty(x => x.Description, "theme description" + identifier)
                .CheckProperty(x => x.Name, "theme " + identifier)
                .CheckProperty(x => x.DateCreated, DateTime.Now.Date)
                .VerifyTheMappings();
        }

        [Test]
        public void CanSaveThemeWithSeries()
        {
            int identifier = random.Next();

            Site site = SitesTest.CreateSite(identifier);
            site.SpatialReference = SpatialReferenceTest.CreateSpatialReference();
            
            Theme newTheme = CreateTheme();
            Series series1 = SeriesTest.CreateSeries(identifier);
            series1.Site = site;
            Series series2 = SeriesTest.CreateSeries(identifier + 1);
            series2.Site = site;

            newTheme.AddSeries(series1);
            newTheme.AddSeries(series2);

            Assert.IsTrue(newTheme.SeriesList.Contains( series1));
            Assert.IsTrue(series1.ThemeList.Contains( newTheme));

            //Assert.AreEqual(newTheme.SeriesList[1], series2);
            //Assert.AreEqual(series2.ThemeList[0], newTheme);
            Assert.IsTrue(newTheme.SeriesList.Contains(series2));
            Assert.IsTrue(series2.ThemeList.Contains(newTheme));

            Session.Save(site.SpatialReference);
            Session.Save(site);

            Session.Save(series1.Variable.TimeUnit);
            Session.Save(series1.Variable.VariableUnit);
            Session.Save(series2.Variable.TimeUnit);
            Session.Save(series2.Variable.VariableUnit);

            Session.Save(series1.Variable);
            Session.Save(series2.Variable);
            Session.Save(series1.Method);
            Session.Save(series2.Method);
            Session.Save(series1.QualityControlLevel);
            Session.Save(series2.QualityControlLevel);
            Session.Save(series1.Source);
            Session.Save(series2.Source);

            Session.Save(series1);
            Session.Save(series2);

            Session.Save(newTheme);
            Session.Flush();

            //test first series
            var savedSeries1 = Session.Get<Series>(series1.Id);

            Assert.AreEqual(savedSeries1, series1);
            Assert.IsTrue(savedSeries1.ThemeList.Contains( newTheme));
        }

        public static Theme CreateTheme()
        {
            Random random = new Random();
            long identifier = random.Next();
            Theme theme = new Theme();
            theme.Description = "theme description " + identifier;
            theme.Name = "theme name " + identifier;
            theme.DateCreated = DateTime.Now.Date;
            return theme;
        }

        //[Test]
        //public void CanMapThemeSeries()
        //{

        //    long idenfier = random.Next();
        //    DateTime date = TestHelpers.LateDateTime();
        //    GeneralCategoryCv generalCategoryCv = new GeneralCategoryCv { Name = "My Category" };
           
        //    List<GenericProvenance> provenances = new List<GenericProvenance>(2);
        //    provenances.Add(GenericProvenanceTest.CreateGenericProvenance(idenfier));
        //    provenances.Add(GenericProvenanceTest.CreateGenericProvenance(random.Next()));

        //    List<SeriesRecord> series = new List<SeriesRecord>(3);
        //      series.Add(  SeriesRecordTest.CreateSeriesRecord(random.Next()));
        //      series.Add(SeriesRecordTest.CreateSeriesRecord(random.Next()));
        //      series.Add(SeriesRecordTest.CreateSeriesRecord(random.Next()));
        //    foreach (var seriesRecord in series)
        //    {
        //        Session.Save(seriesRecord.Site);
        //        Session.Save(seriesRecord.Variable);
        //        Session.Save(seriesRecord.Scale);
        //        Session.Save(seriesRecord);
        //    }

        //    new PersistenceSpecification<ThemeSeries>(Session)
        //    .CheckProperty(x => x.Code, "C" + idenfier)
        //    .CheckProperty(x => x.Description, "test")
        //    .CheckProperty(x => x.Name, "test name")
        //    .CheckProperty(x => x.CreatedOn, date)
        //    .CheckProperty(x => x.CreatedBy, "Nonone")
        //    .CheckProperty(x => x.LastUpdate, date)
        //    .CheckProperty(x => x.LocationName, "Transylania")
        //    .CheckProperty(x => x.ThemeVersion, 99)
        //    .CheckProperty(x => x.GeneralCategory, generalCategoryCv)
        //    .CheckList(x => x.ProvenanceHistory, provenances)
        //    .CheckList(x => x.Series, series )
        //    .VerifyTheMappings();
        //}

        //[Test]
        //public void CanMapThemeSites()
        //{

        //    long idenfier = random.Next();
        //    DateTime date = TestHelpers.LateDateTime();
        //    GeneralCategoryCv generalCategoryCv = new GeneralCategoryCv { Name = "My Category" };

        //    List<GenericProvenance> provenances = new List<GenericProvenance>(2);
        //    provenances.Add(GenericProvenanceTest.CreateGenericProvenance(idenfier));
        //    provenances.Add(GenericProvenanceTest.CreateGenericProvenance(random.Next()));

        //    List<Site> sites = new List<Site>(3);
        //    sites.Add(SitesTest.CreateSite(random.Next()));
        //    sites.Add(SitesTest.CreateSite(random.Next()));
        //    sites.Add(SitesTest.CreateSite(random.Next()));
        //    foreach (var site in sites)
        //    {


        //        Session.Save(site);
        //    }

        //    new PersistenceSpecification<ThemeSites>(Session)
        //    .CheckProperty(x => x.Code, "C" + idenfier)
        //    .CheckProperty(x => x.Description, "test")
        //    .CheckProperty(x => x.Name, "test name")
        //    .CheckProperty(x => x.CreatedOn, date)
        //    .CheckProperty(x => x.CreatedBy, "Nonone")
        //    .CheckProperty(x => x.LastUpdate, date)
        //    .CheckProperty(x => x.LocationName, "Transylania")
        //    .CheckProperty(x => x.ThemeVersion, 99)
        //    .CheckProperty(x => x.GeneralCategory, generalCategoryCv)
        //    .CheckList(x => x.ProvenanceHistory, provenances)
        //    .CheckList(x => x.Sites, sites)
        //    .VerifyTheMappings();
        //}

        //public ThemeBase CreateThemeBase(long identifier)
        //{
        //    DateTime date = TestHelpers.LateDateTime();
        //    GeneralCategoryCv generalCategoryCv = new GeneralCategoryCv { Name = "My Category" };
        //    List<GenericProvenance> provenances = new List<GenericProvenance>(2);
        //    provenances.Add(GenericProvenanceTest.CreateGenericProvenance(identifier));
        //    provenances.Add(GenericProvenanceTest.CreateGenericProvenance(random.Next()));

        //    ThemeBase t = new ThemeBase {
        //        Code = "C" + identifier
        //    ,Description="test"
        //    ,Name="test name"
        //    ,CreatedOn= date
        //    ,CreatedBy= "Nonone"
        //    ,LastUpdate= date
        //    ,LocationName= "Transylania"
        //    ,ThemeVersion= 99
        //    ,GeneralCategory= generalCategoryCv
        //   ,ProvenanceHistory= provenances
        //    }
        //    ;

        //    return t;
        //}
    }
}
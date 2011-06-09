using System;
using System.Collections.Generic;
using HydroDesktop.Database.Tests.MappingTests.DataRepository;
using NUnit.Framework;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Tests.DataManagerTests
{
    [TestFixture]
    public class SeriesRepositoryTest
    {
        static Random random = new Random();
        private RepositoryManager manager;


        [SetUp]
        public void SetupManager()
        {
            manager = GetManager();
        }

        [Test]
        public void CanCheckExistingSeries()
        {
            int rnd = random.Next(1000);
            
            //a theme is created
            Theme theme1 = ThemesTest.CreateTheme();
            
            //a series is created and saved to database
            Series series1 = DataSeriesTest.CreateSeries(rnd);

           // manager.BeginTransaction();
            manager.SaveSeries(series1, theme1, OverwriteOptions.Copy);
          // manager.Commit();

            //a new identical series is created in-memory
            Series series2 = DataSeriesTest.CreateSeries(rnd);
            series2.Site = series1.Site;
            series2.Variable = series1.Variable;
            series2.Method = series1.Method;
            series2.QualityControlLevel = series1.QualityControlLevel;
            series2.Source = series1.Source;

            Series existing = manager.SeriesDAO.CheckForExisting(series2);

            //Assert.AreEqual(series2.Id, 0, "series2 should be unsaved (id==0)");
            //Assert.AreNotEqual(existing.Id, 0, "the 'existing' series should be retrieved from db (id > 0)");

            //the 'existing' series should be equal to 'series1' previously saved
            //Assert.AreEqual(existing, series1, "The 'existing' series should be equal to 'series1' previously saved.");
        }

        [Test]
        public void CanSaveSeriesWithDataValues1()
        {
            int rnd = random.Next(1000);

            //a series with default site, variable, method and quality control is created..
            Series series1 = DataSeriesTest.CreateSeries(rnd);
            series1.AddDataValue(DateTime.Now.Date.AddDays(-1), rnd);
            series1.AddDataValue(DateTime.Now.Date, rnd);
            series1.DataValueList[0].Qualifier = QualifiersTest.CreateQualifier();

            string siteName = series1.Site.Name;
            Theme myTheme = new Theme("My Theme", "the testing theme number 1.");

            manager.SaveSeries(series1, myTheme, OverwriteOptions.Copy);

            //Series fromDB = manager.SeriesDAO.FindUnique("Site.Name", siteName);

            //Assert.AreEqual(fromDB.ValueCount, 2, "the retrieved series should have two data values.");
        }

        [Test]
        public void CanSaveTwoSeries()
        {
            CanSaveManySeries(2, 100, 1);
        }
        [Test]
        public void CanSaveTwoSeriesToThemes()
        {
            CanSaveManySeries(2, 100, 2);
        }


        [TestCase(1,100,1)]
        public void CanSaveManySeries(int numberOfSeries, int dataValueCount, int numberOfThemes){
        int rnd = random.Next(1000);
            List<Theme> themes = new List<Theme>(numberOfThemes);
            List<Series> series = new List<Series>(numberOfSeries);
            int numSaved = 0;
            for (int i = 0; i < numberOfThemes; i++ ) {
           themes.Add(new Theme("my theme " +i, "theme " + i));
 }
           
            Site mySite = SitesTest.CreateSite(rnd);
            mySite.Name = "MY SITE 1";

            DateTime baseDate = new DateTime(2009, 1, 1);
 for (int n = 0; n < numberOfSeries;n++)
            {

            Series series2 = DataSeriesTest.CreateSeries(rnd);
            series2.Site = mySite;

            //add some values to both series
            for (int i = 0; i < dataValueCount; i++)
            {
                int val = random.Next(1000);
                DataValuesTest.CreateDataValue(val * 2, series2, baseDate.AddDays(i));
            }
  series.Add(series2);
            }

            if (numberOfThemes == numberOfSeries)
            {
               for (int n = 0; n < numberOfSeries;n++)
               {
                   numSaved += manager.SaveSeries(series[n],themes[n], OverwriteOptions.Copy);
               }
            } else
            {
                for (int n = 0; n < numberOfSeries; n++)
                {
                    int themeN = random.Next(0, numberOfThemes);
                    numSaved += manager.SaveSeries(series[n], themes[themeN], OverwriteOptions.Copy);
                }

            }
       
        }

        [Test, Combinatorial] public void TestAddDataValuesDateValue(
            [Values(0, 1, 10)]int runSize,
            [Values(true, false)]bool withQualifiers)
        {
            int rnd = random.Next(1000);

              //a series with default site, variable, method and quality control is created..
            Series series1 = DataSeriesTest.CreateSeries(rnd);

            for (int i = 0; i < runSize; i++)
            {
                series1.AddDataValue(DateTime.Now.Date.AddMinutes(-runSize), rnd);

                if (withQualifiers)
                {
                    series1.DataValueList[i].Qualifier = QualifiersTest.CreateQualifier();
                }
            }

            string siteName = series1.Site.Name;
            Theme myTheme = new Theme("My Theme", "the testing theme number 1.");

            Assert.AreEqual(runSize, series1.ValueCount, "the created series should have " + runSize + " values.");

            manager.SaveSeries(series1, myTheme, OverwriteOptions.Copy);

            Series fromDB = manager.SeriesDAO.CheckForExisting(series1);

            Assert.AreEqual(runSize, fromDB.ValueCount, "the retrieved series should have " + runSize + " values.");

            Assert.AreEqual(series1.ValueCount, fromDB.ValueCount, "the retrieved series should have values.");
      
        }

        [Test, Combinatorial]
        public void TestAddDataValuesEmpty(
            [Values(0, 1, 10)]int runSize,
            [Values(true, false)]bool withQualifiers)
        {
            int rnd = random.Next(1000);

            //a series with default site, variable, method and quality control is created..
            Series series1 = DataSeriesTest.CreateSeries(rnd);

            for (int i = 0; i < runSize; i++)
            {
                series1.AddDataValue();

                if (withQualifiers)
                {
                    series1.DataValueList[i].Qualifier = QualifiersTest.CreateQualifier();
                }
            }

            string siteName = series1.Site.Name;
            Theme myTheme = new Theme("My Theme", "the testing theme number 1.");

            Assert.AreEqual(runSize, series1.ValueCount, "the created series should have " + runSize + " values.");

            manager.SaveSeries(series1, myTheme, OverwriteOptions.Copy);

            Series fromDB = manager.SeriesDAO.CheckForExisting(series1);

            Assert.AreEqual(runSize, fromDB.ValueCount, "the retrieved series should have " + runSize + " values.");

            Assert.AreEqual(series1.ValueCount, fromDB.ValueCount, "the retrieved series should have values.");
        }

        [Test, Combinatorial]
        public void TestAddDataValuesDataValue(
            [Values(0, 1, 10)]int runSize,
            [Values(true, false)]bool withQualifiers)
        {
            int rnd = random.Next(1000);

            //a series with default site, variable, method and quality control is created..
            Series series1 = DataSeriesTest.CreateSeries(rnd);

            for (int i = 0; i < runSize; i++)
            {
                DataValue value = new DataValue();
                value.DateTimeUTC = DateTime.Now;
                value.UTCOffset = random.Next(-9, 10);
                value.Value = random.Next();

                series1.AddDataValue(value);

                if (withQualifiers)
                {
                    series1.DataValueList[i].Qualifier = QualifiersTest.CreateQualifier();
                }
            }

            string siteName = series1.Site.Name;
            Theme myTheme = new Theme("My Theme", "the testing theme number 1.");

            Assert.AreEqual(runSize, series1.ValueCount, "the created series should have " + runSize + " values.");

            manager.SaveSeries(series1, myTheme, OverwriteOptions.Copy);

            Series fromDB = manager.SeriesDAO.CheckForExisting(series1);

            Assert.AreEqual(runSize, fromDB.ValueCount, "the retrieved series should have " + runSize + " values.");

            Assert.AreEqual(series1.ValueCount, fromDB.ValueCount, "the retrieved series should have values.");
        }

        [Test, Combinatorial]
        public void AddDataValueUtcTimeTest(
          [Range(-10,14)]  double utcOffset)
        {
            int rnd = random.Next(1000);

            //a series with default site, variable, method and quality control is created..
            Series series1 = DataSeriesTest.CreateSeries(rnd);
            
            DateTime now = DateTime.Now;

            DateTime dt2008 = new DateTime(2008,01,01,01,01,01);

            DataValue val = series1.AddDataValue(dt2008, 1, utcOffset);
            Assert.AreEqual(utcOffset, val.UTCOffset);
            Assert.AreEqual(dt2008, val.LocalDateTime);

            val = series1.AddDataValue(now, 1, utcOffset);
            Assert.AreEqual(utcOffset, val.UTCOffset);
            Assert.AreEqual(now, val.LocalDateTime);

            int offset = random.Next(-10, 14);
            val = series1.AddDataValue(now, 1, offset);
            Assert.AreEqual(offset, val.UTCOffset);
            Assert.AreEqual(now, val.LocalDateTime);
        }

        [Test, Combinatorial]
        public void DataValueUtcTimeTest(
          [Range(-10, 14)]  double utcOffset)
        {
            int rnd = random.Next(1000);

            DateTime now = DateTime.Now;

            DateTime dt2008 = new DateTime(2008, 01, 01, 01, 01, 01);

            DataValue val = new DataValue();
            val.DateTimeUTC = dt2008;
            val.UTCOffset = utcOffset;
            val.Value = random.Next();

            Assert.AreEqual(utcOffset, val.UTCOffset);
            Assert.AreEqual(dt2008, val.LocalDateTime);

            val = new DataValue();
            val.DateTimeUTC = now;
            val.UTCOffset = utcOffset;
            val.Value = random.Next();

            Assert.AreEqual(utcOffset, val.UTCOffset);
            Assert.AreEqual(now, val.LocalDateTime);

           
        }

        [Test, Combinatorial]
        public void TestAddDataValuesTimeValueUtcOffset(
            [Values(0, 1, 10)]int runSize,
            [Values(true, false)]bool withQualifiers)
        {
            int rnd = random.Next(1000);

            //a series with default site, variable, method and quality control is created..
            Series series1 = DataSeriesTest.CreateSeries(rnd);

            for (int i = 0; i < runSize; i++)
            {
                int offset = random.Next(-12, 12);
                DateTime calcTime = DateTime.Now;
                calcTime = calcTime.AddMinutes(-runSize);
                series1.AddDataValue(calcTime, random.Next(), offset);

                if (withQualifiers)
                {
                    series1.DataValueList[i].Qualifier = QualifiersTest.CreateQualifier();
                }
            }

            string siteName = series1.Site.Name;
            Theme myTheme = new Theme("My Theme", "the testing theme number 1.");

            Assert.AreEqual(runSize, series1.ValueCount, "the created series should have " + runSize + " values.");

            manager.SaveSeries(series1, myTheme, OverwriteOptions.Copy);

            Series fromDB = manager.SeriesDAO.CheckForExisting(series1);

            Assert.AreEqual(runSize, fromDB.ValueCount, "the retrieved series should have " + runSize + " values.");

            Assert.AreEqual(series1.ValueCount, fromDB.ValueCount, "the retrieved series should have values.");
        }

        [Test, Combinatorial]
        public void TestAddDataValuesTimeValueUtcOffsetQualifier(
            [Values(0, 1, 10)]int runSize)
            
        {
            int rnd = random.Next(1000);

            //a series with default site, variable, method and quality control is created..
            Series series1 = DataSeriesTest.CreateSeries(rnd);

            for (int i = 0; i < runSize; i++)
            {

                series1.AddDataValue(DateTime.Now.AddMinutes(-runSize), random.Next(), random.Next(-12, 12), QualifiersTest.CreateQualifier());

               
            }

            string siteName = series1.Site.Name;
            Theme myTheme = new Theme("My Theme", "the testing theme number 1.");

            Assert.AreEqual(runSize, series1.ValueCount, "the created series should have " + runSize + " values.");

            manager.SaveSeries(series1, myTheme, OverwriteOptions.Copy);

            Series fromDB = manager.SeriesDAO.CheckForExisting(series1);

            Assert.AreEqual(runSize, fromDB.ValueCount, "the retrieved series should have " + runSize + " values.");

            Assert.AreEqual(series1.ValueCount, fromDB.ValueCount, "the retrieved series should have values.");
        }
        static RepositoryManager GetManager()
        {
            return TestConfig.RepositoryManager;
        }

    }
}
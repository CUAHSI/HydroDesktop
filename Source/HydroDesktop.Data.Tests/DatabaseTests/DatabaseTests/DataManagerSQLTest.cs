using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using HydroDesktop.Database;
using System.Data;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.Data.Tests;

namespace HydroDesktop.Database.Tests.DataManagerTests
{
    [TestFixture]
    public class DataManagerSQLTest
    {
        [Test]
        public void TestGetSeriesListTable()
        {
            RepositoryManagerSQL manager = TestConfig.SQLRepositoryManager;
            DataTable fullSeriesTable = manager.GetSeriesListTable();

            Assert.Greater(fullSeriesTable.Rows.Count, 0);
        }

        [Test]
        public void TestGetSeriesTable_all()
        {
            RepositoryManagerSQL manager = TestConfig.SQLRepositoryManager;
            DataTable fullSeriesTable = manager.GetSeriesTable();

            Assert.Greater(fullSeriesTable.Rows.Count, 0);
        }

        [Test]
        public void TestGetSeriesTable_twoSeries()
        {
            RepositoryManagerSQL manager = TestConfig.SQLRepositoryManager;

            string sql = "SELECT SeriesID from DataSeries";
            DataTable tbl = TestConfig.DbOperations.LoadTable("tbl", sql);
            if (tbl.Rows.Count > 0)
            {
                int[] idList = new int[tbl.Rows.Count];
                for(int i = 0; i < idList.Length ; i++)
                {
                    idList[i] = Convert.ToInt32(tbl.Rows[i][0]);
                }
                DataTable seriesTable = manager.GetSeriesTable(idList);
                Assert.Greater(seriesTable.Rows.Count, 0);
            }
        }

        [Test]
        public void TestGetSeriesTable_oneSeries()
        {
            RepositoryManagerSQL manager = TestConfig.SQLRepositoryManager;

            DataTable seriesTable = manager.GetSeriesTable(1);
            Assert.Greater(seriesTable.Rows.Count, 0);
        }

        [Test]
        public void SaveSeries_Simple()
        {
            RepositoryManagerSQL manager = TestConfig.SQLRepositoryManager;

            Random rnd = new Random();
            int randomNumber = rnd.Next(10000);

            Site mySite = CreateRandomSite();
            Variable myVariable = CreateRandomVariable();
            Method myMethod = CreateRandomMethod();
            QualityControlLevel myQc = CreateRandomQualityControlLevel();
            Source mySource = Source.Unknown; //TODO Change to a 'Random Source'

            Series mySeries = CreateRandomSeries(mySite, myVariable, myMethod, myQc, mySource);

            Theme myTheme = new Theme("DataManagerSQL-Theme");
            
            manager.SaveSeriesAsCopy(mySeries, myTheme);
        }

        [Test]
        public void SaveSeries_TwoSeriesOneSite()
        {
            RepositoryManagerSQL manager = TestConfig.SQLRepositoryManager;

            Random rnd = new Random();
            int randomNumber = rnd.Next(1000);

            Site mySite = CreateRandomSite();
            
            Series mySeries1 = new Series();
            mySeries1.Site = mySite;
            mySeries1.Variable = CreateRandomVariable();

            Series mySeries2 = new Series();
            mySeries2.Variable = CreateRandomVariable();
            mySeries2.Site = mySite;

            Theme myTheme = new Theme("DataManagerSQL-Theme");

            manager.SaveSeriesAsCopy(mySeries1, myTheme);
            manager.SaveSeriesAsCopy(mySeries2, myTheme);
        }

        //public void TestDeleteSeries1()
        //{
        //    //1) create series1 at site1
        //    //2) create series2 at site1
        //    //3) delete series1 - site1 should remain
        //    //4) delete series2 - site2 should be deleted

        //    Random rnd = new Random();

        //    Series s1 = DataSeriesTest.CreateSeries(rnd.Next(10000));

        //    Site mySite = SitesTest.CreateSite(rnd.Next(10000));
            
            
        //    RepositoryManagerSQL manager = TestConfig.SQLRepositoryManager;
        //    DataTable fullSeriesTable = manager.GetSeriesListTable();
        //}

        private static Series CreateRandomSeries(Site site, Variable variable, Method method, QualityControlLevel qc, Source source)
        {
            Random rnd = new Random();
            int numDays = rnd.Next(2000);
            DateTime start = DateTime.Now.Date.AddDays(-numDays);
            double utcOffset = -7;
            
            Series newSeries = new Series(site, variable, method, qc, source);
            newSeries.IsCategorical = false;
            newSeries.LastCheckedDateTime = DateTime.Now;
            newSeries.UpdateDateTime = DateTime.Now;
            newSeries.BeginDateTime = start;
            newSeries.BeginDateTimeUTC = start.AddHours(utcOffset);
            newSeries.EndDateTime = DateTime.Now.Date;
            newSeries.EndDateTimeUTC = newSeries.EndDateTime.AddHours(utcOffset);

            for (int t = 0; t < numDays; t++)
            {
                DateTime obsTime = start.AddDays(t);
                double val = 100.0 * rnd.NextDouble();
                newSeries.DataValueList.Add(new DataValue(val, obsTime, utcOffset));
            }

            return newSeries;
        }

        private static Site CreateRandomSite()
        {
            Random rnd = new Random();
            int randomNumber = rnd.Next(1000);

            double longitude = rnd.NextDouble() * 360 - 180;
            double latitude = rnd.NextDouble() * 180 - 90;
            Site mySite = new Site();
            mySite.Name = "TEST SITE " + randomNumber.ToString();
            mySite.Code = "DataManagerSQL:" + randomNumber.ToString();
            mySite.Comments = "TEST SITE";
            mySite.Latitude = latitude;
            mySite.Longitude = longitude;
            mySite.SpatialReference = new SpatialReference("TEST SRS");
            mySite.LocalProjection = new SpatialReference("TEST LocalProjection");
            mySite.VerticalDatum = "test vertical datum";
            return mySite;
        }

        private static Variable CreateRandomVariable()
        {
            Random rnd = new Random();
            
            int id = rnd.Next();

            Unit variableUnit = CreateUnit();
            Unit timeUnit = CreateTimeUnit();

            Variable variable = new Variable();
            variable.Code = "DataManagerSQL:" + id;
            variable.DataType = "dataType:" + id;
            variable.GeneralCategory = "generalCategory:" + id;
            variable.IsCategorical = false;
            variable.IsRegular = true;
            variable.Name = "name:" + id;
            variable.NoDataValue = -9999.0;
            variable.SampleMedium = "sampleMedium:" + id;
            variable.Speciation = "speciation:" + id;
            variable.TimeSupport = 1.0;
            variable.TimeUnit = timeUnit;
            variable.ValueType = "valueType:" + id;
            variable.VariableUnit = variableUnit;

            return variable;
        }

        public static Unit CreateUnit()
        {
            Random rnd = new Random();
            
            int id = rnd.Next();
            Unit u = new Unit();
            u.Abbreviation = "abbr_" + id;
            u.Name = "UnitName" + id;
            u.UnitsType = "distance";

            return u;
        }

        public static Unit CreateTimeUnit()
        {
            Random rnd = new Random();
            
            int id = rnd.Next();
            Unit u = new Unit();
            u.Abbreviation = "d";
            u.Name = "day";
            u.UnitsType = "time";

            return u;
        }

        public static Method CreateRandomMethod()
        {
            Random rnd = new Random();
            
            Method method = new Method();
            method.Code = rnd.Next(2);
            if (method.Code == 0)
            {
                method.Description = "Field observation using VURV protocol";
                method.Link = "http://hydrodesktop.org";
            }
            else if (method.Code == 1)
            {
                method.Description = "Automatic Grab Sample";
            }
            else
            {
                method.Description = "new unit test method " + rnd.Next(1000);
                method.Link = "new unit test method link " + rnd.Next(1000);
            }
            return method;
        }

        public static QualityControlLevel CreateRandomQualityControlLevel()
        {
            Random rnd = new Random();

            QualityControlLevel qc = new QualityControlLevel();
            int codeNumber = rnd.Next(2);
            qc.Code = codeNumber.ToString();
            if (codeNumber == 0)
            {
                qc.Definition = "Raw Data (DataManagerSQL)";
                qc.Explanation = qc.Definition + "explanation..";
                qc.OriginId = codeNumber;
            }
            else if (codeNumber == 1)
            {
                qc.Definition = "Quality controlled data (DataManagerSQL)";
                qc.Explanation = qc.Definition + "explanation..";
                qc.OriginId = codeNumber;
            }
            else
            {
                qc.Definition = "new unit test QualityControlLevel-" + rnd.Next(1000);
                qc.Explanation = qc.Definition + " explanation.";
            }
            return qc;
        }
    }
}
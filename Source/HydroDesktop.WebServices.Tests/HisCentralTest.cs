using System;
using System.Collections.Generic;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.WebServices.WaterOneFlow;
using NUnit.Framework;

namespace HydroDesktop.WebServices.Tests
{
    [Ignore] // Set Ignore because this tests executes too long with timeout exceptions
    [TestFixture]
    public class HisCentralTest
    {
        [Test]
        [ExpectedException]
        public void TestInvalidURL()
        {
            string badURL = @"http://example.com";
            WaterOneFlowClient client = new WaterOneFlowClient(badURL);     
        }

        [Test]
        public void TestValidURL()
        {
            string goodURL = @"http://his.crwr.utexas.edu/TXEvap/cuahsi_1_0.asmx";
            WaterOneFlowClient client = new WaterOneFlowClient(goodURL);
            IList<Site> sites = client.GetSites();
        }

        [Test]
        public void TestLittleBearRiver_11()
        {
            string url = @"http://water.sdsc.edu/lbrsdsc/cuahsi_1_1.asmx";
            WaterOneFlowClient client = new WaterOneFlowClient(url);
            IList<Site> sites = client.GetSites();
            IList<SeriesMetadata> siteInfoList = client.GetSiteInfo(sites[0].Code);
            IList<Series> series = client.GetValues(siteInfoList[0].Site.Code, siteInfoList[0].Variable.Code, DateTime.MinValue, DateTime.Now);

            Assert.IsNotNull(series);
            Assert.Greater(series.Count, 0);
            Assert.AreNotEqual(series[0].Source.ToString(), "unknown");
            Assert.AreNotEqual(series[0].Method.ToString(), "unknown");
        }

        [Test]
        public void TestTCOON_11()
        {
            string url = @"http://his.crwr.utexas.edu/tcoonts/tcoon.asmx";
            WaterOneFlowClient client = new WaterOneFlowClient(url);
            IList<Site> sites = client.GetSites();
            IList<SeriesMetadata> seriesList = client.GetSiteInfo(sites[0].Code);
            //IList<Series> downloadedData = client.GetValues(seriesList[0].Site.Code, seriesList[0].Variable.Code, DateTime.MinValue, DateTime.Now);

            //Assert.IsNotNull(downloadedData);
            //Assert.Greater(downloadedData.Count, 0);
            //Assert.AreNotEqual(downloadedData[0].Source.ToString(), "unknown");
            //Assert.AreNotEqual(downloadedData[0].Method.ToString(), "unknown");
        }

        [Test]
        [Ignore("appears to be down")]
        public void TestCIMSservice()
        {
            string url = @"http://cbe.cae.drexel.edu/CIMS/cuahsi_1_1.asmx";
            string siteCode = "CIMS:EE3.4";
            string variableCode = "CIMS:NO3F";
            WaterOneFlowClient client = new WaterOneFlowClient(url);
            IList<Series> series = client.GetValues(siteCode, variableCode, DateTime.Now.AddYears(-30), DateTime.Now);
            
            Assert.IsNotNull(series);
            Assert.Greater(series.Count, 0);
            Assert.AreNotEqual(series[0].Source.ToString(), "unknown");
            Assert.AreNotEqual(series[0].Method.ToString(), "unknown");
        }

        [Test]
        public void TestBaltimoreGW_10()
        {
            string url = @"http://his09.umbc.edu/BaltGW/cuahsi_1_0.asmx";
            string siteCode = "BaltimoreGW:WOLDEAMANUEL";
            string variableCode = "BaltimoreGW:WATERASL";
            DateTime beginDate = new DateTime(2000, 1, 1);
            DateTime endDate = DateTime.Now;
            WaterOneFlowClient client = new WaterOneFlowClient(url);
            IList<Series> series = client.GetValues(siteCode, variableCode, beginDate, endDate);
            Assert.IsNotNull(series);
            Assert.Greater(series.Count, 0);
            Assert.AreNotEqual(series[0].Source.ToString(), "unknown");
            Assert.AreNotEqual(series[0].Method.ToString(), "unknown");
        }

        [Test]
        public void TestLittleBearRiver_10()
        {
            string url = @"http://icewater.usu.edu/LittleBearRiver/cuahsi_1_0.asmx";
            WaterOneFlowClient client = new WaterOneFlowClient(url);
            IList<Site> sites = client.GetSites();
            IList<SeriesMetadata> seriesList = client.GetSiteInfo(sites[0].Code);
            IList<Series> downloadedData = client.GetValues(seriesList[0].Site.Code, seriesList[0].Variable.Code, DateTime.MinValue, DateTime.Now);
            
            Assert.IsNotNull(seriesList);
            Assert.Greater(seriesList.Count, 0);
            Assert.AreNotEqual(seriesList[0].Source.ToString(), "unknown");
            Assert.AreNotEqual(seriesList[0].Method.ToString(), "unknown");
        }

        [Test]
        public void TestCRWR_10()
        {
            string url = @"http://his.crwr.utexas.edu/tcoonts/tcoon.asmx";
            WaterOneFlowClient client = new WaterOneFlowClient(url);
            IList<Site> sites = client.GetSites();
            Assert.Greater(sites.Count, 0);
        }

        [Test]
        public void TestNWISDV_10()
        {
            string url = @"http://river.sdsc.edu/wateroneflow/NWIS/Data.asmx";
            string siteCode= "NWISDV:13069500";
            string variableCode = "NWISDV:00060/DataType=Average";
            DateTime start= new DateTime(2010, 1, 1);
            DateTime end= new DateTime(2011, 1, 1);

            WaterOneFlowClient client = new WaterOneFlowClient(url);
            IList<Series> seriesList = client.GetValues(siteCode, variableCode, start, end);
            Assert.Greater(seriesList.Count, 0);
            Assert.Greater(seriesList[0].DataValueList.Count, 0);
        }

        [Test]
        public void TestSeriesWithSamples()
        {
            string url = @"http://water.sdsc.edu/lbrsdsc/cuahsi_1_1.asmx";
            WaterOneFlowClient client = new WaterOneFlowClient(url);
            string siteCode = "LBR11:USU-LBR-Paradise";
            string variableCode = "LBR11:USU39";
            IList<Series> series = client.GetValues(siteCode, variableCode, DateTime.MinValue, DateTime.Now);

            Assert.IsNotNull(series);
            Assert.Greater(series.Count, 0);
            Assert.AreNotEqual(series[0].Source.ToString(), "unknown");
            Assert.AreNotEqual(series[0].Method.ToString(), "unknown");
        }
    }
}
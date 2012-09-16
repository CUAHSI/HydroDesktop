using System;
using System.Globalization;
using System.Net;
using NUnit.Framework;
using HydroDesktop.WebServices.WaterOneFlow;

namespace HydroDesktop.WebServices.Tests.WaterOneFlow
{
    [TestFixture]
    public class WaterOneFlowClientTests
    {
        [Test]
        [TestCase(@"http://icewater.boisestate.edu/dcew2dataservices/cuahsi_1_0.asmx?WSDL")]
        [TestCase(@"http://icewater.boisestate.edu/rcew2dataservices/cuahsi_1_1.asmx?WSDL")]
        [TestCase(@"http://icewater.usu.edu/MudLake/cuahsi_1_0.asmx?WSDL")]
        [TestCase(@"http://hydrodata.info/webservices/cuahsi_1_1.asmx?WSDL")]
        [TestCase(@"http://his.crwr.utexas.edu/TXEvap/cuahsi_1_0.asmx")]
        public void GetSites_SaveXmlFilesFlag_ReturnsSameData(string url)
        {
            var target = new WaterOneFlowClient(url);
            
            target.SaveXmlFiles = false;
            var sites1 = target.GetSites();
            
            target.SaveXmlFiles = true;
            var sites2 = target.GetSites();

            Assert.AreEqual(sites1.Count, sites2.Count);
            for(var i = 0; i<sites1.Count; i++)
            {
                var site1 = sites1[i];
                var site2 = sites2[i];

                Assert.AreEqual(site1.Code, site2.Code);
                Assert.AreEqual(site1.Name, site2.Name);
            }
        }

        [Test]
        [TestCase(@"http://icewater.boisestate.edu/dcew2dataservices/cuahsi_1_0.asmx?WSDL", "DCEW2:dcew.w2")]
        [TestCase(@"http://icewater.boisestate.edu/rcew2dataservices/cuahsi_1_1.asmx?WSDL", "RCEW2:012")]
        [TestCase(@"http://icewater.usu.edu/MudLake/cuahsi_1_0.asmx?WSDL", "MudLake:USU-ML-Causeway")]
        [TestCase(@"http://hydrodata.info/webservices/cuahsi_1_1.asmx?WSDL", "HCLIMATE:114570")]
        [TestCase(@"http://his.crwr.utexas.edu/TXEvap/cuahsi_1_0.asmx", "TxEvap:410040")]
        public void GetSiteInfo_SaveXmlFilesFlag_ReturnsSameData(string url, string siteCode)
        {
            var target = new WaterOneFlowClient(url);

            target.SaveXmlFiles = false;
            var series1 = target.GetSiteInfo(siteCode);

            target.SaveXmlFiles = true;
            var series2 = target.GetSiteInfo(siteCode);

            Assert.AreEqual(series1.Count, series2.Count);
            for (var i = 0; i < series1.Count; i++)
            {
                var site1 = series1[i].Site;
                var site2 = series2[i].Site;
                
                Assert.AreEqual(site1.Code, site2.Code);
                Assert.AreEqual(site1.Name, site2.Name);
                Assert.AreEqual(series1[i].ValueCount, series2[i].ValueCount);
            }
        }

        [Test]
        [TestCase(@"http://icewater.boisestate.edu/dcew2dataservices/cuahsi_1_0.asmx?WSDL", "DCEW2:dcew.w2", "ODMDCEW2:Thravg", "01/01/2000", "01/05/2000")]
        [TestCase(@"http://icewater.boisestate.edu/rcew2dataservices/cuahsi_1_1.asmx?WSDL", "RCEW2:012", "ODMRCEW2:hourly-precipitation", "07/01/2000", "08/01/2000")]
        public void GetValues_SaveXmlFilesFlag_ReturnsSameData(string url, string siteCode, string varCode, string startDate, string endDate)
        {
            const string DATES_FORMAT = "MM/dd/yyyy";
            var provider = CultureInfo.InvariantCulture;
            
            var target = new WaterOneFlowClient(url);
            var start = DateTime.ParseExact(startDate, DATES_FORMAT, provider);
            var end = DateTime.ParseExact(endDate, DATES_FORMAT, provider);
            try
            {
                target.SaveXmlFiles = false;
                var series1 = target.GetValues(siteCode, varCode, start, end);

                target.SaveXmlFiles = true;
                var series2 = target.GetValues(siteCode, varCode, start, end);

                Assert.AreEqual(series1.Count, series2.Count);
                for (var i = 0; i < series1.Count; i++)
                {
                    var site1 = series1[i].Site;
                    var site2 = series2[i].Site;
                    var var1 = series1[i].Variable;
                    var var2 = series2[i].Variable;

                    Assert.AreEqual(site1.Code, site2.Code);
                    Assert.AreEqual(site1.Name, site2.Name);
                    Assert.AreEqual(series1[i].ValueCount, series2[i].ValueCount);
                    Assert.AreEqual(var1.Code, var2.Code);
                    Assert.AreEqual(var1.Name, var2.Name);
                }
            }
            catch (WebException ex)
            {
                if (ex.Status.HasFlag(WebExceptionStatus.ProtocolError) ||
                    ex.Status.HasFlag(WebExceptionStatus.Timeout))
                {
                    Assert.Inconclusive("Unable to test GetValues() from: " + url);
                }
                throw;
            }

        }
    }
}

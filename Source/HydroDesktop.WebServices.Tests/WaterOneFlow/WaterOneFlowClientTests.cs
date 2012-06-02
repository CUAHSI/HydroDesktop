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
    }
}

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
    }
}

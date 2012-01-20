using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.IO;
using HydroDesktop.WebServices.WaterOneFlow;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.WebServices.Tests
{
    [TestFixture]
    public class XmlDownloaderTest
    {
        [TestCase( @"idahoFalls_precipitation.xml")]
        [TestCase( @"pocatello_precipitation.xml")]
        [TestCase( @"rexburg_precipitation.xml")]
        public void TestParseXmlSeries(String fileName)
        {
            WaterOneFlow10Parser parser = new WaterOneFlow10Parser();
            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles");
            string xmlFileName = Path.Combine(basePath, fileName);
                                   
            IList<Series> seriesList = parser.ParseGetValues(xmlFileName);
        }
        
        
        [Test]
        [TestCase(@"http://icewater.boisestate.edu/dcew2dataservices/cuahsi_1_0.asmx")]
        [TestCase(@"http://water.sdsc.edu/lbrsdsc/cuahsi_1_1.asmx")]
      //  [TestCase(@"http://his.crwr.utexas.edu/TXEvap/cuahsi_1_0.asmx")]
        public void TestDownload(string url)
        {
           
                WaterOneFlowClient cl = new WaterOneFlowClient(url);

                string file2 = cl.GetSitesXML();
                Assert.IsTrue(File.Exists(file2), "the GetSites file does not exist.");

                WaterOneFlow10Parser parser1 = new WaterOneFlow10Parser();
                IList<Site> sites = parser1.ParseGetSitesXml(file2);

                foreach (Site site in sites)
                {
                    string fullSiteCode = site.Code;
                    string siteFile = cl.GetSiteInfoXML(fullSiteCode);
                    Assert.IsTrue(File.Exists(siteFile), "the GetSiteInfo file does not exist.");
                }
           
        }
    }
}

using System;
using HydroDesktop.WebServices.WaterOneFlow;
using NUnit.Framework;
using System.IO;

namespace HydroDesktop.WebServices.Tests.WaterOneFlow
{
    [TestFixture]
    public class WaterOneFlow10ParserTests
    {
        [TestCase(@"TestFiles\v10\")]
        public void TestParseDataSeries(String samplesPath)
        {
            var target = new WaterOneFlow10Parser();
            foreach (var fileName in Directory.GetFiles(samplesPath))
            {
                var seriesList = target.ParseGetValues(fileName);

                Assert.Greater(seriesList.Count, 0);
                Assert.Greater(seriesList[0].ValueCount, 0,
                               "Error in filename '" + fileName +
                               "' the number of values in the seriesList must be greater than zero.");
            }
        }

        [TestCase(@"TestFiles\v10sites\")]
        public void TestParseGetSites(String samplesPath)
        {
            var target = new WaterOneFlow10Parser();
            foreach (var fileName in Directory.GetFiles(samplesPath))
            {
                var sites = target.ParseGetSites(fileName);
                Assert.Greater(sites.Count, 0,
                               "Error in file '" + fileName + "' the number of sites must be greater than zero.");
            }
        }

        [Test]
        public void ParseSiteInfo()
        {
            var xmlPath = @"TestFiles\v10sites\Site-DCEW2-dcew.p1-20110216043840885.xml";
            var target = new WaterOneFlow10Parser();

            var result = target.ParseGetSiteInfo(xmlPath);
            Assert.IsTrue(result.Count > 1);

            var series = result[0];

            // Site
            var site = series.Site;
            Assert.AreEqual("Ada", site.County);
            Assert.AreEqual("Idaho", site.State);
            Assert.AreEqual("Comments", site.Comments);
            Assert.AreEqual(null, site.Country);
            Assert.AreEqual(null, site.SiteType);


            //QualityControlLevel
            var qcl = series.QualityControlLevel;
            Assert.AreEqual(2, qcl.OriginId);
        }
    }
}

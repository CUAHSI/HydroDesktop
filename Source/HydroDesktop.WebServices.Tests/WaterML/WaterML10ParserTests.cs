using System;
using HydroDesktop.WebServices.WaterML;
using NUnit.Framework;

namespace HydroDesktop.WebServices.Tests.WaterML
{
    [TestFixture]
    public class WaterML10ParserTests
    {
        [TestCase(@"TestFiles\v10\NWISDV-08157700-NWISDV-00060-DataType-Average-20120603025214538.xml")]
        [TestCase(@"TestFiles\v10\RioGrandeET-SEV-RioGrandeET-ET_Penman-20110211055032021.xml")]
        public void TestParseDataSeries(String fileName)
        {
            var target = new WaterML10Parser();
            var seriesList = target.ParseGetValues(fileName);

            Assert.Greater(seriesList.Count, 0);
            Assert.Greater(seriesList[0].ValueCount, 0,
                           "Error in filename '" + fileName +
                           "' the number of values in the seriesList must be greater than zero.");
        }

        [TestCase(@"TestFiles\v10\Site-DCEW2-dcew.p1-20110216043840885.xml")]
        [TestCase(@"TestFiles\v10\sites20100211051408633.xml")]
        [TestCase(@"TestFiles\v10\sites20100828041541653.xml")]
        [TestCase(@"TestFiles\v10\sites20100831054240254.xml")]
        public void TestParseGetSites(String fileName)
        {
            var target = new WaterML10Parser();
            var sites = target.ParseGetSites(fileName);
            Assert.Greater(sites.Count, 0,
                           "Error in file '" + fileName + "' the number of sites must be greater than zero.");
        }

        [Test]
        public void ParseSiteInfo()
        {
            var xmlPath = @"TestFiles\v10\Site-DCEW2-dcew.p1-20110216043840885.xml";
            var target = new WaterML10Parser();

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

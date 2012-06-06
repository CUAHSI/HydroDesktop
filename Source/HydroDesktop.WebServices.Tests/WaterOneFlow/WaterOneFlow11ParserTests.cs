using HydroDesktop.WebServices.WaterOneFlow;
using NUnit.Framework;

namespace HydroDesktop.WebServices.Tests.WaterOneFlow
{
    [TestFixture]
    public class WaterOneFlow11ParserTests
    {
        [Test]
        public void ParseSite()
        {
            var xmlPath = @"TestFiles\v11\Site-RCEW2-012-20120604043508874.xml";
            var target = new WaterOneFlow11Parser();

            var siteInfo = target.ParseGetSiteInfo(xmlPath);
            var actual = siteInfo[0].Site;

            Assert.AreEqual("Owyhee", actual.County);
            Assert.AreEqual("Idaho", actual.State);
            Assert.AreEqual("Comments", actual.Comments);
            Assert.AreEqual(3.5, actual.PosAccuracy_m);
        }
    }
}

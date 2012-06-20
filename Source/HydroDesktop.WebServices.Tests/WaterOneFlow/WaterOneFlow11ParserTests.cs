using HydroDesktop.WebServices.WaterOneFlow;
using NUnit.Framework;

namespace HydroDesktop.WebServices.Tests.WaterOneFlow
{
    [TestFixture]
    public class WaterOneFlow11ParserTests
    {
        [Test]
        public void ParseSiteInfo()
        {
            var xmlPath = @"TestFiles\v11\Site-RCEW2-012-20120604043508874.xml";
            var target = new WaterOneFlow11Parser();

            var result = target.ParseGetSiteInfo(xmlPath);
            Assert.IsTrue(result.Count > 1);

            var series = result[0];

            // Site
            var site = series.Site;
            Assert.AreEqual("Owyhee", site.County);
            Assert.AreEqual("Idaho", site.State);
            Assert.AreEqual("Comments", site.Comments);
            Assert.AreEqual(3.5, site.PosAccuracy_m);
            Assert.AreEqual("Country", site.Country);
            Assert.AreEqual("SiteType", site.SiteType);

            //QualityControlLevel
            var qcl = series.QualityControlLevel;
            Assert.AreEqual(1, qcl.OriginId);
            Assert.AreEqual("1", qcl.Code);
            Assert.AreEqual("Quality controlled data", qcl.Definition);
            Assert.AreEqual("Explanation", qcl.Explanation);
        }

        [Test]
        public void ParseISOMetadata()
        {
            var xmlPath = @"TestFiles\v11\GetValues_Mendon_usu3.xml";
            var target = new WaterOneFlow11Parser();

            var result = target.ParseGetValues(xmlPath);
            var series = result[0];

            var isoMetadata = series.Source.ISOMetadata;

            Assert.AreEqual("inlandWaters", isoMetadata.TopicCategory);
            Assert.AreEqual("Little Bear River Conservation Effects Assessment Project water quality data.", isoMetadata.Title);
            Assert.AreEqual("Abstract", isoMetadata.Abstract);
            Assert.AreEqual("ProfileVersion", isoMetadata.ProfileVersion);
            Assert.AreEqual("MetadataLink", isoMetadata.MetadataLink);
        }
    }
}

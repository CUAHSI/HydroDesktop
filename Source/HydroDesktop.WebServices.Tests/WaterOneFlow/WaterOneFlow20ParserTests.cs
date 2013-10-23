using System;
using HydroDesktop.WebServices.WaterOneFlow;
using NUnit.Framework;

namespace HydroDesktop.WebServices.Tests.WaterOneFlow
{
    [TestFixture]
    public class WaterOneFlow20ParserTests
    {
        [Test]
        public void ParseSiteInfoV2()
        {
            var xmlPath = @"TestFiles\v20\Site-RCEW2-012-20120604043508874.xml";
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
        public void ParseISOMetadataV2()
        {
            var xmlPath = @"TestFiles\v20\GetValues_Mendon_usu3_wml2.xml";
            var target = new WaterOneFlow20Parser();

            var result = target.ParseGetValues(xmlPath);
            var series = result[0];

            var isoMetadata = series.Source.ISOMetadata;

            Assert.AreEqual("inlandWaters", isoMetadata.TopicCategory);
            Assert.AreEqual("Little Bear River Conservation Effects Assessment Project water quality data.", isoMetadata.Title);
            Assert.AreEqual("Abstract", isoMetadata.Abstract);
            Assert.AreEqual("ProfileVersion", isoMetadata.ProfileVersion);
            Assert.AreEqual("MetadataLink", isoMetadata.MetadataLink);
        }

        [Test]
        public  void ParseDataValuesV2()
        {
            var xmlPath = @"TestFiles\v20\GetValues_Mendon_usu3_wml2.xml";
            var target = new WaterOneFlow20Parser();

            var result = target.ParseGetValues(xmlPath);
            var series = result[0];

            // Check first data value
            var first = series.DataValueList[0];
            Assert.AreEqual(12.91645, first.Value);
            Assert.AreEqual(new DateTime(2005, 08, 05, 07, 00, 00), first.DateTimeUTC);
            Assert.AreEqual(new DateTime(2005, 08, 05, 00, 00, 00), first.LocalDateTime);
            Assert.AreEqual(-7, first.UTCOffset);

            // Check last data value
            var last = series.DataValueList[series.DataValueList.Count - 1];
            Assert.AreEqual(12.95674, last.Value);
            Assert.AreEqual(new DateTime(2005, 08, 06, 07, 00, 00), last.DateTimeUTC);
            Assert.AreEqual(new DateTime(2005, 08, 06, 00, 00, 00), last.LocalDateTime);
            Assert.AreEqual(-7, last.UTCOffset);

            // Check series  BeginDateTime/BeginDateTimeUTC, EndDateTime/EndDateTimeUTC
            var expectedBeginDateTime = first.LocalDateTime;
            var expectedBeginDateTimeUTC = first.DateTimeUTC;

            var expectedEndDateTime = last.LocalDateTime;
            var expectedEndDateTimeUTC = last.DateTimeUTC;

            Assert.AreEqual(expectedBeginDateTime, series.BeginDateTime);
            Assert.AreEqual(expectedBeginDateTimeUTC, series.BeginDateTimeUTC);
            Assert.AreEqual(expectedEndDateTime, series.EndDateTime);
            Assert.AreEqual(expectedEndDateTimeUTC, series.EndDateTimeUTC);
        }
    }
}

using HydroDesktop.Search.Download;
using NUnit.Framework;

namespace HydroDesktop.Search.SearchUnitTests.Download
{
    [TestFixture]
    class OneSeriesDownloadInfoTest
    {

        [Test]
        public void PropertiesExists()
        {
            TestHelper.AssertPropertyExists<OneSeriesDownloadInfo>(OneSeriesDownloadInfo.PROPERTY_Wsdl);
            TestHelper.AssertPropertyExists<OneSeriesDownloadInfo>(OneSeriesDownloadInfo.PROPERTY_VariableName);
            TestHelper.AssertPropertyExists<OneSeriesDownloadInfo>(OneSeriesDownloadInfo.PROPERTY_Status);
            TestHelper.AssertPropertyExists<OneSeriesDownloadInfo>(OneSeriesDownloadInfo.PROPERTY_StartDate);
            TestHelper.AssertPropertyExists<OneSeriesDownloadInfo>(OneSeriesDownloadInfo.PROPERTY_SiteName);
            TestHelper.AssertPropertyExists<OneSeriesDownloadInfo>(OneSeriesDownloadInfo.PROPERTY_Longitude);
            TestHelper.AssertPropertyExists<OneSeriesDownloadInfo>(OneSeriesDownloadInfo.PROPERTY_Latitude);
            TestHelper.AssertPropertyExists<OneSeriesDownloadInfo>(OneSeriesDownloadInfo.PROPERTY_FullVariableCode);
            TestHelper.AssertPropertyExists<OneSeriesDownloadInfo>(OneSeriesDownloadInfo.PROPERTY_FullSiteCode);
            TestHelper.AssertPropertyExists<OneSeriesDownloadInfo>(OneSeriesDownloadInfo.PROPERTY_ErrorMessage);
            TestHelper.AssertPropertyExists<OneSeriesDownloadInfo>(OneSeriesDownloadInfo.PROPERTY_EndDate);
            TestHelper.AssertPropertyExists<OneSeriesDownloadInfo>(OneSeriesDownloadInfo.PROPERTY_DownloadTimeTaken);
        }
    }
}

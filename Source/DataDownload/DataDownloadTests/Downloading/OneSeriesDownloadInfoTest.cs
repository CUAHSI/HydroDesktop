using HydroDesktop.DataDownload.Downloading;
using NUnit.Framework;

namespace HydroDesktop.DataDownload.Tests.Downloading
{
    [TestFixture]
    class OneSeriesDownloadInfoTest
    {
        [Test]
        public void PropertiesExists()
        {
            TestHelper.AssertPropertyExists<OneSeriesDownloadInfo>(OneSeriesDownloadInfo.PROPERTY_Wsdl);
            TestHelper.AssertPropertyExists<OneSeriesDownloadInfo>(OneSeriesDownloadInfo.PROPERTY_VariableName);
            TestHelper.AssertPropertyExists<OneSeriesDownloadInfo>(OneSeriesDownloadInfo.PROPERTY_SiteName);
            TestHelper.AssertPropertyExists<OneSeriesDownloadInfo>(OneSeriesDownloadInfo.PROPERTY_FullVariableCode);
            TestHelper.AssertPropertyExists<OneSeriesDownloadInfo>(OneSeriesDownloadInfo.PROPERTY_FullSiteCode);
            TestHelper.AssertPropertyExists<OneSeriesDownloadInfo>(OneSeriesDownloadInfo.PROPERTY_StatusAsString);
        }
    }
}

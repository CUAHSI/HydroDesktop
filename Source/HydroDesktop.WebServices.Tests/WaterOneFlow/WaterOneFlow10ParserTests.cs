using System;
using System.Collections.Generic;
using HydroDesktop.WebServices.WaterOneFlow;
using HydroDesktop.Interfaces.ObjectModel;
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
            IWaterOneFlowParser parser = new WaterOneFlow10Parser();
            string xmlDirectory = GetXmlTempDirectory() ;

            // we want a clean set of test cases
            CleanAndCopySamplesFiles(xmlDirectory, samplesPath);

            string[] fileNames = Directory.GetFiles(xmlDirectory);

            foreach (string fileName in fileNames)
            {
                IList<Series> seriesList = parser.ParseGetValues(fileName);
                Assert.Greater(seriesList.Count, 0);
                Assert.Greater(seriesList[0].ValueCount, 0, 
                    "Error in filename '"+fileName + "' the number of values in the seriesList must be greater than zero.");
            }
        }


        [TestCase(@"TestFiles\v10sites\")]
        public void TestParseGetSites(String samplesPath)
        {
            IWaterOneFlowParser parser = new WaterOneFlow10Parser();
            string xmlDirectory = GetXmlTempDirectory();

            // we want a clean set of test cases
            CleanAndCopySamplesFiles(xmlDirectory, samplesPath);

            string[] fileNames = Directory.GetFiles(xmlDirectory);

            foreach (string fileName in fileNames)
            {
                IList<Site> sites = parser.ParseGetSitesXml(fileName);
                Assert.Greater(sites.Count, 0, "Error in file '"+fileName+ "' the number of sites must be greater than zero.");
            }

        }

        private void CleanAndCopySamplesFiles(string xmlDirectory, string samplesPath)
        {
            foreach (var file in Directory.GetFiles(xmlDirectory))
            {
                File.Delete(file);
            }
            foreach (var file in Directory.GetFiles(samplesPath))
            {
                string fileFrom = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar +
                                  file;
                string fileTo = Path.Combine(xmlDirectory, Path.GetFileName(file));
                File.Copy(fileFrom, fileTo, true);
            }
        }


        public void TestParseGetSiteInfo()
        {
            IWaterOneFlowParser parser = new WaterOneFlow10Parser();
            string xmlDirectory = GetXmlTempDirectory();
           

            string[] fileNames = Directory.GetFiles(xmlDirectory);

            List<SeriesMetadata> fullSeriesList = new List<SeriesMetadata>();

            foreach (string path in fileNames)
            {
                string fileName = Path.GetFileName(path);
                if (fileName.ToLower().StartsWith("site-"))
                {
                    IList<SeriesMetadata> seriesList = parser.ParseGetSiteInfo(path);
                    fullSeriesList.AddRange(seriesList);
                    //Assert.Greater(seriesList.Count, 0, 
                        //"the number of series in the series list should be > zero." );
                }
            }
            Assert.Greater(fullSeriesList.Count, 0, "the full series list count should be > 0");
        }

        /// <summary>
        /// Gets the temporary directory for xml files downloaded
        /// by HydroDesktop
        /// </summary>
        /// <returns>the directory path</returns>
        public static string GetXmlTempDirectory()
        {
            //Check if we need to create a temporary folder for storing the xml file
            string tempDirectory = Path.Combine(Path.GetTempPath(), @"HydroDesktop\UnitTestFiles");
            if (Directory.Exists(tempDirectory) == false)
            {
                try
                {
                    Directory.CreateDirectory(tempDirectory);
                }
                catch
                {
                    tempDirectory = Path.GetTempPath();
                }
            }
            return tempDirectory;
        }


        public static string GetXmlFileName()
        {
            //return Path.Combine(GetXmlTempDirectory(), "NCDCISD-72586724133-NCDCISD-TMP.xml");
            //return Path.Combine(GetXmlTempDirectory(), "download0010.xml");
            return Path.Combine(GetXmlTempDirectory(), "sites.xml");
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Search;
using NUnit.Framework;

namespace SearchUnitTests
{
    [TestFixture()]
    public class SearchWithFailoveNoMockTest
    {
        //private IHISCentralSearcher HisCentralSearcher;
        private SearchCriteria worksCriteria;
        private DoWorkEventArgs workEventsWorks;
        private DoWorkEventArgs workEventsFails;
        private DoWorkEventArgs workEventsFailWork;

        //private Mock<BackgroundWorker> backgroundWorker;

        private List<string> keywords;
        private List<int> serviceIds;

        private BackgroundSearchWithFailover _search;

        [SetUp]
        public void setup()
        {/*
            backgroundWorker = new Mock<BackgroundWorker>();

            keywords = new List<string> { "aa" };
            serviceIds = new List<int> { 1 };
            worksCriteria = new SearchCriteria
           {
               areaParameter =
                   new SearchCriteria.AreaRectangle { xMax = -112, xMin = -111.5, yMax = 40, yMin = 41 },
               BoundinBoxSearch = true,
               hisCentralURL = "http://example.com",
               startDate = DateTime.Now.AddDays(-1),
               endDate = DateTime.Now
           };

            worksCriteria.keywords.AddRange(keywords);
            worksCriteria.serviceIDs.AddRange(serviceIds);
            */
        }

        [Test()]
        public void TestAllWork()
        {
            /*
            SearchCriteria worksCriteria = new SearchCriteria
                                              {
                                                  areaParameter =
                                                      new SearchCriteria.AreaRectangle { xMax = -112, xMin = -111.5, yMax = 40, yMin = 41 },
                                                  BoundinBoxSearch = true,
                                                  hisCentralURL = "http://example.com",
                                                  startDate = DateTime.Now.AddDays(-1),
                                                  endDate = DateTime.Now
                                              };

            workEventsWorks = new DoWorkEventArgs(worksCriteria);

            _search = new BackgroundSearchWithFailover();
            // _search.Searcher = HisCentralSearcher.Object;
            _search.HISCentralSearchWithFailover(workEventsWorks,
              HydroDesktop.Configuration.Settings.Instance.HISCentralURLList,
               backgroundWorker.Object);*/
        }

        [Test()]
        //   [ExpectedException()]
        public void TestAllFail()
        {
            workEventsFails = new DoWorkEventArgs(worksCriteria);
            _search = new BackgroundSearchWithFailover();
            //          _search.Searcher = HisCentralSearcher.Object;
        /*    Assert.Throws<HydrodesktopSearchException>(
                delegate
                {
                    _search.HISCentralSearchWithFailover(workEventsFails,
                                                         new List<string> { "http://example.com" },
                                                         backgroundWorker.Object);
                }
                );*/
        }

        /* The method is void, so there is no way to set a
         * .Return(void).Callback(throw Exception)
         * Tried .Callback(c => {if (counter >0) { return new mock<>.setup().Throws() }
         * else {return new mock.setup() }
         * */

        [Test()]
        public void TestFailWork()
        {
            //int count = 0;
            // Fails three out of four times;
            /*
            workEventsFailWork = new DoWorkEventArgs(worksCriteria);

            _search = new BackgroundSearchWithFailover();
            // _search.Searcher = HisCentralSearcher;

            var urls = new List<string> { "http://example.com/1", "http://example.com/2", "http://example.com/3", "http://hiscentral.cuahsi.org/webservices/hiscentral.asmx" };
            _search.HISCentralSearchWithFailover(workEventsFailWork,
               urls,
                backgroundWorker.Object);*/
        }
    }
}
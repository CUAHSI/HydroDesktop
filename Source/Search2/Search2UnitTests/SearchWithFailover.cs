using NUnit.Framework;

namespace SearchUnitTests
{
    [TestFixture()]
    public class SearchWithFailoverTest
    {
        /*
        private Mock<IHISCentralSearcher> HisCentralSearcher;

        private DoWorkEventArgs workEventsWorks;
        private DoWorkEventArgs workEventsFails;
        //private DoWorkEventArgs workEventsFailWork;

        private Mock<BackgroundWorker> backgroundWorker;

        private List<string> keywords;
        private List<int> serviceIds;

        private BackgroundSearchWithFailover _search;

        [SetUp]
        public void setup()
        {
            // we are not doing any map stuff, so the mock object does not need any config
            HisCentralSearcher = new Mock<IHISCentralSearcher>();

            backgroundWorker = new Mock<BackgroundWorker>();

            keywords = new List<string> { "aa" };
            serviceIds = new List<int> { 1 };
        }

        [Test()]
        public void TestAllWork()
        {
            HisCentralSearcher.Setup(h => h.GetSeriesCatalogInRectangle(1, 1, 1, 1, It.IsAny<string[]>()
                                                                        , It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                                                                        It.IsAny<int[]>(),
                                                                        It.IsAny<BackgroundWorker>(),
                                                                        It.IsAny<DoWorkEventArgs>()));
            SearchCriteria worksCriteria = new SearchCriteria
                                               {
                                                   areaParameter =
                                                       new SearchCriteria.AreaRectangle { xMax = 1, xMin = 1, yMax = 1, yMin = 1 },
                                                   BoundinBoxSearch = true,
                                                   hisCentralURL = "http://example.com",
                                                   startDate = DateTime.Now.AddDays(-1),
                                                   endDate = DateTime.Now
                                               };

            worksCriteria.keywords.AddRange(keywords);
            worksCriteria.serviceIDs.AddRange(serviceIds);

            workEventsWorks = new DoWorkEventArgs(worksCriteria);

            _search = new BackgroundSearchWithFailover();
            _search.Searcher = HisCentralSearcher.Object;
            var urls = new List<string> { "http://example.com/1", "http://example.com/2" };
            _search.HISCentralSearchWithFailover(workEventsWorks,
               urls,
                backgroundWorker.Object);
        }

        [Test()]
        //   [ExpectedException()]
        public void TestAllFail()
        {
            // fails
            HisCentralSearcher.Setup(h => h.GetSeriesCatalogInRectangle(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(),
                It.IsAny<string[]>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<int[]>(),
                 It.IsAny<BackgroundWorker>(),
                It.IsAny<DoWorkEventArgs>())).Throws(new Exception("Failed Server"));
            SearchCriteria failsCriteria = new SearchCriteria
            {
                areaParameter =
                    new SearchCriteria.AreaRectangle { xMax = 2, xMin = 2, yMax = 2, yMin = 2 },
                BoundinBoxSearch = true,
                hisCentralURL = "http://example.com",
                startDate = DateTime.Now.AddDays(-1),
                endDate = DateTime.Now
            };
            failsCriteria.keywords.AddRange(keywords);
            failsCriteria.serviceIDs.AddRange(serviceIds);

            workEventsFails = new DoWorkEventArgs(failsCriteria);
            _search = new BackgroundSearchWithFailover();
            _search.Searcher = HisCentralSearcher.Object;
            Assert.Throws<HydrodesktopSearchException>(
                delegate
                {
                    _search.HISCentralSearchWithFailover(workEventsFails,
                         new List<string> { "http://example.com" },
                        backgroundWorker.Object);
                }
                    );
        */
            /* The method is void, so there is no way to set a
         * .Return(void).Callback(throw Exception)
         * Tried .Callback(c => {if (counter >0) { return new mock<>.setup().Throws() }
         * else {return new mock.setup() }
         * */
            //[Test()]
            //public void TestFailWork()
            //{
            //    int count = 0;
            //    // Fails second time;
            //    HisCentralSearcher.Setup(h => h.GetSeriesCatalogInRectangle(3, 3, 3, 3, It.IsAny<string[]>()
            //                                                           , It.IsAny<DateTime>(),
            //                                                           It.IsAny<DateTime>(),
            //                                                           It.IsAny<int[]>(),
            //                                                           It.IsAny<BackgroundWorker>(),
            //                                                           It.IsAny<DoWorkEventArgs>()));

            //    SearchCriteria failWorksCriteria = new SearchCriteria
            //    {
            //        areaParameter =
            //            new SearchCriteria.AreaRectangle { xMax = 3, xMin = 3, yMax = 3, yMin = 3 },
            //        BoundinBoxSearch = true,
            //        hisCentralURL = "http://example.com",
            //        startDate = DateTime.Now.AddDays(-1),
            //        endDate = DateTime.Now,
            //        keywords = keywords,
            //        serviceIDs = serviceIds
            //    };
            //    workEventsFailWork = new DoWorkEventArgs(failWorksCriteria);

            //    _search = new BackgroundSearchWithFailover();
            //    _search.Searcher = HisCentralSearcher.Object;

            //    var urls = new List<string> { "http://example.com/1", "http://example.com/2" };
            //    _search.HISCentralSearchWithFailover(workEventsFailWork,
            //       urls,
            //        backgroundWorker.Object);

            //}
        
    }
}
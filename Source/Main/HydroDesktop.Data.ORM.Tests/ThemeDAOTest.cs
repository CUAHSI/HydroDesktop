using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Iesi.Collections.Generic;
using NUnit.Framework;
using HydroDesktop.Database;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Tests.DataManagerTests
{
    [TestFixture]
    public class ThemeDAOTest


    {
        private RepositoryManager manager;
        [SetUp]
        private void setup ()
        {
              manager = TestConfig.RepositoryManager;
        }
        [TearDown]
        private void teardown()
        {
            
        }
        [Test]
        [Ignore("DB no longer has data")]
        public void TestListOfSeriesForTheme()
        {
           
            manager.BeginTransaction();
            Theme theme = manager.ThemeDAO.FindUnique("Id", 1);
            IList<Series> seriesList = theme.SeriesList;
            Assert.Greater(seriesList.Count, 0, "The series list should have at least 1 item");
            foreach (Series series in seriesList)
            {
                Assert.IsNotNull(series.Site, "Each series in the list should have a site object");
                Assert.Greater(series.ValueCount, 0, "The series should have some data values.");
            }
            manager.Commit();
        }

        [Test]
        public void TestGetAllThemes()
        {
          //  RepositoryManager manager = TestConfig.RepositoryManager;
            //manager.BeginTransaction();
            IList<Theme> themes = manager.GetAllThemes();
            foreach (Theme theme in themes)
            {
                IList<Series> seriesList = theme.SeriesList;
                Assert.Greater(seriesList.Count, 0, "The series list should have at least 1 item");
                foreach (Series series in seriesList)
                {
                    Assert.IsNotNull(series.Site, "Each series in the list should have a site object");
                  //  Assert.Greater(series.ValueCount, 0, "The series should have some data values.");
                }
            }
           // manager.Commit();
        }
    }
}
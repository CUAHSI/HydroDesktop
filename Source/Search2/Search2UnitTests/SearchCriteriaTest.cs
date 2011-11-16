using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Data;
using NUnit.Framework;
using HydroDesktop.Search;

namespace HydroDesktop.Search.SearchUnitTests
{
    public class SearchCriteriaTest
    {
        [Test()]
        public void CreateSearchCriteria()
        {
            var criteria = new SearchCriteria();
            Assert.That(criteria != null);
            // and that lists are not null
            Assert.That(criteria.keywords != null);
            Assert.That(criteria.serviceIDs != null);

        }

        [TestCase(1,1,1,1)]

        public void AreaCriteria(double xmin, double xmax, double ymin, double ymax)
        {
            var criteria = new SearchCriteria();
            Assert.That(criteria.BoundinBoxSearch == false);
            var area = new SearchCriteria.AreaRectangle
                           {
                               xMax = xmax,
                               xMin = xmin,
                               yMax = ymax,
                               yMin = ymin
                           };
            Assert.That(xmax == area.xMax);
            Assert.That(ymax == area.yMax);
            Assert.That(xmin == area.xMin);
            Assert.That(ymin == area.yMin);
            // now test that we get an area when added to search
            criteria.areaParameter = area;
            Assert.That(area == criteria.areaParameter);
            var ap = criteria.areaParameter;
            var type = ap.GetType();
            var b = type.Equals(typeof (SearchCriteria.AreaRectangle));
            var bb = criteria.BoundinBoxSearch;
            Assert.That(criteria.BoundinBoxSearch == true);

        }

        [Test()]
        public void FeatureCriteria()
        {
/*
            var criteria = new SearchCriteria();
            Assert.That(criteria.BoundinBoxSearch == false);
            var featureMock = new Moq.Mock<List<IFeature>>();
            var area = featureMock.Object;
           
            // now test that we get an area when added to search
            criteria.areaParameter = area;
            Assert.That(area == criteria.areaParameter);
            var ap = criteria.areaParameter;
            var type = ap.GetType();
            var b = type.Equals(typeof(List<IFeature>));
            var bb = criteria.BoundinBoxSearch;
            Assert.That(criteria.BoundinBoxSearch == false);
*/
        }
    }
}

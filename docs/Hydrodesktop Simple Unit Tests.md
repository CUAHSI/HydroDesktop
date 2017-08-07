In Search, we have created a SearchCriteria.cs class that contains the list of criteria.
{{  public class SearchCriteria
    {
        private List<String> _keywords = new List<string>();
        private List<int> _serviceIDs = new List<int>();
        private Boolean _boundingBoxSearch = false;

        private object _areaSearch;

        public List<string> keywords { get { return _keywords; } }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public List<int> serviceIDs { get { return _serviceIDs; } }
        public string hisCentralURL { get; set; }
        public object areaParameter
        {
            get { return _areaSearch; }
            set
            {
           
            if (value.GetType().Equals(typeof(SearchCriteria.AreaRectangle)))
                {
                    _boundingBoxSearch = true;
                }
                else
                {
                    _boundingBoxSearch = false;
                    
                }
                _areaSearch = value;
            }
        } // will be a List<IFeature> or ArrayList rectangleCoordinates
        public bool BoundinBoxSearch
        {
            get { return _boundingBoxSearch; }
            set { _boundingBoxSearch = value; }
        }

        public class AreaRectangle
        {
            public double xMin { get; set; }
            public double xMax { get; set; }
            public double yMin { get; set; }
            public double yMax { get; set; }
        }
    }}}

Originally, this class was an ArrayList of various parameters. This made it easy to initially create, but difficult to use because every method that received the parameter ArrayList had to know the order of the objects.
{{Need code example pull from svn}}

We would like to test the object
# Create a new project
# Add references to the project/dll being tested, Binaries/numit.framework.dll, and Binaries/Moq.dll
# add a new class
# * add usings
{{ using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using HydroDesktop.Search; // the project
}}
1 and 2 are already completed.

Now, we want to test that when when we set or get an object that that is done correctly. for the Default getters and setters, this will work (ideally we would set tests for everyone incase someone changes them from the default
First we test that the we can create a SearchCriteria object
{{    public class SearchCriteriaTest
    {
        [Test()](Test())
        public void CreateSearchCriteria()
        {
            var criteria = new SearchCriteria();
            Assert.That(criteria != null);
            // and that lists are not null
            Assert.That(criteria.keywords != null);
            Assert.That(criteria.serviceIDs != null);

        }
}
}}
On a note, the 
{{      Assert.That(criteria.keywords != null);
            Assert.That(criteria.serviceIDs != null);
}} were added after a bug was discovered because these were not initialized.

But for this case we want to test that when we add a areaRectangle, that the bounding box is set to true.
{{
       [TestCase(1,1,1,1)](TestCase(1,1,1,1))

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
}}

If we were to fully test this, we would like to know that when we don't add an AreaRectangle that that it is not a bounding box.
If this were running in the application, the SearchCritieria.area would be an List<IFeature>. We will use a concept called "Mock Objects". These are objects that can be configured to look like an object, in this case List<IFeature>, and can be configured to provide controlled responses. We have decided to use [Moq](http://code.google.com/p/moq/).

In orde to setup a mock object in MOQ we setup a Mock using:
{{
Mock featureMock = new Moq.Mock<List<IFeature>>();
List<IFeature> area = featureMock.Object;
}}
Using features of .net 3.5 we can simplify this, and let the .net determine the object types
{{
var featureMock = new Moq.Mock<List<IFeature>>();
var area = featureMock.Object;
}}



{{
 [Test()](Test())
        public void FeatureCriteria()
        {

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

        }
}}

The above is a simple case. But you can see that if a user wishes to add a different type of area object, you will be able to insure that the code will work as expected.

using System;
using System.Collections.Generic;

namespace HydroDesktop.Search
{
    public class SearchCriteria
    {
        private List<String> _keywords = new List<string>();
        private List<int> _serviceIDs = new List<int>();
        private string _searchMethod = Properties.Settings.Default.SearchMethod_HISCentral;
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

        public SearchMode SearchMethod { get; set; }

        public class AreaRectangle
        {
            public double xMin { get; set; }
            public double xMax { get; set; }
            public double yMin { get; set; }
            public double yMax { get; set; }
        }
    }
}

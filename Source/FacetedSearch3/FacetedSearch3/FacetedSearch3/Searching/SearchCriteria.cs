using System;
using System.Collections.Generic;
using FacetedSearch3.Settings;

namespace FacetedSearch3.Searching
{
    public class SearchCriteria
    {
        //todo: Copied from Search2. Need to be refactored.

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
           
            if (value.GetType().Equals(typeof(AreaRectangle)))
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

        public TypeOfCatalog SearchMethod { get; set; }

    }
}

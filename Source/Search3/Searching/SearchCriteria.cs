using System;
using System.Collections.Generic;
using HydroDesktop.WebServices;
using Search3.Settings;

namespace Search3.Searching
{
    public class SearchCriteria
    {
        //todo: Copied from Search2. Need to be refactored.
        
        private Boolean _boundingBoxSearch = false;
        private object _areaSearch;

        
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

        public readonly List<string> Keywords = new List<string>();
        public readonly List<WebServiceNode> WebServices = new List<WebServiceNode>();

        public string hisCentralURL { get; set; }
        public object areaParameter
        {
            get { return _areaSearch; }
            set
            {
           
            if (value.GetType().Equals(typeof(Box)))
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

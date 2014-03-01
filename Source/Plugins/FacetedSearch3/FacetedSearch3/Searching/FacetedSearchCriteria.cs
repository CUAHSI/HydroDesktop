using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FacetedSearch3.Searching
{
    /// <summary>
    /// Subset of SearchCriteria.cs object sufficient for Faceted Search demo
    /// todo: Integrate further with HydroDesktop services that require this information
    /// </summary>
    public class FacetedSearchCriteria
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        private Boolean _boundingBoxSearch = false;
        private object _areaSearch;

        public object areaParameter
        {
            get { return _areaSearch; }
            set
            {

                if (value.GetType().Equals(typeof(FacetedSearch3.Settings.AreaRectangle)))
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
    }
}

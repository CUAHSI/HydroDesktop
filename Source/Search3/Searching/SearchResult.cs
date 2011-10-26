using System;
using System.Collections.Generic;
using System.Linq;
using DotSpatial.Data;
using HydroDesktop.Interfaces.ObjectModel;

namespace Search3.Searching
{
    public class SearchResult
    {
        private readonly IEnumerable<SearchResultItem> _featuresPerDataSource;

        public SearchResult(IEnumerable<SearchResultItem> featuresPerDataSource)
        {
            if (featuresPerDataSource == null) throw new ArgumentNullException("featuresPerDataSource");
            _featuresPerDataSource = featuresPerDataSource;
        }

        /// <summary>
        /// Dictionary of features per DataSources
        /// </summary>
        public IEnumerable<SearchResultItem> ResultItems
        {
            get { return _featuresPerDataSource; }
        }

        public bool IsEmpty()
        {
            return ResultItems.All(item => item.FeatureSet.Features.Count <= 0);
        }
    }

    public class SearchResultItem
    {
        public SeriesDataCart SeriesDataCart { get; private set; }
        public IFeatureSet FeatureSet { get; private set; }

        public SearchResultItem(SeriesDataCart seriesDataCart, IFeatureSet featureSet)
        {
            if (seriesDataCart == null) throw new ArgumentNullException("seriesDataCart");
            if (featureSet == null) throw new ArgumentNullException("featureSet");

            SeriesDataCart = seriesDataCart;
            FeatureSet = featureSet;
        }
    }
}

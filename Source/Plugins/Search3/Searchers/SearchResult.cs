using System;
using System.Collections.Generic;
using System.Linq;
using DotSpatial.Data;

namespace Search3.Searchers
{
    public class SearchResult
    {
        private readonly Dictionary<string, IFeatureSet> _featuresPerDataSource;

        public SearchResult(Dictionary<string, IFeatureSet> featuresPerDataSource)
        {
            if (featuresPerDataSource == null) throw new ArgumentNullException("featuresPerDataSource");
            _featuresPerDataSource = featuresPerDataSource;
        }

        /// <summary>
        /// Dictionary of features per DataSources
        /// </summary>
        public Dictionary<string, IFeatureSet> Features
        {
            get { return _featuresPerDataSource; }
        }

        public bool IsEmpty()
        {
            return Features.All(item => item.Value.Features.Count <= 0);
        }
    }
}

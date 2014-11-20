using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using DotSpatial.Data;

namespace HydroDesktop.Plugins.Search.Searching
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
        public IFeatureSet FeatureSet { get; private set; }
        public string ServiceCode { get; private set; }

        public SearchResultItem(string serviceCode, IFeatureSet featureSet)
        {
            if (serviceCode == null) throw new ArgumentNullException("serviceCode");
            if (featureSet == null) throw new ArgumentNullException("featureSet");
            Contract.EndContractBlock();

            ServiceCode = serviceCode;
            FeatureSet = featureSet;
        }
    }
}

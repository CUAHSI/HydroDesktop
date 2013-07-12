namespace HydroDesktop.Common
{
    /// <summary>
    /// Contains some shared constants to use them in plug-ins
    /// </summary>
    public static class SharedConstants
    {
        /// <summary>
        /// Root Key of Metadata ribbon
        /// </summary>
        public static string MetadataRootKey
        {
            get { return "kHydroMetadata"; }
        }

        /// <summary>
        /// Root Key of Search Ribbon
        /// </summary>
        public static string SearchRootkey
        {
            get { return "kHydroSearchV3"; }
        }

        /// <summary>
        /// Root Key of HydroShare Ribbon
        /// </summary>
        public static string HydroShareRootkey
        {
            get { return "kHydroShare"; }
        }

        /// <summary>
        /// Root Key of Table Ribbon
        /// </summary>
        public static string TableRootKey
        {
            get { return "kHydroTable"; }
        }

        /// <summary>
        /// Key of SeriesSelector dock panel
        /// </summary>
        public static string SeriesViewKey
        {
            get { return "kHydroSeriesView"; }
        }

        /// <summary>
        /// Name of Data Sources group in search ribbon
        /// </summary>
        public static string SearchDataSourcesGroupName
        {
            get { return "Data Sources"; }
        }
    }
}


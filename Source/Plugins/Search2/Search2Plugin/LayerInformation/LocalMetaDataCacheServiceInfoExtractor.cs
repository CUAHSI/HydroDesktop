namespace HydroDesktop.Search.LayerInformation
{
    class LocalMetaDataCacheServiceInfoExtractor : IServiceInfoExtractor
    {
        private readonly MetadataCacheSearcher metaDataSearcher = new MetadataCacheSearcher();

        public string GetServiceDesciptionUrlByServiceUrl(string serviceUrl)
        {
            var webService = metaDataSearcher.GetWebServiceByServiceURL(serviceUrl);
            if (webService != null)
            {
                return  webService.DescriptionURL;
            }

            return null;
        }
    }
}
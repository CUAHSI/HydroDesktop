namespace HydroDesktop.Search.LayerInformation
{
    class LocalInfoExtractor : IServiceInfoExtractor
    {
        private readonly MetadataCacheSearcher metaDataSearcher = new MetadataCacheSearcher();

        public string GetServiceDesciptionUrl(string serviceUrl)
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
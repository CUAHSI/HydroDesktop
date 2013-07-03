using System;
using HydroDesktop.Data.ODMCVServiceClient.Infrastructure.SOAP;

namespace HydroDesktop.Data.ODMCVServiceClient
{
    public class ODMCVServiceClientFactory : IODMCVServiceClientFactory
    {
        #region Singletone implementation

        private ODMCVServiceClientFactory()
        {
            
        }
        private static  readonly Lazy<ODMCVServiceClientFactory> _instance = new Lazy<ODMCVServiceClientFactory>(() => new ODMCVServiceClientFactory(), true);
        public static IODMCVServiceClientFactory Instance
        {
            get { return _instance.Value; }
        }

        #endregion

        public IODMCVServiceClient GetODMCVServiceClient()
        {
            return ODMCVSoapServiceClient.CreateNew();
        }
    }
}
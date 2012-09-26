using System.Configuration;
using HydroDesktop.Common.Logging;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace HydroDesktop.Common
{
    /// <summary>
    /// DI container accesor
    /// </summary>
    internal static class Container
    {
        #region Properties

        static IUnityContainer _currentContainer;
        private static IServiceLocator _serviceLocator;

        /// <summary>
        /// Get the current configured container
        /// </summary>
        /// <returns>Configured container</returns>
        public static IUnityContainer Current
        {
            get
            {
                return _currentContainer;
            }
        }

        #endregion

        #region Constructor

        static Container()
        {
            ConfigureContainer();
            ConfigureFactories();
        }

        #endregion

        #region Methods

        static void ConfigureContainer()
        {
            /*
             * Add here the code configuration or the call to configure the container 
             * using the application configuration file
             */
            _currentContainer = new UnityContainer();
            var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            if (section != null)
            {
                section.Configure(_currentContainer);
            }
        }


        static void ConfigureFactories()
        {
            // Configure services here
            _currentContainer.RegisterType<ILog, TraceLogger>(new ContainerControlledLifetimeManager());
            
            _serviceLocator = new UnityServiceLocator(_currentContainer);
            ServiceLocator.SetLocatorProvider(() => _serviceLocator);
        }

        #endregion
    }
}
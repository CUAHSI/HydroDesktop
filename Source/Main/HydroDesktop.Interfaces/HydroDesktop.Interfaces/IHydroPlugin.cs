using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls;

namespace HydroDesktop.Interfaces
{
    public interface IHydroPlugin : IExtension
    {
        #region Methods

        /// <summary>
        /// Gives all the information about the HydroDesktop application that 
        /// the plugin manager has.
        /// Since the availability of all of these aspects will vary based on
        /// what the developer has linked with his plugin manager as well as what
        /// controls are actually available on his project, it is possible for
        /// any of these items to be null.
        /// </summary>
        void Initialize(IHydroPluginArgs args);

        #endregion
    }
}

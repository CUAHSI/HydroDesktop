using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// This is the manager for HydroDesktop plugins to interact
    /// with the series view and series selector controls
    /// </summary>
    public interface IHydroAppManager
    {
        /// <summary>
        /// Gets or sets the SeriesView component
        /// </summary>
        ISeriesView SeriesView { get; set; }
    }
}

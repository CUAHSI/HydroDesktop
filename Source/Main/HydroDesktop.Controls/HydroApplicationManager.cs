using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Data;
using DotSpatial.Controls;
using DotSpatial.Controls.RibbonControls;
using System.ComponentModel;
using System.Windows.Forms;
using System.ComponentModel.Design;
using HydroDesktop.Configuration;
using HydroDesktop.Interfaces;

namespace HydroDesktop.Controls
{
    /// <summary>
    /// This is the ApplicationManager class for HydroDesktop. It is designed to support
    /// all types of plugins: Regular DotSpatial plugins as well as HydroPlugins.
    /// </summary>
    public class HydroAppManager : AppManager, IHydroAppManager
    {
        /// <summary>
        /// Gets or sets the SeriesView component
        /// </summary>
        public ISeriesView SeriesView { get; set; }
    }
}

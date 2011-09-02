using System;
using System.Collections.Generic;
using System.Text;
using DotSpatial.Controls;
using DotSpatial.Controls.RibbonControls;
using System.Windows.Forms;
using DotSpatial.Data;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// This interface will enable the HydroDesktop plug-ins to interact
    /// directly with the database
    /// </summary>
    public interface IHydroPluginArgs
    {
        #region Properties

        /// <summary>
        /// Gets the Map associated with the plugin manager
        /// </summary>
        IMap Map
        {
            get;
        }

        /// <summary>
        /// Gets the Legend (Table of Contents) associated with the plugin manager
        /// </summary>
        ILegend Legend
        {
            get;
        }

        /// <summary>
        /// Gets the Main Menu, if any, associated with the plugin manager
        /// </summary>
        MenuStrip MainMenu
        {
            get;
        }

        /// <summary>
        /// Gets the ToolStrip if any associated with the plugin manager,
        /// if any.
        /// </summary>
        ToolStrip MainToolStrip
        {
            get;
        }

        /// <summary>
        /// Gets the progress handler that is being used to display status messages.
        /// </summary>
        IProgressHandler ProgressHandler
        {
            get;
        }

        /// <summary>
        /// Gets the list of plugins
        /// </summary>
        List<IMapPlugin> MapPlugins
        {
            get;
        }

        List<IHydroPlugin> HydroPlugins
        {
            get;
        }

        /// <summary>
        /// Gets the ribbon control
        /// </summary>
        DotSpatial.Controls.RibbonControls.Ribbon Ribbon
        {
            get;
        }

        Form MainForm
        {
            get;
        }

        /// <summary>
        /// Gets the actual container for the tool strips
        /// </summary>
        ToolStripContainer ToolStripContainer
        {
            get;
        }

        /// <summary>
        /// Gets the actual panel manager for adding tabs and panels
        /// </summary>
        ITabManager PanelManager
        {
            get;
        }

        /// <summary>
        /// Gets the specialized SerivesView view panel used by the time series plugins
        /// </summary>
        ISeriesView SeriesView
        {
            get;
        }

        /// <summary>
        /// The DataRepository database access tools
        /// </summary>
        IHydroDatabase Database { get; }
        

        #endregion
    }
}

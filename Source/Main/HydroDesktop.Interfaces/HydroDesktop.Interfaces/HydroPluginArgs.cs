using System;
using System.Collections.Generic;
using System.Text;
using DotSpatial.Controls;
using DotSpatial.Data;
using System.Windows.Forms;
using DotSpatial.Controls.RibbonControls;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// This interface will enable the HydroDesktop plug-ins to interact
    /// directly with the database
    /// </summary>
    public class HydroPluginArgs : IHydroPluginArgs
    {
        #region Private Variables

        private readonly IMap _map;
        private readonly ILegend _legend;
        private readonly IProgressHandler _progressHandler;
        private readonly MenuStrip _mainMenu;
        private readonly ToolStrip _mainToolStrip;
        private readonly List<IMapPlugin> _mapPlugins;
        private readonly List<IHydroPlugin> _hydroPlugins;
        private readonly ToolStripContainer _toolStripContainer;
        private readonly ITabManager _tabManager;
        private readonly Ribbon _ribbon;
        private readonly IHydroDatabase _database;
        private readonly Form _mainForm;
        private readonly ISeriesView _seriesView;

        #endregion
        

        #region Constructors

        /// <summary>
        /// Creates a new instance of HydroPluginArgs
        /// </summary>
        public HydroPluginArgs()
        {

        }
        
        /// <summary>
        /// Creates a new instance of the HydroPluginArgs
        /// </summary>
        /// <param name="map">Each Manager is associated with a single map</param>
        /// <param name="legend">The legend</param>
        /// <param name="mainMenu">The main menu</param>
        /// <param name="mainToolStrip">The main toolstrip</param>
        /// <param name="progressHandler">The progress handler</param>
        /// <param name="plugins">The list of map-plugins controlled by the manager</param>
        /// <param name="hydroPlugins">The list of hydro-plugins</param>
        /// <param name="toolStripContainer">The container where any toolstrips should be added</param>
        /// <param name="tabManager">The panel manager for adding tabs and panels</param>
        /// <param name="ribbon">Gets the ribbon control</param>
        /// <param name="hydroDatabase">Gets the HydroDesktop Database access tools</param>
        /// <param name="seriesView"?
        /// 
        //(_map, _legend, _mainMenu, _toolStrip, _progressHandler, _geoPlugins, _hydroPlugins, _toolStripContainer, _panelManager, _ribbon, _mainForm, _hydroDatabase, _seriesView)
        public HydroPluginArgs(IMap map, ILegend legend, MenuStrip mainMenu, ToolStrip mainToolStrip, IProgressHandler progressHandler, List<IMapPlugin> mapPlugins, List<IHydroPlugin> hydroPlugins, ToolStripContainer toolStripContainer, ITabManager tabManager, Ribbon ribbon, Form mainForm, IHydroDatabase hydroDatabase, ISeriesView seriesView )
        {
            _ribbon = ribbon;           
            _map = map;
            _legend = legend;
            _mainMenu = mainMenu;
            _mainToolStrip = mainToolStrip;
            _mapPlugins = mapPlugins;
            _hydroPlugins = hydroPlugins;
            _toolStripContainer = toolStripContainer;
            _progressHandler = progressHandler;
            _tabManager = tabManager;
            _database = hydroDatabase;
            _mainForm = mainForm;
            _seriesView = seriesView;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Map associated with the plugin manager
        /// </summary>
        public IMap Map
        {
            get { return _map; }
        }

        /// <summary>
        /// Gets the Legend (Table of Contents) associated with the plugin manager
        /// </summary>
        public ILegend Legend
        {
            get { return _legend; }
        }

        /// <summary>
        /// Gets the Main Menu, if any, associated with the plugin manager
        /// </summary>
        public MenuStrip MainMenu
        {
            get { return _mainMenu; }
        }

        /// <summary>
        /// Gets the ToolStrip if any associated with the plugin manager,
        /// if any.
        /// </summary>
        public ToolStrip MainToolStrip
        {
            get { return _mainToolStrip; }
        }

        /// <summary>
        /// Gets the progress handler that is being used to display status messages.
        /// </summary>
        public IProgressHandler ProgressHandler
        {
            get { return _progressHandler; }
        }

        /// <summary>
        /// Gets the list of plugins
        /// </summary>
        public List<IMapPlugin> MapPlugins
        {
            get { return _mapPlugins; }
        }

        public List<IHydroPlugin> HydroPlugins
        {
            get { return _hydroPlugins; }
        }

        /// <summary>
        /// Gets the ribbon control
        /// </summary>
        public Ribbon Ribbon
        {
            get { return _ribbon; }
        }

        /// <summary>
        /// Gets the actual container for the tool strips
        /// </summary>
        public ToolStripContainer ToolStripContainer
        {
            get { return _toolStripContainer; }
        }

        /// <inheritdoc />
        public ITabManager PanelManager
        {
            get { return _tabManager; }
        }

        /// <summary>
        /// Gets the main form of the application
        /// </summary>
        public Form MainForm
        {
            get { return _mainForm; }
        }

        /// <summary>
        /// The DataRepository database access tools
        /// </summary>
        public IHydroDatabase Database
        {
            get { return _database; }
        }

        /// <summary>
        /// The Series View main display panel used by the time series plug-ins
        /// </summary>
        public ISeriesView SeriesView
        {
            get { return _seriesView; }
        }

        #endregion
    }
}

using System.Drawing;
using DotSpatial.Controls.Docking;
using WeifenLuo.WinFormsUI.Docking;

namespace HydroDesktop.Plugins.Docking
{
    class DockPanelInfo
    {
        private static int _dockPanelNumber;

        /// <summary>
        /// The Weifen Luo dockable panel (Dock Content)
        /// </summary>
        public DockContent WeifenLuoDockPanel { get; private set; }

        /// <summary>
        /// The DotSpatial dockable panel (used by DS plugin interface)
        /// </summary>
        public DockablePanel DotSpatialDockPanel { get; private set; }

        /// <summary>
        /// The sort order of the dockable panel
        /// </summary>
        public int SortOrder { get; private set; }

        /// <summary>
        /// Unique number of dockpanel (each created DockPanel has new number).
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// Creates a new instance of DockPanelInfo
        /// </summary>
        /// <param name="dotSpatialDockPanel">the DotSpatial DockPanel virtual object</param>
        /// <param name="weifenLuoDockPanel">The physical instance of the dock panel (a weifen luo dock contents)</param>
        /// <param name="sortOrder">the sort order</param>
        public DockPanelInfo(DockablePanel dotSpatialDockPanel, DockContent weifenLuoDockPanel, int sortOrder)
        {
            DotSpatialDockPanel = dotSpatialDockPanel;
            WeifenLuoDockPanel = weifenLuoDockPanel;
            SortOrder = sortOrder;
            Number = _dockPanelNumber++;
        }

        public void SaveSnapshot()
        {
            var panel = WeifenLuoDockPanel;
            Snapshot = new DockPanelSnapshot
                {
                    DockState = panel.DockState,
                    Size = DotSpatialDockPanel.InnerControl.Size,
                    DSPanel = DotSpatialDockPanel,
                };
        }

        public DockPanelSnapshot Snapshot { get; internal set; }
    }

    class DockPanelSnapshot
    {
        public DockState DockState { get; set; }
        public Size Size { get; set; }
        public DockablePanel DSPanel { get; set; }
    }
}

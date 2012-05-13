using DotSpatial.Controls.Docking;
using WeifenLuo.WinFormsUI.Docking;

namespace HydroDesktop.Docking
{
    public class DockPanelInfo
    {
        /// <summary>
        /// The Weifen Luo dockable panel (Dock Content)
        /// </summary>
        public DockContent WeifenLuoDockPanel { get; set; }
        /// <summary>
        /// The DotSpatial dockable panel (used by DS plugin interface)
        /// </summary>
        public DockablePanel DotSpatialDockPanel { get; set; }

        /// <summary>
        /// The sort order of the dockable panel
        /// </summary>
        public int SortOrder { get; set; }

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
        }
    }
}

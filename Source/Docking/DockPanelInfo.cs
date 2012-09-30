using System.Drawing;
using System.Windows.Forms;
using DotSpatial.Controls.Docking;
using WeifenLuo.WinFormsUI.Docking;

namespace HydroDesktop.Docking
{
    class DockPanelInfo
    {
        private readonly DockStyle _originalDockStyle;
        private readonly Size _originalSize;
        private DockPanelSnapshot _snapshot;
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
        /// <param name="originalDockStyle">Original DockStyle of panel.</param>
        /// <param name="originalSize">Original size of inner control.</param>
        public DockPanelInfo(DockablePanel dotSpatialDockPanel, DockContent weifenLuoDockPanel, int sortOrder,
                             DockStyle originalDockStyle, Size originalSize)
        {
            DotSpatialDockPanel = dotSpatialDockPanel;
            WeifenLuoDockPanel = weifenLuoDockPanel;
            SortOrder = sortOrder;
            Number = _dockPanelNumber++;
            _originalDockStyle = originalDockStyle;
            _originalSize = originalSize;
        }

        public void SaveSnapshot()
        {
            var panel = WeifenLuoDockPanel;
            _snapshot = new DockPanelSnapshot
                {
                    DockState = panel.DockState,
                    Size = _originalSize,
                    DockStyle = _originalDockStyle
                };
        }

        public DockPanelSnapshot GetSnapshot()
        {
            return _snapshot;
        }
    }

    class DockPanelSnapshot
    {
        public DockStyle DockStyle { get; set; }
        public DockState DockState { get; set; }
        public Size Size { get; set; }
    }
}

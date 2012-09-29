using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows.Forms;
using DotSpatial.Controls.Docking;
using HydroDesktop.Common;
using WeifenLuo.WinFormsUI.Docking;

namespace HydroDesktop.Docking
{
    /// <summary>
    /// The DockManager implementation for HydroDesktop
    /// </summary>
    public class HydroDockManager : IDockManager, IPartImportsSatisfiedNotification
    {
        #region Fields 

        /// <summary>
        /// The main dock container panel
        /// </summary>
        private DockPanel MainDockPanel { get; set; }

        [Import("Shell")]
        private ContainerControl Shell { get; set; }

        // The lookup list of dock panels (for keeping track of existing panels)
        private readonly Dictionary<string, DockPanelInfo> dockPanelLookup = new Dictionary<string, DockPanelInfo>();

        #endregion

        #region IDockManager Members

        /// <summary>
        /// Occurs when the active panel is changed, meaning a difference panel is activated.
        /// </summary>
        public event EventHandler<DockablePanelEventArgs> ActivePanelChanged;

        /// <summary>
        /// Occurs after a panel is added.
        /// </summary>
        public event EventHandler<DockablePanelEventArgs> PanelAdded;

        /// <summary>
        /// Occurs after a panel is removed.
        /// </summary>
        public event EventHandler<DockablePanelEventArgs> PanelRemoved;

        /// <summary>
        /// Occurs when a panel is closed, which means the panel can still be activated or removed.
        /// </summary>
        public event EventHandler<DockablePanelEventArgs> PanelClosed;


        public void ResetLayout()
        {
            //check the map
            DockContent mapContent = dockPanelLookup["kMap"].WeifenLuoDockPanel;
            if (mapContent.IsFloat)
            {
                mapContent.Dock = DockStyle.Fill;
                mapContent.DockState = DockState.Document;
                mapContent.PanelPane = MainDockPanel.ActiveDocumentPane;
            }


            //first, check the list
            foreach (string key in dockPanelLookup.Keys)
            {
                if (key == SharedConstants.SeriesViewKey)
                {
                    DockContent cnt = dockPanelLookup[key].WeifenLuoDockPanel;

                    cnt.Dock = DockStyle.Left;
                    cnt.DockState = DockState.DockLeft;
                    cnt.PanelPane = dockPanelLookup["kLegend"].WeifenLuoDockPanel.Pane;

                    if (cnt.IsHidden)
                    {
                        cnt.Show();
                    }
                }
                else if (key != "kMap" && key != "kLegend")
                {
                    DockContent cnt = dockPanelLookup[key].WeifenLuoDockPanel;

                    cnt.Dock = DockStyle.Fill;
                    cnt.DockState = DockState.Document;
                    cnt.PanelPane = dockPanelLookup["kMap"].WeifenLuoDockPanel.Pane;

                    if (cnt.IsHidden)
                    {
                        cnt.Show();
                    }
                }
            }
        }

        /// <summary>
        /// Add a dockable panel
        /// </summary>
        /// <param name="panel">The dockable panel</param>
        public void Add(DockablePanel panel)
        {
            string key = panel.Key;
            if (dockPanelLookup.ContainsKey(key))
            {
                throw new ArgumentOutOfRangeException("panel", string.Format("Unable to add panel with Key: {0}, because it already added.", key));
            }

            string caption = panel.Caption;
            var innerControl = panel.InnerControl;
            var dockStyle = panel.Dock;
            var zOrder = panel.DefaultSortOrder;

            Image img = null;
            if (panel.SmallImage != null) img = panel.SmallImage;

            //set dock style of the inner control to Fill
            innerControl.Dock = DockStyle.Fill;

            // make an attempt to start the pane off at the right width.
            if (dockStyle == DockStyle.Right)
                MainDockPanel.DockRightPortion = (double)innerControl.Width / MainDockPanel.Width;

            if (dockStyle == DockStyle.Left)
                MainDockPanel.DockLeftPortion = (double)innerControl.Width / MainDockPanel.Width;

            if (dockStyle == DockStyle.Top)
                MainDockPanel.DockTopPortion = (double)innerControl.Height / MainDockPanel.Height;

            if (dockStyle == DockStyle.Bottom)
                MainDockPanel.DockBottomPortion = (double)innerControl.Height / MainDockPanel.Height;

            //setting document tab strip location to 'bottom'
            if (dockStyle == DockStyle.Fill)
            {
                MainDockPanel.DocumentTabStripLocation = DocumentTabStripLocation.Bottom;
                MainDockPanel.DocumentStyle = DocumentStyle.DockingWindow;
            }

            var content = new DockContent { ShowHint = ConvertToDockState(dockStyle) };

            content.Controls.Add(innerControl);

            content.Text = caption;
            content.TabText = caption;
            content.Tag = key;
            innerControl.Tag = key;

            content.HideOnClose = true;

            if (img != null)
            {
                content.Icon = ImageToIcon(img);
            }

            content.Show(MainDockPanel);

            //event handler for closing
            content.FormClosing += content_FormClosing;
            content.FormClosed += content_FormClosed;

            //the tag is used by the ActivePanelChanged event
            content.Pane.Tag = key;

            //add panel to contents dictionary
            dockPanelLookup.Add(key, new DockPanelInfo(panel, content, zOrder));

            //trigger the panel added event
            OnPanelAdded(key);

            //set the correct sort order
            if (content.Pane.Contents.Count > 1)
            {
                int sortingIndex = ConvertSortOrderToIndex(content, zOrder);
                content.Pane.SetContentIndex(content, sortingIndex);
            }

            //caption - changed
            panel.PropertyChanged += panel_PropertyChanged;
        }

        /// <summary>
        /// Completely removes a dockable panel
        /// </summary>
        /// <param name="key">Unique key of the panel</param>
        public void Remove(string key)
        {
            DockPanelInfo dockInfo;
            if (!dockPanelLookup.TryGetValue(key, out dockInfo)) return;

            var content = dockInfo.WeifenLuoDockPanel;
            content.Close();

            //remove event handlers
            content.FormClosing -= content_FormClosing;
            content.FormClosed -= content_FormClosed;
            dockInfo.DotSpatialDockPanel.PropertyChanged -= panel_PropertyChanged;

            content.Dispose();
            dockPanelLookup.Remove(key);
            OnPanelRemoved(key);
        }

        /// <summary>
        /// Selects a dockable panel (the panel gains focus)
        /// if the panel is hidden, make it visible at its original
        /// location
        /// </summary>
        /// <param name="key">The unique key of the dockable panel to select</param>
        public void SelectPanel(string key)
        {
            DockPanelInfo info;
            if (dockPanelLookup.TryGetValue(key, out info))
            {
                info.WeifenLuoDockPanel.Activate();
            }
        }

        /// <summary>
        /// Hides the Dockable panel (panel is identified by key)
        /// </summary>
        /// <param name="key">the unique key of the dockable panel</param>
        public void HidePanel(string key)
        {
            DockPanelInfo info;
            if (dockPanelLookup.TryGetValue(key, out info))
            {
                info.WeifenLuoDockPanel.IsHidden = true;
            }
        }

        #endregion

        #region IPartImportsSatisfiedNotification Members

        /// <summary>
        /// setup the parent form. This 
        /// occurs when the main form becomes available
        /// </summary>
        public void OnImportsSatisfied()
        {
            MainDockPanel = new DockPanel {Parent = Shell, Dock = DockStyle.Fill};
            MainDockPanel.BringToFront();
            MainDockPanel.DocumentStyle = DocumentStyle.DockingSdi;

            //setup the events
            MainDockPanel.ActiveDocumentChanged += MainDockPanel_ActiveDocumentChanged;

        }

        #endregion

        #region Private methods

        //when the dockable panel property is changed
        void panel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Caption")
            {
                var dp = sender as DockablePanel;
                if (dp != null)
                {
                    var dc = dockPanelLookup[dp.Key].WeifenLuoDockPanel;
                    dc.Text = dp.Caption;
                    dc.TabText = dp.Caption;
                }
            }
        }

        void content_FormClosed(object sender, FormClosedEventArgs e)
        {
            var c = sender as DockContent;
            if (c != null)
            {
                OnPanelClosed(c.Tag.ToString());
           } 
        }

        void content_FormClosing(object sender, FormClosingEventArgs e)
        {
            var c = sender as DockContent;
            if (c != null)
            {
                OnPanelClosed(c.Tag.ToString());
            }
        }

        private static Icon ImageToIcon(Image img)
        {
            var bm = img as Bitmap;
            if (bm != null)
            {
                return Icon.FromHandle(bm.GetHicon());
            }
            return null;
        }

        private static DockState ConvertToDockState(DockStyle dockStyle)
        {
            switch (dockStyle)
            {
                case DockStyle.Bottom:
                    return DockState.DockBottom;
                case DockStyle.Fill:
                    return DockState.Document;
                case DockStyle.Left:
                    return DockState.DockLeft;
                case DockStyle.None:
                    return DockState.Float;
                case DockStyle.Right:
                    return DockState.DockRight;
                case DockStyle.Top:
                    return DockState.DockTop;

                default:
                    throw new NotSupportedException(dockStyle + " not suppored");
            }
        }

        /// <summary>
        /// Raises the ActivePanelChanged event
        /// </summary>
        void MainDockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            if (MainDockPanel.ActiveContent == null ||
                MainDockPanel.ActiveContent.DockHandler == null) return;

            var activeContent = MainDockPanel.ActiveContent.DockHandler.Content as DockContent;          
            if (activeContent == null) return;
            if (activeContent.Tag == null) return;

            var activePanelKey = activeContent.Tag.ToString();
            OnActivePanelChanged(activePanelKey);
        }

        private void OnPanelClosed(string panelKey)
        {
            var handler = PanelClosed;
            if (handler != null)
            {
                handler(this, new DockablePanelEventArgs(panelKey));
            }
        }

        private void OnPanelAdded(string panelKey)
        {
            var handler = PanelAdded;
            if (handler != null)
            {
                handler(this, new DockablePanelEventArgs(panelKey));
            }
        }

        private void OnPanelRemoved(string panelKey)
        {
            var handler = PanelRemoved;
            if (handler != null)
            {
                handler(this, new DockablePanelEventArgs(panelKey));
            }
        }

        private void OnActivePanelChanged(string newActivePanelKey)
        {
            var handler = ActivePanelChanged;
            if (handler != null)
            {
                handler(this, new DockablePanelEventArgs(newActivePanelKey));
            }
        }

        int ConvertSortOrderToIndex(DockContent content, int sortOrder)
        {
            var pane = content.Pane;
            var sortOrderList = new List<int>();

            foreach (DockContent existingContent in pane.Contents)
            {
                var key = existingContent.Tag.ToString();
                DockPanelInfo info;
                if (dockPanelLookup.TryGetValue(key, out info))
                {
                    sortOrderList.Add(info.SortOrder);
                }
            }
            sortOrderList.Sort();
            var index = sortOrderList.IndexOf(sortOrder);
            return index;
        }

        #endregion
    }
}

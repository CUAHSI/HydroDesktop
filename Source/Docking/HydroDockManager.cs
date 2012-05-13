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
        /// <summary>
        /// The main dock container panel
        /// </summary>
        public DockPanel MainDockPanel { get; set; }

        [Import("Shell")]
        public ContainerControl Shell { get; set; }
        
        /// <summary>
        /// The lookup list of dock panels (for keeping track of existing panels)
        /// </summary>
        //private Dictionary<string, DockContent> dockContents = new Dictionary<string,DockContent>();

        //private Dictionary<DockContent, int> sortOrderLookup = new Dictionary<DockContent, int>();

        // The lookup list of dock panels (for keeping track of existing panels)
        private readonly Dictionary<string, DockPanelInfo> dockPanelLookup = new Dictionary<string,DockPanelInfo>();

        /// <summary>The active panel key</summary>
        private string ActivePanelKey { get; set; }

        #region Constructor

        /// <summary>
        /// Create the default docking manager
        /// </summary>
        public HydroDockManager()
        {
            
        }

        #endregion

        #region IPartImportsSatisfiedNotification Members

        /// <summary>
        /// setup the parent form. This 
        /// occurs when the main form becomes available
        /// </summary>
        public void OnImportsSatisfied()
        {
            MainDockPanel = new DockPanel();
            MainDockPanel.Parent = Shell; //using the static variable
            MainDockPanel.Dock = DockStyle.Fill;
            MainDockPanel.BringToFront();
            MainDockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingSdi;

            //setup the events
            MainDockPanel.ActiveDocumentChanged += new EventHandler(MainDockPanel_ActiveDocumentChanged);
        }

        #endregion


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
            string caption = panel.Caption;
            Control innerControl = panel.InnerControl;
            DockStyle dockStyle = panel.Dock;
            short zOrder = panel.DefaultSortOrder;

            Image img = null;
            if (panel.SmallImage != null) img = panel.SmallImage;
 
            //set dock style of the inner control to Fill
            innerControl.Dock = DockStyle.Fill;
            
            // make an attempt to start the pane off at the right width.
            if (dockStyle == DockStyle.Right)
                MainDockPanel.DockRightPortion = (double)innerControl.Width / MainDockPanel.Width;

            //setting document tab strip location to 'bottom'
            if (dockStyle == DockStyle.Fill)
            {
                MainDockPanel.DocumentTabStripLocation = DocumentTabStripLocation.Bottom;
                MainDockPanel.DocumentStyle = DocumentStyle.DockingWindow;
            }

            DockContent content = new DockContent();
            content.ShowHint = ConvertToDockState(dockStyle);
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
            content.FormClosing += new FormClosingEventHandler(content_FormClosing);
            content.FormClosed += new FormClosedEventHandler(content_FormClosed);

            //the tag is used by the ActivePanelChanged event
            content.Pane.Tag = key;

            //add panel to contents dictionary
            if (!dockPanelLookup.ContainsKey(key))
            {
                dockPanelLookup.Add(key, new DockPanelInfo(panel, content, zOrder));
            }

            //trigger the panel added event
            OnPanelAdded(key);

            //set the correct sort order
            if (content.Pane.Contents.Count > 1)
            {
                int sortingIndex = ConvertSortOrderToIndex(content, zOrder);
                content.Pane.SetContentIndex(content, sortingIndex);
            }

            //caption - changed
            panel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(panel_PropertyChanged);
        }

        //when the dockable panel property is changed
        void panel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Caption")
            {
                DockablePanel dp = sender as DockablePanel;
                if (dp != null)
                {
                    string key = dp.Key;
                    if (dockPanelLookup.ContainsKey(dp.Key))
                    {
                        DockContent dc = dockPanelLookup[dp.Key].WeifenLuoDockPanel;
                        dc.Text = dp.Caption;
                        dc.TabText = dp.Caption;
                    }
                }
            }
        }

        void content_FormClosed(object sender, FormClosedEventArgs e)
        {
            DockContent c = sender as DockContent;
            if (c != null)
            {
                OnPanelClosed(c.Tag.ToString());
            }
        }

        void content_FormClosing(object sender, FormClosingEventArgs e)
        {
            DockContent c = sender as DockContent;
            if (c != null)
            {
                OnPanelClosed(c.Tag.ToString());
            }
        }

        private Icon ImageToIcon(Image img)
        {
            Bitmap bm = img as Bitmap;
            if (bm != null)
            {
                return Icon.FromHandle(bm.GetHicon());
            }
            return null;
        }

        public void Remove(string key)
        {
            if (dockPanelLookup.ContainsKey(key))
            {
                DockPanelInfo dockInfo = dockPanelLookup[key];
                
                DockContent content = dockInfo.WeifenLuoDockPanel;
                content.Close();
                
                //remove event handlers
                content.FormClosing -= content_FormClosing;
                content.FormClosed -= content_FormClosed;

                dockInfo.DotSpatialDockPanel.PropertyChanged -= panel_PropertyChanged;

                content.Dispose();
                dockPanelLookup.Remove(key);
                OnPanelRemoved(key);
            }
        }
        public static WeifenLuo.WinFormsUI.Docking.DockState ConvertToDockState(System.Windows.Forms.DockStyle dockStyle)
        {
            switch (dockStyle)
            {
                case System.Windows.Forms.DockStyle.Bottom:
                    return WeifenLuo.WinFormsUI.Docking.DockState.DockBottom;
                case System.Windows.Forms.DockStyle.Fill:
                    return WeifenLuo.WinFormsUI.Docking.DockState.Document;
                case System.Windows.Forms.DockStyle.Left:
                    return WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
                case System.Windows.Forms.DockStyle.None:
                    return WeifenLuo.WinFormsUI.Docking.DockState.Float;
                case System.Windows.Forms.DockStyle.Right:
                    return WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
                case System.Windows.Forms.DockStyle.Top:
                    return WeifenLuo.WinFormsUI.Docking.DockState.DockTop;

                default:
                    throw new NotImplementedException();
            }

        }

        public event EventHandler<DockablePanelEventArgs> ActivePanelChanged;

        public event EventHandler<DockablePanelEventArgs> PanelAdded;

        public event EventHandler<DockablePanelEventArgs> PanelRemoved;

        public event EventHandler<DockablePanelEventArgs> PanelClosed;

        public void SelectPanel(string key)
        {
            if (dockPanelLookup.ContainsKey(key))
            {
                dockPanelLookup[key].WeifenLuoDockPanel.Activate();
            }
        }

        /// <summary>
        /// Raises the ActivePanelChanged event
        /// </summary>
        void MainDockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            if (MainDockPanel.ActiveContent == null) return;
            if (MainDockPanel.ActiveContent.DockHandler == null) return;
            if (MainDockPanel.ActiveContent.DockHandler.Content == null) return;
            
            DockContent activeContent = MainDockPanel.ActiveContent.DockHandler.Content as DockContent;
            if (activeContent == null) return;
            if (activeContent.Tag == null) return;

            string activePanelKey = activeContent.Tag.ToString();
            OnActivePanelChanged(activePanelKey);
        }

        protected void OnPanelClosed(string panelKey)
        {
            if (PanelClosed != null)
            {
                PanelClosed(this, new DockablePanelEventArgs(panelKey));
            }
        }

        protected void OnPanelAdded(string panelKey)
        {
            if (PanelAdded != null)
            {
                PanelAdded(this, new DockablePanelEventArgs(panelKey));
            }
        }

        protected void OnPanelRemoved(string panelKey)
        {
            if (PanelRemoved != null)
            {
                PanelRemoved(this, new DockablePanelEventArgs(panelKey));
            }
        }

        protected void OnActivePanelChanged(string newActivePanelKey)
        {
            if (ActivePanelChanged != null)
            {
                ActivePanelChanged(this, new DockablePanelEventArgs(newActivePanelKey));
            }
        }

        int ConvertSortOrderToIndex(DockContent content, int sortOrder)
        {
            DockPane pane = content.Pane;
            int index = pane.Contents.Count - 1;
            List<int> sortOrderList = new List<int>();

            foreach (DockContent existingContent in pane.Contents)
            {
                string key = existingContent.Tag.ToString();
                if (dockPanelLookup.ContainsKey(key))
                {
                    sortOrderList.Add(dockPanelLookup[key].SortOrder);
                }
            }
            sortOrderList.Sort();
            index = sortOrderList.IndexOf(sortOrder);
            return index;
        }
    }
}

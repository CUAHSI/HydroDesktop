using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls.Docking;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.ComponentModel.Composition;

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
        private Dictionary<string, DockContent> dockContents = new Dictionary<string,DockContent>();

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
            //not implemented
            //TODO: check MW4 implementation
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
 
            //set dock style of the inner control to Fill
            innerControl.Dock = DockStyle.Fill;
            
            // make an attempt to start the pane off at the right width.
            if (dockStyle == DockStyle.Right)
                MainDockPanel.DockRightPortion = (double)innerControl.Width / MainDockPanel.Width;

            DockContent content = new DockContent();
            content.ShowHint = ConvertToDockState(dockStyle);
            content.Controls.Add(innerControl);

            content.Text = caption;
            content.TabText = caption;
            content.Tag = key;
            innerControl.Tag = key;

            content.Show(MainDockPanel);

            //the tag is used by the ActivePanelChanged event
            content.Pane.Tag = key;

            if (!dockContents.ContainsKey(key))
            {
                dockContents.Add(key, content);
            }
        }

        public void Remove(string key)
        {
            if (dockContents.ContainsKey(key))
            {
                DockContent content = dockContents[key];
                content.Close();
                content.Dispose();
                dockContents.Remove(key);
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

        public event EventHandler<ActivePanelChangedEventArgs> ActivePanelChanged;

        public void SelectPanel(string key)
        {
            if (dockContents.ContainsKey(key))
            {
                dockContents[key].Activate();
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

        protected void OnActivePanelChanged(string newActivePanelKey)
        {
            if (ActivePanelChanged != null)
            {
                ActivePanelChanged(this, new ActivePanelChangedEventArgs(newActivePanelKey));
            }
        }
    }
}

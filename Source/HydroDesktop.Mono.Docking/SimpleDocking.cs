// -----------------------------------------------------------------------
// <copyright file="SimpleDocking.cs" company="DotSpatial Team">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Controls.Docking;
using System.ComponentModel.Composition;
using System.Drawing;

namespace DemoMap
{
    /// <summary>
    ///
    /// </summary>
    public class SimpleDocking : IDockManager, IPartImportsSatisfiedNotification
    {

        [Import("Shell", typeof(ContainerControl))]
        private ContainerControl Shell { get; set; }

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use. (Shell will have a value)
        /// </summary>
        public void OnImportsSatisfied()
        {
            SplitContainer innerContainer = new SplitContainer();
            innerContainer.Name = "innerContainer";
            innerContainer.Dock = DockStyle.Fill;
            innerContainer.Panel1MinSize = 10;
            innerContainer.SplitterDistance = 35;
            SplitterPanel legendPanel = innerContainer.Panel1;
            SplitterPanel contentPanel = innerContainer.Panel2;

            contentTabs = new TabControl();
            contentTabs.Name = "contentTabs";
            contentTabs.Dock = DockStyle.Fill;

            legendTabs = new TabControl();
            legendTabs.Name = "legendTabs";
            legendTabs.Dock = DockStyle.Fill;

            legendPanel.Controls.Add(legendTabs);
            contentPanel.Controls.Add(contentTabs);

            SplitContainer container = (SplitContainer)Shell.Controls.Find("splitcontainer", false).FirstOrDefault();
            if (container == null)
            {
                container = new SplitContainer();
                container.Orientation = Orientation.Horizontal;
                container.Name = "splitcontainer";
                container.Dock = DockStyle.Fill;
                container.Panel1MinSize = 5;
                container.SplitterDistance = 25;
                Shell.Controls.Add(container);
            }
            container.Panel2.Controls.Add(innerContainer);
        }

        private TabControl legendTabs = null;
        private TabControl contentTabs = null;

        #region IDockManager Members

        /// <summary>
        /// Removes the specified panel.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(string key)
        {
            /*foreach (Form form in forms)
            {
                if (form.Name == key)
                {
                    form.Close();
                    forms.Remove(form);
                    break;
                }
            }*/
        }

        /// <summary>
        /// Occurs when the active panel is changed.
        /// </summary>
        public event EventHandler<DockablePanelEventArgs> ActivePanelChanged;

        /// <summary>
        /// Selects the panel.
        /// </summary>
        /// <param name="key">The key.</param>
        public void SelectPanel(string key)
        {
        }

        /// <summary>
        /// Adds the specified panel.
        /// </summary>
        /// <param name="panel"></param>
        public void Add(DockablePanel panel)
        {
            Add(panel.Key, panel.Caption, panel.InnerControl, panel.Dock);
        }

        /// <summary>
        /// Resets the layout of the dock panels to a developer specified location.
        /// </summary>
        public void ResetLayout()
        {
        }

        public event EventHandler<DockablePanelEventArgs> PanelClosed;

        public event EventHandler<DockablePanelEventArgs> PanelAdded;

        public event EventHandler<DockablePanelEventArgs> PanelRemoved;

        #endregion

        #region OnPanelRemoved

        /// <summary>
        /// Triggers the PanelRemoved event.
        /// </summary>
        public virtual void OnPanelRemoved(DockablePanelEventArgs ea)
        {
            if (PanelRemoved != null)
                PanelRemoved(null/*this*/, ea);
        }

        #endregion

        #region OnPanelAdded

        /// <summary>
        /// Triggers the PanelAdded event.
        /// </summary>
        public virtual void OnPanelAdded(DockablePanelEventArgs ea)
        {
            if (PanelAdded != null)
                PanelAdded(null/*this*/, ea);
        }

        #endregion

        #region OnPanelClosed

        /// <summary>
        /// Triggers the PanelClosed event.
        /// </summary>
        public virtual void OnPanelClosed(DockablePanelEventArgs ea)
        {
            if (PanelClosed != null)
                PanelClosed(null/*this*/, ea);
        }

        #endregion

        #region OnActivePanelChanged

        /// <summary>
        /// Triggers the ActivePanelChanged event.
        /// </summary>
        public virtual void OnActivePanelChanged(DockablePanelEventArgs ea)
        {
            if (ActivePanelChanged != null)
                ActivePanelChanged(null/*this*/, ea);
        }

        #endregion

        /// <summary>
        /// Adds the specified panel.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="caption">The caption of the panel and any tab button.</param>
        /// <param name="panel">The panel.</param>
        /// <param name="dockStyle">The dock location.</param>
        public void Add(string key, string caption, Control panel, DockStyle dockStyle)
        {
            if (panel == null) return;
            panel.Dock = DockStyle.Fill;

            TabPage page = new TabPage();
            page.Controls.Add(panel);
            page.Name = key;
            page.Text = caption;
            if (dockStyle == DockStyle.Left)
            {
                legendTabs.TabPages.Add(page);
            }
            else
            {
                contentTabs.TabPages.Add(page);
            }
            page.Enter += page_Activated;
        }

        private void page_Activated(object sender, EventArgs e)
        {
            OnActivePanelChanged(new DockablePanelEventArgs((sender as TabPage).Name));
        }

        public void HidePanel(string key)
        {

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls.Docking;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HydroDesktop.Main
{
    public class DockingManager : IDockManager
    {
        public DockPanel MainDockPanel { get; set; }

        /// <summary>
        /// Create the default docking manager using the main form as container
        /// </summary>
        public DockingManager()
        {
            MainDockPanel = new DockPanel();
            MainDockPanel.Parent = mainRibbonForm.Shell; //using the static variable
            MainDockPanel.Dock = DockStyle.Fill;
            MainDockPanel.BringToFront();
            MainDockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingSdi;
        }
        
        public DockingManager(DockPanel rootDockPanel)
        {
            MainDockPanel = rootDockPanel;
            MainDockPanel.Dock = DockStyle.Fill;
            MainDockPanel.BringToFront();
            MainDockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingSdi;
        }

        public void Add(string key, string caption, System.Windows.Forms.Control panel, System.Windows.Forms.DockStyle dockStyle)
        {
            // make an attempt to start the pane off at the right width.
            if (dockStyle == DockStyle.Right)
                MainDockPanel.DockRightPortion = (double)panel.Width / MainDockPanel.Width;

            DockContent content = new DockContent();
            content.ShowHint = ConvertToDockState(dockStyle);
            content.Controls.Add(panel);

            content.TabText = caption;

            content.Show(MainDockPanel);
        }

        public void Remove(string key)
        {
            //todo:
            throw new NotImplementedException();
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

    }
}

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
        private DockPanel dockManager;

        public DockingManager(DockPanel dockManager)
        {
            dockManager.Dock = DockStyle.Fill;
            dockManager.BringToFront();
            dockManager.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingSdi;

            this.dockManager = dockManager;
        }

        public void Add(string key, System.Windows.Forms.Control panel, System.Windows.Forms.DockStyle dockStyle)
        {
            panel.Dock = DockStyle.Fill;

            DockContent content = new DockContent();
            content.ShowHint = ConvertToDockState(dockStyle);
            content.Controls.Add(panel);
            content.Show(dockManager);

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

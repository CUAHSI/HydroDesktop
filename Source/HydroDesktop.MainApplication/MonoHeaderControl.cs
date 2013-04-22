using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using DotSpatial.Controls.Header;
using System.Windows.Forms;
using System.Diagnostics;

namespace DemoMap
{
    [Export(typeof(IHeaderControl))]
    class MonoHeaderControl : HeaderControl, IPartImportsSatisfiedNotification
    {
        private MainMenu mainmenu;

        [Import("Shell", typeof(ContainerControl))]
        private ContainerControl Shell { get; set; }

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use. (Shell will have a value)
        /// </summary>
        public void OnImportsSatisfied()
        {
            mainmenu = new MainMenu();

            Form form = Shell as Form;
            form.Menu = mainmenu;
        }

        public override void SelectRoot(string key)
        {
            // we won't do anything here.
        }

        public override void Add(SimpleActionItem item)
        {
            MenuItem menu = new MenuItem(item.Caption);

            menu.Name = item.Key;
            menu.Enabled = item.Enabled;
            menu.Visible = item.Visible;
            menu.Click += (sender, e) => item.OnClick(e);

            MenuItem root = null;

            if (!mainmenu.MenuItems.ContainsKey(item.RootKey))
            {
                root = new MenuItem(item.RootKey);
            }
            else
            {
                root = mainmenu.MenuItems.Find(item.RootKey, true).ElementAt(0);
            }

            try
            {
                root.MenuItems.Add(menu);
            }
            catch (Exception e)
            {
                Debug.Print(e.StackTrace);
            }

        }

        public override void Add(MenuContainerItem item)
        {
            
        }

        public override void Add(RootItem item)
        {
            MenuItem submenu = null;
            if (!this.mainmenu.MenuItems.ContainsKey(item.Key))
            {
                submenu = new MenuItem();
                submenu.Name = item.Key;
                submenu.Visible = item.Visible;
                submenu.Text = item.Caption;
                submenu.MergeOrder = item.SortOrder;
                mainmenu.MenuItems.Add(submenu);
            }
            /*else
            {
                this.mainmenu.MenuItems.Find(item.Key, true).ElementAt(0) as MenuItem;
                submenu.Name = item.Key;
                submenu.Visible = item.Visible;
                submenu.Text = item.Caption;
                submenu.MergeOrder = item.SortOrder;
            }*/
        }

        public override void Add(DropDownActionItem item)
        {
            
        }

        public override void Add(SeparatorItem item)
        {
            
        }

        public override void Add(TextEntryActionItem item)
        {
            
        }
    }
}

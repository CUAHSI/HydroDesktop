using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using DotSpatial.Controls.Header;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;

namespace DemoMap
{
    [Export(typeof(IHeaderControl))]
    class SimpleMonoHeaderControl : HeaderControl, IPartImportsSatisfiedNotification
    {
        private TabControl tabcontrol;

        [Import("Shell", typeof(ContainerControl))]
        private ContainerControl Shell { get; set; }

        private bool _nextItemBeginsGroup = false;
        private const string STR_DefaultGroupName = "Default Group";

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use. (Shell will have a value)
        /// </summary>
        public void OnImportsSatisfied()
        {
            Form form = Shell as Form;
            tabcontrol = new TabControl();

            tabcontrol.Name = "tabcontrol";
            tabcontrol.Dock = DockStyle.Fill;
            SplitContainer container = (SplitContainer)Shell.Controls.Find("splitcontainer", false).FirstOrDefault();
            if (container == null)
            {
                container = new SplitContainer();
                container.Orientation = Orientation.Horizontal;
                container.Name = "splitcontainer";
                container.Dock = DockStyle.Fill;
                container.Panel1MinSize = 5;
                container.SplitterDistance = 20;
                Shell.Controls.Add(container);
            }
            container.Panel1.Controls.Add(tabcontrol);
        }

        public override void SelectRoot(string key)
        {
            // we won't do anything here.
        }

        public override void Add(SimpleActionItem item)
        {
            Button button = new Button();

            button.Name = item.Key;
            button.Text = item.Caption;
            button.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            button.Image = item.SmallImage;
            button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            button.Enabled = item.Enabled;
            button.Visible = item.Visible;
            button.Click += (sender, e) => item.OnClick(e);

            item.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SimpleActionItem_PropertyChanged);
            addControlToRoot(button, item.RootKey, item.GroupCaption);
        }

        public override void Add(MenuContainerItem item)
        {
            Button button = new Button();
            button.Name = item.Key;
            button.Text = item.Caption;
            button.Enabled = item.Enabled;
            button.Visible = item.Visible;
            //button.Click += (sender, e) => item.OnClick(e);
        }

        public override void Add(RootItem item)
        {
            if (!this.tabcontrol.TabPages.ContainsKey(item.Key))
            {
                tabcontrol.TabPages.Add(item.Key, item.Caption);
                TabPage tabPage = tabcontrol.TabPages[item.Key];
                tabPage.Name = item.Key;
                item.PropertyChanged += new PropertyChangedEventHandler(RootItem_PropertyChanged);
                FlowLayoutPanel layout = new FlowLayoutPanel();
                layout.Name = "container";
                layout.BorderStyle = BorderStyle.Fixed3D;
                layout.Dock = DockStyle.Fill;
                layout.WrapContents = true;
                layout.HorizontalScroll.Enabled = true;
                layout.HorizontalScroll.Visible = true;
                layout.VerticalScroll.Enabled = true;
                layout.VerticalScroll.Visible = true;

                tabPage.Controls.Add(layout);
            }
        }

        public override void Add(DropDownActionItem item)
        {
            ComboBox combo = new ComboBox();
            combo.Name = item.Key;

            ParseAllowEditingProperty(item, combo);

            if (item.Width != 0)
            {
                combo.Width = item.Width;
            }

            combo.Items.AddRange(item.Items.ToArray());
            combo.SelectedIndexChanged += delegate
                                            {
                                                item.PropertyChanged -= DropDownActionItem_PropertyChanged;
                                                item.SelectedItem = combo.SelectedItem;
                                                item.PropertyChanged += DropDownActionItem_PropertyChanged;
                                            };
            addControlToRoot(combo, item.RootKey, item.GroupCaption);
            item.PropertyChanged += DropDownActionItem_PropertyChanged;
        }

        public override void Add(SeparatorItem item)
        {
            _nextItemBeginsGroup = true;
        }

        public override void Add(TextEntryActionItem item)
        {
            TextBox textBox = new TextBox();
            textBox.Name = item.Key;
            if (item.Width != 0)
            {
                textBox.Width = item.Width;
            }
            textBox.TextChanged += delegate
                                        {
                                            item.PropertyChanged -= TextEntryActionItem_PropertyChanged;
                                            item.Text = textBox.Text;
                                            item.PropertyChanged += TextEntryActionItem_PropertyChanged;
                                        };

            addControlToRoot(textBox, item.RootKey, item.GroupCaption);
            item.PropertyChanged += TextEntryActionItem_PropertyChanged;
        }

        private void DropDownActionItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as DropDownActionItem;
            var guiItem = this.GetItem(item.Key) as ComboBox;

            switch (e.PropertyName)
            {
                case "AllowEditingText":
                    ParseAllowEditingProperty(item, guiItem);
                    break;

                case "Width":
                    guiItem.Width = item.Width;
                    break;

                case "SelectedItem":
                    guiItem.SelectedItem = item.SelectedItem;
                    break;

                case "FontColor":
                    guiItem.ForeColor = item.FontColor;
                    break;

                case "ToggleGroupKey":
                    break;

                case "MultiSelect":
                    break;

                default:
                    ActionItem_PropertyChanged(item, e);
                    break;
            }
        }

        private void TextEntryActionItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as TextEntryActionItem;
            var guiItem = this.GetItem(item.Key) as TextBox;

            switch (e.PropertyName)
            {
                case "Width":
                    guiItem.Width = item.Width;
                    break;

                case "Text":
                    guiItem.Text = item.Text;
                    break;

                case "FontColor":
                    guiItem.ForeColor = item.FontColor;
                    break;

                default:
                    ActionItem_PropertyChanged(item, e);
                    break;
            }
        }

        private void ActionItem_PropertyChanged(ActionItem item, PropertyChangedEventArgs e)
        {
            Control guiItem = GetItem(item.Key);
            Debug.Print("ACTIONITEM_PROPERTYCHANGED: " + item.Caption);

            switch (e.PropertyName)
            {
                case "Caption":
                    guiItem.Text = item.Caption;
                    break;

                case "Enabled":
                    guiItem.Enabled = item.Enabled;
                    break;

                case "Visible":
                    guiItem.Visible = item.Visible;
                    break;

                case "ToolTipText":
                    //guiItem.ToolTipText = item.ToolTipText;
                    break;

                case "GroupCaption":
                    // todo: change group
                    break;

                case "RootKey":
                    // todo: change root
                    // note, this case will also be selected in the case that we set the Root key in our code.
                    break;

                case "Key":
                default:
                    throw new NotSupportedException(" This Header Control implementation doesn't have an implemenation for or has banned modifying that property.");
            }
        }

        private void RootItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as RootItem;
            var guiItem = this.GetItem(item.Key);

            switch (e.PropertyName)
            {
                case "Caption":
                    guiItem.Text = item.Caption;
                    break;

                case "Visible":
                    guiItem.Visible = item.Visible;
                    break;

                case "SortOrder":
                    break;
                default:
                    break;
            }
        }

        void SimpleActionItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as SimpleActionItem;
            var guiItem = this.GetItem(item.Key);

            switch (e.PropertyName)
            {
                case "SmallImage":
                    break;

                case "LargeImage":
                    break;

                case "MenuContainerKey":
                    Trace.WriteLine("MenuContainerKey must not be changed after item is added to header.");
                    break;

                case "ToggleGroupKey":
                    Trace.WriteLine("ToggleGroupKey must not be changed after item is added to header.");
                    break;

                default:
                    ActionItem_PropertyChanged(item, e);
                    break;
            }
        }


        private void ParseAllowEditingProperty(DropDownActionItem item, ComboBox guiItem)
        {
            if (item.AllowEditingText)
            {
                guiItem.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
            {
                guiItem.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }

        private Control GetItem(string key)
        {
            Control item = tabcontrol.Controls.Find(key, true).FirstOrDefault();
            Debug.Print("ITEM: " + item.Text + " FOUND.");
            return item;
        }

        private TabPage GetTabPage(string key)
        {
            return tabcontrol.TabPages[key];
        }

        private FlowLayoutPanel GetRootContainer(TabPage root)
        {
            FlowLayoutPanel layout = (FlowLayoutPanel)root.Controls.Find("container", true).FirstOrDefault();
            return layout;
        }

        private void addControlToRoot(Control control, string rootkey, string groupCaption)
        {
            try
            {
                FlowLayoutPanel layout = GetOrCreateGroup(GetTabPage(rootkey), groupCaption);
                layout.Controls.Add(control);

                if (layout.Controls.Count <= 4)
                {
                    layout.Width = 1;
                    foreach (Control c in layout.Controls)
                    {
                        layout.Width += (c.Width + 10);
                    }
                    layout.Height = control.Height + 10;
                }
                else
                {
                    layout.Height = (control.Height * ((layout.Controls.Count / 4) + 2));
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.StackTrace);
            }
        }

        private void ProcessSeparator(object barItemLink)
        {
            /*if (_nextItemBeginsGroup)
            {
                barItemLink.BeginGroup = _nextItemBeginsGroup;
                _nextItemBeginsGroup = false;
            }*/
        }

        private FlowLayoutPanel GetOrCreateGroup(TabPage page, string groupCaption)
        {
            // make sure the group caption is present or use a default.
            if (groupCaption == null)
            {
                groupCaption = STR_DefaultGroupName;
            }

            FlowLayoutPanel group = (FlowLayoutPanel)GetRootContainer(page).Controls.Find(groupCaption, true).FirstOrDefault();
            if (group == null)
            {
                group = new FlowLayoutPanel();
                group.Name = groupCaption;
                group.Text = groupCaption;
                group.Width = 1;
                group.Height = 1;
                group.BorderStyle = BorderStyle.Fixed3D;
                group.WrapContents = true;
                group.HorizontalScroll.Enabled = true;
                group.VerticalScroll.Enabled = true;
                GetRootContainer(page).Controls.Add(group);
            }

            return group;
        }
    }
}

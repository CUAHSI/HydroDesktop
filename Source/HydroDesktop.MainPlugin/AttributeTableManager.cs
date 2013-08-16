using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls;
using DotSpatial.Symbology;
using DotSpatial.Controls.Header;
using HydroDesktop.Main.Properties;

namespace HydroDesktop.Main
{
    /// <summary>
    /// This class is responsible for managing the buttons
    /// and context menus for launching the attribute data explorer
    /// </summary>
    public class AttributeTableManager
    {
        AppManager App { get; set; }
        private SimpleActionItem _btnAttributeTable;
        private bool _showTableManagerPanel;
        
        public AttributeTableManager(AppManager app)
        {
            App = app;
            Activate();
        }
        
        
        //context menu item name
        //TODO: make this localizable
        const string contextMenuItemName = "View Attribute Table";

        public void Activate()
        {
            App.HeaderControl.Add(_btnAttributeTable = new SimpleActionItem(HeaderControl.HomeRootItemKey, "View Attribute Table", AttributeTable_Click) { GroupCaption = "Map Tool", SmallImage = Resources.table_16x16, LargeImage = Resources.table_32x32, Enabled = true, ToggleGroupKey = "table" });
            App.Map.LayerAdded += Map_LayerAdded;
            App.SerializationManager.Deserializing += SerializationManager_Deserializing;
            App.DockManager.PanelHidden += DockManager_PanelHidden;
                ;
            App.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;

            _showTableManagerPanel = false;
            
            // TODO: if layers were loaded before this plugin, do something about adding them to the context menu.
            //base.Activate();
        }

        void DockManager_ActivePanelChanged(object sender, DotSpatial.Controls.Docking.DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey.Equals("kDataExplorer"))
            {

                if (!_showTableManagerPanel)
                {
                    _btnAttributeTable.Toggle();
                }
                _showTableManagerPanel = true;
            }
        }

        void DockManager_PanelHidden(object sender, DotSpatial.Controls.Docking.DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey == "kDataExplorer")
            {

                if (_showTableManagerPanel)
                {
                    _btnAttributeTable.Toggle();
                }
                _showTableManagerPanel = false;
            }
        }

        private void SerializationManager_Deserializing(object sender, SerializingEventArgs e)
        {
            // context menu items are added to layers when opening a project
            // this call is necessary because the LayerAdded event doesn't fire when a project is opened.
            foreach (ILayer layer in App.Map.MapFrame.GetAllLayers())
            {
                IFeatureLayer fl = layer as IFeatureLayer;
                if (fl != null)
                {
                    if (!fl.ContextMenuItems.Exists(item => item.Name == contextMenuItemName))
                    {
                        // add context menu item.
                        var menuItem = new SymbologyMenuItem(contextMenuItemName, delegate { ShowAttributes(fl); });
                        menuItem.Image = Resources.table_16x16;
                        fl.ContextMenuItems.Insert(2, menuItem);
                    }
                }
            }
            //attach layer added events to existing groups
            foreach (var grp in App.Map.MapFrame.GetAllGroups())
            {
                grp.LayerAdded += Map_LayerAdded;
            }
        }

        private void Map_LayerAdded(object sender, LayerEventArgs e)
        {
            if (e.Layer == null)
                return;

            AddContextMenuItems(e.Layer);
        }

        private void AddContextMenuItems(ILayer addedLayer)
        {
            IMapGroup grp = addedLayer as IMapGroup;
            if (grp != null)
            {
                // map.layerAdded event doesn't fire for groups. Therefore, it's necessary
                // to handle this event separately for groups.
                grp.LayerAdded += Map_LayerAdded;
            }

            if (addedLayer == null || addedLayer.ContextMenuItems.Exists(item => item.Name == contextMenuItemName))
            {
                // assume menu item already exists. Do nothing.
                return;
            }

            // add context menu item.
            var menuItem = new SymbologyMenuItem(contextMenuItemName, delegate { ShowAttributes(addedLayer as IFeatureLayer); });
            menuItem.Image = Resources.table_16x16;
            addedLayer.ContextMenuItems.Insert(2, menuItem);
        }

        public  void Deactivate()
        {
            App.HeaderControl.RemoveAll();

            // detach events
            DetachLayerAddedEvents();
            App.SerializationManager.Deserializing -= SerializationManager_Deserializing;
            // remove context menu items.
            RemoveContextMenuItems();
        }

        private void DetachLayerAddedEvents()
        {
            App.Map.LayerAdded -= Map_LayerAdded;
            foreach (var grp in App.Map.MapFrame.GetAllGroups())
            {
                grp.LayerAdded -= Map_LayerAdded;
            }
        }

        private void RemoveContextMenuItems()
        {
            foreach (ILayer lay in App.Map.MapFrame.GetAllLayers())
            {
                if (lay.ContextMenuItems.Exists(item => item.Name == contextMenuItemName))
                {
                    lay.ContextMenuItems.Remove(lay.ContextMenuItems.First(item => item.Name == contextMenuItemName));
                    return;
                }
            }
        }

        private bool ShowAttributes(IFeatureLayer layer)
        {
            bool isActive = false;

            if (layer != null)
            {
                layer.IsSelected = true;
                App.DockManager.SelectPanel("kDataExplorer");
                isActive = true;
            }

            return isActive;
        }

        /// <summary>
        /// Open attribute table
        /// </summary>
        private void AttributeTable_Click(object sender, EventArgs e)
        {
            _showTableManagerPanel = !_showTableManagerPanel;

            if (_showTableManagerPanel)
            {
                IMapFrame mainMapFrame = App.Map.MapFrame;
                List<ILayer> layers = mainMapFrame.GetAllLayers();
                bool isActive = false;

                foreach (ILayer layer in layers.Where(l => l.IsSelected))
                {
                    IFeatureLayer fl = layer as IFeatureLayer;

                    if (fl == null)
                        continue;
                    isActive = ShowAttributes(fl);

                    if (isActive)
                        break;
                }

                if (isActive == false)
                {
                    _showTableManagerPanel = !_showTableManagerPanel;
                    _btnAttributeTable.Toggle();
                }
            }
            else
            {
                App.DockManager.HidePanel("kDataExplorer");
            }
        }
    }
}

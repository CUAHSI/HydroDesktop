using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls;

namespace HydroDesktop.Main
{
    /// <summary>
    /// This class tracks the changes in current project
    /// to determine the project state (saved or unsaved)
    /// </summary>
    public class ProjectChangeTracker
    {    
        /// <summary>
        /// Creates a new instance of project change tracker
        /// </summary>
        /// <param name="DotSpatialAppManager">the associated App Manager</param>
        public ProjectChangeTracker(AppManager DotSpatialAppManager)
        {
            DotSpatialApp = DotSpatialAppManager;

            DotSpatialApp.SerializationManager.Serializing += new EventHandler<SerializingEventArgs>(SerializationManager_Serializing);
            DotSpatialApp.SerializationManager.Deserializing += new EventHandler<SerializingEventArgs>(SerializationManager_Deserializing);

            AddEventHandlers();
        }

        void SerializationManager_Deserializing(object sender, SerializingEventArgs e)
        {
            RemoveEventHandlers();
            AddEventHandlers();
        }

        void SerializationManager_Serializing(object sender, SerializingEventArgs e)
        {
            OnProjectModified(true);
        }
        
        /// <summary>
        /// Gets or sets the associated DotSpatial application
        /// </summary>
        public AppManager DotSpatialApp
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the status of current project
        /// </summary>
        public bool ProjectIsSaved
        {
            get;
            set;
        }

        #region Events
        /// <summary>
        /// Fires when the current project is modified with unsaved changes
        /// </summary>
        public event EventHandler ProjectModified;
        #endregion

        #region Private Methods

        private void OnProjectModified(bool isSaved)
        {
            ProjectIsSaved = isSaved;
            
            if (ProjectModified != null)
            {
                ProjectModified(this, null);
            }
        }

        private void AddEventHandlers()
        {
            DotSpatialApp.Map.Layers.ItemChanged += new EventHandler(Layers_ItemChanged);
            DotSpatialApp.Map.MapFrame.ItemChanged += new EventHandler(MapFrame_ItemChanged);
            DotSpatialApp.Map.MapFrame.ViewExtentsChanged += new EventHandler<DotSpatial.Data.ExtentArgs>(MapFrame_ViewExtentsChanged);
        }

        private void RemoveEventHandlers()
        {
            DotSpatialApp.Map.Layers.ItemChanged -= Layers_ItemChanged;
            DotSpatialApp.Map.MapFrame.ItemChanged -= MapFrame_ItemChanged;
            DotSpatialApp.Map.MapFrame.ViewExtentsChanged -= MapFrame_ViewExtentsChanged;
        }

        void MapFrame_ViewExtentsChanged(object sender, DotSpatial.Data.ExtentArgs e)
        {
            OnProjectModified(false);
        }

        void MapFrame_ItemChanged(object sender, EventArgs e)
        {
            OnProjectModified(false);
        }

        void Layers_ItemChanged(object sender, EventArgs e)
        {
            OnProjectModified(false);
        }

        #endregion
    }
}

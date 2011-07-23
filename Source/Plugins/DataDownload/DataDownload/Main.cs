using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Symbology;

namespace DataDownload
{
    public class Main : Extension, IMapPlugin
    {
        #region Fields

        const string TableTabKey = "kHome";
        private IMapPluginArgs _mapArgs;

        #endregion

        public void Initialize(IMapPluginArgs args)
        {
            if (args == null) throw new ArgumentNullException("args");
            _mapArgs = args;

            // Initialize menu
            var btnDownload = new SimpleActionItem("Download", DoDownload)
                                  {RootKey = TableTabKey, GroupCaption = "Search"};
            args.AppManager.HeaderControl.Add(btnDownload);

            // Subscribe to events
            _mapArgs.Map.LayerAdded += Map_LayerAdded;
        }
       
        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        protected override void OnDeactivate()
        {
            _mapArgs.Map.LayerAdded -= Map_LayerAdded;
            _mapArgs.AppManager.HeaderControl.RemoveItems();

            // This line ensures that "Enabled" is set to false.
            base.OnDeactivate();
        }

        void Map_LayerAdded(object sender, LayerEventArgs e)
        {
            if (IsSearchLayer(e.Layer))
            {
                MessageBox.Show("OK");
            }
        }

        private void DoDownload(object sender, EventArgs args)
        {
            MessageBox.Show("Download");
        }

        /// <summary>
        /// Check layer for search attributes
        /// </summary>
        /// <param name="layer">Layer to check</param>
        /// <returns>True - layer is search layer, otherwise - false.</returns>
        private bool IsSearchLayer(ILayer layer)
        {
            Debug.Assert(layer != null);

            var featureLayer = layer as IFeatureLayer;
            if (featureLayer == null) return false;

            if (featureLayer is PointLayer) return true;

            var searchColumns = new[]
                                    {"ServCode", "ServUrl", "SiteCode", "VarCode", "StartDate", "EndDate", "ValueCount"};
            var layerColumns = featureLayer.DataSet.GetColumns();

            foreach (var sColumn in searchColumns)
            {
                var hasColumn = layerColumns.Any(dataColumn => dataColumn.ColumnName == sColumn);
                if (!hasColumn)
                    return false;
            }
            return true;
        }
    }
}

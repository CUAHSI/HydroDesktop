using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Symbology;
using HydroDesktop.Search.Download;

namespace HydroDesktop.Search
{
    public partial class SearchResultsControl : UserControl
    {
        #region Fields

        private MapGroup _laySearchResult;
        private Map _map;

        #endregion

        #region Constructors

        public SearchResultsControl()
        {
            InitializeComponent();
            
            searchDataGridView.SelectionChanged += searchDataGridView1_SelectionChanged;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Count of selected row in the dataGridView
        /// </summary>
        public int SelectedRowsCount
        {
            get { return searchDataGridView.SelectedRows.Count; }
        }

        #endregion

        #region Public methods

        public void SetDataSource(IMapFeatureLayer dataSource)
        {
            searchDataGridView.SetDataSource(dataSource);
        }

        public IList<OneSeriesDownloadInfo> GetSelectedSeriesAsDownloadInfo(DateTime startDate, DateTime endDate)
        {
            var downloadList = new List<OneSeriesDownloadInfo>();
            var fileNameList = new List<String>(); 
            foreach (var selFeature in searchDataGridView.MapLayer.Selection.ToFeatureList())
            {
                var row = selFeature.DataRow;

                var di = new OneSeriesDownloadInfo
                {
                    SiteName = row["SiteName"].ToString(),
                    FullSiteCode = row["SiteCode"].ToString(),
                    FullVariableCode = row["VarCode"].ToString(),
                    Wsdl = row["ServiceURL"].ToString(),
                    StartDate = startDate,
                    EndDate = endDate,
                    VariableName = row["VarName"].ToString(),
                    Latitude = Convert.ToDouble(row["Latitude"]),
                    Longitude = Convert.ToDouble(row["Longitude"])
                };

                var fileBaseName = di.FullSiteCode + "|" + di.FullVariableCode;
                if (fileNameList.Contains(fileBaseName)) continue;

                fileNameList.Add(fileBaseName);
                downloadList.Add(di);
            }

            return downloadList;
        }
        
        public void SetLayerSearchResult(Map map, MapGroup laySearchResult)
        {
            //to prevent the first row of data grid view from becoming selected
            searchDataGridView.ClearSelection();

            _map = map;

            // UnAttach previous layer
            if (_laySearchResult != null)
            {
                _laySearchResult.Layers.LayerRemoved -= Layers_LayerRemoved;
                _laySearchResult.Layers.LayerAdded -= Layers_LayerAdded;
            }

            comboDataSource.SelectedIndexChanged -= comboDataSource_SelectedIndexChanged;

            // Clear combobox
            while (comboDataSource.Items.Count > 0)
            {
                RemoveLayeFromCombo(((LayerWrapper) comboDataSource.Items[0]).Layer);
            }

            // attach new search results layer
            _laySearchResult = laySearchResult;
            if (_laySearchResult != null)
            {
                _laySearchResult.Layers.LayerRemoved += Layers_LayerRemoved;
                _laySearchResult.Layers.LayerAdded += Layers_LayerAdded;

                AddLayerToCombo(new DummyLayer());                  // add dummy layer to indicate no selected layers
                foreach (var layer in _laySearchResult.GetLayers())
                {
                    AddLayerToCombo(layer);
                }

                comboDataSource.SelectedIndexChanged += comboDataSource_SelectedIndexChanged;
                comboDataSource.SelectedIndex = 1;
            }
        }
        
        #endregion

        #region Private methods

        private void AddLayerToCombo(ILayer layer)
        {
            comboDataSource.Items.Add(new LayerWrapper(layer));
            layer.VisibleChanged += layer_VisibleChanged;
        }
        private void RemoveLayeFromCombo(ILayer layer)
        {
            var ind = GetIndexOfLayerInCombobox(layer);
            if (ind >= 0)
            {
                layer.VisibleChanged -= layer_VisibleChanged;
                comboDataSource.Items.RemoveAt(ind);
            }
        }

        /// <summary>
        /// Index of layer in combobox
        /// </summary>
        /// <param name="layer">Layer to be found</param>
        /// <returns>Index, or -1, if not found.</returns>
        private int GetIndexOfLayerInCombobox(ILayer layer)
        {
            int ind = -1;
            for (int i = 0; i < comboDataSource.Items.Count; i++)
                if (((LayerWrapper)comboDataSource.Items[i]).Layer == layer)
                {
                    ind = i;
                    break;
                }
            return ind;
        }

        void layer_VisibleChanged(object sender, EventArgs e)
        {
            var layer = sender as IMapFeatureLayer;
            if (layer == null) return;

            if (layer.IsVisible)
            {
                var ind = GetIndexOfLayerInCombobox(layer);
                if (ind >= 0)
                {
                    comboDataSource.SelectedIndex = ind;
                }
            }else
            {
                var hasVisible = _laySearchResult.GetLayers().Any(lrs => lrs.IsVisible);
                if (!hasVisible)
                {
                    comboDataSource.SelectedIndex = 0;
                }
            }
        }

        void comboDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_laySearchResult == null) return;

            comboDataSource.SelectedIndexChanged -= comboDataSource_SelectedIndexChanged;

            var hasVisibleLayer = false;
            var current = (LayerWrapper)comboDataSource.SelectedItem;
            foreach (var layer in _laySearchResult.GetLayers())
            {
                var state = layer == current.Layer;
                var rendItem = layer as IRenderableLegendItem;
                if (rendItem != null)
                {
                    rendItem.VisibleChanged -= layer_VisibleChanged; // we call this hanlder once time after all items will be processed
                    rendItem.IsVisible = state;                      // force a re-draw in the case where we are talking about layers.
                    rendItem.VisibleChanged += layer_VisibleChanged;

                    if (rendItem.IsVisible)
                    {
                        hasVisibleLayer = true;
                        layer_VisibleChanged(rendItem, EventArgs.Empty);
                        SetDataSource(rendItem as IMapFeatureLayer);
                    }
                }
                else
                {
                    layer.Checked = state;
                }
            }
            if (!hasVisibleLayer)
            {
                SetDataSource(null);
            }

            if (_map != null)
            {
                _map.Refresh();

                var legend = (Legend) _map.Legend;//TODO: hack
                legend.RefreshNodes();
            }

            comboDataSource.SelectedIndexChanged += comboDataSource_SelectedIndexChanged;
        }

        void Layers_LayerAdded(object sender, LayerEventArgs e)
        {
            AddLayerToCombo(e.Layer);
        }
        void Layers_LayerRemoved(object sender, LayerEventArgs e)
        {
            RemoveLayeFromCombo(e.Layer);
        }

        private void searchDataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            var numSelected = searchDataGridView.SelectedRows.Count;
            lblDataSeries.Text = String.Format("{0} out of {1} series selected", numSelected, searchDataGridView.RowCount);
        }

        #endregion

        #region Helpers

        class LayerWrapper
        {
            public ILayer Layer { get; private set; }

            public LayerWrapper(ILayer layer)
            {
                if (layer == null) throw new ArgumentNullException("layer");
                Layer = layer;
            }

            public override string ToString()
            {
                return Layer.LegendText;
            }
        }

        sealed class DummyLayer : Layer
        {
            public DummyLayer()
            {
                /*
                 if LegendText == null (by default it is),
                 than LayerWrapper.ToString() returns null, and after this
                  
                 comboDataSource.Add(...) throws OutOfMemoryException :) "Too many items in the combo box"                  
                 
                 http://connect.microsoft.com/VisualStudio/feedback/details/417533/outofmemoryexception-too-many-items-in-the-combo-box
                */
                LegendText = string.Empty;
            }
        }

        #endregion
    }
}

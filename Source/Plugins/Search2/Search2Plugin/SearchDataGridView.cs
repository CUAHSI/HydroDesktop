using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Symbology;
using System.Data;


    /// <summary>
    /// Specialized DataGridView for GIS attribute table selection and 
    /// searching. The table is linked to the map layer. When a row is
    /// selected in the table, the corresponding feature is selected in the map.
    /// When a feature is selected in the map, the corresponding row is selected
    /// in the attribute table.
    /// </summary>
    public class SearchDataGridView : DataGridView
    {
        #region Variables
        //indicates that the data source has been changed
        private bool _dataSourceChanged = false;
        private int _dataBindingEventCount = 0;

        private IMapFrame _mapFrame = null;
        private IMapFeatureLayer _layer = null;

        private bool _ignoreMapSelection = false;
        
        private bool _sorting = false;
        private bool _cellMouseUp = false;
        #endregion


        #region Constructor

        public SearchDataGridView()
        {
            this.AllowUserToAddRows = false;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.EditMode = DataGridViewEditMode.EditProgrammatically;
            //mogikanin <<trial check by dg>> dated 6/6/2011
            CellFormatting += SearchDataGridView_CellFormatting;
        }
        // mogikanin <<trial check by dg>>
        void SearchDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (Columns[e.ColumnIndex].Name != "ValueCount") return;

            int oldValue;
            if (!Int32.TryParse(e.Value.ToString(), out oldValue)) return;

            if (oldValue == 0)
            {
                e.Value = "unknown";
                e.FormattingApplied = true;
            }
        }
        //mogikanin trial ends dated 6/6/2011
        #endregion

        #region Properties
        /// <summary>
        /// If set to true, the map is automatically zoomed to
        /// the features corresponding to selected rows in the table
        /// </summary>
        public bool ZoomToSelected 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets the map layer that is linked with the table in the data grid view
        /// </summary>
        public IMapFeatureLayer MapLayer
        {
            get { return _layer; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Set the data source of the data grid view
        /// </summary>
        /// <param name="sourceLayer">The source map feature layer</param>
        public void SetDataSource(IMapFeatureLayer sourceLayer)
        {
            _layer = sourceLayer;
            _layer.SelectionEnabled = true;
            _layer.IsSelected = true;

            //set the map
            _mapFrame = _layer.MapFrame as IMapFrame;

            //Changed by Jiri - Use a copy of table to fix the AttributeTable editor bug
            DataTable newTable = CopyDataTable(_layer.DataSet.DataTable);

            //add the 'fid' column
            int lastColumn = newTable.Columns.Count;
            if (newTable.Columns.Contains("mw_fid"))
            {
                lastColumn = newTable.Columns.IndexOf("mw_fid");
            }
            else
            {
                newTable.Columns.Add("mw_fid", typeof(Int32));
            }
            int rowIndex = 0;

            foreach (DataRow row in newTable.Rows)
            {
                row[lastColumn] = rowIndex;
                rowIndex++;
            }

            _dataSourceChanged = true;

            
            this.DataSource = newTable;
            //if (this.Columns.Contains("mw_fid"))
            //{
            //    this.Columns["mw_fid"].Visible = false;
            //}

            _layer.SelectionChanged += new EventHandler(layer_SelectionChanged);
        }

        //Creates an in-memory copy of the DataTable which is not linked to the original table
        private DataTable CopyDataTable(DataTable originalTable)
        {
            DataTable newTable = new DataTable();
            foreach (DataColumn col in originalTable.Columns)
            {
                newTable.Columns.Add(new DataColumn(col.ColumnName, col.DataType));
            }

            // Use the ImportRow method to copy from Products table to its clone.
            for (int i = 0; i < originalTable.Rows.Count; i++)
            {
                newTable.ImportRow(originalTable.Rows[i]);
            }
            return newTable;
        }

        /// <summary>
        /// To refresh the map selection (similar as in MouseUp event)
        /// </summary>
        public void RefreshMapSelection()
        {
            _ignoreMapSelection = true;

            foreach (DataGridViewRow dgr in this.Rows)
            {
                int fid = (int)dgr.Cells["mw_fid"].Value;
                _layer.DrawnStates[fid].Selected = dgr.Selected;
            }
            //to force to refresh the map
            _mapFrame.ResetBuffer();
            //zoom to selected features
            if (ZoomToSelected == true)
            {
                _layer.ZoomToSelectedFeatures();
            }

            _ignoreMapSelection = false;
        }

        void layer_SelectionChanged(object sender, EventArgs e)
        {
            if (_ignoreMapSelection == false)
            {
                foreach (DataGridViewRow dgr in this.Rows)
                {
                    int fid = (int)dgr.Cells["mw_fid"].Value;
                    dgr.Selected = _layer.DrawnStates[fid].Selected;
                }
            }
        }

        #endregion

        /// <summary> 
        /// to ensure that no rows are selected by default
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDataBindingComplete(DataGridViewBindingCompleteEventArgs e)
        {
            base.OnDataBindingComplete(e);

            if (_dataSourceChanged == true)
            {
                _dataBindingEventCount++;
            }

            if (_dataBindingEventCount >= 2 && _dataSourceChanged == true)
            {
                _dataSourceChanged = false;
                _dataBindingEventCount = 0;
                this.ClearSelection();

                //to select rows corresponding to selected features in the layer
                if (_layer.Selection.Count > 0)
                {
                    _ignoreMapSelection = true;
                    foreach (DataGridViewRow dgr in this.Rows)
                    {
                        int fid = (int)dgr.Cells["mw_fid"].Value;
                        dgr.Selected = _layer.DrawnStates[fid].Selected;
                    }
                    _ignoreMapSelection = false;
                }
            }
            DataGridView dgv = new DataGridView();

        }

        //to select all when pressing CTRL+A
        protected override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);

            if (_layer.DataSet.Features.Count != this.RowCount) return;

            if (!_sorting && this.SelectedRows.Count == RowCount)
            {
                foreach (DataGridViewRow dgr in this.Rows)
                {
                    int fid = (int)dgr.Cells["mw_fid"].Value;
                    _layer.DrawnStates[fid].Selected = dgr.Selected;
                }
                ////to force to refresh the map
                _mapFrame.ResetBuffer();
                ////zoom to selected features
                if (ZoomToSelected == true)
                {
                   _layer.ZoomToSelectedFeatures();
                }
            }
        }
        

        //this event is used for sorting
        protected override void OnCellMouseUp(DataGridViewCellMouseEventArgs e)
        {
            //_cellMouseUp = true;
            
            base.OnCellMouseUp(e);

            _ignoreMapSelection = true;

            if (e.RowIndex < 0)
            {
                _sorting = true;
                
                //mouse up on the column header means sorting
                if (_layer.Selection.Count == 0)
                {
                    this.ClearSelection();
                }
                else
                {
                    foreach (DataGridViewRow dgr in this.Rows)
                    {
                        int fid = (int)dgr.Cells["mw_fid"].Value;
                        dgr.Selected = _layer.DrawnStates[fid].Selected;
                    }
                    this.Refresh();
                }

                _sorting = false;
            }
            else
            {
                foreach (DataGridViewRow dgr in this.Rows)
                {
                    int fid = (int)dgr.Cells["mw_fid"].Value;
                    _layer.DrawnStates[fid].Selected = dgr.Selected;
                }
                //to force to refresh the map
                _mapFrame.ResetBuffer();
                //zoom to selected features
                if (ZoomToSelected == true)
                {
                    _layer.ZoomToSelectedFeatures();
                }
            }

            _ignoreMapSelection = false;
            //_cellMouseUp = false;
        }
    }


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MapWindow.Map;
using MapWindow.Drawing;
using System.Data;

namespace HydroDesktopControls
{
    public class MwDataGridView : DataGridView
    {
        #region Variables
        //indicates that the data source has been changed
        private bool _dataSourceChanged = false;
        private int _dataBindingEventCount = 0;

        private IMapFrame _mapFrame = null;
        private IMapFeatureLayer _layer = null;

        private bool _ignoreMapSelection = false;
        #endregion


        #region Constructor

        public MwDataGridView()
        {
            this.AllowUserToAddRows = false;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.EditMode = DataGridViewEditMode.EditProgrammatically;
        }

        #endregion

        #region Properties
        
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
            
            //add the 'fid' column
            int lastColumn = _layer.DataSet.DataTable.Columns.Count;
            if (_layer.DataSet.DataTable.Columns.Contains("mw_fid"))
            {
                lastColumn = _layer.DataSet.DataTable.Columns.IndexOf("mw_fid");
            }
            else
            {
                _layer.DataSet.DataTable.Columns.Add("mw_fid", typeof(Int32));
            }
            int rowIndex = 0;

            foreach (DataRow row in _layer.DataSet.DataTable.Rows)
            {
                row[lastColumn] = rowIndex;
                rowIndex++; 
            }
            
            _dataSourceChanged = true;
            this.DataSource = sourceLayer.DataSet.DataTable;
            if (this.Columns.Contains("mw_fid"))
            {
                this.Columns["mw_fid"].Visible = false;
            }

            _layer.SelectionChanged += new EventHandler(layer_SelectionChanged);
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
            }
        }

        protected override void OnCellMouseUp(DataGridViewCellMouseEventArgs e)
        {
            base.OnCellMouseUp(e);

            _ignoreMapSelection = true;

            if (e.RowIndex < 0)
            {
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
                }
            }
            else
            {
                //mouse up on other row
                
                foreach (DataGridViewRow dgr in this.Rows)
                {
                    int fid = (int)dgr.Cells["mw_fid"].Value;
                    _layer.DrawnStates[fid].Selected = dgr.Selected;

                }
                _mapFrame.ResetBuffer();
            }

            _ignoreMapSelection = false;
        }
    }
}

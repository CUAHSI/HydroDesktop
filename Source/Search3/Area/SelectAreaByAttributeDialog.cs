using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Symbology;

namespace Search3.Area
{
    public partial class SelectAreaByAttributeDialog : Form
    {
        #region Fields

        private readonly Map _map;

        #endregion

        #region Constructors

        private SelectAreaByAttributeDialog(Map map)
        {
            if (map == null) throw new ArgumentNullException("map");

            _map = map;

            InitializeComponent();
            BindMapToForm();
        }

        #endregion

        #region Public methods

        public static DialogResult ShowDialog(Map map)
        {
            if (map == null) throw new ArgumentNullException("map");

            using (var form = new SelectAreaByAttributeDialog(map))
            {
                return form.ShowDialog();
            }
        }

        #endregion

        #region Private methods

        private void BindMapToForm()
        {
            cmbActiveLayer.SelectedIndexChanged -= cmbActiveLayer_SelectedIndexChanged;
            var layers = AreaHelper.GetAllPolygonLayers(_map).ToList<ILegendItem>();
            cmbActiveLayer.DisplayMember = "LegendText";
            cmbActiveLayer.DataSource = layers;

            var selectedLayer = AreaHelper.GetAllSelectedPolygonLayers(_map).FirstOrDefault();
            cmbActiveLayer.SelectedItem = selectedLayer;
            cmbActiveLayer.SelectedIndexChanged += cmbActiveLayer_SelectedIndexChanged;
            cmbActiveLayer_SelectedIndexChanged(cmbActiveLayer, EventArgs.Empty);
        }

        void cmbActiveLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedLayer = cmbActiveLayer.SelectedItem as IMapPolygonLayer;
            if (selectedLayer == null)
            {
                cmbField.DataSource = null;
                return;
            }

            // Select layer in the map
            foreach (IMapPolygonLayer layer in cmbActiveLayer.Items)
                layer.IsSelected = layer == selectedLayer;
            _map.Legend.RefreshNodes();

            // Update fields
            cmbField.SelectedIndexChanged -= cmbField_SelectedIndexChanged;
            var dataColumns = new DataColumn[selectedLayer.DataSet.DataTable.Columns.Count];
            selectedLayer.DataSet.DataTable.Columns.CopyTo(dataColumns, 0);
            cmbField.DisplayMember = "ColumnName";
            cmbField.DataSource = dataColumns;
            cmbField.SelectedIndexChanged += cmbField_SelectedIndexChanged;
            cmbField_SelectedIndexChanged(cmbField, EventArgs.Empty);
        }

        void cmbField_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dataColumn = cmbField.SelectedItem as DataColumn;
            if (dataColumn == null)
            {
                return;
            }

            var selectedLayer = cmbActiveLayer.SelectedItem as IMapPolygonLayer;
            if (selectedLayer == null) return;

            var uniqueValues = new SortedSet<string>();
            foreach(DataRow row in selectedLayer.DataSet.DataTable.Rows)
                uniqueValues.Add(row[dataColumn.ColumnName].ToString());

            cmbValues.DataSource = uniqueValues.ToList();

            if (!selectedLayer.IsVisible) 
                selectedLayer.IsVisible = true;
        }

        private void SelectShapesInTheMap()
        {
            var selectedLayer = cmbActiveLayer.SelectedItem as IMapPolygonLayer;
            if (selectedLayer == null) return;

            var dataColumn = cmbField.SelectedItem as DataColumn;
            if (dataColumn == null) return;

            if (cmbValues.SelectedItem == null) return;
            var selectedValue = cmbValues.SelectedItem.ToString();
            if (String.IsNullOrWhiteSpace(selectedValue)) return;

            string selectedColumn = cmbField.Text;

            string filterEx = string.Format("[{0}] = '{1}'", selectedColumn, selectedValue);
            selectedLayer.SelectByAttribute(filterEx, ModifySelectionMode.Append);
            _map.MapFrame.IsSelected = false;

            //zoom to selection
            selectedLayer.ZoomToSelectedFeatures();

            selectedLayer.IsSelected = false;
            selectedLayer.IsSelected = true;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            SelectShapesInTheMap();
            btnApply.Enabled = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SelectShapesInTheMap();
        }

        #endregion

        private void cmbValues_SelectedValueChanged(object sender, EventArgs e)
        {
            btnApply.Enabled = true;
        }
    }
}

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
                lbValues.DataSource = null;
                return;
            }

            var selectedLayer = cmbActiveLayer.SelectedItem as IMapPolygonLayer;
            if (selectedLayer == null) return;

            var uniqueValues = new SortedSet<string>();
            foreach(DataRow row in selectedLayer.DataSet.DataTable.Rows)
                uniqueValues.Add(row[dataColumn.ColumnName].ToString());
            lbValues.DataSource = uniqueValues.ToList();
        }

        private void teFirstLetters_TextChanged(object sender, EventArgs e)
        {
            if (lbValues.Items.Count == 0) return;

            var text = teFirstLetters.Text;
            for (int i = 0; i < lbValues.Items.Count; i++)
            {
                var curItem =  lbValues.Items[i].ToString();
                var selected = string.IsNullOrEmpty(text)
                                   ? (text == curItem)
                                   : curItem.StartsWith(text, StringComparison.InvariantCultureIgnoreCase);
                lbValues.SetSelected(i, selected);

            }
        }

        private void SelectShapesInTheMap()
        {
            var selectedLayer = cmbActiveLayer.SelectedItem as IMapPolygonLayer;
            if (selectedLayer == null) return;

            var dataColumn = cmbField.SelectedItem as DataColumn;
            if (dataColumn == null) return;

            if (lbValues.SelectedItems.Count == 0)
            {
                selectedLayer.UnSelectAll();
                _map.Refresh();
                return;
            }

            var selectedValues = new List<string>(lbValues.SelectedItems.Count);
            selectedValues.AddRange(from object item in lbValues.SelectedItems select item.ToString());

            foreach(var feature in selectedLayer.DataSet.Features)
            {
                if (selectedValues.Contains(feature.DataRow[dataColumn.ColumnName].ToString()))
                    selectedLayer.Select(feature);
                else
                    selectedLayer.UnSelect(feature);
            }
            _map.Refresh();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            SelectShapesInTheMap();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SelectShapesInTheMap();
        }

        #endregion
    }
}

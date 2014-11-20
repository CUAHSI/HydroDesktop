using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Controls;
using Wizard.UI;

namespace HydroDesktop.Plugins.DataImport.CommonPages.SelectLayer
{
    public partial class DataLayerPage : InternalWizardPage
    {
        #region Fields

        private readonly WizardContext _context;

        #endregion

        #region Constructors

        public DataLayerPage(WizardContext context)
        {
            _context = context;
            InitializeComponent();

            chbCreateNewLayer.Checked = true;
        }

        #endregion

        #region Private methods

        private void DataLayerPage_SetActive(object sender, CancelEventArgs e)
        {
            SetWizardButtons(WizardButtons.Next | WizardButtons.Back);

            cmbLayers.Items.Clear();
            foreach (var layer in _context.Settings.Map.GetAllLayers().OfType<IMapPointLayer>())
            {
                cmbLayers.Items.Add(layer.LegendText);
            }

            var legendText = string.Format("Imported Data ({0})", Path.GetFileNameWithoutExtension(_context.Settings.PathToFile));
            tbNewLayer.Text = legendText;
        }

        private void DataLayerPage_WizardNext(object sender, WizardPageEventArgs e)
        {
            var layerName = chbCreateNewLayer.Checked ? tbNewLayer.Text : cmbLayers.Text;
            if (String.IsNullOrWhiteSpace(layerName))
            {
                MessageBox.Show(this, "Please enter name of layer to import.", "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }

            _context.Settings.LayerName = layerName;
        }

        private void chbCreateNewLayer_CheckedChanged(object sender, EventArgs e)
        {
            var newLayer = chbCreateNewLayer.Checked;
            tbNewLayer.Enabled = newLayer;
            cmbLayers.Enabled = !newLayer;
        }

        #endregion
    }
}

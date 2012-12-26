using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Symbology;
using HydroDesktop.Common;
using HydroDesktop.Common.Tools;
using HydroDesktop.Common.UserMessage;

namespace DataAggregation.MergeTool
{
    partial class MergeLayersForm : Form
    {
        private readonly AppManager _app;

        public MergeLayersForm(AppManager app)
        {
            if (app == null) throw new ArgumentNullException("app");

            _app = app;
            InitializeComponent();
        }

        private void MergeLayersForm_Load(object sender, EventArgs e)
        {
            tbLayerName.Text = "Merge_Layer";

            dgvLayers.AutoGenerateColumns = false;
            dgvLayers.Columns.Clear();
            dgvLayers.Columns.Add(new DataGridViewCheckBoxColumn
                {
                    DataPropertyName = NameHelper<LayerInfo>.Name(x => x.Checked),
                    HeaderText = "Check",
                    ReadOnly = false,
                });
            dgvLayers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = NameHelper<LayerInfo>.Name(x => x.Name),
                    HeaderText = "Name",
                    ReadOnly = true,
                    
                });
            dgvLayers.DataSource = MergeLayers.GetDataSiteLayers(_app.Map).
                                               Select(s => new LayerInfo(s))
                                              .ToList();
            dgvLayers.AutoResizeColumns();
            EnableControls(true);
        }

        private void EnableControls(bool enable)
        {
            dgvLayers.Enabled = enable;
            btnOK.Enabled = enable;
            btnCancel.Enabled = enable;
            tbLayerName.Enabled = enable;
            paProgress.Visible = !enable;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrEmpty(tbLayerName.Text))
            {
                AppContext.Instance.Get<IUserMessage>().Info("Enter new layer name.");
                tbLayerName.Focus();
                return;
            }
            var selectedLayers = ((IEnumerable<LayerInfo>) dgvLayers.DataSource).Where(s => s.Checked).ToList();
            if (selectedLayers.Count < 2)
            {
                AppContext.Instance.Get<IUserMessage>().Info("Select at least 2 layers.");
                dgvLayers.Focus();
                return;
            }

            // Start background worker
            EnableControls(false);
            var bw = new BackgroundWorker();
            bw.DoWork += (o, args) => MergeLayers.Merge(new MergeData
                {
                    NewLayerName = tbLayerName.Text,
                    Layers = selectedLayers.Select(s => s.Layer).ToList(),
                });
            bw.RunWorkerCompleted += delegate(object o, RunWorkerCompletedEventArgs args)
                {
                    EnableControls(true);
                    if (args.Error == null)
                    {
                        AppContext.Instance.Get<IUserMessage>().Info("Finished successfully.");
                    }
                    else
                    {
                        AppContext.Instance.Get<IUserMessage>().Error("Finished with error: " + args.Error, args.Error);
                    }

                    //var resultLayer = (IFeatureLayer)args.Result;
                    
                    DialogResult = DialogResult.OK;
                };
            bw.RunWorkerAsync();
        }

        #region Nested class

        private class LayerInfo
        {
            public LayerInfo(IFeatureLayer layer)
            {
                Layer = layer;
            }
            public IFeatureLayer Layer { get; private set; }

            public string Name { get { return Layer.LegendText; } }
            public bool Checked { get; set; }
        }

        #endregion
    }
}

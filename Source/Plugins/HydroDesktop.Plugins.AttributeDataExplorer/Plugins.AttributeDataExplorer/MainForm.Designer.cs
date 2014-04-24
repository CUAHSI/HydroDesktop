using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace HydroDesktop.Plugins.AttributeDataExplorer
{
	public class MainForm : XtraForm
	{
		private readonly AppManager _App;

		private readonly LayerManager _layerManager;

		private SelectionSynchronization _SelectionSynchronization;

		private FeatureLayerDisplayFilter _Filter;

		private PersistLayoutInMemory _PersistLayout;

		private PreventFirstRowSelection _PreventFirstRowSelection;

		private ShowMessageWhenDataSourceIsNull _ShowNoDataMessage;

		public static bool IsDataBinding;

		public static bool IsLayoutRestoring;

		private IContainer components;

		private GridControl gridControl1;

		private GridView gridView1;

		public Panel hostpanel;

		static MainForm()
		{
		}

		public MainForm()
		{
			this.InitializeComponent();
		}

		public MainForm(AppManager app) : this()
		{
			this._App = app;
			this._layerManager = app.Map.GetLayerManager();
			this._layerManager.ActiveLayerChanged += new EventHandler(this.LayerManager_ActiveLayerChanged);
			this._Filter = new FeatureLayerDisplayFilter(this._App.Map, this.gridView1);
			this._PersistLayout = new PersistLayoutInMemory(this._App.Map, this.gridView1);
			this._SelectionSynchronization = new SelectionSynchronization(this._App.Map, this.gridView1);
			this._PreventFirstRowSelection = new PreventFirstRowSelection(this.gridView1);
			this._ShowNoDataMessage = new ShowMessageWhenDataSourceIsNull(this.gridView1);
            app.SerializationManager.Deserializing += OnDeserializingProject;
			this.RefreshData(this._layerManager.ActiveLayer);
		}

		private bool AreThereOnlyAFewColumns(DataTable table)
		{
			if (table == null)
			{
				return true;
			}
			return table.Columns.Count <= 8;
		}

		private void BindData(DataTable table)
		{
			MainForm.IsDataBinding = true;
			if (table != null)
			{
				this.gridControl1.Text = table.GetHashCode().ToString();
			}
			this.gridControl1.DataSource = table;
			this.gridView1.PopulateColumns();
			MainForm.IsDataBinding = false;
			if (table != null && this.gridView1.Columns.Count >= 8)
			{
				this.gridView1.BestFitMaxRowCount = 16;
				this.gridView1.BestFitColumns();
			}
			this.gridView1.OptionsView.ColumnAutoWidth = this.AreThereOnlyAFewColumns(table);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.components != null)
				{
					this.components.Dispose();
				}
				this._layerManager.ActiveLayerChanged -= new EventHandler(this.LayerManager_ActiveLayerChanged);
				_App.SerializationManager.Deserializing -= OnDeserializingProject;
				this._SelectionSynchronization = null;
				this._Filter.ShowAllFeatures();
				this._Filter = null;
				this._PersistLayout = null;
				this._PreventFirstRowSelection = null;
				LayerManagerExt.ClearCache();
			}
			base.Dispose(disposing);
		}

		private DataTable GetDataFromCurrentLayer(ILayer iLayer)
		{
			IMapFeatureLayer mapFeatureLayer = iLayer as IMapFeatureLayer;
            if (mapFeatureLayer == null || mapFeatureLayer.DataSet == null || mapFeatureLayer.DataSet.Filename == null)
			{
				return null;
			}
			return mapFeatureLayer.DataSet.DataTable;
		}

		private void gridControl1_DoubleClick(object sender, EventArgs e)
		{
			this.gridView1.OptionsBehavior.Editable = !this.gridView1.OptionsBehavior.Editable;
		}

		private void InitializeComponent()
		{
			this.hostpanel = new Panel();
			this.gridControl1 = new GridControl();
			this.gridView1 = new GridView();
			this.hostpanel.SuspendLayout();
			((ISupportInitialize)this.gridControl1).BeginInit();
			((ISupportInitialize)this.gridView1).BeginInit();
			base.SuspendLayout();
			this.hostpanel.Controls.Add(this.gridControl1);
			this.hostpanel.Dock = DockStyle.Fill;
			this.hostpanel.Location = new Point(0, 0);
			this.hostpanel.Name = "hostpanel";
			this.hostpanel.Size = new System.Drawing.Size(0x1b2, 0x19c);
			this.hostpanel.TabIndex = 0;
			this.gridControl1.Dock = DockStyle.Fill;
			this.gridControl1.Location = new Point(0, 0);
			this.gridControl1.MainView = this.gridView1;
			this.gridControl1.Name = "gridControl1";
			this.gridControl1.Size = new System.Drawing.Size(0x1b2, 0x19c);
			this.gridControl1.TabIndex = 0;
			this.gridControl1.ViewCollection.AddRange(new BaseView[] { this.gridView1 });
			this.gridControl1.DoubleClick += new EventHandler(this.gridControl1_DoubleClick);
			this.gridView1.GridControl = this.gridControl1;
			this.gridView1.Name = "gridView1";
			this.gridView1.OptionsBehavior.Editable = false;
			this.gridView1.OptionsFind.AllowFindPanel = false;
			this.gridView1.OptionsSelection.MultiSelect = true;
			this.gridView1.OptionsView.ShowAutoFilterRow = true;
			this.gridView1.OptionsView.ShowFooter = true;
			this.gridView1.OptionsView.ShowGroupPanel = false;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(0x1b2, 0x19c);
			base.Controls.Add(this.hostpanel);
			base.Name = "MainForm";
			this.Text = "Attribute Data Explorer";
			this.hostpanel.ResumeLayout(false);
			((ISupportInitialize)this.gridControl1).EndInit();
			((ISupportInitialize)this.gridView1).EndInit();
			base.ResumeLayout(false);
		}

		private void LayerManager_ActiveLayerChanged(object sender, EventArgs e)
		{
			this.RefreshData(this._layerManager.ActiveLayer);
		}

		private void OnDeserializingProject(object sender, SerializingEventArgs e)
		{
			this._layerManager.WireUpMapEvents();
			this._SelectionSynchronization.WireUpMapEvents();
		}

		private void RefreshData(ILayer iLayer)
		{
			this.BindData(this.GetDataFromCurrentLayer(iLayer));
			if (iLayer == null)
			{
				this.Text = "ADE";
				return;
			}
			this.Text = string.Format("ADE - {0}", iLayer.LegendText);
		}

		public void UILoaded()
		{
			this._SelectionSynchronization.ShowSelectionFromCurrentLayer(this._App.Map);
		}
	}
}
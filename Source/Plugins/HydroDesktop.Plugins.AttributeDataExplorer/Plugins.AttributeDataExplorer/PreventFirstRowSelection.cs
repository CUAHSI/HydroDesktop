using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;

namespace HydroDesktop.Plugins.AttributeDataExplorer
{
	public class PreventFirstRowSelection
	{
		protected readonly GridView _GridView;

		private int _FilteredSelectedRowsCount = -1;

		public PreventFirstRowSelection(GridView gridView)
		{
			this._GridView = gridView;
			this._GridView.ColumnFilterChanged += new EventHandler(this.GridView_ColumnFilterChanged);
			this._GridView.SelectionChanged += new SelectionChangedEventHandler(this.GridView_SelectionChanged);
		}

		private void GridView_ColumnFilterChanged(object sender, EventArgs e)
		{
			if (MainForm.IsDataBinding)
			{
				return;
			}
			if (MainForm.IsLayoutRestoring)
			{
				return;
			}
			this._FilteredSelectedRowsCount = this._GridView.SelectedRowsCount;
		}

		private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this._FilteredSelectedRowsCount == 0)
			{
				this._FilteredSelectedRowsCount = -1;
				this._GridView.ClearSelection();
			}
		}
	}
}
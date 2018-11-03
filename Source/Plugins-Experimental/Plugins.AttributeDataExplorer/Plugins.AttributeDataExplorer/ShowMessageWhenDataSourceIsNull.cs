using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HydroDesktop.Plugins.AttributeDataExplorer
{
	public class ShowMessageWhenDataSourceIsNull
	{
		private const string NoData = "Select a feature layer to examine attribute data.";

		private GridView _ActiveView;

		private Font _Font;

		public GridControl ActiveGridControl
		{
			get
			{
				return this.ActiveView.GridControl;
			}
		}

		public GridView ActiveView
		{
			get
			{
				return this._ActiveView;
			}
			set
			{
				this._ActiveView = value;
			}
		}

		public Font PaintFont
		{
			get
			{
				if (this._Font == null)
				{
					return AppearanceObject.DefaultFont;
				}
				return this._Font;
			}
			set
			{
				this._Font = value;
			}
		}

		public ShowMessageWhenDataSourceIsNull(GridView view)
		{
			this.ActiveView = view;
			this.SubscribeEvents();
		}

		private void ActiveView_CustomDrawEmptyForeground(object sender, CustomDrawEventArgs e)
		{
			this.DrawNothingToSee(e);
		}

		private void DrawNothingToSee(CustomDrawEventArgs e)
		{
			if (this.ActiveGridControl.DataSource == null)
			{
				Graphics graphics = e.Graphics;
				Font paintFont = this.PaintFont;
				Brush gray = Brushes.Gray;
				Rectangle seeBounds = this.NothingToSeeBounds(e.Bounds);
				graphics.DrawString("Select a feature layer to examine attribute data.", paintFont, gray, seeBounds.Location);
			}
		}

		private Rectangle GetForegroundBounds()
		{
			return (this.ActiveView.GetViewInfo() as GridViewInfo).ViewRects.Rows;
		}

		private Size GetStringSize(string s, Font font)
		{
			Graphics graphic = Graphics.FromHwnd(this.ActiveGridControl.Handle);
			return graphic.MeasureString(s, font).ToSize();
		}

		private Rectangle NothingToSeeBounds(Rectangle bounds)
		{
			Size stringSize = this.GetStringSize("Select a feature layer to examine attribute data.", this.PaintFont);
			int width = (bounds.Width - stringSize.Width) / 2;
			int y = bounds.Y + 50;
			return new Rectangle(new Point(width, y), stringSize);
		}

		private void SubscribeEvents()
		{
			this.ActiveView.CustomDrawEmptyForeground += new CustomDrawEventHandler(this.ActiveView_CustomDrawEmptyForeground);
		}
	}
}